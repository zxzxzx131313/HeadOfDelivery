using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSpawner : MonoBehaviour
{
    [SerializeField]
    private Tile _tile;
    private Tilemap _headTile;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var tm in GameObject.FindObjectsOfType<Tilemap>())
        {
            if (tm.CompareTag("Headtile"))
                _headTile = tm;
        }
    }

    /**
     * <summary>Spawn a platform tile at the given position</summary>
     * <param name="position">the position to spawn the tile in cell space</param>
     */
    public void SpawnTile(Vector3Int position)
    {

        if (!_headTile.HasTile(position))
        {
            _headTile.SetTile(position, _tile);
        }
    }

    public void Restart()
    {
        _headTile.ClearAllTiles();
    }
}
