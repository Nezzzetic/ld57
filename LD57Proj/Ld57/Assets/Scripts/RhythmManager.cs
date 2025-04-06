using System;
using System.Collections.Generic;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public List<List<float>> rhythmSeries = new List<List<float>>();
    private int currentSequenceIndex = 0;
    public List<float> currentBeats = new List<float>();
    private int hitCount = 0;
    private int nextExpectedBeat = 0;
    private float startTime = -1f;

    public float timingWindow = 0.25f; // allowed error

    public HoleManager holeManager;
    public RhythmTimeline rhythmTimeline;
    public AudioSource WinRound;
    public AudioSource ResetAudio;
    public AudioSource HitAudio;
    public AudioClip[] HitSound;
    public GameObject Tutor;
    public Animator Black;

    public void LoadRhythmSequence(List<float> beats)
    {
        currentBeats = beats;
        hitCount = 0;
        nextExpectedBeat = 0;
        startTime = -1f;
        Debug.Log($"Loading rhythm sequence {currentSequenceIndex}");
        OnStart?.Invoke(); // Timeline will rebuild on this
        rhythmTimeline?.ResetTimeline();
    }

    public List<float> GetCurrentBeats()
    {
        return currentBeats;
    }

    public void LoadNextRhythmInSeries()
    {
        
        currentSequenceIndex++;
        WinRound.Play();
        if (currentSequenceIndex >= rhythmSeries.Count)
        {
            Debug.Log("All rhythm sequences complete!");
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
            return;
        }

        Debug.Log($"Loading rhythm sequence {currentSequenceIndex}");
        LoadRhythmSequence(rhythmSeries[currentSequenceIndex]);
        ResetRhythm();
        holeManager?.ActivateNextHole();
        
    }


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
        return currentBeats.Count > 0 ? currentBeats[currentBeats.Count - 1] + 1.0f : 5f;
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
        if (nextExpectedBeat >= currentBeats.Count)
        {
            Debug.Log("All beats checked.");
            return false;
        }

        float expected = currentBeats[nextExpectedBeat];
        bool isHit = Mathf.Abs(echoTime - expected) <= timingWindow;

        if (isHit)
        {
            Debug.Log($"Hit! Expected {expected:F2}s, got {echoTime:F2}s");
            nextExpectedBeat++;
        }
        else
        {
            Debug.Log($"Miss. Expected {expected:F2}s, got {echoTime:F2}s");
        }

        
        return isHit;
    }


    public enum RhythmResult
    {
        Hit,
        Miss,
        Ignore,
        Win
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

    private RhythmResult EvaluateTiming(float echoTime)
    {
        if (nextExpectedBeat >= currentBeats.Count)
            return RhythmResult.Ignore;

        float expected = currentBeats[nextExpectedBeat];
        float findExpected = 10;
        foreach (var beat in currentBeats)
        {
            if (Mathf.Abs(echoTime - beat) < findExpected) findExpected = Mathf.Abs(echoTime - beat);
        }
        //bool isHit = Mathf.Abs(echoTime - expected) <= timingWindow;
        bool isHit = findExpected <= timingWindow;


        if (isHit)
        {
            nextExpectedBeat++;
            hitCount++;
            if (hitCount == currentBeats.Count)
            {
                Tutor.SetActive(false);
                LoadNextRhythmInSeries();
                return RhythmResult.Win;
            }
            HitAudio.clip = HitSound[hitCount-1];
            HitAudio.Play();
            return RhythmResult.Hit;
        }

        return RhythmResult.Miss;
    }



    void Update()
    {
        if (startTime < 0f || currentBeats.Count == 0)
            return;

        float maxAllowedTime = currentBeats[currentBeats.Count - 1] + timingWindow;
        float elapsed = Time.time - startTime;

        if (elapsed > maxAllowedTime)
        {
            Debug.Log("Rhythm sequence expired. Resetting holes.");
            ResetRhythm();
            ResetAudio.Play();

            Black.SetTrigger("Black");
        }


    }

    public void ResetRhythm()
    {
        
        startTime = -1f;
        nextExpectedBeat = 0;
        hitCount = 0;
        // Reset holes to Active (from HoleManager)
        if (holeManager != null)
            holeManager.ResetActivateAll();

        rhythmTimeline?.ResetTimeline();
    }

    public void StartTimer(float time)
    {
        if (startTime < 0f)
        {
            startTime = time;
            Debug.Log("Rhythm started at first drop.");
            OnStart?.Invoke(); // for timeline, etc.
        }
    }

    void Start()
    {
        rhythmSeries = new List<List<float>>()
    {
        new List<float> { 1.5f},
        new List<float> { 1.5f, 6.75f},
        new List<float> { 5f, 5.25f, 5.5f},
        new List<float> { 5f, 6f, 8.25f},
        new List<float> { 3f, 4.75f, 10.0f,12},
        new List<float> { 7, 7.25f, 7.5f, 7.75f }
    };

        LoadRhythmSequence(rhythmSeries[0]);
    }

}
