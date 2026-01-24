using UnityEngine;

public readonly struct DialogueOutcomeContext
{
    public readonly object Source;

    public DialogueOutcomeContext(object source)
    {
        Source = source;
    }
}

public abstract class DialogueOutcomeAction : ScriptableObject
{
    public abstract void Execute(DialogueOutcomeContext ctx);
}
