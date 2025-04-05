using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    public List<Hole> allHoles = new List<Hole>();
    private int currentHoleIndex = 0;

    [SerializeField]
    private List<Hole> orderedHoles = new List<Hole>(); // drag in Unity in order


    void Start()
    {
        RegisterAllHoles(); // optional
        ActivateHoleAtIndex(currentHoleIndex);
    }

    public void ActivateHoleAtIndex(int index)
    {
        ResetAll(); // deactivate everything

        if (index >= 0 && index < orderedHoles.Count)
        {
            orderedHoles[index].SetState(HoleState.Active);
        }
    }

    public void RegisterHole(Hole hole)
    {
        if (!allHoles.Contains(hole))
        {
            allHoles.Add(hole);
            hole.OnStateChanged += OnHoleStateChanged;
        }
    }

    public void UnregisterHole(Hole hole)
    {
        if (allHoles.Contains(hole))
        {
            allHoles.Remove(hole);
            hole.OnStateChanged -= OnHoleStateChanged;
        }
    }

    public void ActivateHoles(List<Hole> holesToActivate)
    {
        foreach (var hole in allHoles)
        {
            hole.SetState(HoleState.Inactive);
        }

        foreach (var hole in holesToActivate)
        {
            hole.SetState(HoleState.Active);
        }
    }

    public void ResetAll()
    {
        foreach (var hole in allHoles)
        {
            hole.SetState(HoleState.Inactive);
        }
    }

    private void OnHoleStateChanged(HoleState newState)
    {
        if (newState == HoleState.Landed)
        {
            if (AllCurrentHolesFinished())
            {
                AdvanceToNextHole();
            }
        }
    }

    void AdvanceToNextHole()
    {
        currentHoleIndex++;
        if (currentHoleIndex < orderedHoles.Count)
        {
            ActivateHoleAtIndex(currentHoleIndex);
        }
        else
        {
            Debug.Log("All holes completed!");
        }
    }

    bool AllCurrentHolesFinished()
    {
        Hole current = orderedHoles[currentHoleIndex];
        return current.CurrentState == HoleState.Landed;
    }


    public bool AllHolesFinished()
    {
        foreach (var hole in allHoles)
        {
            if (hole.CurrentState == HoleState.Active || hole.CurrentState == HoleState.FlyingStone)
                return false;
        }
        return true;
    }

    public void RegisterAllHoles()
    {
        allHoles.Clear();
        allHoles.AddRange(FindObjectsOfType<Hole>());

        foreach (var hole in allHoles)
        {
            hole.OnStateChanged += OnHoleStateChanged;
        }
    }
}
