using UnityEngine;

public class RockController : MonoBehaviour
{
    private Rock heldRock = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldRock == null)
            {
                TryPickUpRock();
            }
            else
            {
                ReleaseHeldRock();
            }
        }

        if (heldRock != null)
        {
            Vector3 mouseWorldPos = GetMouseWorldPositionOnGround();
            heldRock.UpdateHeldPosition(mouseWorldPos + Vector3.up * 0.5f);
        }
    }

    void TryPickUpRock()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Rock rock = hit.collider.GetComponent<Rock>();
            if (rock != null && rock.CurrentState!=RockState.EnteringHole)
            {
                heldRock = rock;
                heldRock.SetState(RockState.Held);
            }
        }
    }

    void ReleaseHeldRock()
    {
        heldRock.SetState(RockState.Falling);
        heldRock = null;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    Vector3 GetMouseWorldPositionOnGround()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // ground at Y = 0

        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

}
