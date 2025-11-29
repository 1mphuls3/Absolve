using System.Collections;
using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rigidBody;
    [SerializeField] public Transform player;
    [SerializeField] public Collider2D attackHitbox;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public Animator animator;
    protected IEnemyState currentState;
    public Vector2 targetPos;
    public float moveSpeed;

    protected float xPosLastFrame;
    public Vector2 facingDir;

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        currentState.Update(this);

        FlipSprite();
    }
    protected abstract bool MoveTo(Vector2 pos, float speed);
    public void ChangeState(IEnemyState state)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = state;
        currentState.Enter(this);
    }
    protected virtual void FlipSprite()
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

        xPosLastFrame = transform.position.x;
    }
    protected Vector2 GetOppositeDir(Vector2 dir)
    {
        return dir == Vector2.left ? Vector2.right : Vector2.left;
    }
}
