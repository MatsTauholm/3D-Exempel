using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerShooting : MonoBehaviour
{

    void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            Shoot();
            Debug.Log("Firebutton pressed");
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if(Physics.Raycast(ray, out hit)) 
        {
            Debug.Log("Hit: " + hit.collider.name);
        }
    }
}
