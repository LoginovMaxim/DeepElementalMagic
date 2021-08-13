using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

public class Liquid : MonoBehaviour
{
    public bool IsHorizontalCalculate;
    
    [SerializeField] private Tilemap _staticTilemap;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private TileUtils.Timer _timer;

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
        /*
        if (IsHorizontalCalculate)
        {
            TileUtils.HorizontalCalculateAllTilemapUp(_tilemap, currentTilePosition =>
            {
                if (_staticTilemap.HasTile(currentTilePosition + Vector3Int.down))
                    return;

                if (CheckTile(currentTilePosition, Vector3Int.down))
                    return;

                //TileUtils.CalculateCountNearLateralVoid(_tilemap, _staticTilemap, currentTilePosition, out var isRight);

                //CheckTile(currentTilePosition, isRight ? Vector3Int.right : Vector3Int.left);

                TileUtils.CalculateWaterPressure(_tilemap, currentTilePosition, Vector3Int.up);
            });
        }
        else
        {
            TileUtils.CalculateAllTilemapUp(_tilemap, currentTilePosition =>
            {
                if (_staticTilemap.HasTile(currentTilePosition + Vector3Int.down))
                    return;

                if (CheckTile(currentTilePosition, Vector3Int.down))
                    return;

                //TileUtils.CalculateCountNearLateralVoid(_tilemap, _staticTilemap, currentTilePosition, out var isRight);

                //CheckTile(currentTilePosition, isRight ? Vector3Int.right : Vector3Int.left);

                TileUtils.CalculateWaterPressure(_tilemap, currentTilePosition, Vector3Int.up);
            });
        }
        */
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
