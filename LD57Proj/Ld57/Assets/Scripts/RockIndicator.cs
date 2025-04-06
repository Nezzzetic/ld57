using UnityEngine;

public class RockIndicator : MonoBehaviour
{
    public float speed = 60f;
    public float endX = 0f;
    public float delta = 0.1f;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector2 pos = rect.anchoredPosition;
        pos.x = Mathf.MoveTowards(pos.x, endX, Time.deltaTime * speed);
        rect.anchoredPosition = pos;
        if (endX-pos.x  < delta) Destroy(gameObject);
    }
}
