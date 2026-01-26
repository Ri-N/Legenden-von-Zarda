

using UnityEngine;

public interface IUIElementController
{
    UIElement Element { get; }

    void SetHidden(bool hidden);
}
