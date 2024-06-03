using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hound : Enemy
{
    // Start is called before the first frame update
    
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgecheckY;
    [SerializeField] private float chargeSpeedMultiplier;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask whatIsGround;

    float Timer;

    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Hound_Idle);
        rb.gravityScale = 12f;

    }

    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }

        Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Hound_Idle:
               

                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgecheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }
                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStart, _wallCheckDir, ledgeCheckX * 10);
                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                    ChangeState(EnemyStates.Hound_Suprised);
                }
                if (transform.localScale.x > 0)
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                }
                break;
            case EnemyStates.Hound_Suprised:
                rb.velocity = new Vector2(0, jumpForce);
                //anim.SetBool("Suprised", GetCurrentEnemyState == EnemyStates.Hound_Suprised);
               ChangeState(EnemyStates.Hound_Charge);
                
                break;
            case EnemyStates.Hound_Charge:
                Timer += Time.deltaTime;
                if(Timer < chargeDuration)
                {
                    if (Physics2D.Raycast(transform.position, Vector2.down, ledgecheckY, whatIsGround))
                    {
                        if (transform.localScale.x > 0)
                        {
                            rb.velocity = new Vector2(speed * chargeSpeedMultiplier, rb.velocity.y);
                        }
                        else
                        {
                            rb.velocity = new Vector2(-speed * chargeSpeedMultiplier, rb.velocity.y);
                        }
                    }
                    else
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
                else
                {
                    Timer = 0;
                    ChangeState(EnemyStates.Hound_Idle);
                }
              
                break;
        }
       
    }
    
    protected override void ChangeCurrentAnimation()
    {
        if(GetCurrentEnemyState == EnemyStates.Hound_Idle)
        {
            anim.speed = 1;
            //anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Hound_Idle);
        }
        if(GetCurrentEnemyState == EnemyStates.Hound_Charge)
        {
            anim.speed = chargeSpeedMultiplier;
            //anim.SetBool("Charge", GetCurrentEnemyState == EnemyStates.Hound_Charge);
        }
    }


}