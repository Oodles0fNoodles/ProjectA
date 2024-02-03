using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class CharacterController2D : MonoBehaviour
{
    // Move player in 2D space
    public float maxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float gravityScale = 3.0f; // Increased gravity
    public Camera mainCamera;
    public float cameraOffset = 2.0f; // Offset to adjust camera position

    bool facingRight = true;
    float moveDirection = 1; // Constantly moving to the right
    bool isGrounded = false;
    Vector3 cameraPos;
    Rigidbody2D r2d;
    BoxCollider2D mainCollider;
    Transform t;

    // Rotation during jump
    float rotationSpeed = 360f;

    // Use this for initialization
    void Start()
    {
        t = transform;
        r2d = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<BoxCollider2D>();
        r2d.freezeRotation = true;
        r2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        r2d.gravityScale = gravityScale;
        facingRight = t.localScale.x > 0;

        if (mainCamera)
        {
            cameraPos = mainCamera.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Change facing direction
        if (moveDirection > 0 && !facingRight)
        {
            facingRight = true;
            t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, transform.localScale.z);
        }
        if (moveDirection < 0 && facingRight)
        {
            facingRight = false;
            t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight);
        }

        // Camera follow
        if (mainCamera)
        {
            float newX = Mathf.Lerp(mainCamera.transform.position.x, t.position.x + cameraOffset, Time.deltaTime * 5f);
            mainCamera.transform.position = new Vector3(newX, cameraPos.y, cameraPos.z);
        }
    }

    void FixedUpdate()
    {
        Bounds colliderBounds = mainCollider.bounds;
        float colliderRadius = mainCollider.size.x * 0.4f * Mathf.Abs(transform.localScale.x);
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius);

        isGrounded = false;
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != mainCollider)
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        if (isGrounded)
        {
            float angle = Mathf.LerpAngle(t.eulerAngles.z, 0f, Time.fixedDeltaTime * 5f);
            t.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {

            t.Rotate(Vector3.forward * -rotationSpeed * Time.fixedDeltaTime);
        }

        r2d.velocity = new Vector2((moveDirection) * maxSpeed, r2d.velocity.y);
    }
}
