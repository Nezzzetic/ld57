using UnityEngine;

public class Hole : MonoBehaviour
{
    public float echoDelay = 1.0f;
    public AudioClip echoSound;
    public RhythmManager rhythmManager;

    public Renderer markerRenderer;
    public Color defaultColor = Color.gray;
    public Color usedColor = Color.red;
    public Color successColor = Color.green;
    public Color failColor = new Color(1f, 0.65f, 0f); // orange-yellow

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = echoSound;
        audioSource.playOnAwake = false;

        if (markerRenderer != null)
            markerRenderer.material.color = defaultColor;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rock"))
        {
            if (markerRenderer != null)
                markerRenderer.material.color = usedColor;

            Invoke(nameof(PlayEcho), echoDelay);
        }
    }

    void PlayEcho()
    {
        audioSource.Play();
        float echoTime = Time.time;

        // Notify RhythmManager and get feedback
        RhythmManager.RhythmResult result = rhythmManager.RegisterEchoWithResult(echoTime);

        if (markerRenderer == null) return;

        if (result == RhythmManager.RhythmResult.Hit)
            markerRenderer.material.color = successColor;
        else if (result == RhythmManager.RhythmResult.Miss)
            markerRenderer.material.color = failColor;
    }
}
