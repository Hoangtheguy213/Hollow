﻿using JetBrains.Annotations;
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
    [SerializeField] private float timeBetweenAttack;
    private float timeSinceAttack;

    [SerializeField] Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
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
    [SerializeField] float recoilYSpeed = 100;
    private int stepXRecoiled, stepYRecoiled;
    [Space(5)]

    [Header("Health setting")]
    public int health;
    public int maxHealth;
    public int maxTotalHealth = 10;
    public int heartShards; 
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
    public bool halfMana;

    public ManaOrbsHandler manaOrbsHandler;
    public int orbShard;
    public int manaOrbs;
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

    [Header("Wall Jump Settings: ")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpingDuration;
    [SerializeField] private Vector2 wallJumpingPower;
    float wallJumpingDirection;
    bool isWallSliding;
    bool isWallJumping;
    [Space(5)]

    [Header("Camera setting: ")]
    [SerializeField] private float playerFallSpeedThreshold = -10;
    [Space(5)]

    [HideInInspector] public PlayerStateList pState;
    private Rigidbody2D rb;
    private Animator anim;
    private float xAxist, yAxist;
    private bool canDash;
    private bool dashed;
    private float gravity;
    bool openMap;
    bool openInventory;

    public static Player Instance;

    //unlock stuff variable
    public bool unlockWallJump, unlockDash, unlockVarJump;
    public bool unlockSideSpell, unlockUpSpell, unlockDownSpell;

    private SpriteRenderer sr;


    private void Awake()
    {
        if (Instance != null && Instance != this)
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
        sr = GetComponent<SpriteRenderer>();
        manaOrbsHandler = FindObjectOfType<ManaOrbsHandler>();

        gravity = rb.gravityScale;
        Mana = mana;
        manaStorage.fillAmount = Mana;
        Health = maxHealth;
        if (Health == 0)
        {
            pState.alive = false;
            GameManager.Instance.RespawnPlayer();
        }
        SaveData.Instance.LoadPlayerData();
    }

    //tao vien hitbox cho attack
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }


    void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;

        if (pState.cutscene) return;
        if (pState.alive)
        {
            getInput();
            ToggleMap();
            ToggleInventory();
            Heal();

        }
        UpdateJumpVariables();
        RestoreTimeScale();
        UpdateCameraYDampForPlayerFall();

        if (pState.Dashing || pState.healing) return;
        if (pState.alive)
        {
            if (!isWallJumping)
            {
                Flip();
                Move();
                Jump();
            }

            if (unlockWallJump)
            {
                WallSlide();
                WallJump();
            }

            if (unlockDash)
            {
                StartDash();
            }
            Attack();
            CastSpell();
        }
        //if (pState.healing) { return; }
        FlashWhileInvincible();
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(Death());
        }
    }


    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.GetComponent<Enemy>() != null && pState.casting)
        {
            _other.GetComponent<Enemy>().EnemyHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (pState.Dashing || pState.healing || pState.cutscene) return;
        Recoil();
    }
    void getInput()
    {
        if (pState.healing) return;
        xAxist = Input.GetAxisRaw("Horizontal");
        yAxist = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
        openMap = Input.GetButton("Map");
        openInventory = Input.GetButton("Inventory");

        if (Input.GetButton("Cast/Healing"))
        {
            CastOrHealingTimer += Time.deltaTime;
        }
        //else if (!Input.GetButton("Cast/Healing"))
        //{
        //    CastOrHealingTimer = 0;
        //}

    }

    void ToggleMap()
    {
        if (openMap)
        {
            UIManager.Instance.mapHandler.SetActive(true);
        }
        else
        {
            UIManager.Instance.mapHandler.SetActive(false);
        }
    }
    void ToggleInventory()
    {
        if (openInventory)
        {
            UIManager.Instance.inventory.SetActive(true);
        }
        else
        {
            UIManager.Instance.inventory.SetActive(false);
        }
    }
    //transform sprite
    void Flip()
    {
        if (xAxist < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxist > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }
    private void Move()
    {
        if (pState.healing)
        {
            // Nếu đang healing, giữ player ở vị trí hiện tại
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }
        rb.velocity = new Vector2(walkSpeed * xAxist, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    void UpdateCameraYDampForPlayerFall()
    {
        //if falling past a certain speed threshold
        if (rb.velocity.y < playerFallSpeedThreshold && !CameraManager.Instance.isLerpingYDamping && !CameraManager.Instance.hasLerpedYDamping)
        {
            StartCoroutine(CameraManager.Instance.LerpYDamping(true));
        }
        //if standing still or moving up
        if (rb.velocity.y >= 0 && !CameraManager.Instance.isLerpingYDamping && CameraManager.Instance.hasLerpedYDamping)
        {
            //reset camera function
            CameraManager.Instance.hasLerpedYDamping = false;
            StartCoroutine(CameraManager.Instance.LerpYDamping(false));
        }
    }
    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if (Grounded())
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
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        if (Grounded()) Instantiate(dashEffect, transform);
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
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");


            if (yAxist == 0 || yAxist < 0 && Grounded())
            {
                int _recoilLeftOrRight = pState.lookingRight ? 1 : 1;
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilXSpeed);
                Instantiate(slashEffect, SideAttackTransform);
            }
            else if (yAxist > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 95, UpAttackTransform);
            }
            else if (yAxist < 0 && !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackAbleLayer);
        //List<Enemy> hitEnemies = new List<Enemy>();
        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            //Enemy e = objectsToHit[i].GetComponent<Enemy>();
            //if (e && !hitEnemies.Contains(e))
            //{
            //    e.EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);
            //    hitEnemies.Add(e);
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage, _recoilDir, _recoilStrength);
            }

            if (objectsToHit[i].CompareTag("Enemy"))
            {
                if (!halfMana && mana < 1 || (halfMana && mana < 0.5))
                {
                    mana += manaGain;

                }
                else
                {
                    manaOrbsHandler.UpdateMana(manaGain * 3);
                }
                //Mana += manaGain;
                //Debug.Log("mana gain" + manaGain);
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
            if (pState.lookingRight)
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
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCouter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if (pState.recoilingX && stepXRecoiled < recoilXSteps)
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
        if (pState.alive)
        {
            Health -= Mathf.RoundToInt(_damage);
            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopTakingDamage());
            }
        }

    }
    IEnumerator StopTakingDamage()
    {

        pState.invincible = true;
        GameObject _bloodSpurtparticales = Instantiate(bloodSpurt, transform.position, Quaternion.identity);

        Destroy(_bloodSpurtparticales, 1.5f);

        anim.SetTrigger("TakeDamage");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;

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
            }
            else
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
        if (_delay > 0)
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

    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        GameObject _bloodSpurtparticales = Instantiate(bloodSpurt, transform.position, Quaternion.identity);

        Destroy(_bloodSpurtparticales, 1.5f);
        anim.SetTrigger("Death");

        //corpse cant be touch when player dead
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        GetComponent<BoxCollider2D>().enabled = false;


        yield return new WaitForSeconds(0.9f);
        StartCoroutine(UIManager.Instance.ActivateDeathSceen());

        yield return new WaitForSeconds(0.9f);
        Instantiate(GameManager.Instance.shade, transform.position, Quaternion.identity);
    }

    public void Respawned()
    {
        if (!pState.alive)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            GetComponent<BoxCollider2D>().enabled = true;
            pState.alive = true;
            halfMana = true;
            UIManager.Instance.SwitchMana(UIManager.ManaState.HalfMana);
            Mana = 0;
            Health = maxHealth;
            anim.Play("Idle");
        }
    }

    public void RestoreMana()
    {
        halfMana = false;
        UIManager.Instance.SwitchMana(UIManager.ManaState.FullMana);
    }
    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                if (onHealthChangedCallBack != null)
                {
                    onHealthChangedCallBack.Invoke();
                }
            }
        }
    }
    void Heal()
    {
        if (Input.GetButton("Cast/Healing") && CastOrHealingTimer > 0.12f && Health < maxHealth && Mana > 0 && !pState.Jumping && !pState.Dashing)
        {
            pState.healing = true;
            anim.SetBool("Healing", true);

            //healing
            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }
            //drain mana
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;

            Mana -= Time.deltaTime * manaDrainSpeed;
        }
        else
        {
            pState.healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;
        }

    }

    public float Mana
    {
        get { return mana; }
        set
        {
            //if mana stats change
            if (mana != value)
            {
                if (!halfMana)
                {
                    mana = Mathf.Clamp(value, 0, 1);
                }
                else
                {
                    mana = Mathf.Clamp(value, 0, 0.5f);
                }

                manaStorage.fillAmount = Mana;

            }
        }
    }
    void CastSpell()
    {
        if (Input.GetButtonUp("Cast/Healing") && CastOrHealingTimer <= 0.12f && timeSinceCast >= timeBetweenCast && Mana > manaSpellCost)
        {
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }
        if (!Input.GetButton("Cast/Healing"))
        {
            CastOrHealingTimer = 0;
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


        //side cast
        if ((yAxist == 0 || (yAxist < 0 && Grounded())) && unlockSideSpell)
        {
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.15f);
            GameObject _fireball = Instantiate(sideSpellFire, SideAttackTransform.position, Quaternion.identity);

            //flip fireball
            if (pState.lookingRight)
            {
                _fireball.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                _fireball.transform.eulerAngles = new Vector2(_fireball.transform.eulerAngles.x, 180);
            }
            pState.recoilingX = true;

            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            yield return new WaitForSeconds(0.35f);
        }
        //up cast
        else if ((yAxist > 0) && unlockUpSpell)
        {
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.15f);
            Instantiate(upSpellWater, transform);
            rb.velocity = Vector2.zero;

            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            yield return new WaitForSeconds(0.35f);
        }
        //down cast
        else if ((yAxist < 0 && !Grounded()) && unlockDownSpell)
        {
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.15f);
            downSpellFire.SetActive(true);

            Mana -= manaSpellCost;
            manaOrbsHandler.usedMana = true;
            manaOrbsHandler.countDown = 3f;
            yield return new WaitForSeconds(0.35f);
        }

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

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 3)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            pState.Jumping = false;
        }
        //allow player jump if grounded
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.Jumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);

            pState.Jumping = true;
        }
        // var jump
        if (!Grounded() && airJumpCouter < maxAirJump && Input.GetButtonDown("Jump") && unlockVarJump)
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
    private bool Walled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    void WallSlide()
    {
        if (Walled() && !Grounded() && xAxist != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = !pState.lookingRight ? 1 : -1;

            CancelInvoke(nameof(StopWallJumping));
        }
        if (Input.GetButtonDown("Jump") && isWallSliding)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);

            dashed = false;
            airJumpCouter = 0;

            if (pState.lookingRight && transform.eulerAngles.y == 0 || (!pState.lookingRight && transform.eulerAngles.y != 0))
            {
                pState.lookingRight = !pState.lookingRight;
                int _yRotation = pState.lookingRight ? 0 : 180;

                transform.eulerAngles = new Vector2(transform.eulerAngles.x, _yRotation);
            }
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    void StopWallJumping()
    {
        isWallJumping = false;
    }
}
