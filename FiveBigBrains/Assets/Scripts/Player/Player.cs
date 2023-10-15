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
    public Weapon currentWeapon;
    public Spear spearPrefab;
    public GameObject ShieldPrefab;

    public int currentDirection; // 0: left; 1: right;

    private Renderer bodyRenderer;

    private bool isGrounded = true;
    private Rigidbody2D rb;
    
    //the variable to be used in Taunt; 
    public bool isTaunted=false; 
    public bool isFreezed=false;
    private float tauntCooldown = 0f;
    private float tauntDuration = 4f;
    private float freezeDuration = 2f;
    private bool canTaunt = true;

    public void TakeDamage(int damageAmount)
    {
        remainingLives -= damageAmount;

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
    }

    private void initializePlayerColor() 
    {
        bodyRenderer = transform.Find("Body").GetComponent<Renderer>();
        bodyRenderer.material.color = playerColor;
    }

    private void initializePlayerWeapon()
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

    private void Update()
    {
        Attack();
        Defense();
        Jump();
        Move();
        Taunt();
    }

    private void Attack() {
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
    private void Defense() { 
        // for WASD
        if (controlType == PlayerControlType.WASD && Input.GetKeyDown(KeyCode.E))
        {   
            // Generate a shield
            GameObject shield = Instantiate(ShieldPrefab);

            // make the shield transparent to the player
            Physics2D.IgnoreCollision(shield.GetComponent<Collider2D>(), currentWeapon.GetComponent<Collider2D>());

            // put the shield in front of the player
            if(currentDirection == 1) {
                shield.transform.position = transform.position + new Vector3(1, 0, 0);
            } else {
                shield.transform.position = transform.position + new Vector3(-1, 0, 0);
            }

            // destroy the shield after 1s
            Destroy(shield, 1f);

        }

        if (controlType == PlayerControlType.ARROW_KEYS && Input.GetKeyDown(KeyCode.K)) {
            // Generate a shield
            GameObject shield = Instantiate(ShieldPrefab);

            // make the shield transparent to the player's spear
            Physics2D.IgnoreCollision(shield.GetComponent<Collider2D>(), currentWeapon.GetComponent<Collider2D>());

            // put the shield in front of the player
            if(currentDirection == 1) {
                shield.transform.position = transform.position + new Vector3(1, 0, 0);
            } else {
                shield.transform.position = transform.position + new Vector3(-1, 0, 0);
            }

            // destroy the shield after 1s
            Destroy(shield, 2f);

        }
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
            currentDirection = 1;
            transform.localScale = new Vector3(1, 1, 1);
        }
            
        else if (h < -0.01f)
        {
            currentDirection = 0;
            transform.localScale = new Vector3(-1, 1, 1);
        }

        Vector3 moveDirection = new Vector3(h, 0, v).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }
    
    private void Taunt()
    {
        if (tauntCooldown > 0)
            tauntCooldown -= Time.deltaTime;

        // Enable the taunt when cooldown is over
        if (tauntCooldown <= 0)
            canTaunt = true;

        // Handle WASD player taunt button (for this example, let's use "Q" for player 1)
        if (controlType == PlayerControlType.WASD && Input.GetKeyDown(KeyCode.Q) && canTaunt)
        {
            StartCoroutine(TauntSelf(GameManager.player1Instance,GameManager.player2Instance));
            StartCoroutine(TauntOpponent(GameManager.player1Instance,GameManager.player2Instance));
            tauntCooldown = 6f;
            canTaunt = false;
        }

        // Handle ARROW_KEYS player taunt button (for this example, let's use "RightShift" for player 2)
        else if (controlType == PlayerControlType.ARROW_KEYS && Input.GetKeyDown(KeyCode.RightShift) && canTaunt)
        {
            StartCoroutine(TauntSelf(GameManager.player2Instance,GameManager.player1Instance));
            StartCoroutine(TauntOpponent(GameManager.player2Instance,GameManager.player1Instance));
            tauntCooldown = 6f;
            canTaunt = false;
        }
    }
     
    private IEnumerator TauntSelf(Player self,Player opponent){
        float originalSpeed = moveSpeed;

        // Freeze the taunting player and change direction based on the opponent's position on the X-axis
        moveSpeed = 0;
        if (self.transform.position.x < opponent.transform.position.x)
        {
            currentDirection = 0;
       
        }
        else
        {
            currentDirection = 1;
        
        }
        isFreezed=true;
        yield return new WaitForSeconds(freezeDuration);
        isFreezed=false;
        // Unfreeze the taunting player
        moveSpeed = originalSpeed;
    }

    private IEnumerator TauntOpponent(Player self,Player opponent)
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

}
