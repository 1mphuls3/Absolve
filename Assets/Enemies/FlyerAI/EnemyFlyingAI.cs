using UnityEngine;

public class EnemyFlyingAI : EnemyAI
{
    [SerializeField] public AnimationCurve curve;
    [SerializeField] public float moveAcceleration = 10f;
    protected override void Start()
    {
        ChangeState(new PerchState());
    }

    protected override void Update()
    {
        base.Update();

        MoveTo(targetPos, moveSpeed);
    }
   
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetPos);
    }
    protected override bool MoveTo(Vector2 pos, float speed)
    {
        Vector2 direction = (pos - rigidBody.position).normalized;
        Vector2 targetSpeed = direction * speed;

        float acceleration = moveAcceleration;
        rigidBody.linearVelocity = Vector2.MoveTowards(
            rigidBody.linearVelocity,
            targetSpeed,
            acceleration * Time.fixedDeltaTime
        );

        return Vector2.Distance(rigidBody.position, pos) < 0.2f;  
    }

    protected override void FlipSprite()
    {
        if (transform.position.x > xPosLastFrame)
        {
            spriteRenderer.flipX = false;
            facingDir = Vector2.right;
        }
        else if (transform.position.x < xPosLastFrame)
        {
            spriteRenderer.flipX = true;
            facingDir = Vector2.left;
        }

        if (animator.GetBool("isAttacking")) return;

        xPosLastFrame = transform.position.x;
    }
}
