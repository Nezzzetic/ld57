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
    public RhythmManager RhythmManager;
    public GameObject top;
    public GameObject OpenFX;
    public GameObject WithRockFX;

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
                if (top != null) top.SetActive(true);
                break;

            case HoleState.Active:
                if (top!=null) top.SetActive(false);
                if (OpenFX != null) { OpenFX.SetActive(true);Destroy(OpenFX, 4); }
                if (WithRockFX != null) { WithRockFX.SetActive(false); }
                markerRenderer.material.color = activeColor;
                PlayEchoTimer = -1;
                audioSource.Stop();
                if (currentRock != null) { Destroy(currentRock.gameObject); currentRock = null; }
                break;

            case HoleState.FlyingStone:
                markerRenderer.material.color = flyingColor;
                audioSource.clip = currentRock.enterSound;
                audioSource.pitch = 4 - echoDelay / 2;
                if (WithRockFX != null) { WithRockFX.SetActive(true); }
                float delay = echoDelay;
                float fallTime = Time.time;
                rhythmTimeline?.ShowFallingRock(fallTime, delay);
                audioSource.Play();
                break;

            case HoleState.Landed:

                if (WithRockFX != null) { WithRockFX.SetActive(false); }
                // Result color (green/yellow) applied after echo
                break;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (state == HoleState.Inactive) return;
        if (state != HoleState.Active) { RhythmManager.ResetRhythm(); RhythmManager.ResetAudio.Play();
            RhythmManager.Black.SetTrigger("Black");
        }
        else
        {
            if (other.CompareTag("Rock"))
            {

                Rock rock = other.GetComponent<Rock>();
                if (rock != null)
                    rock.SetState(RockState.EnteringHole);
                currentRock = rock;
                SetState(HoleState.FlyingStone);
                rhythmManager?.StartTimer(Time.time);
                PlayEchoTimer = echoDelay;

            }
        }
    }

    void PlayEcho()
    {
        float echoTime = Time.time;

        RhythmResult result = rhythmManager.RegisterEchoWithResult(echoTime);

        if (result == RhythmResult.Hit)
        {
            markerRenderer.material.color = successColor;
            if (currentRock != null)
            {
                audioSource.clip = currentRock.echoSoundGood;
                audioSource.Play();
            }
        }
        else if (result == RhythmResult.Miss)
        {
            markerRenderer.material.color = failColor;
            if (currentRock != null)
            {
                audioSource.clip = currentRock.echoSound;
                audioSource.Play();
            }
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
