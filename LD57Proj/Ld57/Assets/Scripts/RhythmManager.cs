using System.Collections.Generic;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public List<float> targetBeats = new List<float>(); // seconds from first echo
    public float timingWindow = 0.25f; // allowed error

    public float startTime = -1f;
    private int nextExpectedBeat = 0;

    //public void RegisterEcho(float worldTime)
    //{
    //    float songTime;

    //    if (startTime < 0f)
    //    {
    //        startTime = worldTime;
    //        songTime = 0f;
    //        Debug.Log("Timer started!");
    //    }
    //    else
    //    {
    //        songTime = worldTime - startTime;
    //    }

    //    Debug.Log($"Echo at {songTime:F2}s");
    //    CheckTiming(songTime);
    //}

    //void CheckTiming(float echoTime)
    //{
    //    if (nextExpectedBeat >= targetBeats.Count)
    //    {
    //        Debug.Log("All beats checked.");
    //        return;
    //    }

    //    float expected = targetBeats[nextExpectedBeat];
    //    if (Mathf.Abs(echoTime - expected) <= timingWindow)
    //    {
    //        Debug.Log($"Hit! Expected {expected:F2}s, got {echoTime:F2}s");
    //        nextExpectedBeat++;
    //    }
    //    else if (echoTime < expected - timingWindow)
    //    {
    //        Debug.Log("Too early.");
    //    }
    //    else
    //    {
    //        Debug.Log($"Miss. Expected {expected:F2}s, got {echoTime:F2}s");
    //        nextExpectedBeat++;
    //    }
    //}

    public event System.Action<float> OnEchoRegistered;
    public event System.Action OnStart;

    public float GetDuration()
    {
        return targetBeats.Count > 0 ? targetBeats[targetBeats.Count - 1] + 1.0f : 5f;
    }

    public void RegisterEcho(float worldTime)
    {
        float songTime;

        if (startTime < 0f)
        {
            startTime = worldTime;
            songTime = 0f;
            Debug.Log("Timer started!");
            OnStart?.Invoke();
        }
        else
        {
            songTime = worldTime - startTime;
        }

        OnEchoRegistered?.Invoke(songTime);
        CheckTiming(songTime);
    }


    public bool RegisterEchoWithFeedback(float worldTime)
    {
        float songTime;

        if (startTime < 0f)
        {
            startTime = worldTime;
            songTime = 0f;
            Debug.Log("Timer started!");
            OnStart?.Invoke();
        }
        else
        {
            songTime = worldTime - startTime;
        }

        OnEchoRegistered?.Invoke(songTime);

        return CheckTiming(songTime);
    }

    private bool CheckTiming(float echoTime)
    {
        if (nextExpectedBeat >= targetBeats.Count)
        {
            Debug.Log("All beats checked.");
            return false;
        }

        float expected = targetBeats[nextExpectedBeat];
        bool isHit = Mathf.Abs(echoTime - expected) <= timingWindow;

        if (isHit)
        {
            Debug.Log($"Hit! Expected {expected:F2}s, got {echoTime:F2}s");
        }
        else
        {
            Debug.Log($"Miss. Expected {expected:F2}s, got {echoTime:F2}s");
        }

        nextExpectedBeat++;
        return isHit;
    }


    public enum RhythmResult
    {
        Hit,
        Miss,
        Ignore
    }

    public RhythmResult RegisterEchoWithResult(float worldTime)
    {
        float songTime;

        if (startTime < 0f)
        {
            startTime = worldTime;
            songTime = 0f;
            Debug.Log("Timer started!");
            OnStart?.Invoke();
        }
        else
        {
            songTime = worldTime - startTime;
        }

        OnEchoRegistered?.Invoke(songTime);
        return EvaluateTiming(songTime);
    }

    public  RhythmResult EvaluateTiming(float echoTime)
    {
        if (nextExpectedBeat >= targetBeats.Count)
        {
            Debug.Log("All beats checked.");
            return RhythmResult.Ignore;
        }

        float expected = targetBeats[nextExpectedBeat];
        bool isHit = Mathf.Abs(echoTime - expected) <= timingWindow;

        if (isHit)
        {
            Debug.Log($"Hit! Expected {expected:F2}s, got {echoTime:F2}s");
            nextExpectedBeat++;
            return RhythmResult.Hit;
        }
        else
        {
            Debug.Log($"Miss. Expected {expected:F2}s, got {echoTime:F2}s");
            nextExpectedBeat++;
            return RhythmResult.Miss;
        }
    }

}
