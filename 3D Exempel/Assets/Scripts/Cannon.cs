using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] GameObject cannonballPrefab;
    [SerializeField] Transform firePoint;

    [SerializeField] float shootForce = 20f;
    [SerializeField] float fireRate = 1f; // shots per second

    private float nextFireTime = 0f;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void Shoot()
    {
        GameObject cannonball = Instantiate(cannonballPrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = cannonball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = firePoint.up * shootForce;
        }
    }
}