using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Horizontal movement Settings:")]
    [SerializeField] private float walkSpeed = 1;

    [Header("Vertical movement Setting")]
    [SerializeField] private float jumpForce = 45;
    private int jumpBufferCounter = 0;
    [SerializeField] private int JumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCouter = 0;
    [SerializeField] private int maxAirJump;

    [Header("Ground check settings:")]
    
    [SerializeField] Transform groundCheckPoints;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Dash settings:")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCoolDown;
    [SerializeField] GameObject dashEffect;

    [Header("Attack settings:")]
    bool attack = false;
    float timeBetweenAttack, timeSinceAttack;

    [SerializeField] Transform SideAttackTransform,UpAttackTransform,DownAttackTransform;
    [SerializeField] Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] LayerMask attackAbleLayer;
    [SerializeField] float damage;
    [SerializeField] GameObject slashEffect;
    bool restoreTime;
    float restoreTimeSpeed;

    [Header("Recoil")]
    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed= 100;
    private int stepXRecoiled, stepYRecoiled;

    [Header ("Health setting")]
    public int health;
    public int maxHealth;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallBack;

    float healTimer;
    [SerializeField] float timeToHeal;

    [Header("Mana Settings")]
    [SerializeField] UnityEngine.UI.Image manaStorage;
    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    [Space(5)]

    [Header("Spell Settings: ")]
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast = 0.5f;
    [SerializeField] float spellDamage;
    [SerializeField] float downSpellForce;
    [SerializeField] GameObject sideSpellFire;
    [SerializeField] GameObject downSpellFire;
    [SerializeField] GameObject upSpellWater;
    float timeSinceCast;
    float CastOrHealingTimer;
    [Space(5)]

    [HideInInspector] public PlayerStateList pState;
    private Rigidbody2D rb;
    private Animator anim;
    private float xAxist, yAxist;
    private bool canDash;
    private bool dashed;
    private float gravity;

    public static Player Instance;

    private SpriteRenderer sr;

    private void Awake()
    {
        if(Instance !=null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        
    }
    void Start()
    {

        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();   
        sr= GetComponent<SpriteRenderer>();
        gravity =rb.gravityScale;
        Mana = mana;
        manaStorage.fillAmount = Mana;
        Health = maxHealth;
    }

    //tao vien hitbox cho attack
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    // Update is called once per frame
    void Update()
    {
        if (pState.cutscene) return;
        getInput();

        UpdateJumpVariables();
        if (pState.Dashing) return;
        Move();
        Jump();
        CastSpell();
        Flip();
        StartDash();
        Attack();
        RestoreTimeScale();
        FlashWhileInvincible();
        Heal();
        if (pState.healing) return;
        
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.GetComponent<Enemy>() != null && pState.casting)
        {
            _other.GetComponent<Enemy>().EnemyHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }

    private void FixedUpdate()
    {
        if(pState.Dashing || pState.healing || pState.cutscene) return;
        Recoil();
    }
    void getInput()
    {
        xAxist = Input.GetAxisRaw("Horizontal");
        yAxist = Input.GetAxisRaw("Vertical");
        attack =Input.GetButtonDown("Attack");

        if (Input.GetButton("Cast/Healing"))
        {
            CastOrHealingTimer += Time.deltaTime;
        }
        else
        {
            CastOrHealingTimer = 0;
        }
    }

    //transform sprite
    void Flip()
    {
        if(xAxist < 0)
        {
            transform.localScale = new Vector2(-1,transform.localScale.y);
            pState.lookingRight =false;
        }
        else if(xAxist > 0) 
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight=true;
        }
    }
    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxist , rb.velocity.y);
        anim.SetBool("Walking",rb.velocity.x!=0 && Grounded());
    }
    void StartDash()
    {
        if (Input.GetButtonDown("Dash")  && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if(Grounded())
        {
            dashed = false;
        }
    }
    IEnumerator Dash()
    {
        canDash = false;
        pState.Dashing = true;
        anim.SetTrigger("Dashing");
        print("dashing");
        rb.gravityScale = 0;
        int _dir =pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        if(Grounded()) Instantiate(dashEffect,transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.Dashing = false;
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        pState.invincible = true;

        //If exit direction is upwards
        if (_exitDir.y > 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        //If exit direction requires horizontal movement
        if (_exitDir.x != 0)
        {
            xAxist = _exitDir.x > 0 ? 1 : -1;
            Move();
        }

        Flip();
        yield return new WaitForSeconds(_delay);
        pState.invincible = false;
        pState.cutscene = false;
    }
    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if(attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");


            if (yAxist == 0 || yAxist < 0 && Grounded())
            {
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, recoilXSpeed);
                Instantiate(slashEffect, SideAttackTransform);
            }
            else if (yAxist > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 95, UpAttackTransform);
            }
            else if (yAxist < 0 && !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea , ref bool _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit= Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea,0, attackAbleLayer);
        List<Enemy> hitEnemies = new List<Enemy>();
        if(objectsToHit.Length > 0 )
        {
            _recoilDir = true;
        }

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy e = objectsToHit[i].GetComponent<Enemy>();
            if (e && !hitEnemies.Contains(e))
            {
                e.EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);
                hitEnemies.Add(e);

                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    Mana += manaGain;
                    Debug.Log("mana gain" + manaGain);
                }
            }

        }
    }

    void SlashEffectAtAngle(GameObject _slasEffect, int _effectAngle, Transform _attackTransform)
    {
        _slasEffect = Instantiate(_slasEffect, _attackTransform);
        _slasEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slasEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Recoil()
    {
        if (pState.recoilingX)
        {
            if(pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
              
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }
        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxist < 0)
            {
               
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x , -recoilYSpeed);
            }
            airJumpCouter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if(pState.recoilingX && stepXRecoiled < recoilXSteps)
        {
            stepXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if (pState.recoilingY && stepYRecoiled < recoilYSteps)
        {
            stepYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }
        if (Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepXRecoiled = 0;
        pState.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepYRecoiled = 0;
        pState.recoilingY = false;
    }
    public void TakeDamage(float _damage)
    {
        if (!pState.invincible) // Check if the character is not invincible
        {
            Debug.Log("Taking damage: " + _damage);
            Health -= Mathf.RoundToInt(_damage);

            Debug.Log("Health after damage: " + Health);
            StartCoroutine(StopTakingDamage());
        }
        else
        {
            Debug.Log("Currently invincible, no damage taken.");
        }


    }
    IEnumerator StopTakingDamage()
    {
        anim.SetTrigger("TakeDamage");
        pState.invincible = true;
        GameObject _bloodSpurtparticales = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        _bloodSpurtparticales.transform.SetParent(transform);
        Destroy(_bloodSpurtparticales, 1.5f);
        
       
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
        Debug.Log("No longer invincible.");
    }
    void FlashWhileInvincible()
    {
        if (pState.cutscene) return;
        sr.material.color = pState.invincible ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;

    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }
    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;
        if(_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
    }
    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }
    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                if(onHealthChangedCallBack != null)
                {
                    onHealthChangedCallBack.Invoke();
                }
            }
        }
    }
    void Heal()
    {
        if(Input.GetButton("Cast/Healing") && CastOrHealingTimer> 0.05f && Health< maxHealth && Mana>0 && !pState.Jumping && !pState.Dashing)
        {
            pState.healing = true;
            anim.SetBool("Healing", true);
            healTimer += Time.deltaTime;
            
            if(healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }
            Mana -= Time.deltaTime * manaDrainSpeed;
        }
        else
        {
            pState.healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;
        }
    }

    float Mana
    {
        get { return mana; }
        set
        {
            //if mana stats change
            if (mana != value)
            {
                mana = Mathf.Clamp(value, 0, 1);
                manaStorage.fillAmount = Mana;
            }
        }
    }
    void CastSpell()
    {
        if(Input.GetButtonUp("Cast/Healing") && CastOrHealingTimer <= 0.05f && timeSinceCast >= timeBetweenCast && Mana> manaSpellCost)
        {
            pState.casting = true;
            timeSinceAttack = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }
        if (Grounded())
        {
            //disable downspell if on ground
            downSpellFire.SetActive(false);
        }
        //if down spell is active, force player down until grounded
        if (downSpellFire.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }
    }


    IEnumerator CastCoroutine()
    {
        anim.SetBool("Casting",true);
        yield return new WaitForSeconds(0.15f);

        //side cast
        if(yAxist==0|| (yAxist<0 && Grounded()))
        {
            GameObject _fireball = Instantiate(sideSpellFire, SideAttackTransform.position, Quaternion.identity);

        //flip fireball
        if(pState.lookingRight)
            {
                _fireball.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                _fireball.transform.eulerAngles = new Vector2(_fireball.transform.eulerAngles.x, 180);
            }
            pState.recoilingX = true;
        }
        //up cast
        else if(yAxist>0)
        {
            Instantiate(upSpellWater, transform);
            rb.velocity = Vector2.zero;
        }
        //down cast
        else if (yAxist < 0 && !Grounded())
        {
            downSpellFire.SetActive(true);
        }
        Mana -= manaSpellCost;
        yield return new WaitForSeconds(0.35f);
        anim.SetBool("Casting", false);
        pState.casting = false;
    }
    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoints.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoints.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoints.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void Jump()
    {
        //cancel animation jump
        if(Input.GetButtonUp("Jump") &&rb.velocity.y > 3)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            pState.Jumping = false;
        }
        //cho phep player jump neu grounded
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.Jumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);

            pState.Jumping = true;
        }
        if (!Grounded() && airJumpCouter < maxAirJump && Input.GetButtonDown("Jump"))
        {
            pState.Jumping = true;
            airJumpCouter++;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }
        anim.SetBool("Jumping", !Grounded());
    }
    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.Jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCouter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = JumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }
}
