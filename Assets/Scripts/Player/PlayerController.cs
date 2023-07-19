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
    private Vector2 inputDirection;

    public LevelStats stats;


    [Header("Basic Variable")]
    public float speed;
    public float thrust;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
    }

    private void Awake()
    {
        inputControl = new PlayerInputControl();
        inputControl.Gameplay.Jump.started += Jump;
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
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
        // resolving clutching to wall while falling problem

        if (!physicsCheck.IsGround() && physicsCheck.IsBlockFacing(inputDirection.x))
        {
            inputDirection = new Vector2(0, inputDirection.y);
        }

    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {

        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        int faceDir = (int)transform.localScale.x;

        if (inputDirection.x > 0)
            faceDir = 1;
        if (inputDirection.x < 0)
            faceDir = -1;

        // Flip
        transform.localScale = new Vector3(faceDir, 1, 1);
    }

    void AlignToGrid()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Ceil(pos.x) - 0.5f;
        rb.MovePosition(pos);
    }

    void OnChangeLevel(int level)
    {
        OnDisable();
        AlignToGrid();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        //Debug.Log("isground:" + physicsCheck.IsGround());
        if (physicsCheck.IsGround())
            rb.AddForce(transform.up * thrust, ForceMode2D.Impulse);
    }

}
