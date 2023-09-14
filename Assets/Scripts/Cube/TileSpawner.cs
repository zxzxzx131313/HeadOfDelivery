using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;


public class TileSpawner : MonoBehaviour
{
    [SerializeField] private RuleTile _tile;
    [SerializeField] private LevelStats stats;
    [SerializeField] private GameStateSave states;
    private Tilemap _headTile;
    private int tiles_count;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var tm in GameObject.FindObjectsOfType<Tilemap>())
        {
            if (tm.CompareTag("Headtile"))
                _headTile = tm;
        }

    }

    private void OnEnable()
    {
        stats.LevelChanged += AddTileCount;
    }

    private void OnDisable()
    {
        stats.LevelChanged -= AddTileCount;
    }

    /**
     * <summary>Spawn a platform tile at the given position</summary>
     * <param name="position">the position to spawn the tile in cell space</param>
     */
    public void SpawnTile(Vector3Int position, float inkLeft)
    {

        if (!_headTile.HasTile(position))
        {
            //_tile.color = new Vector4(1, 1, 1, 1f*inkLeft);
            _headTile.SetTile(position, _tile);
            tiles_count++;
        }
    }

    public bool HasHeadTile(Vector3Int position)
    {
        return _headTile.HasTile(position);
    }

    public void HideTile(Vector3Int position)
    {
        Color hide = new Color(1f, 1f, 1f, 0f);
        _headTile.SetColor(position, hide);
    }

    public void ShowTileDelay(Vector3Int position)
    {
        StartCoroutine("ShowTile", position);
    }

    IEnumerator ShowTile(Vector3Int position)
    {
        yield return new WaitForSeconds(0.2f);

        Color hide = new Color(1f, 1f, 1f, 1f);
        _headTile.SetColor(position, hide);
    }

    public void Restart()
    {
        _headTile.ClearAllTiles();
        tiles_count = 0;
    }

    void AddTileCount(int level)
    {
        states.StampCount += tiles_count;
        tiles_count = 0;
    }
}
