using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    public List<Hole> allHoles = new List<Hole>();
    public int currentHoleIndex = 1;

    [SerializeField]
    private List<Hole> orderedHoles;



    void Start()
    {
        RegisterAllHoles(); // optional
        ActivateHoleToIndex(currentHoleIndex);
    }

    public void ActivateHoleAtIndex(int index)
    {
        ResetAll(); // deactivate everything

        if (index >= 0 && index < orderedHoles.Count)
        {
            orderedHoles[index].SetState(HoleState.Active);
        }
    }

    public void ActivateHoleToIndex(int index)
    {
        ResetAll(); // deactivate everything
        for (int i = 0; i < index+1; i++)
        {
            if (i < allHoles.Count)
            {
                allHoles[i].SetState(HoleState.Active);
            }
        }
        
    }

    public void RegisterHole(Hole hole)
    {
        if (!allHoles.Contains(hole))
        {
            //allHoles.Add(hole);
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

    public void ResetActivateAll()
    {
        foreach (var hole in allHoles)
        {
            if (hole.CurrentState!=HoleState.Inactive)
                hole.SetState(HoleState.Active);
        }
    }

    private void OnHoleStateChanged(HoleState newState)
    {

    }

    public void ActivateNextHole()
    {
        if (currentHoleIndex >= orderedHoles.Count)
        {
            Debug.Log("All holes activated.");
            return;
        }
        currentHoleIndex++;
        var hole = orderedHoles[currentHoleIndex];

        if (hole.CurrentState == HoleState.Inactive)
        {
            hole.SetState(HoleState.Active);
            
        }
        else
        {
            Debug.LogWarning($"Hole {currentHoleIndex} is already activated or used.");
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
        //allHoles.Clear();
        //allHoles.AddRange(FindObjectsOfType<Hole>());

        foreach (var hole in allHoles)
        {
            hole.OnStateChanged += OnHoleStateChanged;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) ) ResetActivateAll();
    }
}
