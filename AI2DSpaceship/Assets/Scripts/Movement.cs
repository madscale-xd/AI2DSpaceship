using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    public float maxSensingDistance = 5.0f;
    public float avoidDistanceFront = 1.0f;
    public float avoidDistanceSide = 3.0f;
    public float avoidanceTurnAngle_deg = 45.0f;
    public float incAngleMag_deg = 0.1f;

    GameObject hitObject = null;
    bool avoidObstacle = false;
    float turnAngle_deg = 0;
    float incAngle_deg = 0;

    float lockedIncAngle_deg = 0;
    bool directionLocked = false;
    public float directionLockDuration = 1.5f; // How long to keep the same direction after avoidance
    float timeSinceLastAvoid = 0f;

    public float rayOffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        AvoidObstacle();

        if (!avoidObstacle)
        {
            timeSinceLastAvoid += Time.deltaTime;

            if (directionLocked && timeSinceLastAvoid >= directionLockDuration)
            {
                directionLocked = false;
                lockedIncAngle_deg = 0f;
                timeSinceLastAvoid = 0f;
            }
        }
        else
        {
            // Reset timer while still avoiding
            timeSinceLastAvoid = 0f;
        }
    }

    private void AvoidObstacle()
{
    // Raycasting in 2D (only consider X-Y plane)
    Ray rayFrontRight = new Ray(transform.position + transform.right * 0.8f, transform.up); // Use up for forward direction in 2D
    Ray rayFrontLeft = new Ray(transform.position - transform.right * 1.8f, transform.up); // Same here

    bool isObstacleInFront = CheckRay(rayFrontRight, avoidDistanceFront) || CheckRay(rayFrontLeft, avoidDistanceFront);

    Vector3 initialDirection = transform.up; // In 2D, forward direction is up (Y-axis)

   if (isObstacleInFront && !avoidObstacle)
    {   
        Debug.Log("Obstacle detected!");
        avoidObstacle = true;

        // Only choose a new direction if not currently locked
        if (!directionLocked)
        {
            Ray rayRight = new Ray(transform.position, transform.right);
            Ray rayLeft = new Ray(transform.position, -transform.right);

            RaycastHit2D hitRight = Physics2D.Raycast(rayRight.origin, rayRight.direction, maxSensingDistance);
            RaycastHit2D hitLeft = Physics2D.Raycast(rayLeft.origin, rayLeft.direction, maxSensingDistance);

            float rightDist = hitRight.collider != null ? hitRight.distance : maxSensingDistance;
            float leftDist = hitLeft.collider != null ? hitLeft.distance : maxSensingDistance;

            if (rightDist < avoidDistanceSide && leftDist < avoidDistanceSide)
            {
                // Both sides blocked
                avoidObstacle = false;
            }
            else if (rightDist > leftDist)
            {
                lockedIncAngle_deg = incAngleMag_deg;
            }
            else if (leftDist > rightDist)
            {
                lockedIncAngle_deg = -incAngleMag_deg;
            }
            else
            {
                lockedIncAngle_deg = Random.value < 0.5f ? incAngleMag_deg : -incAngleMag_deg;
            }

            directionLocked = true;
        }

        incAngle_deg = lockedIncAngle_deg;
    }


    if (avoidObstacle)
    {
        turnAngle_deg += incAngle_deg;
        transform.Rotate(0, 0, incAngle_deg); // Rotate only around Z-axis in 2D

        if (Mathf.Abs(turnAngle_deg) >= Mathf.Abs(avoidanceTurnAngle_deg))
        {
            turnAngle_deg = 0;
            avoidObstacle = false;
        }
    }
    else
    {
        // Smooth rotation towards the initial direction
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0); // No Y-axis in 2D, rotation is around Z-axis
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);

        // Move in 2D space (only considering the X-Y plane)
        transform.Translate(transform.up * 9 * Time.deltaTime, Space.World); // Movement along direction the object is facing
    }
}


   private bool CheckRay(Ray ray, float avoidDistance)
    {
        // Add offset in both forward (up) and side (left/right) directions
        Vector3 offsetOrigin = ray.origin + transform.up * rayOffset + transform.right * 0.5f; // This adds a side offset (change 0.5f to spread farther apart)

        RaycastHit2D hit = Physics2D.Raycast(offsetOrigin, ray.direction, maxSensingDistance);
        bool avoidObstacle = false;

        if (hit.collider != null && hit.collider.gameObject != this.gameObject)
        {
            hitObject = hit.collider.gameObject;

            if (hitObject.GetComponent<Renderer>() != null)
                hitObject.GetComponent<Renderer>().material.color = Color.red;

            if (hit.distance < avoidDistance)
            {
                avoidObstacle = true;
                Debug.DrawLine(offsetOrigin, hit.point, Color.red);
            }
            else
            {
                Debug.DrawLine(offsetOrigin, hit.point, Color.yellow);
            }
        }
        else
        {
            if (hitObject != null && hitObject.GetComponent<Renderer>() != null)
                hitObject.GetComponent<Renderer>().material.color = Color.yellow;

            Debug.DrawLine(offsetOrigin, offsetOrigin + ray.direction * maxSensingDistance, Color.green);
        }

        return avoidObstacle;
    }
}