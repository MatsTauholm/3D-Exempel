using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCameraRotate : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            transform.DORotate(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90, 0.0f), 0.5f)
            .SetEase(Ease.OutBounce);
        }

        if (Input.GetKeyDown("q"))
        {
            transform.DORotate(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 90, 0.0f), 0.5f)
            .SetEase(Ease.OutBounce);
        }
    }

    private Vector3 SnappedVector()
    {
        var endValue = 0.0f;
        var currentY = Mathf.Ceil(transform.rotation.eulerAngles.y);

        endValue = currentY switch
        {
            >= 0 and <= 90 => 45.0f,
            >= 91 and <= 180 => 135.0f,
            >= 181 and <= 270 => 225.0f,
            _ => 315.0f
        };

        return new Vector3(transform.rotation.eulerAngles.x, endValue, 0.0f);
    }
}
