using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HeadDropOffTile : MonoBehaviour
{
    Tilemap _tile;
    EdgeCollider2D _edge;
    void Start()
    {
        _tile = GetComponent<Tilemap>();
        _edge = GameObject.FindGameObjectWithTag("EdgeCollider").GetComponent<EdgeCollider2D>();
    }

    public void GetTileInView()
    {
        BoundsInt bound = new BoundsInt();
        bound.min = _tile.WorldToCell(_edge.points[0]);
        bound.max = _tile.WorldToCell(_edge.points[2]);
        TileBase[] tiles = _tile.GetTilesBlock(bound);
    }
}
