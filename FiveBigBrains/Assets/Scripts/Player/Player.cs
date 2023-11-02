using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerControlType { WASD, ARROW_KEYS }

    public PlayerControlType controlType;
    public Color playerColor;
    public float moveSpeed = 5.0f;
    public float jumpForce = 16.0f;
    public int remainingLives = 3;
    private GameObject[] heads = new GameObject[3];
    public Weapon currentWeapon;
    public Spear spearPrefab;
    public GameObject ShieldPrefab;
    public Player opponent;
    public float deathYThreshold = -20f;

    public int currentDirection; // 0: left; 1: right;

    private Renderer bodyRenderer;

    private bool isGrounded = true;
    private Rigidbody2D rb;

    public bool isSpearAttacking = false;

    // the variable to be used in Taunt; 
    public bool isTaunted = false;
    public bool isFreezed = false;
    private float tauntCooldown = 0f;
    private float tauntDuration = 6f;
    private float freezeDuration = 2f;
    private const float tauntedHeadSizeRatio = 2f;
    public float headResizeDuration = 0.5f; // Time it takes for the head to resize.

    // the defense-related variables
    public bool isDefensing = false;
    protected float nextDefendTime = 0;
    private float defenseDuration = 1f;
    private float defenseCooldown = 2f;

    // the shooting-angle related variables
    private float shootingAngle = 0f; // 0 means straight ahead
    private float maxShootingAngle = 45f;
    private float angleAdjustmentSpeed = 90f; // per second

    public delegate void PlayerLivesChanged(Player player);
    public event PlayerLivesChanged OnPlayerDied;

    public void TakeDamage(int damageAmount)
    {
        if (isDefensing)
        {
            return;
        }

        for (int i = remainingLives - 1; i > remainingLives - damageAmount - 1; i--)
        {
            if (i < heads.Length && i >= 0 && heads[i] != null)
            {
                Destroy(heads[i]);
            }
        }
        remainingLives -= damageAmount;
        if (remainingLives <= 0)
        {
            OnPlayerDied?.Invoke(this);
            Die();
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // initialize rb
        initializePlayerColor();
        initializePlayerWeapon();
        initializePlayerDirection();
        initializePlayerHeads();
    }

    private void Update()
    {
        Attack();
        //Defense();
        Jump();
        Move();
        Taunt();
        CheckForFallDamage();
        UpdateShootingAngle();
    }

    private void initializePlayerColor()
    {
        bodyRenderer = transform.Find("Body").GetComponent<Renderer>();
        bodyRenderer.material.color = playerColor;
    }

    public void initializePlayerWeapon()
    {
        Weapon newWeapon = Instantiate(spearPrefab, transform);
        currentWeapon = newWeapon;
        currentWeapon.owningPlayer = this;
    }

    private void initializePlayerDirection()
    {
        if (controlType == PlayerControlType.WASD)
        {
            TurnRight();
        }
        if (controlType == PlayerControlType.ARROW_KEYS)
        {
            TurnLeft();
        }
    }

    void initializePlayerHeads()
    {
        heads[0] = transform.Find("Head1").gameObject;
        heads[1] = transform.Find("Head2").gameObject;
        heads[2] = transform.Find("Head3").gameObject;

        foreach (GameObject head in heads)
        {
            if (head == null)
            {
                Debug.LogError("Error: cannot find all heads.");
            }
        }
    }


    private void Attack()
    {
        if (isFreezed)
        {
            return;
        }

        if (
            (controlType == PlayerControlType.WASD && Input.GetKeyDown(KeyCode.R) && currentWeapon != null) ||
            (controlType == PlayerControlType.ARROW_KEYS && Input.GetKeyDown(KeyCode.L) && currentWeapon != null)
           )
        {
            currentWeapon.TryAttack();
        }
    }

    private void UpdateShootingAngle()
    {
        if (currentWeapon == null || !currentWeapon.isShootingAngleAdjustable)
        {
            return;
        }

        float adjustment = 0;

        if (controlType == PlayerControlType.WASD)
        {
            if (Input.GetKey(KeyCode.W)) adjustment = 1;
            if (Input.GetKey(KeyCode.S)) adjustment = -1;
        }
        else if (controlType == PlayerControlType.ARROW_KEYS)
        {
            if (Input.GetKey(KeyCode.UpArrow)) adjustment = 1;
            if (Input.GetKey(KeyCode.DownArrow)) adjustment = -1;
        }

        shootingAngle += adjustment * angleAdjustmentSpeed * Time.deltaTime;
        shootingAngle = Mathf.Clamp(shootingAngle, -maxShootingAngle, maxShootingAngle);

        if (currentWeapon != null)
        {
            MagnifyGun gun = currentWeapon.GetComponent<MagnifyGun>();
            if (gun != null)
            {
                gun.SetShootingAngle(shootingAngle);
            }
        }
    }

    // instantiate a shield in front of the player and destroy it after 1s
    private void Defense()
    {
        if (isFreezed)
        {
            return;
        }


        // for WASD
        if (controlType == PlayerControlType.WASD && Input.GetKeyDown(KeyCode.E) || controlType == PlayerControlType.ARROW_KEYS && Input.GetKeyDown(KeyCode.K))
        {
            // determine if we can defend now
            if (Time.time < nextDefendTime)
            {
                return;
            }

            nextDefendTime = Time.time + defenseCooldown;
            isDefensing = true;

            // Generate a shield
            GameObject shield = Instantiate(ShieldPrefab, transform.position + new Vector3(0, 1.11f, 0), Quaternion.identity, transform);

            // Destroy the shield after 1s
            Destroy(shield, defenseDuration);

            // change isDefensing to false after 1s
            StartCoroutine(defenceSleeping());

        }
    }
    // helper function to sleep for 1s
    IEnumerator defenceSleeping()
    {
        yield return new WaitForSeconds(defenseDuration);
        isDefensing = false;
        print("Done defending!");
    }

    private void Jump()
    {
        if (controlType == PlayerControlType.WASD && Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
        else if (controlType == PlayerControlType.ARROW_KEYS && Input.GetKeyDown(KeyCode.Return) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void Move()
    {
        float h = 0;

        if (isFreezed || isSpearAttacking)
        {
            return;
        }

        if (controlType == PlayerControlType.WASD)
        {
            h = Input.GetAxis("HorizontalWASD");
        }
        else if (controlType == PlayerControlType.ARROW_KEYS)
        {
            h = Input.GetAxis("Horizontal");
        }

        if (h > 0.01f)
        {
            TurnRight();
        }

        else if (h < -0.01f)
        {
            TurnLeft();
        }

        Vector3 moveDirection = new Vector3(h, 0, 0).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    private void Taunt()
    {
        if (tauntCooldown > 0)
            tauntCooldown -= Time.deltaTime;

        // Handle WASD player taunt button
        if (
            (controlType == PlayerControlType.WASD && Input.GetKeyDown(KeyCode.Q) && tauntCooldown <= 0) ||
            (controlType == PlayerControlType.ARROW_KEYS && Input.GetKeyDown(KeyCode.RightShift) && tauntCooldown <= 0)
        )
        {
            StartCoroutine(TauntSelf());
            StartCoroutine(TauntOpponent());
            tauntCooldown = 6f;
        }
    }

    public void TurnLeft()
    {
        currentDirection = 0;
        transform.localScale = new Vector3(-1, 1, 1);
    }

    public void TurnRight()
    {
        currentDirection = 1;
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void Flip()
    {
        if (currentDirection == 0)
        {
            TurnRight();
        }
        else
        {
            TurnLeft();
        }
    }

    // Side-effect for self after taunting
    private IEnumerator TauntSelf()
    {

        if (transform.position.x < opponent.transform.position.x)
        {
            TurnLeft();
        }
        else
        {
            TurnRight();
        }

        isFreezed = true;
        yield return new WaitForSeconds(freezeDuration);
        isFreezed = false;
    }

    private IEnumerator TauntOpponent()
    {
        opponent.isTaunted = true;
        StartCoroutine(opponent.ChangeHeadSize(tauntedHeadSizeRatio, headResizeDuration));
        yield return new WaitForSeconds(tauntDuration);
        opponent.isTaunted = false;
        StartCoroutine(opponent.ChangeHeadSize(1 / tauntedHeadSizeRatio, headResizeDuration));
    }

    // Enlarge the head when the player is taunted
    private void EnlargeHead()
    {
        Transform head3Trans = transform.Find("Head3");
        Transform head2Trans = transform.Find("Head2");
        Transform head1Trans = transform.Find("Head1");
        if (head3Trans != null)
        {
            head3Trans.localScale *= tauntedHeadSizeRatio;
        }
        if (head2Trans != null)
        {
            head2Trans.localScale *= tauntedHeadSizeRatio;
        }
        if (head1Trans != null)
        {
            head1Trans.localScale *= tauntedHeadSizeRatio;
        }

        Transform headCollisionBoxTransform = transform.Find("HeadCollisionBox");
        if (headCollisionBoxTransform != null)
        {
            BoxCollider2D collider = headCollisionBoxTransform.GetComponent<BoxCollider2D>();
            collider.size *= tauntedHeadSizeRatio;
        }
    }

    private IEnumerator ChangeHeadSize(float targetSizeRatio, float duration)
    {
        float startTime = Time.time;
        Transform[] heads =
        {
            transform.Find("Head1"),
            transform.Find("Head2"),
            transform.Find("Head3")
        };

        // Store initial sizes
        Vector3[] initialSizes = new Vector3[heads.Length];
        for (int i = 0; i < heads.Length; i++)
        {
            if (heads[i] != null)
                initialSizes[i] = heads[i].localScale;
        }

        Transform headCollisionBoxTransform = transform.Find("HeadCollisionBox");
        BoxCollider2D collider = headCollisionBoxTransform?.GetComponent<BoxCollider2D>();
        Vector2 initialColliderSize = collider ? collider.size : Vector2.one;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            for (int i = 0; i < heads.Length; i++)
            {
                if (heads[i] != null)
                    heads[i].localScale = Vector3.Lerp(initialSizes[i], initialSizes[i] * targetSizeRatio, t);
            }

            if (collider)
                collider.size = Vector2.Lerp(initialColliderSize, initialColliderSize * targetSizeRatio, t);

            yield return null;
        }

        // Ensure it reaches the target size
        for (int i = 0; i < heads.Length; i++)
        {
            if (heads[i] != null)
                heads[i].localScale = initialSizes[i] * targetSizeRatio;
        }

        if (collider)
            collider.size = initialColliderSize * targetSizeRatio;
    }

    private void ShrinkHead()
    {
        Transform head3Trans = transform.Find("Head3");
        Transform head2Trans = transform.Find("Head2");
        Transform head1Trans = transform.Find("Head1");
        if (head3Trans != null)
        {
            head3Trans.localScale /= tauntedHeadSizeRatio;
        }
        if (head2Trans != null)
        {
            head2Trans.localScale /= tauntedHeadSizeRatio;
        }
        if (head1Trans != null)
        {
            head1Trans.localScale /= tauntedHeadSizeRatio;
        }

        Transform headCollisionBoxTransform = transform.Find("HeadCollisionBox");
        if (headCollisionBoxTransform != null)
        {
            BoxCollider2D collider = headCollisionBoxTransform.GetComponent<BoxCollider2D>();
            collider.size /= tauntedHeadSizeRatio;
        }
    }

    void CheckForFallDamage()
    {
        if (transform.position.y < deathYThreshold)
        {
            TakeDamage(100);
        }
    }

    private void Die()
    {
        //Destroy the eye
        Transform eyeTrans = transform.Find("Eye");
        if (eyeTrans != null)
        {
            GameObject eyeObj = eyeTrans.gameObject;
            Destroy(eyeObj);
        }

        if (controlType == PlayerControlType.WASD)
        {
            Debug.Log("Player1 died!");
        }
        else
        {
            Debug.Log("Player2 died!");
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform")
        {
            foreach (ContactPoint2D point in collision.contacts)
            {
                if (point.normal.y > 0.5)  // Check if the collision is mostly upwards
                {
                    isGrounded = true;
                    break;
                }
            }
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {

        }
    }

}
