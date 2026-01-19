using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

public class Highlighter : MonoBehaviour
{

    public static Highlighter Instance { get; private set; }

    [SerializeField] private Color highlightEmissionColor = Color.white;

    [Tooltip("Multiplier for the emission color.")]
    [Min(0f)]
    [SerializeField] private float highlightIntensity = .4f;

    [Tooltip("If true, highlights all child renderers of the interactable object.")]
    [SerializeField] private bool includeChildren = true;


    private readonly Dictionary<Renderer, Color> originalEmission = new();
    private readonly List<Renderer> currentRenderers = new();

    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private MaterialPropertyBlock mpb;
    private Player player;


    private void Awake()
    {
        Instance = this;
        mpb = new MaterialPropertyBlock();
    }

    private void Start()
    {
        player = Player.Instance;
        if (player != null)
            player.OnInteractableChanged += HandleChanged;
    }

    private void Highlight(IInteractable interactable)
    {
        if (interactable == null) { return; }


        Component component = interactable as Component;
        if (component == null) { return; }

        currentRenderers.Clear();

        if (includeChildren)
            component.GetComponentsInChildren(true, currentRenderers);
        else
        {
            Renderer r = component.GetComponent<Renderer>();
            if (r != null) currentRenderers.Add(r);
        }


        foreach (Renderer rend in currentRenderers)
        {
            if (rend == null) { continue; }

            Material mat = rend.sharedMaterial;
            if (mat == null) { continue; }
            if (!mat.HasProperty(EmissionColorId)) { continue; }
            // Many shaders require the emission keyword to be enabled for _EmissionColor to have any visible effect
            mat.EnableKeyword("_EMISSION");

            if (!originalEmission.ContainsKey(rend))
            {
                originalEmission[rend] = mat.GetColor(EmissionColorId);
            }

            rend.GetPropertyBlock(mpb);
            mpb.SetColor(EmissionColorId, highlightEmissionColor * highlightIntensity);
            rend.SetPropertyBlock(mpb);
        }
    }

    private void HideHighlighting(IInteractable interactable)
    {
        if (interactable == null) { return; }

        Component component = interactable as Component;
        if (component == null) { return; }


        currentRenderers.Clear();

        if (includeChildren)
        {
            component.GetComponentsInChildren(true, currentRenderers);
        }
        else
        {
            Renderer r = component.GetComponent<Renderer>();
            if (r != null) { currentRenderers.Add(r); }
        }

        foreach (Renderer rend in currentRenderers)
        {
            if (rend == null) { continue; }

            if (originalEmission.TryGetValue(rend, out var original))
            {
                rend.GetPropertyBlock(mpb);
                mpb.SetColor(EmissionColorId, original);
                rend.SetPropertyBlock(mpb);
                originalEmission.Remove(rend);
            }
            else
            {
                rend.SetPropertyBlock(null);
            }
        }

        currentRenderers.Clear();
    }

    private void HandleChanged(IInteractable prev, IInteractable current)
    {
        if (prev != null) HideHighlighting(prev);
        if (current != null) Highlight(current);
    }
}
