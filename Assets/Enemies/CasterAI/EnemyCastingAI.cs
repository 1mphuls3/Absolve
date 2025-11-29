using UnityEngine;

public class EnemyCastingAI : EnemyAI
{
    [SerializeField] public GameObject spell;
    protected override void Start()
    {
        ChangeState(new IdleWispState());
    }

    protected override void Update()
    {
        base.Update();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetPos);
    }
    protected override bool MoveTo(Vector2 pos, float speed)
    {
        return true;
    }

    protected override void FlipSprite()
    {
        Vector2 pos = player.position;
        if (transform.position.x < pos.x)
        {
            spriteRenderer.flipX = false;
            facingDir = Vector2.right;
        }
        else if (transform.position.x > pos.x)
        {
            spriteRenderer.flipX = true;
            facingDir = Vector2.left;
        }

        xPosLastFrame = transform.position.x;
    }
}
