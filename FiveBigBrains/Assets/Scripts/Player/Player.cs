using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerControlType { WASD, ARROW_KEYS }

    public PlayerControlType controlType;
    public Color playerColor;
    public float moveSpeed = 5.0f;
    public int remainingLives = 3;
    private GameObject[] heads = new GameObject[3];
    public Weapon currentWeapon;
    public Spear spearPrefab;
    public GameObject ShieldPrefab;
    public Player opponent;
    public float deathYThreshold = -15f;

    public int currentDirection; // 0: left; 1: right;

    private Renderer bodyRenderer;

    private Rigidbody2D rb;

    public bool isSpearAttacking = false;

    // jump-related variables
    public float jumpForce = 16.0f;
    public int maxJumps = 2;
    private int jumpsDone = 0;
    private float nextJumpTime = 0f;
    public float jumpInterval = 0.15f;
    private bool isGrounded = true;

    // the variable to be used in Taunt; 
    public bool isTaunted = false;
    public bool isCrouching = false;
    private float tauntCooldown = 0f;
    private float tauntDuration = 6f;
    private float freezeDuration = 2f;
    private const float tauntedHeadSizeRatio = 2f;
    public float headResizeDuration = 0.5f; // Time it takes for the head to resize.
    private int initialHealthOnFreeze;

    // the defense-related variables
    public bool isDefensing = false;
    protected float nextDefendTime = 0;
    private float defenseDuration = 1f;
    private float defenseCooldown = 2f;

    // the shooting-angle related variables
    private float shootingAngle = 0f; // 0 means straight ahead
    private float maxShootingAngle = 80f;
    //private float angleAdjustmentSpeed = 90f; // per second
    private float angleOscillationSpeed = 2.0f;
    private float angleOscillationAmplitude = 75.0f;
    public float weaponPickupTime;

    // Crouch
    [SerializeField] private Transform bodyTransform; // Assign this in the inspector
    private Vector3 originalBodyScale; // To store the original scale
    private Vector3 originalBodyPosition; // To store the original position
    public Vector3 crouchScale = new Vector3(1f, 0.5f, 1f); // Adjust as needed
    public Vector3 crouchPositionOffset = new Vector3(0f, -0.25f, 0f); // Adjust as needed

    // Taunt
    public const float headEnlargeVolume = 0.35f;
    //public const float headShrinkRate = 0.0001f;
    public const float totalShrinkPerSecond = 0.12f;
    private float currentHeadScale = 1f;
    private Vector2 originalColliderSize;
    public bool everTaunt = false;


    public delegate void PlayerLivesChanged(Player player);
    public event PlayerLivesChanged OnPlayerDied;

    public bool isBeingDamagedByLaser;

    // head explosion object
    public GameObject headExplosion;

    // Taking Damage
    private HashSet<string> processedEvents = new HashSet<string>();

    public bool hasReportedDie = false;

    public bool canAttack=true; 

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
                // when a head is destroyed, display the explosion
                Destroy(heads[i]);
                explodeHead();
            }
        }
        remainingLives -= damageAmount;

        if (remainingLives <= 0)
        {
            Die();
            if (!hasReportedDie)
            {
                hasReportedDie = true;
                OnPlayerDied?.Invoke(this);
            }
        }

        updateHeadHitBox();
    }

    public void TakeDamageWithEventID(int damageAmount, string interactionId)
    {
        if (processedEvents.Contains(interactionId))
        {
            return;
        }
        processedEvents.Add(interactionId);

        TakeDamage(damageAmount);
    }

    // function to instantiate head explosion
    void explodeHead()
    {
        GameObject explosion = (GameObject)Instantiate(headExplosion);
        //explosion.transform.position = transform.position;

        // float yOffset = 1.8f; // Adjust this value to control how much above the current position
        // Vector3 newPosition = transform.position + new Vector3(0f, yOffset, 0f);
        // explosion.transform.position = newPosition;
        Vector3 eyeTrans = transform.Find("Eye").position;
        //explosion.transform.position = this.transform.position;
        explosion.transform.position = eyeTrans;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // initialize rb
        initializePlayerColor();
        initializePlayerWeapon();
        initializePlayerDirection();
        initializePlayerHeads();

        originalBodyScale = bodyTransform.localScale;
        originalBodyPosition = bodyTransform.localPosition;
        Transform headCollisionBoxTransform = transform.Find("HeadCollisionBox");
        if (headCollisionBoxTransform != null)
        {
            BoxCollider2D collider = headCollisionBoxTransform.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                originalColliderSize = collider.size;
            }
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }
        Attack();
        //Defense();
        Jump();
        Move();
        Crouch();
        ShrinkHead();
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
        if(canAttack==false){
            return;
        }
        if (
            (controlType == PlayerControlType.WASD && Input.GetKeyDown(KeyCode.Space) && currentWeapon != null) ||
            (controlType == PlayerControlType.ARROW_KEYS && Input.GetKeyDown(KeyCode.Return) && currentWeapon != null)
           )
        {
            if (currentWeapon.TryAttack())
            {
                // Data-report related logic
                if (GameManager.instance != null && GameManager.instance.currentMapStats != null)
                {
                    GameManager.instance.currentMapStats.AttackCount += 1;
                }
            }
        }
    }

    private void UpdateShootingAngle()
    {
        if (currentWeapon == null || !currentWeapon.isShootingAngleAdjustable)
        {
            return;
        }

        //float adjustment = 0;

        //if (controlType == PlayerControlType.WASD)
        //{
        //    if (Input.GetKey(KeyCode.W)) adjustment = 1;
        //    if (Input.GetKey(KeyCode.S)) adjustment = -1;
        //}
        //else if (controlType == PlayerControlType.ARROW_KEYS)
        //{
        //    if (Input.GetKey(KeyCode.UpArrow)) adjustment = 1;
        //    if (Input.GetKey(KeyCode.DownArrow)) adjustment = -1;
        //}

        //shootingAngle += adjustment * angleAdjustmentSpeed * Time.deltaTime;
        //shootingAngle = Mathf.Clamp(shootingAngle, -maxShootingAngle, maxShootingAngle);

        //if (currentWeapon != null)
        //{
        //    MagnifyGun gun = currentWeapon.GetComponent<MagnifyGun>();
        //    if (gun != null)
        //    {
        //        gun.SetShootingAngle(shootingAngle);
        //    }
        //}

        // Automatically swing the shooting angle using a sine wave function, based on weaponPickupTime
        float timeSincePickedUp = Time.time - weaponPickupTime;
        shootingAngle = Mathf.Sin(timeSincePickedUp * angleOscillationSpeed) * angleOscillationAmplitude;
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
        if (isCrouching)
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
        if (jumpsDone >= maxJumps)
        {
            return;
        }

        if (Time.time < nextJumpTime)
        {
            return;
        }

        if (
            (controlType == PlayerControlType.WASD && Input.GetKeyDown(KeyCode.W)) ||
            (controlType == PlayerControlType.ARROW_KEYS && Input.GetKeyDown(KeyCode.UpArrow))
        )
        {
            rb.velocity = new Vector2(rb.velocity.x, 0); // Reset the vertical velocity (to avoid adding forces together)
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumpsDone++;
            nextJumpTime = Time.time + jumpInterval;

            // Data-report related logic
            if (GameManager.instance != null && GameManager.instance.currentMapStats != null)
            {
                GameManager.instance.currentMapStats.JumpCount += 1;
            }
        }
    }

    private void Crouch()
    {
        if (
            (controlType == PlayerControlType.WASD && Input.GetKeyDown(KeyCode.S)) ||
            (controlType == PlayerControlType.ARROW_KEYS && Input.GetKeyDown(KeyCode.DownArrow))
        )
        {
            isCrouching = true;
            CheckOpponentHeadSize(); // check if triggers a taunting.
            // Apply crouch visual effect
            changeHeadsPosition(1.29f);
            bodyTransform.localScale = crouchScale;
            bodyTransform.localPosition += crouchPositionOffset;
        }
        else if (
            (controlType == PlayerControlType.WASD && Input.GetKeyUp(KeyCode.S)) ||
            (controlType == PlayerControlType.ARROW_KEYS && Input.GetKeyUp(KeyCode.DownArrow))
        )
        {
            isCrouching = false;
            // Reset the body to its original scale and position
            changeHeadsPosition(2f);
            bodyTransform.localScale = originalBodyScale;
            bodyTransform.localPosition = originalBodyPosition;
        }
    }

    private void CheckOpponentHeadSize()
    {
        // Check if this player is crouching and facing away from the opponent
        if (opponent != null && IsFacingAwayFromOpponent())
        {
            // Enlarge opponent's head
            opponent.EnlargeHead();

            everTaunt = true;
        }
    }

    private bool IsFacingAwayFromOpponent()
    {
        return (currentDirection == 0 && opponent.transform.position.x > transform.position.x) ||
               (currentDirection == 1 && opponent.transform.position.x < transform.position.x);
    }

    public void EnlargeHead()
    {
        float currentVolume = 4f / 3f * Mathf.PI * Mathf.Pow(currentHeadScale / 2f, 3);
        float newVolume = currentVolume + headEnlargeVolume;
        currentHeadScale = Mathf.Pow((3f * newVolume) / (4f * Mathf.PI), 1f / 3f) * 2f;
        updateHeadSize();  
    }

    void updateHeadSize()
    {
        Transform head3Trans = transform.Find("Head3");
        Transform head2Trans = transform.Find("Head2");
        Transform head1Trans = transform.Find("Head1");

        if (head3Trans != null)
        {
            head3Trans.localScale = 2 * new Vector3(currentHeadScale, currentHeadScale, currentHeadScale);
        }
        if (head2Trans != null)
        {
            head2Trans.localScale = 1.5f * new Vector3(currentHeadScale, currentHeadScale, currentHeadScale);
        }
        if (head1Trans != null)
        {
            head1Trans.localScale = new Vector3(currentHeadScale, currentHeadScale, currentHeadScale);
        }

        updateHeadHitBox();
    }

    void updateHeadHitBox()
    {
        Transform headCollisionBoxTransform = transform.Find("HeadCollisionBox");
        if (headCollisionBoxTransform != null)
        {
            BoxCollider2D collider = headCollisionBoxTransform.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                float scaleMultiplier = currentHeadScale;

                if (transform.Find("Head3") == null)
                {
                    scaleMultiplier *= 1.5f / 2.0f;  // 如果head3不在了
                }
                else if (transform.Find("Head2") == null)
                {
                    scaleMultiplier *= 1.0f / 2.0f;  // 如果head2不在了
                }

                collider.size = originalColliderSize * scaleMultiplier;
            }
        }
    }

    private void ShrinkHead()
    {
        //currentHeadScale = Mathf.Max(1f, currentHeadScale - headShrinkRate);
        float shrinkThisFrame = totalShrinkPerSecond * Time.deltaTime;
        currentHeadScale = Mathf.Max(1f, currentHeadScale - shrinkThisFrame);
        updateHeadSize();
    }

    public void ResetHeadSize()
    {
        print(0);
    }

    private void Move()
    {
        float h = 0;

        if (isSpearAttacking)
        {
            return;
        }

        if (controlType == PlayerControlType.WASD)
        {
            h = Input.GetAxisRaw("HorizontalWASD");
        }
        else if (controlType == PlayerControlType.ARROW_KEYS)
        {
            h = Input.GetAxisRaw("Horizontal");
        }

        if (h > 0.01f)
        {
            TurnRight();
        }

        else if (h < -0.01f)
        {
            TurnLeft();
        }

        // reduce speed if player is crouching
        float _moveSpeed;
        if (isCrouching)
        {
            _moveSpeed = moveSpeed * 0.3f;
        }
        else
        {
            _moveSpeed = moveSpeed;
        }

        Vector3 moveDirection = new Vector3(h, 0, 0).normalized;
        transform.Translate(moveDirection * _moveSpeed * Time.deltaTime, Space.World);

        // Data-report related logic
        if (GameManager.instance != null && GameManager.instance.currentMapStats != null)
        {
            GameManager.instance.currentMapStats.MovementDistance += moveDirection.magnitude * _moveSpeed * Time.deltaTime;
        }
    }



    // Side-effect for self after taunting
    private IEnumerator TauntSelf()
    {
        initialHealthOnFreeze = remainingLives;

        isCrouching = true;
        yield return new WaitForSeconds(freezeDuration);
        isCrouching = false;

        ReportHealthLostDuringTauntFreeze(initialHealthOnFreeze - remainingLives);
    }

    private IEnumerator TauntOpponent()
    {
        opponent.isTaunted = true;
        StartCoroutine(opponent.ChangeHeadSize(tauntedHeadSizeRatio, headResizeDuration));
        yield return new WaitForSeconds(tauntDuration);
        opponent.isTaunted = false;
        StartCoroutine(opponent.ChangeHeadSize(1 / tauntedHeadSizeRatio, headResizeDuration));
    }

    private void ReportHealthLostDuringTauntFreeze(int healthLost)
    {
        var eventData = new HealthLostDuringTauntFreezeEvent(healthLost);

        GameReport.Instance.PostDataToFirebase("HealthLostDuringTauntFreeze", eventData);
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

    void changeHeadsPosition(float posY)
    {
        List<Transform> Transforms = new List<Transform> {
            transform.Find("Head3"),
            transform.Find("Head2"),
            transform.Find("Head1"),
            transform.Find("Eye")
        };

        foreach (Transform trans in Transforms)
        {
            if (trans != null)
            {
                trans.localPosition = new Vector3(trans.localPosition.x, posY, trans.localPosition.z);
            }
        }

    }

    //// Enlarge the head when the player is taunted
    //private void EnlargeHead()
    //{
    //    Transform head3Trans = transform.Find("Head3");
    //    Transform head2Trans = transform.Find("Head2");
    //    Transform head1Trans = transform.Find("Head1");
    //    if (head3Trans != null)
    //    {
    //        head3Trans.localScale *= tauntedHeadSizeRatio;
    //    }
    //    if (head2Trans != null)
    //    {
    //        head2Trans.localScale *= tauntedHeadSizeRatio;
    //    }
    //    if (head1Trans != null)
    //    {
    //        head1Trans.localScale *= tauntedHeadSizeRatio;
    //    }

    //    Transform headCollisionBoxTransform = transform.Find("HeadCollisionBox");
    //    if (headCollisionBoxTransform != null)
    //    {
    //        BoxCollider2D collider = headCollisionBoxTransform.GetComponent<BoxCollider2D>();
    //        collider.size *= tauntedHeadSizeRatio;
    //    }
    //}

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

    //private void ShrinkHead()
    //{
    //    Transform head3Trans = transform.Find("Head3");
    //    Transform head2Trans = transform.Find("Head2");
    //    Transform head1Trans = transform.Find("Head1");
    //    if (head3Trans != null)
    //    {
    //        head3Trans.localScale /= tauntedHeadSizeRatio;
    //    }
    //    if (head2Trans != null)
    //    {
    //        head2Trans.localScale /= tauntedHeadSizeRatio;
    //    }
    //    if (head1Trans != null)
    //    {
    //        head1Trans.localScale /= tauntedHeadSizeRatio;
    //    }

    //    Transform headCollisionBoxTransform = transform.Find("HeadCollisionBox");
    //    if (headCollisionBoxTransform != null)
    //    {
    //        BoxCollider2D collider = headCollisionBoxTransform.GetComponent<BoxCollider2D>();
    //        collider.size /= tauntedHeadSizeRatio;
    //    }
    //}

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

        //
        if (isCrouching)
        {
            ReportHealthLostDuringTauntFreeze(initialHealthOnFreeze - remainingLives);
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform" || collision.gameObject.tag == "IronBall")
        {
            foreach (ContactPoint2D point in collision.contacts)
            {
                if (point.normal.y > 0.5)  // Check if the collision is mostly upwards
                {
                    isGrounded = true;
                    jumpsDone = 0;
                    break;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform" || collision.gameObject.tag == "IronBall")
        {
            isGrounded = false;
        }
    }
}
