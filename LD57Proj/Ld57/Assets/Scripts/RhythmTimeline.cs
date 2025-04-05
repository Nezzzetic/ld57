using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        // Preload all expected markers and position them statically
        foreach (float beatTime in rhythmManager.targetBeats)
        {
            GameObject marker = Instantiate(expectedMarkPrefab, timelineFill);
            var rect = marker.GetComponent<RectTransform>();

            float y = beatTime * pixelsPerSecond;
            rect.anchoredPosition = new Vector2(0, y);

            var scrolling = marker.AddComponent<ScrollingMark>();
            scrolling.spawnTime = beatTime;
            scrolling.pixelsPerSecond = pixelsPerSecond;
            scrolling.originalY = y;
            scrollingMarks.Add(scrolling);
        }

        rhythmManager.OnStart += () =>
        {
            timelineStarted = true;
            startTime = Time.time;
        };

        rhythmManager.OnEchoRegistered += AddActualMarker;
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

    void AddActualMarker(float songTime)
    {
        GameObject marker = Instantiate(actualMarkPrefab, timelineFill);
        var rect = marker.GetComponent<RectTransform>();

        float y = songTime * pixelsPerSecond;
        rect.anchoredPosition = new Vector2(0, y);

        var scrolling = marker.AddComponent<ScrollingMark>();
        scrolling.spawnTime = songTime;
        scrolling.originalY = y;
        scrolling.pixelsPerSecond = pixelsPerSecond;
        scrollingMarks.Add(scrolling);
    }
}
