using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmTimeline : MonoBehaviour
{
    public RhythmManager rhythmManager;
    public RectTransform timelineFill; // Full bar
    public RectTransform currentLine;
    public GameObject expectedMarkPrefab;
    public GameObject actualMarkPrefab;

    private float totalDuration = 5f;
    private bool timelineStarted = false;
    private List<RectTransform> expectedMarks = new List<RectTransform>();

    void Start()
    {
        totalDuration = rhythmManager.GetDuration();

        // Spawn expected markers
        foreach (float beat in rhythmManager.targetBeats)
        {
            var mark = Instantiate(expectedMarkPrefab, timelineFill);
            float y =  (beat / totalDuration) * timelineFill.rect.height;
            mark.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
            expectedMarks.Add(mark.GetComponent<RectTransform>());
        }

        rhythmManager.OnEchoRegistered += AddActualMark;
        rhythmManager.OnStart += () => timelineStarted = true;
    }

    void Update()
    {
        if (!timelineStarted || rhythmManager.startTime < 0f) return;

        float elapsed = Time.time - rhythmManager.startTime;
        float y = Mathf.Clamp01(elapsed / totalDuration) * timelineFill.rect.height;

        //currentLine.anchoredPosition = new Vector2(0, y);
    }

    void AddActualMark(float songTime)
    {
        var mark = Instantiate(actualMarkPrefab, timelineFill);
        float y = (songTime / totalDuration) * timelineFill.rect.height;
        mark.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
    }
}
