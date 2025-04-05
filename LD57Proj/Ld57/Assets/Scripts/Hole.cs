using UnityEngine;
using static RhythmManager;
using static UnityEngine.CullingGroup;

public class Hole : MonoBehaviour
{
    public float echoDelay = 1.0f;
    public AudioClip echoSound;
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


    void Start()
{
    audioSource = gameObject.AddComponent<AudioSource>();
    audioSource.clip = echoSound;
    audioSource.playOnAwake = false;

    SetState(HoleState.Active);

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
                break;

            case HoleState.FlyingStone:
                markerRenderer.material.color = flyingColor;
                break;

            case HoleState.Landed:
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

            SetState(HoleState.FlyingStone);

            Invoke(nameof(PlayEcho), echoDelay);
        }
    }

    void PlayEcho()
    {
        audioSource.Play();
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

        SetState(HoleState.Landed);
    }
}
