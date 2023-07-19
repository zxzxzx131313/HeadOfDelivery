using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;

    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;

    public Vector2 inputDirection;


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

    public void OnEnable()
    {
        inputControl.Enable();
    }

    public void OnDisable()
    {
        inputControl.Disable();
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
    //private void LateUpdate()
    //{
    //    // for pixel perfect adjustment
    //    if (!Keyboard.current.anyKey.wasPressedThisFrame && physicsCheck.IsGround())
    //    {
    //        Vector3 position = transform.position;
             
    //        position.x = Mathf.Round(transform.position.x * Graphics.PPU) / Graphics.PPU;
    //        position.y = Mathf.Round(Mathf.Round(transform.position.y * Graphics.PPU) / Graphics.PPU);

    //        rb.MovePosition(position);
    //    }
    //}

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

    public void AlignToGrid()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Ceil(pos.x) - 0.5f;
        rb.MovePosition(pos);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        //Debug.Log("isground:" + physicsCheck.IsGround());
        if (physicsCheck.IsGround())
            rb.AddForce(transform.up * thrust, ForceMode2D.Impulse);
    }

}
