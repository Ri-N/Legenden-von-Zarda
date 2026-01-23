using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Outcomes/Cycle Until Time Phase")]
public class CycleUntilTimePhaseOutcome : DialogueOutcomeAction
{
    [SerializeField] private StoryTimePhase targetPhase = StoryTimePhase.Morning;
    [SerializeField] private float transitionSecondsPerStep = 10f;

    [Header("Sleep Blur")]
    [SerializeField] private bool playSleepBlur = true;
    [SerializeField] private float blurFadeInSeconds = 0.6f;
    [SerializeField] private float blurFadeOutSeconds = 0.6f;
    [SerializeField, Range(0f, 1f)] private float blurPeakWeight = 1f;

    [Header("Player Lock")]
    [SerializeField] private bool lockPlayerDuringSleep = true;

    public override void Execute(DialogueOutcomeContext ctx)
    {
        TimeManager timeManager = TimeManager.Instance;
        EnvironmentController environmentController = EnvironmentController.Instance;

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


        if (lockPlayerDuringSleep)
        {
            if (Player.Instance != null)
                Player.Instance.SetCanMove(false);

            UIController.RequestHide(UIController.UIElement.PlayerInteract);
        }

        void Restore(bool reachedTarget)
        {
            timeManager.CycleEnded -= Restore;

            if (lockPlayerDuringSleep)
            {
                if (Player.Instance != null)
                    Player.Instance.SetCanMove(true);

                UIController.RequestShow(UIController.UIElement.PlayerInteract);
            }

            if (playSleepBlur)
            {
                environmentController.SetBlur(0f, blurFadeOutSeconds);
            }
        }

        timeManager.CycleEnded += Restore;

        if (playSleepBlur)
        {
            environmentController.SetBlur(blurPeakWeight, blurFadeInSeconds);
        }

        timeManager.CycleUntilPhase(targetPhase, transitionSecondsPerStep);
    }
}
