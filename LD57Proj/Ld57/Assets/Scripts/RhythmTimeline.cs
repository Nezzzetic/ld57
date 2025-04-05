using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RhythmTimeline : MonoBehaviour
{
    public RhythmManager rhythmManager;
    public RectTransform timelineFill;
    public GameObject expectedMarkPrefab;
    public GameObject actualMarkPrefab;
    public float pixelsPerSecond = 60f;

    private bool timelineStarted = false;
    private float startTime = 0f;

    private List<ScrollingMark> scrollingMarks = new List<ScrollingMark>();

    void Start()
    {
        rhythmManager.OnStart += () =>
        {
            timelineStarted = true;
            startTime = Time.time;
        };

        rhythmManager.OnEchoRegistered += AddActualMark;

        BuildInitialTimeline();
    }

    public void ResetTimeline()
    {
        // Destroy all current markers
        foreach (var mark in scrollingMarks)
        {
            if (mark != null)
                Destroy(mark.gameObject);
        }

        scrollingMarks.Clear();
        BuildInitialTimeline();

        timelineStarted = false;
        startTime = -1f;
    }

    private void BuildInitialTimeline()
    {
        foreach (float beat in rhythmManager.targetBeats)
        {
            var marker = Instantiate(expectedMarkPrefab, timelineFill);
            var rect = marker.GetComponent<RectTransform>();
            float y = beat * pixelsPerSecond;
            rect.anchoredPosition = new Vector2(0, y);

            var scroll = marker.AddComponent<ScrollingMark>();
            scroll.spawnTime = beat;
            scroll.pixelsPerSecond = pixelsPerSecond;
            scroll.originalY = y;

            scrollingMarks.Add(scroll);
        }
    }

    void Update()
    {
        if (!timelineStarted) return;

        float songTime = Time.time - startTime;

        foreach (var mark in scrollingMarks)
        {
            float y = mark.originalY - (songTime * pixelsPerSecond);
            mark.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
        }
    }

    private void AddActualMark(float songTime)
    {
        var marker = Instantiate(actualMarkPrefab, timelineFill);
        var rect = marker.GetComponent<RectTransform>();
        float y = songTime * pixelsPerSecond;
        rect.anchoredPosition = new Vector2(0, y);

        var scroll = marker.AddComponent<ScrollingMark>();
        scroll.spawnTime = songTime;
        scroll.originalY = y;
        scroll.pixelsPerSecond = pixelsPerSecond;

        scrollingMarks.Add(scroll);
    }
}
