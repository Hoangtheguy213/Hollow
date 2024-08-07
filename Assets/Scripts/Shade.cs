using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shade : Enemy
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;
    float timer;

    public static Shade Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject );
        }
        else
        {
            Instance = this;
        }
        SaveData.Instance.SaveShadeData();
    }
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Shade_Idle);
    }
    protected override void Update()
    {
        base.Update();
        if (!Player.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Shade_Idle);
        }
    }
    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, Player.Instance.transform.position);
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Shade_Idle:
                //rb.velocity = new Vector2(0, 0);
                if (_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Shade_Chase);
                }
                break;
            case EnemyStates.Shade_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, Player.Instance.transform.position, Time.deltaTime * speed));
                FlipShade();
                if (_dist > chaseDistance)
                {
                    ChangeState(EnemyStates.Shade_Idle);
                }
                break;
            case EnemyStates.Shade_Stunned:
                timer += Time.deltaTime;
                if (timer > stunDuration)
                {
                    ChangeState(EnemyStates.Shade_Idle);
                    timer = 0;
                }
                break;
            case EnemyStates.Shade_Death:
                Death(Random.Range(1, 3));
                break;
        }
    }

    public override void EnemyHit(float _dameDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_dameDone, _hitDirection, _hitForce);
        if (health > 0)
        {
            ChangeState(EnemyStates.Shade_Stunned);
        }
        else
        {
            ChangeState(EnemyStates.Shade_Death);
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }
    protected override void ChangeCurrentAnimation()
    {
        if(GetCurrentEnemyState == EnemyStates.Shade_Idle)
        {
            anim.Play("Idle");
        }
        anim.SetBool("Walking", GetCurrentEnemyState == EnemyStates.Shade_Chase);
        if (GetCurrentEnemyState == EnemyStates.Shade_Death)
        {
            Player.Instance.RestoreMana();
            SaveData.Instance.SavePlayerData();
            anim.SetTrigger("Death");
            Destroy(gameObject, 0.5f);
        }
    }
    protected override void Attack()
    {
        anim.SetTrigger("Attacking");
        Player.Instance.TakeDamage(damage);

    }
    void FlipShade()
    {
        sr.flipX = Player.Instance.transform.position.x < transform.position.x;
    }
}
