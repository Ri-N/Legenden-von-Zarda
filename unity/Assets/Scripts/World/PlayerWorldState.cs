using System;

using UnityEngine;

public sealed class PlayerWorldState : MonoBehaviour
{
    public static PlayerWorldState Instance { get; private set; }

    public PlayerArea CurrentArea { get; private set; } = PlayerArea.Room;

    public event Action<PlayerArea, PlayerAreaContext> AreaChangeRequested;

    public event Action<PlayerArea, PlayerAreaContext> AreaChanged;

    private Player player;

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

    private void Start()
    {
        if (player == null)
        {
            player = Player.Instance;
        }
    }

    public void RequestArea(PlayerArea target, object source = null)
    {
        var ctx = new PlayerAreaContext(source);

        AreaChangeRequested?.Invoke(target, ctx);

        ApplyTransition(target, ctx);

        CurrentArea = target;

        AreaChanged?.Invoke(CurrentArea, ctx);
    }

    private void ApplyTransition(PlayerArea target, PlayerAreaContext ctx)
    {
        if (player == null)
        {
            Debug.LogError("PlayerWorldState: Player reference is null.", this);
            return;
        }

        player.ApplyArea(target);

        if (ctx.Source is IHasTeleportPoints tp)
        {
            Transform destination = target switch
            {
                PlayerArea.Village => tp.ExitPoint,
                PlayerArea.Room => tp.EntryPoint,
                _ => null,
            };

            if (destination != null)
            {
                player.MoveTo(destination.position);
            }
        }
    }
}

/// <summary>
/// Context passed along with area changes.
/// </summary>
public readonly struct PlayerAreaContext
{
    public readonly object Source;

    public PlayerAreaContext(object source)
    {
        Source = source;
    }
}
