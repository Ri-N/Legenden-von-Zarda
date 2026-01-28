using UnityEngine;

public class House : MonoBehaviour
{
    [Header("House")]
    [SerializeField] private GameObject roof;

    private bool subscribed;

    private void OnEnable()
    {
        subscribed = false;
        TrySubscribe();

        if (!subscribed)
        {
            InvokeRepeating(nameof(TrySubscribe), 0.05f, 0.25f);
        }
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(TrySubscribe));

        if (subscribed && PlayerWorldState.Instance != null)
        {
            PlayerWorldState.Instance.AreaChanged -= OnAreaChanged;
        }

        subscribed = false;
    }

    private void TrySubscribe()
    {
        var ws = PlayerWorldState.Instance;
        if (ws == null) return;

        if (!subscribed)
        {
            ws.AreaChanged += OnAreaChanged;
            subscribed = true;
        }

        UpdateRoof(ws.CurrentArea);

        CancelInvoke(nameof(TrySubscribe));
    }

    private void OnAreaChanged(PlayerArea newArea, PlayerAreaContext ctx)
    {
        UpdateRoof(newArea);
    }

    private void UpdateRoof(PlayerArea area)
    {
        if (roof == null)
        {
            Debug.LogWarning($"{nameof(House)} on '{name}' has no roof assigned.");
            return;
        }

        roof.SetActive(area == PlayerArea.Village);
    }

    public void DisableRoof()
    {
        if (roof == null)
        {
            Debug.LogWarning($"{nameof(House)} on '{name}' has no roof assigned.");
            return;
        }

        roof.SetActive(false);
    }
}
