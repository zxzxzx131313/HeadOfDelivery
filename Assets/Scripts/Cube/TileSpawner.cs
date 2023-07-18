using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSpawner : MonoBehaviour
{
    [SerializeField]
    private Tile _tile;
    private Tilemap _headTile;
    private HeadDice _dice;
    private GameObject _body;
    // Start is called before the first frame update
    void Start()
    {
        _dice = GetComponent<HeadDice>();
        _body = GameObject.FindGameObjectWithTag("Body");
        //_tile = Resources.Load<Tile>("TilePalette/Platform/tile_normal");
        foreach (var tm in GameObject.FindObjectsOfType<Tilemap>())
        {
            if (tm.CompareTag("Headtile"))
                _headTile = tm;
        }
    }

    /**
     * <summary>Spawn a platform tile at the given position</summary>
     * <param name="position">the position to spawn the tile</param>
     */
    public void SpawnTile(Vector3 position)
    {
        var pos = _headTile.WorldToCell(position);
        var body_pos = _headTile.WorldToCell(_body.transform.position);
        var head_pos = _headTile.WorldToCell(_body.transform.position + Vector3.up);
        if (!_headTile.HasTile(pos) && pos != body_pos && pos != head_pos)
        {
            _headTile.SetTile(pos, _tile);
        }
    }

    public void Restart()
    {
        _headTile.ClearAllTiles();
    }
}
