using UnityEngine;

public interface IHasTeleportPoints
{
    Transform EntryPoint { get; }
    Transform ExitPoint { get; }
}
