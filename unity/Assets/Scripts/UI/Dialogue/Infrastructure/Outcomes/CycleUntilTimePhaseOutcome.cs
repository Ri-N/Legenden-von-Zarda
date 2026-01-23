using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Outcomes/Cycle Until Time Phase")]
public class CycleUntilTimePhaseOutcome : DialogueOutcomeAction
{
    [SerializeField] private StoryTimePhase targetPhase = StoryTimePhase.Morning;
    [SerializeField] private float transitionSecondsPerStep = 10f;

    public override void Execute(DialogueOutcomeContext ctx)
    {
        TimeManager timeManager = FindFirstObjectByType<TimeManager>();
        if (timeManager == null)
        {
            Debug.LogError("CycleUntilTimePhaseOutcome: No TimeManager found in the scene.");
            return;
        }

        timeManager.CycleUntilPhase(targetPhase, transitionSecondsPerStep);
    }
}
