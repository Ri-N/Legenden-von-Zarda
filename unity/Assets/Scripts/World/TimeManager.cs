using System.Collections;

using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Story Time")]
    [SerializeField] private StoryTimePhase startPhase = StoryTimePhase.Morning;
    [SerializeField] private float defaultTransitionSeconds = 10f;

    private int minutes;
    public int Minutes
    {
        get => minutes;
        private set
        {
            minutes = Mathf.Clamp(value, 0, 59);
            environment?.ApplySunRotation(Hours, Minutes);
        }
    }

    [SerializeField] private int hours = 8;
    public int Hours
    {
        get => hours;
        private set
        {
            hours = Mathf.Clamp(value, 0, 23);
            environment?.ApplySunRotation(Hours, Minutes);
        }
    }

    private int days;
    public int Days
    {
        get => days;
        private set => days = Mathf.Max(0, value);
    }

    public StoryTimePhase CurrentPhase { get; private set; }

    [SerializeField] private EnvironmentController environment;

    private Coroutine skyboxRoutine;
    private Coroutine lightRoutine;
    private Coroutine sunRoutine;
    private Coroutine cycleRoutine;

    private void Start()
    {
        if (environment == null)
        {
            Debug.LogError("TimeManager: EnvironmentController reference is missing.", this);
            return;
        }

        SetPhaseImmediate(startPhase);
    }

    /// <summary>
    /// Story-driven time change. Transitions visuals, then holds the time until the next call.
    /// </summary>
    public void TriggerTimeTransition(StoryTimePhase targetPhase, float? transitionSeconds = null, bool cancelActiveCycle = true)
    {
        float time = transitionSeconds ?? defaultTransitionSeconds;
        if (time <= 0f)
        {
            SetPhaseImmediate(targetPhase);
            return;
        }

        if (targetPhase == CurrentPhase)
            return;

        // Stop ongoing transitions so we don't stack coroutines.
        if (skyboxRoutine != null) StopCoroutine(skyboxRoutine);
        if (lightRoutine != null) StopCoroutine(lightRoutine);
        if (sunRoutine != null) StopCoroutine(sunRoutine);

        if (cancelActiveCycle)
        {
            if (cycleRoutine != null) StopCoroutine(cycleRoutine);
            cycleRoutine = null;
        }

        var from = environment != null ? environment.GetVisualsForPhase(CurrentPhase) : default;
        var to = environment != null ? environment.GetVisualsForPhase(targetPhase) : default;

        skyboxRoutine = StartCoroutine(RunSkyboxTransition(from.skybox, to.skybox, time));
        lightRoutine = StartCoroutine(RunLightTransition(to.lightGradient, time));

        // Lerp the sun direction to avoid snapping shadows/highlights.
        int fromHours = Hours;
        int fromMinutes = Minutes;
        var toTime = GetClockForPhase(targetPhase);
        sunRoutine = StartCoroutine(RunSunTransition(fromHours, fromMinutes, toTime.hours, toTime.minutes, time));

        // Phase changes logically now; the clock is applied at the end of the sun lerp.
        CurrentPhase = targetPhase;
    }

    /// <summary>
    /// Instantly sets visuals and time to the given phase (no transition).
    /// </summary>
    public void SetPhaseImmediate(StoryTimePhase phase)
    {
        // Stop ongoing transitions.
        if (skyboxRoutine != null) StopCoroutine(skyboxRoutine);
        if (lightRoutine != null) StopCoroutine(lightRoutine);
        if (sunRoutine != null) StopCoroutine(sunRoutine);
        skyboxRoutine = null;
        lightRoutine = null;
        sunRoutine = null;
        if (cycleRoutine != null) StopCoroutine(cycleRoutine);
        cycleRoutine = null;

        var v = environment != null ? environment.GetVisualsForPhase(phase) : default;
        environment?.ApplySkyboxImmediate(v.skybox);
        environment?.ApplyLightImmediate(v.lightGradient);

        SetClockForPhase(phase);
        CurrentPhase = phase;
    }

    /// <summary>
    /// Advances the time phase step-by-step (wrapping around) until the target phase is reached.
    /// Useful for "sleep until morning" or similar story beats.
    /// </summary>
    public void CycleUntilPhase(StoryTimePhase targetPhase, float? transitionSecondsPerStep = null)
    {
        if (cycleRoutine != null)
            StopCoroutine(cycleRoutine);

        float stepSeconds = transitionSecondsPerStep ?? defaultTransitionSeconds;
        if (stepSeconds <= 0f)
        {
            // If no transition time is desired, just jump directly.
            SetPhaseImmediate(targetPhase);
            return;
        }

        cycleRoutine = StartCoroutine(CycleUntilPhaseRoutine(targetPhase, stepSeconds));
    }

    private static StoryTimePhase GetNextPhase(StoryTimePhase current)
    {
        switch (current)
        {
            case StoryTimePhase.Morning:
                return StoryTimePhase.Lunch;
            case StoryTimePhase.Lunch:
                return StoryTimePhase.Evening;
            case StoryTimePhase.Evening:
                return StoryTimePhase.Night;
            case StoryTimePhase.Night:
            default:
                return StoryTimePhase.Morning;
        }
    }

    private IEnumerator CycleUntilPhaseRoutine(StoryTimePhase targetPhase, float stepSeconds)
    {
        if (environment == null)
        {
            Debug.LogError("TimeManager: EnvironmentController reference is missing.", this);
            cycleRoutine = null;
            yield break;
        }

        bool requireAtLeastOneStep = CurrentPhase == targetPhase;

        while (requireAtLeastOneStep || CurrentPhase != targetPhase)
        {
            requireAtLeastOneStep = false;

            var next = GetNextPhase(CurrentPhase);
            TriggerTimeTransition(next, stepSeconds, cancelActiveCycle: false);
            yield return new WaitForSeconds(stepSeconds);
        }

        cycleRoutine = null;
    }

    private static (int hours, int minutes) GetClockForPhase(StoryTimePhase phase)
    {
        switch (phase)
        {
            case StoryTimePhase.Morning:
                return (8, 0);
            case StoryTimePhase.Lunch:
                return (12, 0);
            case StoryTimePhase.Evening:
                return (18, 0);
            case StoryTimePhase.Night:
                return (22, 0);
            default:
                return (8, 0);
        }
    }

    private void SetClockForPhase(StoryTimePhase phase)
    {
        var t = GetClockForPhase(phase);
        Hours = t.hours;
        Minutes = t.minutes;
    }

    private IEnumerator RunSunTransition(int fromHours, int fromMinutes, int toHours, int toMinutes, float time)
    {
        if (environment == null)
        {
            sunRoutine = null;
            yield break;
        }

        yield return environment.LerpSunRotation(fromHours, fromMinutes, toHours, toMinutes, time);

        // Apply the final clock values (also ensures deterministic final sun rotation).
        Hours = toHours;
        Minutes = toMinutes;

        sunRoutine = null;
    }

    private IEnumerator RunSkyboxTransition(Texture2D a, Texture2D b, float time)
    {
        if (environment == null)
        {
            skyboxRoutine = null;
            yield break;
        }

        yield return environment.LerpSkybox(a, b, time);
        skyboxRoutine = null;
    }

    private IEnumerator RunLightTransition(Gradient lightGradient, float time)
    {
        if (environment == null)
        {
            lightRoutine = null;
            yield break;
        }

        yield return environment.LerpLight(lightGradient, time);
        lightRoutine = null;
    }
}
