using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 1f;
    public Vector3 offset = new Vector3(0, 5, -10);

    private float defaultSmoothSpeed;
    private float timer = 0f;
    private float initialSmoothSpeed = 1f;
    private float delayDuration = 1f;

    void Start()
    {
        defaultSmoothSpeed = smoothSpeed;
        smoothSpeed = initialSmoothSpeed;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Update timer and reset smooth speed after delay
        if (timer < delayDuration)
        {
            timer += Time.deltaTime;
            if (timer >= delayDuration)
            {
                smoothSpeed = defaultSmoothSpeed;
            }
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
