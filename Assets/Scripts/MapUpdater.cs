using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class MapUpdater : MonoBehaviour
{
    private List<IMapUpdater> _mapUpdaters;
    private Map _map;

    private void Start()
    {
        _mapUpdaters = new List<IMapUpdater>();
        _mapUpdaters.AddRange(GetComponents<IMapUpdater>());
        _map = FindObjectOfType<Map>();
    }

    private void Update()
    {
        foreach (var mapUpdater in _mapUpdaters)
        {
            mapUpdater.DoUpdate();
        }

        foreach (var mapUpdater in _mapUpdaters)
        {
            while (mapUpdater.LiquidTiles.Count > 0)
            {
                UpdateTilemap(mapUpdater.LiquidTiles.Dequeue());
            }
        }
    }
    
    public void UpdateTilemap(TileCode tileCode)
    {
        foreach (var tilemap in _map.MatrixTilemap.TilemapByCodes.Values)
        {
            tilemap.SetTile(tileCode.Position, null);
        }
        
        if (tileCode.Code == 0)
            return;
        
        _map.MatrixTilemap.TilemapByCodes[tileCode.Code].SetTile(
            tileCode.Position, _map.MatrixTilemap.TileByCodes[tileCode.Code]);
    }
}

public struct TileCode
{
    public Vector3Int Position;
    public byte Code;

    public TileCode(Vector3Int position, byte code)
    {
        Position = position;
        Code = code;
    }
}
