using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownCameraRotate : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.qKey.isPressed)
        {
            transform.DORotate(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90, 0.0f), 0.5f)
            .SetEase(Ease.OutBounce);
        }

        if (Keyboard.current.eKey.isPressed)
        {
            transform.DORotate(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 90, 0.0f), 0.5f)
            .SetEase(Ease.OutBounce);
        }
    }
}
