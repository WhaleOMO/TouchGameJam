using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Player : MonoBehaviour
{
    public float Gravity = -10f;
    public float RunSpeed = 40f;
    public LayerMask CrouchLayer;

    public Color GreenColor;

    private Animator animator;
    private SpriteRenderer sRenderer;
    private MovementController movementController;
    private BoxCollider2D collider2D;
    private Material material;
    private Vector2 velocity;
    private float horizontalMove;

    private bool isWalking;
    private bool isCrouching;
    private bool isFacingRight = true;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        sRenderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<BoxCollider2D>();
        movementController = GetComponent<MovementController>();
        material = sRenderer.material;
    }
    
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        velocity.x = input.x * RunSpeed;
        
        isWalking = Mathf.Abs(velocity.x) > 0.5;
        isCrouching = CheckIfOverlapWithCrouchable();

        if (!isCrouching)
        {
            this.Gravity = -10f;
            velocity.y = Mathf.Min(velocity.y, velocity.y + Gravity * Time.deltaTime);
        }
        else
        {
            velocity.y = input.y;
            this.Gravity = 0;
        }
        
                
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isCrouching", isCrouching);
        movementController.Move(velocity * Time.deltaTime, isCrouching);
        
        AutoFlip();
        
        // Temp test
        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangeColor(GreenColor);
        }
    }
    
    bool CheckIfOverlapWithCrouchable()
    {
        ContactFilter2D filter2D = new ContactFilter2D()
        {
            layerMask = CrouchLayer,
            useLayerMask = true,
            useTriggers = true
        };
        Collider2D[] results = new Collider2D[5];
        int amount = collider2D.OverlapCollider(filter2D, results);
        for (int i = 0; i < Mathf.Min(amount, 5); i++)
        {
            if (((1<<results[i].gameObject.layer) & CrouchLayer) != 0)
            {
                return true;
            }
        }

        return false;
    }

    void AutoFlip()
    {
        // If the input is moving the player right and the player is facing left...
        if (velocity.x > 0 && !isFacingRight)
        {
            isFacingRight = true;
            sRenderer.flipX = false;
        } // Otherwise if the input is moving the player left and the player is facing right...
        else if (velocity.x < 0 && isFacingRight)
        {
            isFacingRight = false;
            sRenderer.flipX = true;
        }
    }

    public async void ChangeColor(Color targetColor)
    {
        int colID = Shader.PropertyToID("_ClothCol");
        int oriColId = Shader.PropertyToID("_OriginCol");
        int progId = Shader.PropertyToID("_ChangeProgress");
        
        Color oriColor = material.GetColor(colID);
        TimeSpan deltaTimeSpan = TimeSpan.FromMilliseconds(10);
        
        material.SetInt(Shader.PropertyToID("_ColorIsChanging"),1);
        material.SetColor(oriColId, oriColor);
        material.SetColor(colID, targetColor);
        
        for (int i = 0; i < 100; i++)
        {
            // Color mixColor = Color.Lerp(oriColor, targetColor, i / 100.0f);
            // material.SetColor(colID, mixColor);
            material.SetFloat(progId, i/100f);
            await Task.Delay(deltaTimeSpan);
        }
        
        material.SetInt(Shader.PropertyToID("_ColorIsChanging"),0);
    }
    
}
