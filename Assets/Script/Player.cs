using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Player : MonoBehaviour
{
    public float gravity = -20f;
    public float runSpeed = 40f;
    public LayerMask crouchLayer;

    private Animator animator;
    private SpriteRenderer renderer;
    private MovementController movementController;
    private BoxCollider2D collider2D;
    private Vector2 velocity;
    private float horizontalMove;

    private bool isWalking;
    private bool isCrouching;
    private bool isFacingRight = true;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<BoxCollider2D>();
        movementController = GetComponent<MovementController>();
    }
    
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        velocity.x = input.x * runSpeed;
        
        isWalking = Mathf.Abs(velocity.x) > 0.5;
        isCrouching = CheckIfOverlapWithCrouchable();

        if (!isCrouching)
            velocity.y += gravity * Time.deltaTime;

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isCrouching", isCrouching);
        movementController.Move(velocity * Time.deltaTime);

        // If the input is moving the player right and the player is facing left...
        if (velocity.x > 0 && !isFacingRight)
        {
            isFacingRight = true;
            renderer.flipX = false;
        } // Otherwise if the input is moving the player left and the player is facing right...
        else if (velocity.x < 0 && isFacingRight)
        {
            isFacingRight = false;
            renderer.flipX = true;
        }
    }

    bool CheckIfOverlapWithCrouchable()
    {
        ContactFilter2D filter2D = new ContactFilter2D()
        {
            layerMask = crouchLayer,
            useLayerMask = true,
        };
        Collider2D[] results = new Collider2D[5];
        int amount = collider2D.OverlapCollider(filter2D, results);
        for (int i = 0; i < Mathf.Min(amount, 5); i++)
        {
            if (((1<<results[i].gameObject.layer) & crouchLayer) != 0)
            {
                return true;
            }
        }

        return false;
    }
}
