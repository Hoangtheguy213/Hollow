using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FireMan : Enemy
{
    // Start is called before the first frame update
    [SerializeField] private float flipWaitTime;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgecheckY;
    [SerializeField] private LayerMask whatIsGround;

    float Timer;
  
    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
        
    }
    // Update is called once per frame
    //protected override void Update()
    //{
    //    base.Update ();
    //    if (!isRecoiling)
    //    {
    //        transform.position = Vector2.MoveTowards(transform.position, new Vector2
    //            (Player.Instance.transform.position.x, transform.position.y),speed* Time.deltaTime);
    //    }
    //}
    protected override void UpdateEnemyStates()
    {
        if(health <= 0)
        {
            Death(0.05f);
        }
        switch(GetCurrentEnemyState)
        {
            case EnemyStates.FireMan_Idle:
                Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
                Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgecheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    ChangeState(EnemyStates.FireMan_Flip);
                }
                if (transform.localScale.x>0)
                {
                    rb.velocity = new Vector2( speed, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2 ( -speed, rb.velocity.y); 
                }
                break;
            case EnemyStates.FireMan_Flip:
                Timer += Time.deltaTime;
                if(Timer > flipWaitTime)
                {
                    Timer = 0;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    ChangeState(EnemyStates.FireMan_Idle);
                }
                break;
        }
    }
}
