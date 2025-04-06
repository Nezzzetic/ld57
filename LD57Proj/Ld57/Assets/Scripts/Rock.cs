using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rock : MonoBehaviour
{
    public RockState CurrentState { get; private set; } = RockState.Idle;
    private Rigidbody rb;
    public Collider Mycollider;
    public AudioClip echoSound;
    public AudioClip echoSoundGood;
    public AudioClip enterSound;

    public event Action<RockState> OnStateChanged;

    private Vector3 targetPosition;
    public float followSpeed = 20f; // how fast it snaps into position

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (CurrentState == RockState.Held)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        }
    }

    public void SetState(RockState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState); // notify listeners

        switch (CurrentState)
        {
            case RockState.Held:
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                Mycollider.enabled= false;
                break;

            case RockState.Falling:
            case RockState.Idle:
                rb.useGravity = true;
                Mycollider.enabled = true;
                break;
        }
    }

    public void UpdateHeldPosition(Vector3 target)
    {
        if (CurrentState == RockState.Held)
        {
            targetPosition = target;
        }
    }
}
