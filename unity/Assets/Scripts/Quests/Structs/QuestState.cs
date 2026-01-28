using System;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Runtime state for a single quest instance (progress, flags, branch, variables).
/// Stored by QuestLog and queried by dialogue/interaction gating.
/// </summary>
[Serializable]
public struct QuestState
{
    [SerializeField] private QuestStatus status;
    [SerializeField] private string currentStepId;
    [SerializeField] private QuestBranch branch;

    // Unity does not serialize HashSet/Dictionary reliably in the inspector/runtime.
    // We store flags/vars as serializable lists.
    [SerializeField] private List<string> flags;
    [SerializeField] private List<IntVar> intVars;

    public QuestStatus Status => status;
    public string CurrentStepId => currentStepId;
    public QuestBranch Branch => branch;

    public IReadOnlyList<string> Flags => flags;
    public IReadOnlyList<IntVar> IntVars => intVars;

    public QuestState(QuestStatus initialStatus, string initialStepId = null, QuestBranch initialBranch = QuestBranch.None)
    {
        status = initialStatus;
        currentStepId = initialStepId ?? string.Empty;
        branch = initialBranch;
        flags = new List<string>(8);
        intVars = new List<IntVar>(4);
    }

    public static QuestState CreateNew()
        => new QuestState(QuestStatus.NotStarted, string.Empty, QuestBranch.None);

    public void SetStatus(QuestStatus newStatus)
        => status = newStatus;

    public void SetStep(string stepId)
        => currentStepId = stepId ?? string.Empty;

    public void SetBranch(QuestBranch newBranch)
        => branch = newBranch;

    public bool HasFlag(string flag)
    {
        if (string.IsNullOrWhiteSpace(flag))
            return false;

        EnsureCollections();

        for (int i = 0; i < flags.Count; i++)
        {
            if (string.Equals(flags[i], flag, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    public void SetFlag(string flag, bool value)
    {
        if (string.IsNullOrWhiteSpace(flag))
            return;

        EnsureCollections();

        int idx = IndexOfFlag(flag);
        if (value)
        {
            if (idx < 0)
                flags.Add(flag);
        }
        else
        {
            if (idx >= 0)
                flags.RemoveAt(idx);
        }
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        if (string.IsNullOrWhiteSpace(key))
            return defaultValue;

        EnsureCollections();

        int idx = IndexOfIntVar(key);
        return idx >= 0 ? intVars[idx].value : defaultValue;
    }

    public void SetInt(string key, int value)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        EnsureCollections();

        int idx = IndexOfIntVar(key);
        if (idx >= 0)
        {
            IntVar v = intVars[idx];
            v.value = value;
            intVars[idx] = v;
        }
        else
        {
            intVars.Add(new IntVar(key, value));
        }
    }

    public bool RemoveInt(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        EnsureCollections();

        int idx = IndexOfIntVar(key);
        if (idx < 0)
            return false;

        intVars.RemoveAt(idx);
        return true;
    }

    private void EnsureCollections()
    {
        flags ??= new List<string>(8);
        intVars ??= new List<IntVar>(4);
    }

    private int IndexOfFlag(string flag)
    {
        for (int i = 0; i < flags.Count; i++)
        {
            if (string.Equals(flags[i], flag, StringComparison.Ordinal))
                return i;
        }
        return -1;
    }

    private int IndexOfIntVar(string key)
    {
        for (int i = 0; i < intVars.Count; i++)
        {
            if (string.Equals(intVars[i].key, key, StringComparison.Ordinal))
                return i;
        }
        return -1;
    }

    [Serializable]
    public struct IntVar
    {
        public string key;
        public int value;

        public IntVar(string key, int value)
        {
            this.key = key ?? string.Empty;
            this.value = value;
        }
    }
}
