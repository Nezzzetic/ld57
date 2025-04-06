using UnityEngine;
using UnityEngine.Audio;
using static RhythmManager;
using static UnityEngine.CullingGroup;

public class Hole : MonoBehaviour
{
    public float echoDelay = 1.0f;
    public RhythmManager rhythmManager;

    public Renderer markerRenderer;
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.blue;
    public Color flyingColor = Color.red;
    public Color successColor = Color.green;
    public Color failColor = new Color(1f, 0.65f, 0f); // orange-yellow

    private AudioSource audioSource;
    private HoleState state = HoleState.Inactive;
    public HoleState CurrentState => state;
    private Rock currentRock;

    private float PlayEchoTimer=-1;
    public RhythmTimeline rhythmTimeline;

    void Awake()
{
    audioSource = gameObject.AddComponent<AudioSource>();
    audioSource.playOnAwake = false;
        audioSource.volume = 0.3f;

    SetState(HoleState.Inactive);
    PlayEchoTimer = -1;
    FindObjectOfType<HoleManager>()?.RegisterHole(this);
}

    public event System.Action<HoleState> OnStateChanged;

    public void SetState(HoleState newState)
    {
        if (state == newState) return;

        state = newState;
        OnStateChanged?.Invoke(state); // notify listeners

        if (markerRenderer == null) return;

        switch (state)
        {
            case HoleState.Inactive:
                markerRenderer.material.color = inactiveColor;
                break;

            case HoleState.Active:
                markerRenderer.material.color = activeColor;
                PlayEchoTimer = -1;
                audioSource.Stop();
                if (currentRock != null) { Destroy(currentRock); currentRock = null; }
                break;

            case HoleState.FlyingStone:
                markerRenderer.material.color = flyingColor;
                audioSource.clip = currentRock.enterSound;
                float delay = echoDelay;
                float fallTime = Time.time;
                rhythmTimeline?.ShowFallingRock(fallTime, delay);
                audioSource.Play();
                break;

            case HoleState.Landed:
                if (currentRock != null) {
                    audioSource.clip = currentRock.echoSound;
                    audioSource.Play();
                }
                
                // Result color (green/yellow) applied after echo
                break;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (state != HoleState.Active) return;

        if (other.CompareTag("Rock"))
        {
            Rock rock = other.GetComponent<Rock>();
            if (rock != null)
                rock.SetState(RockState.EnteringHole);
            currentRock = rock;
            SetState(HoleState.FlyingStone);
            rhythmManager?.StartTimer(Time.time);
            PlayEchoTimer =echoDelay;
        }
    }

    void PlayEcho()
    {
        float echoTime = Time.time;

        RhythmResult result = rhythmManager.RegisterEchoWithResult(echoTime);

        if (result == RhythmResult.Hit)
        {
            markerRenderer.material.color = successColor;
        }
        else if (result == RhythmResult.Miss)
        {
            markerRenderer.material.color = failColor;
        }
        if (result != RhythmResult.Win)
            SetState(HoleState.Landed);
    }

    private void Update()
    {
        if (PlayEchoTimer <= 0) return;
        PlayEchoTimer -= Time.deltaTime;
        if (PlayEchoTimer <= 0)
        {
            PlayEcho();
        }
    }
}
