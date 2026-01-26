using TMPro;

using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;

    private PlayerStats stats;

    private void Awake()
    {
        if (goldText == null)
            goldText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        ResolveStats();
        if (stats != null)
            stats.StatChanged += OnStatChanged;

        Refresh();
    }

    private void OnDisable()
    {
        if (stats != null)
            stats.StatChanged -= OnStatChanged;
    }

    private void ResolveStats()
    {
        stats = PlayerStats.Instance;
        if (stats == null)
            stats = FindFirstObjectByType<PlayerStats>();
    }

    private void OnStatChanged(StatType type, int newValue)
    {
        if (type != StatType.Gold)
            return;

        Refresh();
    }

    private void Refresh()
    {
        if (goldText == null)
            return;

        int value = 0;
        if (stats != null)
            value = stats.Gold;

        goldText.text = value.ToString();
    }
}
