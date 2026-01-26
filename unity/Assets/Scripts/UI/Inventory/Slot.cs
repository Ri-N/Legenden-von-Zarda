using System;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("The Icon Image child that displays the item sprite.")]
    [SerializeField] private Image iconImage;

    [Header("Runtime")]
    [SerializeField] private int index = -1;

    /// <summary>
    /// Raised when the slot is clicked. Provides the slot index.
    /// </summary>
    public event Action<int> Clicked;

    public int Index => index;

    private void Reset()
    {

        Transform icon = transform.Find("Icon");
        if (icon != null)
            iconImage = icon.GetComponent<Image>();
    }

    private void Awake()
    {
        // Ensure empty slot visuals if the prefab is instantiated at runtime
        if (iconImage != null)
        {
            iconImage.preserveAspect = true;
            if (iconImage.sprite == null)
                iconImage.enabled = false;
        }
    }

    public void Init(int slotIndex)
    {
        index = slotIndex;
    }

    public void SetIcon(Sprite sprite)
    {
        if (iconImage == null) return;

        iconImage.sprite = sprite;
        iconImage.enabled = sprite != null;
    }

    public void Clear()
    {
        SetIcon(null);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (index >= 0)
            Clicked?.Invoke(index);
    }
}
