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

    public int currentDirection; // 0: left; 1: right;

    private Renderer playerRenderer;
    private bool isGrounded = true;
    private Rigidbody2D rb;

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
        playerRenderer = GetComponent<Renderer>();
        playerRenderer.material.color = playerColor;
    }

    public void initializePlayerWeapon()
    {
        currentWeapon = gameObject.AddComponent<Spear>();
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
        if (controlType == PlayerControlType.WASD && Input.GetKeyDown(KeyCode.Q) && currentWeapon != null)
        {
            currentWeapon.TryAttack();
        }

        if (controlType == PlayerControlType.ARROW_KEYS && Input.GetKeyDown(KeyCode.L) && currentWeapon != null)
        {
            currentWeapon.TryAttack();
        }
    }

    private void Defense() { }

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
    
    private void Taunt() { }

    private void Die()
    {
        // TODO
        Debug.Log("Player died!");
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform")
        {
            isGrounded = true;
        }

    }

}
