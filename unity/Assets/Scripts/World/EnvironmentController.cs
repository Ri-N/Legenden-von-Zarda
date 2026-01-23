using System.Collections;

using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    [Header("Skyboxes")]
    [SerializeField] private Texture2D skyboxNight;
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;

    [Header("Light Gradients")]
    [SerializeField] private Gradient graddientNightToSunrise;
    [SerializeField] private Gradient graddientSunriseToDay;
    [SerializeField] private Gradient graddientDayToSunset;
    [SerializeField] private Gradient graddientSunsetToNight;

    [Header("Scene References")]
    [SerializeField] private Light globalLight;

    public (Texture2D skybox, Gradient lightGradient) GetVisualsForPhase(StoryTimePhase phase)
    {
        // Lunch uses the same visuals as Day by default.
        switch (phase)
        {
            case StoryTimePhase.Morning:
                return (skyboxDay, graddientSunriseToDay);
            case StoryTimePhase.Lunch:
                return (skyboxDay, graddientSunriseToDay);
            case StoryTimePhase.Evening:
                return (skyboxSunset, graddientDayToSunset);
            case StoryTimePhase.Night:
            default:
                return (skyboxNight, graddientSunsetToNight);
        }
    }

    public void ApplySkyboxImmediate(Texture2D texture)
    {
        if (RenderSettings.skybox == null) return;
        RenderSettings.skybox.SetTexture("_Texture1", texture);
        RenderSettings.skybox.SetTexture("_Texture2", texture);
        RenderSettings.skybox.SetFloat("_Blend", 0f);
    }

    public void ApplyLightImmediate(Gradient lightGradient)
    {
        if (globalLight == null || lightGradient == null) return;
        globalLight.color = lightGradient.Evaluate(1f);
        RenderSettings.fogColor = globalLight.color;
    }

    public void ApplySunRotation(int hours, int minutes)
    {
        if (globalLight == null) return;

        // Map clock time (0..1440) to a full 360° rotation.
        float t = (hours * 60f + minutes) / 1440f;
        float angle = t * 360f;

        // Rotate around world up to keep it simple and deterministic.
        globalLight.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }

    public IEnumerator LerpSkybox(Texture2D a, Texture2D b, float time)
    {
        if (RenderSettings.skybox == null || a == null || b == null)
        {
            ApplySkyboxImmediate(b != null ? b : a);
            yield break;
        }

        RenderSettings.skybox.SetTexture("_Texture1", a);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", 0);

        for (float i = 0; i < time; i += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", i / time);
            yield return null;
        }

        RenderSettings.skybox.SetTexture("_Texture1", b);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", 0);
    }

    public IEnumerator LerpLight(Gradient lightGradient, float time)
    {
        if (globalLight == null || lightGradient == null)
            yield break;

        for (float i = 0; i < time; i += Time.deltaTime)
        {
            globalLight.color = lightGradient.Evaluate(i / time);
            RenderSettings.fogColor = globalLight.color;
            yield return null;
        }
    }

    private static float ClockToAngle(int hours, int minutes)
    {
        float t = (hours * 60f + minutes) / 1440f;
        return t * 360f;
    }

    public IEnumerator LerpSunRotation(int fromHours, int fromMinutes, int toHours, int toMinutes, float time)
    {
        if (globalLight == null)
            yield break;

        if (time <= 0f)
        {
            ApplySunRotation(toHours, toMinutes);
            yield break;
        }

        float fromAngle = ClockToAngle(fromHours, fromMinutes);
        float toAngle = ClockToAngle(toHours, toMinutes);

        for (float i = 0; i < time; i += Time.deltaTime)
        {
            float t = i / time;
            float angle = Mathf.LerpAngle(fromAngle, toAngle, t);
            globalLight.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            yield return null;
        }

        globalLight.transform.rotation = Quaternion.AngleAxis(toAngle, Vector3.up);
    }
}
