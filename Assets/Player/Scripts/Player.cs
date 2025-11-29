using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] public PlayerHealth health;
    [SerializeField] public PlayerMovement movement;
    [SerializeField] public PlayerAttack attack;
    [SerializeField] public Collider2D playerCollider;
    [SerializeField] public EchoManager manager;
    [SerializeField] public List<Image> activeEchoIcons = new List<Image>();
    [SerializeField] public AudioManager audioManager;

    public List<EchoAbillity> activeEchoes = new List<EchoAbillity>();

    private void Awake()
    {
        attack.player = this;
        movement.player = this;
        health.player = this;
    }

    private void Start()
    {
        foreach (var echo in activeEchoes)
        {
            echo.OnEquip(this);
        }
    }

    public void BroadcastAttack()
    {
        foreach (var echo in activeEchoes)
        {
            echo.OnAttack(this);
        }
    }

    public void BroadcastHitEnemy(EnemyHealth enemy, ref DamageData damage)
    {
        foreach (var echo in activeEchoes)
        {
            echo.OnHitEnemy(this, ref damage);
        }

        print(damage.Resolve());
    }

    public void BroadcastTakeDamage(ref DamageData damage)
    {
        foreach (var echo in activeEchoes)
        {
            echo.OnTakeDamage(this, ref damage);
        }
    }

    public void BroadcastTakeSelfDamage(ref DamageData damage)
    {
        foreach (var echo in activeEchoes)
        {
            echo.OnTakeSelfDamage(this, ref damage);
        }
    }

    public void BroadcastDashStart()
    {
        foreach (var echo in activeEchoes)
        {
            echo.OnDashStart(this);
        }
    }
    public void BroadcastDashComplete()
    {
        foreach (var echo in activeEchoes)
        {
            echo.OnDashComplete(this);
        }
    }

    private void Update()
    {
        foreach (var echo in activeEchoes)
        {
            echo.OnUpdate(this);
        }
    }
}
