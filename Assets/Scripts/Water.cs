using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Water : MonoBehaviour
{
    [SerializeField] private Tilemap _staticTilemap;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Utils.Timer _timer;

    private void Update()
    {
        _timer.ElapsedTime -= Time.deltaTime;
        
        if (!(_timer.ElapsedTime < 0)) 
            return;
        
        DoSimulate();
        _timer.ElapsedTime = _timer.TimeDelay;
    }

    private void DoSimulate()
    {
        Utils.CalculateAllTilemapUp(_tilemap, currentTilePosition =>
        {
            if (_staticTilemap.HasTile(currentTilePosition + Vector3Int.down))
                return;
            
            if (CheckTile(currentTilePosition, Vector3Int.down))
                return;

            Utils.CalculateCountNearLateralVoid(_tilemap, _staticTilemap, currentTilePosition, out var isRight);

            CheckTile(currentTilePosition, isRight ? Vector3Int.right : Vector3Int.left);
            
            Utils.CalculateWaterPressure(_tilemap, currentTilePosition);
        });
    }

    private bool CheckTile(Vector3Int currentPosition, Vector3Int direction)
    {
        if (_tilemap.HasTile(currentPosition + direction)) 
            return false;
        
        _tilemap.SetTile(currentPosition + direction, _tilemap.GetTile(currentPosition));
        _tilemap.SetTile(currentPosition, null);
        
        return true;
    }
}
