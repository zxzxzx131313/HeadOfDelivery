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
    [SerializeField] private DropPoints beginPoints;
    [SerializeField] private AudioRandomPlayer tile_sound;

    private Tilemap _platformTilemap;
    private Tilemap _headTilemap;

    public bool IsAttached { get; private set; }
    public UnityAction<Vector2,bool> SendDirection;
    public UnityAction TileSpawned;

    private GameObject _body;
    private TileSpawner _spawner;
    private SpriteRenderer _sprite;
    private HeadDice _dice;

    CubeInputControl cubeControl;
    Vector2 dirCheck;
    Vector3 _detach_pos;
    Vector3 _begin_pos;
    bool animation_stopped = false;
    bool moved = false;

    float time = 0f;
    int current_step = 0;


    [Header("Head Movement")]
    public float speed;

    [Header("Head Event")]
    public GameEvent OnShowHint;
    public GameEvent OnHideHint;

    private void Awake()
    {
        cubeControl = new CubeInputControl();
        cubeControl.Gameplay.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());
        _begin_pos = transform.position;
    }

    public void OnEnable()
    {
        cubeControl.Enable();
        //stats.StepsLeftChanged += CheckStepLeft;
        stats.LevelChanged += LevelBegin;
    }

    public void OnDisable()
    {
        cubeControl.Disable();
        //stats.StepsLeftChanged -= CheckStepLeft;
        stats.LevelChanged -= LevelBegin;
    }

    private void Start()
    {

        _body = transform.parent.gameObject;
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

        if (moved) AutoOffFaceHint();

        //if (Keyboard.current.rKey.wasPressedThisFrame)
        //{
        //    Restart();
        //}
    }

    public CubeInputControl GetCubeInputControl()
    {
        return cubeControl;
    }
    void AutoOffFaceHint()
    {
        // for auto off hint dice faces
        //if (cubeControl.Gameplay.Move.triggered)
        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            OnHideHint.Raise();
        }
        
        if (IsAttached)
        {
            OnHideHint.Raise();
        }
    }

    public void ResetHintTimer()
    {
        time = hintDisplayTimeInSec;
    }

    public void Restart()
    {
        if (IsAttached)
        {
            RestartDetach();
            _body.GetComponent<PlayerController>().OnRestartLevel(_detach_pos - Vector3.up);
        }
        // prevent restart in transitional animation 
        if (animation_stopped)
        {
            _spawner.Restart();
            LeanTween.move(gameObject, _begin_pos, 0.5f).setEaseInOutQuad();

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
            OnEnable();
            

            OnShowHint.Raise();
            moved = false;
        }
    }

    public void OnPaused()
    {
        if (IsAttached)
        {
            _body.GetComponent<PlayerController>().OnPaused();
        }
        else
        {
            if (cubeControl.Gameplay.enabled) OnDisable();
            else OnEnable();
        }
    }

    public void LevelBegin(int level)
    {
        moved = false;
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

        Debug.Log(_detach_pos);
    }

    public void RestartDetach()
    {
        IsAttached = false;
        // show sprite after detached from body
        _sprite.enabled = true;
    }

    public void SetBeginState()
    {
        animation_stopped = true;
        _begin_pos = beginPoints.GetDropPointInLevel(stats.Level);
        _dice.SetInitialState(stats.Level);
        //if (_dice.top_face.IsColored)
        //{

        //    anim.SetBool("IsColored", true);
        //}
        //else
        //{
        //    anim.SetBool("IsColored", false);
        //}
        //anim.SetTrigger("ChangeFace");
    }

    public void SetAnimationBegin()
    {
        animation_stopped = false;
    }

    private void Move(Vector2 direction)
    {
        if (!moved)
        {
            moved = true;
            OnHideHint.Raise();
        }

        if (IsMoveable(direction, transform.position, _body.transform.position) && !IsAttached)
        {
            dirCheck = direction;

            bool stamped = false;

            anim.SetFloat("Horizontal", direction.x);
            anim.SetFloat("Vertical", direction.y);
            anim.SetFloat("Speed", direction.sqrMagnitude);

            transform.position += (Vector3)direction;
            Vector3 world_pos = transform.position;

            _dice.HandleTurn(direction);
            anim.SetTrigger("Moving");

            Vector3Int pos = _platformTilemap.WorldToCell(world_pos);
            Vector3Int detach = _platformTilemap.WorldToCell(_detach_pos);
            if (_dice.top_face.IsColored)
            {

                anim.SetBool("IsColored", true);
            }
            else
            {
                anim.SetBool("IsColored", false);
            }

            if (_dice.top_face.opposite.IsColored && stats.StepsLeft > 0)
            {
                // do not spawn tile at body and original head location
                if (pos != detach && pos != detach + Vector3Int.up && !_spawner.HasHeadTile(pos))
                {
                    // for turning animation to complete;
                    current_step = stats.StepsLeft;
                    Spawntile(pos);
                    //Invoke("Spawntile", 0.2f);
                    stamped = true;
                    tile_sound.PlaySingle();
                }
            }

            //if (direction.x > 0)
            //    _sprite.flipX = false;
            //if (direction.x < 0)
            //    _sprite.flipX = true;
            if (!stamped)
                tile_sound.PlayRandom();



            if (pos == detach)
            {
                OnAttachHead.Raise();
            }

            OnShowHint.Raise();
            ResetHintTimer();

            SendDirection?.Invoke(direction, stamped);
            stats.StepsLeft--;

            //Debug.Log("step--");
        }
    }


    public bool IsOnHeadTile(Vector3 pos)
    {
        Vector3Int cell_pos = _platformTilemap.WorldToCell(pos);
        return _spawner.HasHeadTile(cell_pos); 
    } 

    private void Spawntile(Vector3Int cell_pos)
    {
        _spawner.SpawnTile(cell_pos, (float)current_step / stats.LevelSteps[stats.Level]);
        _spawner.HideTile(cell_pos);
        TileSpawned?.Invoke();
        ShowTileAfterAnimation(cell_pos);
    }

    private void ShowTileAfterAnimation(Vector3Int cell_pos)
    {
        _spawner.ShowTileDelay(cell_pos);
    }

    private bool IsMoveable(Vector2 direction, Vector3 pos, Vector3 body_pos)
    {

        Vector3Int gridPosition = _platformTilemap.WorldToCell(pos + (Vector3)direction);
        Vector3Int bodyPosition = _platformTilemap.WorldToCell(body_pos);
        if (!_platformTilemap.HasTile(gridPosition) && !_headTilemap.HasTile(gridPosition) && 
            IsInBounds(pos + (Vector3)direction) && gridPosition != bodyPosition)
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
