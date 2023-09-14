using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class LevelCompleteTrigger : MonoBehaviour
{
    [SerializeField] private LevelStats stats;
    [SerializeField] private GameStateSave state;

    //public GameEvent OnNextLevelStart;
    //public GameEvent OnChangeLevel;

    Tilemap _tiles;
    //Vector3Int _enter_on_tile;
    //Vector3Int _last_exit_tile;

    // note: Tiles are made 2 units high to prevent player jump and enter into the collision area which results in initial starting position offset.
    private void Start()
    {
        _tiles = GetComponent<Tilemap>();
        //_enter_on_tile = Vector3Int.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Body"))
        {
            //Vector3Int _tile = _tiles.WorldToCell(collision.transform.position);

            //_enter_on_tile = _tile;
            state.SetLevelComplete(stats.Level);
            stats.ClearCurrentLevelSteps();

        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    //Debug.Log("next");
    //    if (collision.CompareTag("Body"))
    //    {
    //        Vector3Int _exit_on_tile = _tiles.WorldToCell(collision.transform.position);
    //        if (_last_exit_tile.x < _exit_on_tile.x && _exit_on_tile.x - _enter_on_tile.x > 0)
    //        {

    //            stats.Level++;

    //            //OnChangeLevel.Raise();
    //            // show animation a bit later
    //            Invoke("RaiseNextLevelEvent", 1f);

    //            _last_exit_tile = _exit_on_tile;
    //        }

    //    }

    //}
}
