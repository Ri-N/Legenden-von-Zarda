using System;
using System.Collections;

using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// Fired when a CycleUntilPhase operation starts.
    /// </summary>
    public event Action<StoryTimePhase> CycleStarted;

    /// <summary>
    /// Fired when a CycleUntilPhase operation ends.
    /// reachedTarget == true means the cycle completed naturally by reaching the target.
    /// reachedTarget == false means the cycle was interrupted/canceled.
    /// </summary>
    public event Action<bool> CycleEnded;

    public bool IsCycling => cycleRoutine != null;

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

    private EnvironmentController environment;

    private Coroutine skyboxRoutine;
    private Coroutine lightRoutine;
    private Coroutine sunRoutine;
    private Coroutine cycleRoutine;

    private void StopActiveCycle(bool reachedTarget)
    {
        if (cycleRoutine != null)
        {
            StopCoroutine(cycleRoutine);
            cycleRoutine = null;
        }

        CycleEnded?.Invoke(reachedTarget);
    }

    private void Start()
    {
        environment = EnvironmentController.Instance;

        if (environment == null)
        {
            Debug.LogError("TimeManager: EnvironmentController reference is missing.", this);
            return;
        }

        SetPhaseImmediate(startPhase);
    }

    /// <summary>
    /// Calculates how long a CycleUntilPhase operation will take in seconds, based on the current phase,
    /// the target phase, and the configured step duration.
    /// </summary>
    public float CalculateCycleUntilPhaseDuration(StoryTimePhase targetPhase, float? transitionSecondsPerStep = null)
    {
        float stepSeconds = transitionSecondsPerStep ?? defaultTransitionSeconds;
        if (stepSeconds <= 0f)
            return 0f;

        int steps = CountPhaseSteps(CurrentPhase, targetPhase);
        return steps * stepSeconds;
    }

    private static int CountPhaseSteps(StoryTimePhase from, StoryTimePhase to)
    {
        // Full cycle if already at the target.
        if (from == to)
            return 4;

        int steps = 0;
        StoryTimePhase cur = from;

        // Safety guard: we only have 4 phases, so we should never need more than 4 steps.
        while (cur != to && steps < 10)
        {
            cur = GetNextPhase(cur);
            steps++;
        }

        return steps;
    }

    /// <summary>
    /// Story-driven time change. Transitions visuals, then holds the time until the next call.
    /// </summary>
    public void CycleUntilPhase(StoryTimePhase targetPhase, float? transitionSecondsPerStep = null)
    {
        if (cycleRoutine != null)
            StopActiveCycle(reachedTarget: false);

        float stepSeconds = transitionSecondsPerStep ?? defaultTransitionSeconds;
        if (stepSeconds <= 0f)
        {
            // If no transition time is desired, just jump directly.
            SetPhaseImmediate(targetPhase);
            return;
        }

        CycleStarted?.Invoke(targetPhase);
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

            StoryTimePhase next = GetNextPhase(CurrentPhase);
            TriggerTimeTransition(next, stepSeconds, cancelActiveCycle: false);
            yield return new WaitForSeconds(stepSeconds);
        }

        cycleRoutine = null;
        CycleEnded?.Invoke(true);
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
        (int hours, int minutes) t = GetClockForPhase(phase);
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

        if (cancelActiveCycle && cycleRoutine != null)
        {
            StopActiveCycle(reachedTarget: false);
        }

        (Texture2D skybox, _) = environment != null ? environment.GetVisualsForPhase(CurrentPhase) : default;
        (Texture2D skybox, Gradient lightGradient) to = environment != null ? environment.GetVisualsForPhase(targetPhase) : default;

        skyboxRoutine = StartCoroutine(RunSkyboxTransition(skybox, to.skybox, time));
        lightRoutine = StartCoroutine(RunLightTransition(to.lightGradient, time));

        int fromHours = Hours;
        int fromMinutes = Minutes;
        (int hours, int minutes) toTime = GetClockForPhase(targetPhase);
        sunRoutine = StartCoroutine(RunSunTransition(fromHours, fromMinutes, toTime.hours, toTime.minutes, time));

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
        if (cycleRoutine != null)
            StopActiveCycle(reachedTarget: false);

        (Texture2D skybox, Gradient lightGradient) = environment != null ? environment.GetVisualsForPhase(phase) : default;
        environment?.ApplySkyboxImmediate(skybox);
        environment?.ApplyLightImmediate(lightGradient);

        SetClockForPhase(phase);
        CurrentPhase = phase;
    }
}
