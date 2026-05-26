using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}