using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelTrigger : MonoBehaviour
{
    [Header("Trigger stats")]
    [SerializeField] private int NegativeDirectionLevel;
    [SerializeField] private int PositiveDirectionLevel;
    [SerializeField] private Vector2 TriggerType;

    [Header("Game States")]
    [SerializeField] private LevelStats stats;
    [SerializeField] private GameStateSave state;
    [SerializeField] private GameEvent OnLevelBegin;

    EdgeColliderSetting edgeCollider;
    Tilemap _tiles;

    Vector3Int _enter_on_tile;
    // Start is called before the first frame update
    void Start()
    {
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

            Vector2 direction = ((Vector2Int)(_exit_on_tile - _enter_on_tile)) * TriggerType;
            direction.Normalize();
            Debug.Log(direction);
            edgeCollider.CollisionAndCameraPanToNextLevel(direction);

            if (direction.x > 0 || direction.y > 0)
                stats.Level = PositiveDirectionLevel;
            else if (direction.x < 0 || direction.y < 0)
                stats.Level = NegativeDirectionLevel;

            //if (_exit_on_tile.x - _enter_on_tile.x > 0 || _exit_on_tile.y - _enter_on_tile.y > 0)
            //{

            //    //stats.Level++;

            //    //OnChangeLevel.Raise();
            //    // show animation a bit later
            //    ///Invoke("RaiseNextLevelEvent", 1f);
            //    stats.Level = PositiveDirectionLevel;
            //}
            //else
            //{
            //    stats.Level = NegativeDirectionLevel;

            //}
            if (!state.IsLevelAnimationPlayed(stats.Level))
            {
                Invoke("RaiseNextLevelEvent", 1f);
            }

        }

    }

    void RaiseNextLevelEvent()
    {

        OnLevelBegin.Raise();
    }





}
