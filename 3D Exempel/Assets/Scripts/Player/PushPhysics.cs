using UnityEngine;

public class PushPhysics : MonoBehaviour
{
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb != null && !rb.isKinematic)
        {
            Vector3 pushDir = hit.moveDirection;
            pushDir.y = 0; // don't push up/down

            rb.linearVelocity = pushDir * 5f; // or use AddForce for more realism
        }
    }
}
