using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum LevelRelation { PosIncludeNeg, NegIncludePos, Independent};
public enum TriggerDirection { Horizontal, Vertical };
public class LevelSwitchTrigger : MonoBehaviour
{
    [Header("Trigger stats")]
    [SerializeField] private int NegativeDirectionLevel;
    [SerializeField] private int PositiveDirectionLevel;
    [SerializeField] private TriggerDirection TriggerType;
    [SerializeField] private LevelRelation LevelConnection;

    [Header("Game States")]
    [SerializeField] private LevelStats stats;
    [SerializeField] private GameStateSave state;
    [SerializeField] private GameEvent OnLevelBegin;

    EdgeColliderSetting edgeCollider;
    Tilemap _tiles;

    Vector3Int _enter_on_tile;
    PhysicsCheck _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Body").GetComponent<PhysicsCheck>();
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("next");
        if (collision.CompareTag("Body"))
        {
            Vector3Int _exit_on_tile = _tiles.WorldToCell(collision.transform.position);

            Vector2 direction = ((Vector2Int)(_exit_on_tile - _enter_on_tile)) * TriggerDirectionVector(TriggerType);
            direction.Normalize();
            Debug.Log(direction + " " + Time.time);
            edgeCollider.CollisionAndCameraPanToNextLevel(direction);


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


            if (!state.IsLevelAnimationPlayed(stats.Level) && LevelConnection == LevelRelation.Independent)
            {
                Invoke("RaiseNextLevelEvent", 1f);
            }
            

        }

    }

    void RaiseNextLevelEvent()
    {
        Debug.Log("onlevelbegin");
        OnLevelBegin.Raise();
    }


    Vector2 TriggerDirectionVector(TriggerDirection direction)
    {
        //while (!_player.IsGround()) { }
        if (direction == TriggerDirection.Horizontal)
            return new Vector2(1, 0);
        else
            return new Vector2(0, 1);
    }


}
