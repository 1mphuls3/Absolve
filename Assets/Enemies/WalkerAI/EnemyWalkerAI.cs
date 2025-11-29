using UnityEngine;

public class EnemyWalkerAI : EnemyAI
{
    protected override void Start()
    {
        ChangeState(new PatrolState());
    }

    protected override void Update()
    {
        base.Update();

        MoveTo(targetPos, moveSpeed);

        if (rigidBody.linearVelocityX >= 0.04f || rigidBody.linearVelocityX <= -0.04f)
        {
            animator.SetBool("isWalking", true);
        }
        else if (animator.GetBool("isWalking"))
        {
            animator.SetBool("isWalking", false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetPos);
    }

    protected override bool MoveTo(Vector2 pos, float speed)
    {
        Vector3 relativePos = pos - rigidBody.position;
        relativePos.Normalize();
        float targetSpeed = speed * relativePos.x;
        rigidBody.linearVelocity = new Vector2(targetSpeed, rigidBody.linearVelocity.y);
        return Mathf.Abs(rigidBody.position.x - pos.x) < 0.2f;
    }
}
