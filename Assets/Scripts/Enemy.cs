using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected float speed;

    [SerializeField] protected float damage;
    [SerializeField] protected GameObject greenBlood;

    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;
    protected enum EnemyStates
    {
        //fireman
        FireMan_Idle,
        FireMan_Flip,

        //ghost
        Ghost_Idle,
        Ghost_Chase,
        Ghost_Stunned,
        Ghost_Death,

        //Hound
        Hound_Idle,
        Hound_Suprised,
        Hound_Charge
    }
    protected EnemyStates currentEnemyState;

    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return currentEnemyState; }
        set
        {
            if(currentEnemyState != value)
            {
                currentEnemyState = value;
                ChangeCurrentAnimation();
            }
        }
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        
       
        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        else
        {
            UpdateEnemyStates();
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            GameObject _GreenBlood = Instantiate(greenBlood, transform.position, Quaternion.identity);
            Destroy(_GreenBlood, 5.5f );
           rb.velocity = _hitForce * recoilFactor*_hitDirection;
        }
    }
    protected void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !Player.Instance.pState.invincible && !Player.Instance.pState.invincible && health >0)
        {
            Attack();
            Player.Instance.HitStopTime(0, 5, 0.5f);
        }
    }

    protected virtual void Death(float _destroyTime)
    {
        Destroy (gameObject, _destroyTime);
    }
    protected virtual void UpdateEnemyStates() { }
    protected virtual void ChangeCurrentAnimation() { }
    protected void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyState = _newState;
    }
    protected virtual void Attack()
    {
        Player.Instance.TakeDamage(damage);
    }

}