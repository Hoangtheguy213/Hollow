using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Enemy
{
    // Start is called before the first frame update
    [SerializeField] private float chaseDistance;
    float timer;
    [SerializeField] private float stunDuration;
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Ghost_Idle);
    }
    protected override void Update()
    {
        base.Update();
        if (!Player.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Ghost_Idle);
        }
    }
    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, Player.Instance.transform.position);
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Ghost_Idle:
                rb.velocity = new Vector2(0, 0);
                if (_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Ghost_Chase);   
                }
                break;
            case EnemyStates.Ghost_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, Player.Instance.transform.position, Time.deltaTime * speed));
                FlipGhost();
                if (_dist > chaseDistance)
                {
                    ChangeState(EnemyStates.Ghost_Idle);
                }
                break;
            case EnemyStates.Ghost_Stunned:
                timer += Time.deltaTime;
                if(timer > stunDuration)
                {
                    ChangeState(EnemyStates.Ghost_Idle);
                    timer = 0;
                }
                break;
            case EnemyStates.Ghost_Death:
                Death(Random.Range(1, 3));
                break;
        }
    }

    public override void EnemyHit(float _dameDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_dameDone, _hitDirection, _hitForce);
        if (health > 0)
        {
            ChangeState(EnemyStates.Ghost_Stunned);
        }
        else
        {
            ChangeState(EnemyStates.Ghost_Death);
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }
    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle",GetCurrentEnemyState==EnemyStates.Ghost_Idle);
        anim.SetBool("Chase",GetCurrentEnemyState==EnemyStates.Ghost_Chase);
        anim.SetBool("Stunned",GetCurrentEnemyState==EnemyStates.Ghost_Stunned);
        if (GetCurrentEnemyState == EnemyStates.Ghost_Death)
        {
            anim.SetTrigger("Death");
        }
    }
    void FlipGhost()
    {
        sr.flipX = Player.Instance.transform.position.x > transform.position.x;
    }
}
