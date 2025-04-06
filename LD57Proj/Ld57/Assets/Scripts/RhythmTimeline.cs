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

    public GameObject rockIndicatorPrefab; // assign in inspector
    public RectTransform rockIndicatorContainer;
    public float rockSlideSpeed = 60f;
    public float rockStartOffsetX = -200f;
    public float rockEndX = 0f;

    private List<RockIndicator> rockIndicators = new List<RockIndicator>();

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

        foreach (var mark in rockIndicators)
        {
            if (mark != null)
                Destroy(mark.gameObject);
        }

        scrollingMarks.Clear();
        rockIndicators.Clear();
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

    public void ShowFallingRock(float fallTime, float delay)
    {
        var rockUI = Instantiate(rockIndicatorPrefab, rockIndicatorContainer);
        var rect = rockUI.GetComponent<RectTransform>();

        float startX = -10f;
        float endX = 0f;

        float totalTravelTime = delay;
        float pixelsPerSecond = Mathf.Abs(endX - startX) / totalTravelTime;
        Debug.Log("totalTravelTime "+ totalTravelTime+ " pixelsPerSecond " + pixelsPerSecond);

        var indicator = rockUI.AddComponent<RockIndicator>();
        indicator.speed = pixelsPerSecond;
        indicator.endX = endX;

        rect.anchoredPosition = new Vector2(startX, 0f); // align with beat bar
        rockIndicators.Add(indicator);
    }
}
