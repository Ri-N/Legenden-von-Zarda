using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Outcomes/Cycle Until Time Phase")]
public class CycleUntilTimePhaseOutcome : DialogueOutcomeAction
{
    [SerializeField] private StoryTimePhase targetPhase = StoryTimePhase.Morning;
    [SerializeField] private float transitionSecondsPerStep = 10f;

    [Header("Sleep Blur")]
    [SerializeField] private bool playBlur = true;
    [SerializeField] private float blurFadeInSeconds = 0.6f;
    [SerializeField] private float blurFadeOutSeconds = 0.6f;
    [SerializeField, Range(0f, 1f)] private float blurPeakWeight = 1f;

    [Header("Player Lock")]
    [SerializeField] private bool lockPlayerDuringSleep = true;

    [Tooltip("Which BlockReason should be applied while the time cycle runs (sleep/skip time).")]
    [SerializeField] private BlockReason blockReason = BlockReason.Sleep;

    public override void Execute(DialogueOutcomeContext ctx)
    {
        TimeManager timeManager = TimeManager.Instance;
        EnvironmentController environmentController = EnvironmentController.Instance;
        GameBlockController blockController = GameBlockController.Instance;

        if (timeManager == null)
        {
            Debug.LogError("CycleUntilTimePhaseOutcome: No TimeManager found in the scene.");
            return;
        }

        if (environmentController == null)
        {
            Debug.LogError("CycleUntilTimePhaseOutcome: No EnvironmentController found in the scene.");
            return;
        }

        // This outcome is the “sleep start” notification point.
        if (lockPlayerDuringSleep)
        {
            if (blockController != null)
                blockController.SetBlocked(blockReason, true);
        }

        void Restore(bool reachedTarget)
        {
            timeManager.CycleEnded -= Restore;

            // This outcome is the “sleep end” notification point.
            if (lockPlayerDuringSleep)
            {
                if (blockController != null)
                    blockController.SetBlocked(blockReason, false);
            }

            if (playBlur)
            {
                environmentController.SetBlur(0f, blurFadeOutSeconds);
            }
        }

        timeManager.CycleEnded += Restore;

        if (playBlur)
        {
            environmentController.SetBlur(blurPeakWeight, blurFadeInSeconds);
        }

        timeManager.CycleUntilPhase(targetPhase, transitionSecondsPerStep);
    }
}
