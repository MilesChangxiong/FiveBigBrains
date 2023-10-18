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

    public int currentDirection; // 0: left; 1: right;

    private Renderer bodyRenderer;

    private bool isGrounded = true;
    private Rigidbody2D rb;

    //the variable to be used in Taunt; 
    public bool isTaunted = false;
    public bool isFreezed = false;
    private float tauntCooldown = 0f;
    private float tauntDuration = 4f;
    private float freezeDuration = 2f;

    //the defense-related variables
    public bool isDefensing = false;
    protected float nextDefendTime = 0;

    public delegate void PlayerLivesChanged(Player player);
    public event PlayerLivesChanged OnPlayerLivesChanged;

    public void TakeDamage(int damageAmount)
    {
        // print("I'm taking damage!");
        // print("I'm defending: " + isDefensing);
        // when oppenent is defending, no damage
        if(isDefensing)
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
        OnPlayerLivesChanged?.Invoke(this);
        if (remainingLives <= 0)
        {
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
        Defense();
        Jump();
        Move();
        Taunt();
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

        // Ingore collision between spears
        GameObject[] spears = GameObject.FindGameObjectsWithTag("Spear"); // Find all spears with the same tag

        foreach (GameObject otherSpear in spears)
        {
            if (otherSpear != currentWeapon.gameObject)
            {
                Physics2D.IgnoreCollision(currentWeapon.GetComponent<Collider2D>(), otherSpear.GetComponent<Collider2D>());
            }
        }
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
        if (controlType == PlayerControlType.WASD && Input.GetKeyDown(KeyCode.R) && currentWeapon != null)
        {
            currentWeapon.TryAttack();
        }

        if (controlType == PlayerControlType.ARROW_KEYS && Input.GetKeyDown(KeyCode.L) && currentWeapon != null)
        {
            currentWeapon.TryAttack();
        }

        //Destroy the eye when all heads gone
        Transform head1Trans = transform.Find("Head1");
        if (head1Trans == null)
        {
            Transform eyeTrans = transform.Find("Eye");
            if (eyeTrans != null)
            {
                GameObject eyeObj = eyeTrans.gameObject;
                Destroy(eyeObj);
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
            if (CanDefend())
            {
                nextDefendTime = Time.time + 1f;
            } else
            {
                print("Cannot defend now, current time is " + Time.time + ", next defend time is " + nextDefendTime);
                return;
            }

            // isDefensing
            isDefensing = true;
            print("I'm defending!");

            // Generate a shield
            GameObject shield = Instantiate(ShieldPrefab);

            // make the shield transparent to the player
            // TODO: should change to the opponent's currentWeapon
            Physics2D.IgnoreCollision(shield.GetComponent<Collider2D>(), currentWeapon.GetComponent<Collider2D>());

            // put the shield in front of the player
            if (currentDirection == 1)
            {
                shield.transform.position = transform.position + new Vector3(1, 0, 0);
            }
            else
            {
                shield.transform.position = transform.position + new Vector3(-1, 0, 0);
            }

            // Destroy the shield after 1s
            Destroy(shield, 1f);

            // change isDefensing to false after 1s
            StartCoroutine(Sleeping());
            
        }
    }
    // helper function to sleep for 1s
    IEnumerator Sleeping()
    {
        yield return new WaitForSeconds(1);
        isDefensing = false;
        print("Done defending!");
    }

    // determine if we can defend now
    private bool CanDefend()
    {
        return Time.time >= nextDefendTime;
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
        float v = 0;

        if (isFreezed)
        {
            return;
        }

        if (controlType == PlayerControlType.WASD)
        {
            h = Input.GetAxis("HorizontalWASD");
            v = Input.GetAxis("VerticalWASD");
        }
        else if (controlType == PlayerControlType.ARROW_KEYS)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }

        if (h > 0.01f)
        {
            TurnRight();
        }

        else if (h < -0.01f)
        {
            TurnLeft();
        }

        Vector3 moveDirection = new Vector3(h, 0, v).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    private void Taunt()
    {
        if (tauntCooldown > 0)
            tauntCooldown -= Time.deltaTime;

        // Handle WASD player taunt button
        if (controlType == PlayerControlType.WASD && Input.GetKeyDown(KeyCode.Q) && tauntCooldown <= 0)
        {
            StartCoroutine(TauntSelf(this, opponent));
            StartCoroutine(TauntOpponent(this, opponent));
            tauntCooldown = 6f;
        }

        // Handle ARROW_KEYS player taunt button
        else if (controlType == PlayerControlType.ARROW_KEYS && Input.GetKeyDown(KeyCode.RightShift) && tauntCooldown <= 0)
        {
            StartCoroutine(TauntSelf(this, opponent));
            StartCoroutine(TauntOpponent(this, opponent));
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
    private IEnumerator TauntSelf(Player self, Player opponent)
    {

        if (self.transform.position.x < opponent.transform.position.x)
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

    private IEnumerator TauntOpponent(Player self, Player opponent)
    {
        opponent.isTaunted = true;
        opponent.EnlargeHead();
        yield return new WaitForSeconds(tauntDuration);
        opponent.isTaunted = false;
        opponent.ShrinkHead();
    }

    // Enlarge the head when the player is taunted
    private void EnlargeHead()
    {
        Transform head3Trans = transform.Find("Head3");
        Transform head2Trans = transform.Find("Head2");
        Transform head1Trans = transform.Find("Head1");
        if (head3Trans != null)
        {
            head3Trans.localScale *= 1.3f;
        }
        if (head2Trans != null)
        {
            head2Trans.localScale *= 1.3f;
        }
        if (head1Trans != null)
        {
            head1Trans.localScale *= 1.3f;
        }
    }

    private void ShrinkHead()
    {
        Transform head3Trans = transform.Find("Head3");
        Transform head2Trans = transform.Find("Head2");
        Transform head1Trans = transform.Find("Head1");
        if (head3Trans != null)
        {
            head3Trans.localScale /= 1.3f;
        }
        if (head2Trans != null)
        {
            head2Trans.localScale /= 1.3f;
        }
        if (head1Trans != null)
        {
            head1Trans.localScale /= 1.3f;
        }
    }

    private void Die()
    {

        if (controlType == PlayerControlType.WASD)
        {
            Debug.Log("Player1 died!");
        }
        else
        {
            Debug.Log("Player2 died!");
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform")
        {
            isGrounded = true;
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {

        }
    }

}
