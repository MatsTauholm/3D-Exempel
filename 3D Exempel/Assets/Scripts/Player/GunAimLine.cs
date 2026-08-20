using UnityEngine;
using UnityEngine.InputSystem;

public class GunAimLine : MonoBehaviour
{
    [SerializeField] private Transform muzzle;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private LayerMask aimSurfaceLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [SerializeField] private float maxAimDistance = 20f;

    private void Update()
    {
        Vector3 aimPoint = GetMouseAimPoint();

        Vector3 direction = aimPoint - muzzle.position; //Get the direction from the muzzle to the aim point

        // Ignore vertical difference
        direction.y = 0f;
        direction.Normalize();

        // Start with the maximum possible distance
        Vector3 endPoint = muzzle.position + direction * maxAimDistance;

        // Check for walls, enemies, obstacles, etc.
        if (Physics.Raycast(
            muzzle.position,
            direction,
            out RaycastHit hit,
            maxAimDistance,
            obstacleLayer))
        {
            endPoint = hit.point; // If we hit something, set the end point to the hit point
        }

        lineRenderer.SetPosition(0, muzzle.position);
        lineRenderer.SetPosition(1, endPoint);
    }

    private Vector3 GetMouseAimPoint()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return muzzle.position + transform.forward * maxAimDistance;
    }
}