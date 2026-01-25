using UnityEngine;

public static class InteractionConstraintUtil
{
    public static bool IsAreaAllowed(PlayerArea[] allowedAreas, PlayerArea current)
    {
        // Null/empty means "allowed everywhere".
        if (allowedAreas == null || allowedAreas.Length == 0)
        {
            return true;
        }

        for (int i = 0; i < allowedAreas.Length; i++)
        {
            if (allowedAreas[i] == current)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsSideAllowed(InteractionSide allowedSide, Transform reference, Vector3 playerWorldPos)
    {
        if (allowedSide == InteractionSide.Both)
        {
            return true;
        }

        if (reference == null)
        {
            return false;
        }

        float dot = Vector3.Dot(reference.forward, playerWorldPos - reference.position);
        bool isFront = dot >= 0f;

        return allowedSide == InteractionSide.FrontOnly ? isFront : !isFront;
    }
}
