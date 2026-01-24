using UnityEngine;

public readonly struct DialogueOutcomeContext
{
    public readonly object Interactor;
    public DialogueOutcomeContext(object interactor) => Interactor = interactor;
}

public abstract class DialogueOutcomeAction : ScriptableObject
{
    public abstract void Execute(DialogueOutcomeContext ctx);
}
