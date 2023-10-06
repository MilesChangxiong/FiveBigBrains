using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpForce = 10.0f;
    private bool isGrounded = true;
    private bool isHopping;
    private GameObject[] objs;
    private Rigidbody2D rb;
    private float hopTimer;
    public float hopDuration = 0.5f;

    void Start()
    {
        objs = GameObject.FindGameObjectsWithTag("Player");
        if (objs.Length > 0)
        {
            // Move the first player and get its Rigidbody
            rb = objs[0].GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        // Move horizontally
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 moveDirection = new Vector2(horizontalInput, 0);
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);

        // Jump when spacebar is pressed and the player is grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isHopping = true;
            hopTimer = 0;
        }

        if (isHopping)
        {
            hopTimer += Time.deltaTime;
            if (hopTimer >= hopDuration)
            {
                isHopping = false;
                // Apply an opposing force to bring the player back to the ground
                rb.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
}
