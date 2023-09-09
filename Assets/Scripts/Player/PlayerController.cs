using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInputControl inputControl;

    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    //private Vector2 inputDirection;

    public LevelStats stats;
    public GameStateSave state;


    [Header("Basic Variable")]
    public float speed;
    public float thrust;
    public float maxSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
    }

    private void Awake()
    {
        inputControl = new PlayerInputControl();
        inputControl.Gameplay.Jump.started += ctx => Jump(ctx);
        //inputControl.Gameplay.Move. += ctx => Move(ctx.ReadValue<Vector2>());
    }

    void OnEnable()
    {
        inputControl.Enable();
        stats.LevelChanged += OnChangeLevel;
    }

    void OnDisable()
    {
        inputControl.Disable();
        stats.LevelChanged -= OnChangeLevel;
    }


    private void Update()
    {
        //inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
        // resolving clutching to wall while falling problem

        if (inputControl.Gameplay.Move.IsPressed())
            Move(inputControl.Gameplay.Move.ReadValue<Vector2>());

    }

    private void FixedUpdate()
    {
        //Move();
    }

    // jumping not as high while moving was due to not using input system trigger ispressed but using values in fixedupdate and changing velocity
    public void Move(Vector2 inputDirection)
    {

        bool faceDir = false;

        if (inputDirection.x > 0)
            faceDir = false;
        if (inputDirection.x < 0)
            faceDir = true;

        // Flip
        //transform.localScale = new Vector3(faceDir, 1, 1);
        GetComponent<SpriteRenderer>().flipX = faceDir;
        Vector2 velocity = rb.velocity;
        if (!physicsCheck.IsBlockFacing(inputDirection.x))
        {


            rb.velocity = new Vector2(inputDirection.x * speed, velocity.y);
        }
        else
        {


            rb.velocity = new Vector2(0, velocity.y);
        }
        //rb.AddForce(new Vector2( inputDirection.x * speed, 0), ForceMode2D.Impulse);

        //if (inputDirection.x > 0)
        //{
        //    velocity.x = MathF.Min(velocity.x, maxSpeed);
        //}
        //if (inputDirection.x < 0)
        //{
        //    velocity.x = Mathf.Max(velocity.x, -maxSpeed);
        //}
        //rb.velocity = velocity;


    }


    void AlignToGrid()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Ceil(pos.x) - 0.5f;
        rb.MovePosition(pos);
    }

    public void OnChangeLevel(int level)
    {
        if (!state.IsLevelAnimationPlayed(level))
        {
            // stops player moving in scene changing, due to acceleration
            rb.velocity = new Vector2(0, rb.velocity.y);
            OnDisable();
            AlignToGrid();
        }
    }

    public void OnPaused()
    {
        if (inputControl.Gameplay.enabled) OnDisable();
        else OnEnable();
    }

    public void OnRestartLevel(Vector3 init_pos)
    {
        OnDisable();
        LeanTween.move(gameObject, init_pos, 0.5f).setEaseInOutQuad();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        //Debug.Log("isground:" + physicsCheck.IsGround());
        if (physicsCheck.IsGround())
            rb.AddForce(transform.up * thrust, ForceMode2D.Impulse);
    }

}
