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

    }
}
