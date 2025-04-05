using UnityEngine;

public class Hole : MonoBehaviour
{
    public float echoDelay = 1.0f;
    public AudioClip echoSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = echoSound;
        audioSource.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rock"))
        {
            Invoke(nameof(PlayEcho), echoDelay);
        }
    }

    void PlayEcho()
    {
        audioSource.Play();
    }
}
