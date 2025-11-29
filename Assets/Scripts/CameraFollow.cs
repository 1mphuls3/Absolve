using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform followTarget;
    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float speedMultiplier = 0.5f;
    [SerializeField] private float offsetSmoothTime = 0.2f;

    [SerializeField] public bool isCameraShake = false;
    [SerializeField] private float cameraShakeRadius = 0.5f;
    [SerializeField] public float cameraShakeSpeed = 0.2f;

    private float currentOffsetX;
    private float targetOffsetX;

    void Start()
    {
        currentOffsetX = offset.x;
    }

    void LateUpdate()
    {
        bool facingLeft = followTarget
            .GetComponent<PlayerMovement>().spriteRenderer.flipX;

        // Flip target offset based on facing direction
        targetOffsetX = facingLeft ? -offset.x : offset.x;

        // Smoothly interpolate between current and target
        currentOffsetX = Mathf.Lerp(currentOffsetX, targetOffsetX,Time.deltaTime / offsetSmoothTime);

        Vector3 targetPos = new Vector3(
            followTarget.position.x + currentOffsetX,
            followTarget.position.y + offset.y,
            offset.z);

        float distance = Vector3.Distance(transform.position, targetPos);
        float dynamicSpeed = baseSpeed + distance * speedMultiplier;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, dynamicSpeed * Time.deltaTime);

        if(isCameraShake) CameraShake(cameraShakeRadius, cameraShakeSpeed);
    }

    public IEnumerator ShakeCamera(float time, float speed)
    {
        cameraShakeSpeed = speed;
        isCameraShake = true;
        yield return new WaitForSeconds(time);
        isCameraShake = false;
    }

    public void CameraShake(float radius, float speed)
    {
        float angle = Mathf.Deg2Rad * Random.Range(0, 360);
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);

        Vector3 targetPos = transform.position + new Vector3(x, y);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }
}
