using UnityEngine;

public class DropZone : MonoBehaviour
{
    public Hole linkedHole;

    public bool CanAcceptDrop()
    {
        return linkedHole != null && linkedHole.CurrentState == HoleState.Active;
    }
}
