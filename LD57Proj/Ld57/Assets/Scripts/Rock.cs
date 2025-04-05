using UnityEngine;

public class Rock : MonoBehaviour
{
    private bool isHeld = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isHeld)
        {
            // Follow mouse in world space
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f; // Distance from camera
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = new Vector3(worldPos.x, 2f, worldPos.z); // Float above ground
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!isHeld)
            {
                TryPickUp();
            }
            else
            {
                Drop();
            }
        }
    }

    void TryPickUp()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                isHeld = true;
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
            }
        }
    }

    void Drop()
    {
        isHeld = false;
        rb.useGravity = true;
    }
}
