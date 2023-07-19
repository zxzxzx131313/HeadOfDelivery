using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class CubeController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private Animator anim;
    [SerializeField] private GameEvent OnAttachHead;
    [SerializeField] private float hintDisplayTimeInSec = 1f;
    [SerializeField] private LevelStats stats;

    private Tilemap _platformTilemap;
    private Tilemap _headTilemap;

    public bool IsAttached { get; private set; }

    private GameObject _body;
    private TileSpawner _spawner;
    private SpriteRenderer _sprite;
    private HeadDice _dice;

    CubeInputControl cubeControl;
    Vector2 dirCheck;
    Vector3 _detach_pos;
    Vector3 _begin_pos;
    bool animation_stopped = false;

    float time = 0f;


    [Header("Head Movement")]
    public float speed;

    [Header("Head Event")]
    public GameEvent OnShowHint;
    public GameEvent OnHideHint;
    public float LevelAnimationLength = 4.6f;

    private void Awake()
    {
        cubeControl = new CubeInputControl();
        cubeControl.Gameplay.Move.started += ctx => Move(ctx.ReadValue<Vector2>());
    }

    public void OnEnable()
    {
        cubeControl.Enable();
        stats.StepsLeftChanged += CheckStepLeft;
    }

    public void OnDisable()
    {
        cubeControl.Disable();
        stats.StepsLeftChanged -= CheckStepLeft;
    }

    private void Start()
    {

        _body = GameObject.FindGameObjectWithTag("Body");
        _spawner = GetComponent<TileSpawner>();
        _sprite = GetComponent<SpriteRenderer>();
        _dice = GetComponent<HeadDice>();
        
        IsAttached = true;
        OnDisable();

        foreach (var tm in GameObject.FindObjectsOfType<Tilemap>())
        {
            if (tm.CompareTag("Headtile")) _headTilemap = tm;
            else if (tm.CompareTag("Ground")) _platformTilemap = tm;
        }
    }

    private void Update()
    {
        dirCheck = cubeControl.Gameplay.Move.ReadValue<Vector2>();

        anim.SetFloat("Horizontal", dirCheck.x);
        anim.SetFloat("Vertical", dirCheck.y);
        anim.SetFloat("Speed", dirCheck.sqrMagnitude);

        AutoOffFaceHint();

        if (Keyboard.current.rKey.wasPressedThisFrame && !IsAttached)
        {
            Restart();
        }
    }
    void AutoOffFaceHint()
    {
        // for auto off hint dice faces
        if (cubeControl.Gameplay.Move.triggered)
        {
            time = hintDisplayTimeInSec;
        }
        else
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                OnHideHint.Raise();
            }
        }
        if (IsAttached)
        {
            OnHideHint.Raise();
        }
    }

    void Restart()
    {
        if (animation_stopped)
        {
            _spawner.Restart();
            LeanTween.move(gameObject, _begin_pos, 0.5f).setEaseInOutQuad();
            transform.position = _begin_pos;
            _dice.Restart();
            if (_dice.top_face.IsColored)
            {

                anim.SetBool("IsColored", true);
            }
            else
            {
                anim.SetBool("IsColored", false);
            }
            // might need additional animation for the restart transition
            anim.SetTrigger("ChangeFace");
            stats.RestartLevel();
            OnEnable();
        }
    }

    /**
 * <summary>
 * Attach head back to its body if head is close enough to its body.
 * </summary>
 */
    public void AttachHead()
    {
        // TODO: where would the head be allowed to attach back, within one unit? can diagonally one-unit distance be attached? Might determined by level design.
        // or potentially a special item get in the game can allow for diagonal attaching -- TBD.

        IsAttached = true;
        _sprite.enabled = false;
        transform.position = _detach_pos;
        OnDisable();   
    }

    public void DetachHead()
    {
        IsAttached = false;
        // show sprite after detached from body
        _sprite.enabled = true;
        _detach_pos = _body.transform.position + Vector3.up;
    }

    public void SetBeginState()
    {
        animation_stopped = true;
        _begin_pos = transform.position;
    }

    public void SetAnimationBegin()
    {
        animation_stopped = false;
    }

    private void Move(Vector2 direction)
    {
        if (IsMoveable(direction) && !IsAttached)
        {
            transform.position += (Vector3)direction;

            _dice.HandleTurn(direction);

            Vector3Int pos = _platformTilemap.WorldToCell(transform.position);
            Vector3Int detach = _platformTilemap.WorldToCell(_detach_pos);

            if (_dice.top_face.IsColored)
            {

                anim.SetBool("IsColored", true);
            }
            else
            {
                anim.SetBool("IsColored", false);
            }
            if (_dice.top_face.opposite.IsColored)
            {
                // do not spawn tile at body and original head location
                if (pos != detach && pos != detach + Vector3Int.up)
                {
                    _spawner.SpawnTile(pos);
                }
            }

            if (direction.x > 0)
                _sprite.flipX = false;
            if (direction.x < 0)
                _sprite.flipX = true;


            anim.SetTrigger("Moving");


            if (pos == detach)
            {
                OnAttachHead.Raise();
            }

            OnShowHint.Raise();

            stats.StepsLeft--;
        }
    }

    private void CheckStepLeft(int step)
    {
        if (step == 0)
        {
            OnDisable();
        }
    }

    private bool IsMoveable(Vector2 direction)
    {

        Vector3Int gridPosition = _platformTilemap.WorldToCell(transform.position + (Vector3)direction);
        Vector3Int bodyPosition = _platformTilemap.WorldToCell(_body.transform.position);
        if (!_platformTilemap.HasTile(gridPosition) && !_headTilemap.HasTile(gridPosition) && 
            IsInBounds(transform.position + (Vector3)direction) && gridPosition != bodyPosition)
            return true;
        return false;
    }

    public bool IsInBounds(Vector2 pos)
    {

        var cam = Camera.main;

        float half_height = cam.orthographicSize;
        float half_width = half_height * cam.aspect;

        if (pos.x > cam.transform.position.x - half_width && pos.y >= cam.transform.position.y - half_height &&
            pos.x < cam.transform.position.x + half_width && pos.y < cam.transform.position.y + half_height)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
