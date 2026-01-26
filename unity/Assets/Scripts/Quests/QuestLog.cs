using System;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Runtime manager for all quests.
/// Stores QuestState per questId and provides operations to start/advance quests.
/// Quest definitions are ScriptableObjects referenced in the inspector.
/// </summary>
public class QuestLog : MonoBehaviour
{
    [Header("Quest Definitions")]
    [Tooltip("All quests known to this game/session.")]
    [SerializeField] private List<QuestDefinition> questDefinitions = new();

    [Header("Runtime State")]
    [Tooltip("Serialized runtime states (for debugging & persistence-ready structure).")]
    [SerializeField] private List<QuestRuntimeEntry> runtime = new();
    private Player player;
    private readonly Dictionary<string, QuestDefinition> definitionById = new(StringComparer.Ordinal);
    private readonly Dictionary<string, int> runtimeIndexById = new(StringComparer.Ordinal);

    public event Action<string, QuestState> QuestStateChanged;

    private void Awake()
    {
        RebuildDefinitionIndex();
        RebuildRuntimeIndex();
    }

    private void Start()
    {
        player = Player.Instance;
    }

    private void RebuildDefinitionIndex()
    {
        definitionById.Clear();

        if (questDefinitions == null)
            return;

        for (int i = 0; i < questDefinitions.Count; i++)
        {
            QuestDefinition def = questDefinitions[i];
            if (def == null || string.IsNullOrWhiteSpace(def.questId))
                continue;

            // Last one wins if duplicates exist.
            definitionById[def.questId] = def;
        }
    }

    private void RebuildRuntimeIndex()
    {
        runtimeIndexById.Clear();

        if (runtime == null)
            return;

        for (int i = 0; i < runtime.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(runtime[i].questId))
                continue;

            runtimeIndexById[runtime[i].questId] = i;
        }
    }

    // ------------------------------
    // Queries
    // ------------------------------

    public QuestDefinition GetDefinition(string questId)
    {
        if (string.IsNullOrWhiteSpace(questId))
            return null;

        definitionById.TryGetValue(questId, out QuestDefinition def);
        return def;
    }

    public QuestState GetStateOrDefault(string questId)
    {
        if (string.IsNullOrWhiteSpace(questId))
            return QuestState.CreateNew();

        if (!runtimeIndexById.TryGetValue(questId, out int idx))
            return QuestState.CreateNew();

        return runtime[idx].state;
    }

    public bool TryGetState(string questId, out QuestState state)
    {
        state = QuestState.CreateNew();

        if (string.IsNullOrWhiteSpace(questId))
            return false;

        if (!runtimeIndexById.TryGetValue(questId, out int idx))
            return false;

        state = runtime[idx].state;
        return true;
    }

    public bool IsActive(string questId)
        => GetStateOrDefault(questId).Status == QuestStatus.Active;

    public bool IsCompleted(string questId)
        => GetStateOrDefault(questId).Status == QuestStatus.Completed;

    // ------------------------------
    // Mutations / API
    // ------------------------------

    public bool StartQuest(string questId, UnityEngine.Object source = null)
    {
        QuestDefinition def = GetDefinition(questId);
        return StartQuest(def, source);
    }

    public bool StartQuest(QuestDefinition definition, UnityEngine.Object source = null)
    {
        if (definition == null)
        {
            Debug.LogError($"[{nameof(QuestLog)}] StartQuest called with null definition.", this);
            return false;
        }

        if (string.IsNullOrWhiteSpace(definition.questId))
        {
            Debug.LogError($"[{nameof(QuestLog)}] QuestDefinition has no questId.", definition);
            return false;
        }

        // Ensure it's a known definition in this log.
        definitionById[definition.questId] = definition;

        QuestState state = GetOrCreateState(definition.questId, out int idx);

        // If already active or completed, do nothing.
        if (state.Status != QuestStatus.NotStarted)
            return false;

        state.SetStatus(QuestStatus.Active);

        QuestStepDefinition? first = definition.GetFirstStep();
        if (first.HasValue)
        {
            QuestStepDefinition firstStep = first.Value;
            state.SetStep(firstStep.stepId);
            WriteState(idx, state);

            ExecuteEnterActions(definition, firstStep, state, source);
            EvaluateAndAdvance(definition.questId, source);
        }
        else
        {
            // No steps: mark completed to avoid a broken state.
            state.SetStep(string.Empty);
            state.SetStatus(QuestStatus.Completed);
            WriteState(idx, state);
        }

        return true;
    }

    public void SetStep(string questId, string stepId, UnityEngine.Object source = null, bool runEnterActions = true, bool evaluateAfter = true)
    {
        if (string.IsNullOrWhiteSpace(questId))
            return;

        QuestDefinition def = GetDefinition(questId);
        QuestState state = GetOrCreateState(questId, out int idx);

        if (state.Status == QuestStatus.NotStarted)
            state.SetStatus(QuestStatus.Active);

        state.SetStep(stepId);
        WriteState(idx, state);

        if (runEnterActions && def != null)
        {
            QuestStepDefinition? step = def.GetStep(stepId);
            if (step.HasValue)
                ExecuteEnterActions(def, step.Value, state, source);
        }

        if (evaluateAfter)
            EvaluateAndAdvance(questId, source);
    }

    public void CompleteQuest(string questId, UnityEngine.Object source = null)
    {
        if (string.IsNullOrWhiteSpace(questId))
            return;

        QuestState state = GetOrCreateState(questId, out int idx);
        if (state.Status == QuestStatus.Completed)
            return;

        state.SetStatus(QuestStatus.Completed);
        WriteState(idx, state);
    }

    public void SetFlag(string questId, QuestFlag flag, bool value, UnityEngine.Object source = null)
    {
        SetFlag(questId, flag.ToString(), value, source);
    }

    public void SetFlag(string questId, string flag, bool value, UnityEngine.Object source = null)
    {
        if (string.IsNullOrWhiteSpace(questId))
            return;

        QuestState state = GetOrCreateState(questId, out int idx);
        state.SetFlag(flag, value);
        WriteState(idx, state);

        // Some flags can make steps completable.
        EvaluateAndAdvance(questId, source);
    }

    public void SetBranch(string questId, QuestBranch branch)
    {
        if (string.IsNullOrWhiteSpace(questId))
            return;

        QuestState state = GetOrCreateState(questId, out int idx);
        state.SetBranch(branch);
        WriteState(idx, state);
    }

    public void SetInt(string questId, string key, int value, UnityEngine.Object source = null)
    {
        if (string.IsNullOrWhiteSpace(questId))
            return;

        QuestState state = GetOrCreateState(questId, out int idx);
        state.SetInt(key, value);
        WriteState(idx, state);

        EvaluateAndAdvance(questId, source);
    }

    public int GetInt(string questId, string key, int defaultValue = 0)
    {
        QuestState state = GetStateOrDefault(questId);
        return state.GetInt(key, defaultValue);
    }

    // ------------------------------
    // Step evaluation
    // ------------------------------

    /// <summary>
    /// Checks if the current step of the quest can be completed.
    /// If yes: executes onComplete actions and advances to nextStepId.
    /// This will loop through auto-completable steps but is guarded.
    /// </summary>
    public void EvaluateAndAdvance(string questId, UnityEngine.Object source = null)
    {
        if (string.IsNullOrWhiteSpace(questId))
            return;

        QuestDefinition def = GetDefinition(questId);
        if (def == null)
            return;

        QuestState state = GetOrCreateState(questId, out int idx);
        if (state.Status != QuestStatus.Active)
            return;

        // Guard against accidental infinite loops from misconfigured nextStepId.
        const int maxTransitionsPerCall = 16;
        int transitions = 0;

        while (transitions++ < maxTransitionsPerCall)
        {
            string stepId = state.CurrentStepId;
            QuestStepDefinition? step = def.GetStep(stepId);
            if (!step.HasValue)
                break;

            QuestStepDefinition currentStep = step.Value;

            QuestContext ctx = new QuestContext(this, def, state, player, source);
            if (!currentStep.AreCompletionConditionsMet(ctx))
                break;

            ExecuteCompleteActions(def, currentStep, state, source);

            if (string.IsNullOrWhiteSpace(currentStep.nextStepId))
            {
                // No next step: quest remains active unless an action completed it.
                // If you want linear quests to auto-complete, add a CompleteQuestAction.
                break;
            }

            state.SetStep(currentStep.nextStepId);
            WriteState(idx, state);

            QuestStepDefinition? next = def.GetStep(currentStep.nextStepId);
            if (next.HasValue)
                ExecuteEnterActions(def, next.Value, state, source);
            else
                break;
        }

        // If we hit the guard, warn to help debugging.
        if (transitions >= maxTransitionsPerCall)
        {
            Debug.LogWarning($"[{nameof(QuestLog)}] EvaluateAndAdvance reached guard limit for quest '{questId}'. Check your nextStepId chain.", this);
        }
    }

    private void ExecuteEnterActions(QuestDefinition def, QuestStepDefinition step, QuestState state, UnityEngine.Object source)
    {
        if (step.onEnterActions == null || step.onEnterActions.Count == 0)
            return;

        QuestContext ctx = new QuestContext(this, def, state, player, source);
        for (int i = 0; i < step.onEnterActions.Count; i++)
        {
            QuestAction action = step.onEnterActions[i];
            if (action == null)
                continue;

            action.Execute(ctx);
        }
    }

    private void ExecuteCompleteActions(QuestDefinition def, QuestStepDefinition step, QuestState state, UnityEngine.Object source)
    {
        if (step.onCompleteActions == null || step.onCompleteActions.Count == 0)
            return;

        QuestContext ctx = new QuestContext(this, def, state, player, source);
        for (int i = 0; i < step.onCompleteActions.Count; i++)
        {
            QuestAction action = step.onCompleteActions[i];
            if (action == null)
                continue;

            action.Execute(ctx);
        }
    }

    // ------------------------------
    // Internal helpers
    // ------------------------------

    private QuestState GetOrCreateState(string questId, out int idx)
    {
        if (runtimeIndexById.TryGetValue(questId, out idx))
            return runtime[idx].state;

        idx = runtime.Count;
        QuestRuntimeEntry entry = new QuestRuntimeEntry
        {
            questId = questId,
            state = QuestState.CreateNew(),
        };
        runtime.Add(entry);
        runtimeIndexById[questId] = idx;
        return entry.state;
    }

    private void WriteState(int idx, QuestState state)
    {
        QuestRuntimeEntry entry = runtime[idx];
        entry.state = state;
        runtime[idx] = entry;

        QuestStateChanged?.Invoke(entry.questId, state);
    }

    [Serializable]
    private struct QuestRuntimeEntry
    {
        public string questId;
        public QuestState state;
    }
}
