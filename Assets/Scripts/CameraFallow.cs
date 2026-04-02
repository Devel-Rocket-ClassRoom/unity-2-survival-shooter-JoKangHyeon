using UnityEngine;

public class CameraFallow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    public float speed = 1f;
    public float deadZoneSpeed = 4f;
    public float softZone = 1f;
    public float deadZone = 2f;

    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        float distanceToTarget = Vector3.Distance(targetPosition, transform.position);

        //Debug.Log(distanceToTarget);
        if (distanceToTarget > softZone)
        {
            if (distanceToTarget > deadZone)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            }
        }
    }
}
