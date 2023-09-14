using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using Cinemachine;


public enum LevelRelation { PosIncludeNeg, NegIncludePos, Independent};
public enum TriggerDirection { Horizontal, Vertical, Diagonal };
public class LevelSwitchTrigger : MonoBehaviour
{
    [Header("Trigger stats")]
    [SerializeField] private int NegativeDirectionLevel;
    [SerializeField] private int PositiveDirectionLevel;
    [SerializeField] private TriggerDirection TriggerType;
    [SerializeField] private LevelRelation LevelConnection;
    [SerializeField] private Vector2 LevelOffset;
    [SerializeField] private UnityEvent OnLevelSwitch;
    [SerializeField] private Collider2D Blocker;

    [Header("Game States")]
    [SerializeField] private LevelStats stats;
    [SerializeField] private GameStateSave state;
    [SerializeField] private GameEvent OnLevelBegin;

    EdgeColliderSetting edgeCollider;
    Tilemap _tiles;

    Vector3Int _enter_on_tile;
    PhysicsCheck _player;
    GameObject _body;

    // Start is called before the first frame update
    void Start()
    {
        _body = GameObject.FindGameObjectWithTag("Body");
        _player = _body.GetComponent<PhysicsCheck>();
        _tiles = GameObject.FindGameObjectWithTag("Ground").GetComponent<Tilemap>();
        edgeCollider = GameObject.FindGameObjectWithTag("EdgeCollider").GetComponent<EdgeColliderSetting>();
        _enter_on_tile = Vector3Int.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Body"))
        {
            Vector3Int _tile = _tiles.WorldToCell(collision.transform.position);

            _enter_on_tile = _tile;

        }
    }

    private void OnEnable()
    {
        state.OnLevelComplete += DisableBlocker;
    }

    private void OnDisable()
    {
        state.OnLevelComplete -= DisableBlocker;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("next");
       if (collision.CompareTag("Body"))
        {
            Vector3Int _exit_on_tile = _tiles.WorldToCell(collision.transform.position);

            Vector2 direction = ((Vector2Int)(_exit_on_tile - _enter_on_tile));
            direction *= TriggerDirectionVector(TriggerType);
            direction.Normalize();

            Debug.Log(direction + " " + Time.time);

            if (direction != Vector2.zero)
            {

                if (state.IsLevelComplete(stats.Level) || LevelConnection != LevelRelation.Independent)
                {
                    if (direction.x > 0 || direction.y > 0)
                    {
                    if (LevelConnection == LevelRelation.Independent)
                            stats.Level = PositiveDirectionLevel;
                    }
                    else if (direction.x < 0 || direction.y < 0)
                    {
                    if (LevelConnection == LevelRelation.Independent)
                            stats.Level = NegativeDirectionLevel;
                    }
                    edgeCollider.CollisionAndCameraPanToNextLevel(direction + LevelOffset * (direction.x+direction.y));
                }
                else
                {
                    Blocker.enabled = true;
                }

                if (!state.IsLevelAnimationPlayed(stats.Level) && LevelConnection == LevelRelation.Independent)
                {
                    Invoke("RaiseNextLevelEvent", 1f);
                    OnLevelSwitch?.Invoke();
                }

            }
        }

    }

    void RaiseNextLevelEvent()
    {
        Debug.Log("onlevelbegin");
        OnLevelBegin.Raise();
    }

    void DisableBlocker(int level)
    {
        Blocker.enabled = false;
    }

    Vector2 TriggerDirectionVector(TriggerDirection direction)
    {
        //while (!_player.IsGround()) { }
        //if (direction == TriggerDirection.Horizontal)
        //    return new Vector2(1, 0);
        //else
        //    return new Vector2(0, 1);

        switch (direction)
        {
            case TriggerDirection.Horizontal:
                return new Vector2(1, 0);
            case TriggerDirection.Vertical:
                return new Vector2(0, 1);
            default :
                return new Vector2(0, 0);

        }
    }
}
