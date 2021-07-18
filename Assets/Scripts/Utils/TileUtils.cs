using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Utils
{
    public static class TileUtils
    {
        public const float MapCellSize = 0.16f;

        public static void CalculateAllTilemapDown(Tilemap tilemap, Action<Vector3Int> doSomething)
        {
            var currentTile = new Vector3Int();
            
            for (var x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
            {
                for (var y = tilemap.cellBounds.yMax; y > tilemap.cellBounds.yMin; y--)
                {
                    currentTile.x = x;
                    currentTile.y = y;
                    
                    if (!tilemap.HasTile(currentTile))
                        continue;
                    
                    doSomething?.Invoke(currentTile);
                }
            }
        }
        
        public static void CalculateAllTilemapUp(Tilemap tilemap, Action<Vector3Int> doSomething)
        {
            var currentTile = new Vector3Int();
            
            for (var x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
            {
                for (var y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
                {
                    currentTile.x = x;
                    currentTile.y = y;

                    if (!tilemap.HasTile(currentTile))
                        continue;

                    doSomething?.Invoke(currentTile);
                }
            }
        }
        
        public static void HorizontalCalculateAllTilemapUp(Tilemap tilemap, Action<Vector3Int> doSomething)
        {
            var currentTile = new Vector3Int();
            
            for (var y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                for (var x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
                {
                    currentTile.x = x;
                    currentTile.y = y;

                    if (!tilemap.HasTile(currentTile))
                        continue;

                    doSomething?.Invoke(currentTile);
                }
            }
        }

        public static void CalculateCountNearLateralVoid(Tilemap tilemap, Tilemap staticTilemap, Vector3Int position, out bool isRight)
        {
            var rightOffset = position + Vector3Int.right + Vector3Int.down;
            var leftOffset = position + Vector3Int.left + Vector3Int.down;
            
            for (var x = 0; x < 100; x++)
            {
                if (!tilemap.HasTile(leftOffset) && !staticTilemap.HasTile(leftOffset))
                {
                    isRight = false;
                    return;
                }
                
                if (!tilemap.HasTile(rightOffset) && !staticTilemap.HasTile(rightOffset))
                {
                    isRight = true;
                    return;
                }
                
                rightOffset += Vector3Int.right;
                leftOffset += Vector3Int.left;
            }

            isRight = true;
        }

        public static void CalculateWaterPressure(Tilemap tilemap, Vector3Int position, Vector3Int flowDirection)
        {
            if (tilemap.HasTile(position + flowDirection))
                return;
            
            var logicPositions = new Dictionary<Vector3Int, bool>();
            var minYPosition = position.y;

            var checkPosition = position;
            logicPositions.Add(checkPosition, true);
            
            while (true)
            {
                if (!PathFindingAboveWaterTile(minYPosition, tilemap, checkPosition, out var abovePosition,
                    ref logicPositions))
                {
                    checkPosition = logicPositions.FirstOrDefault(l => l.Value == false).Key;
                    
                    if (checkPosition == default)
                        return;
                    
                    continue;
                }
                
                tilemap.SetTile(position + flowDirection, tilemap.GetTile(abovePosition));
                tilemap.SetTile(abovePosition, null);
                return;
            }
        }

        public static bool PathFindingAboveWaterTile(float minYPosition, Tilemap tilemap, Vector3Int checkPosition, out Vector3Int abovePosition, ref Dictionary<Vector3Int, bool> logicPositions)
        {
            abovePosition = checkPosition;
            logicPositions[checkPosition] = true;
            
            for (var i = 0; i < DirectionUtils.FourDirections.Length; i++)
            {
                var position = checkPosition + DirectionUtils.FourDirections[i];
                
                if (!tilemap.HasTile(position))
                    continue;

                if (logicPositions.ContainsKey(position) && logicPositions[position])
                    continue;
                
                if (minYPosition < position.y)
                {
                    abovePosition = position;
                    return true;
                }
                
                if (!logicPositions.ContainsKey(position))
                {
                    logicPositions.Add(position, false);
                }
                else
                {
                    logicPositions[position] = true;
                }
            }
            
            return false;
        }

        public static float GetNormalizeValue(float maxValue, float currentValue)
        {
            return currentValue / maxValue;
        }
        
        /*
        public static void CalculateCave(Tilemap tilemap, Vector3Int position)
        {
            var logicPositions = new Dictionary<Vector3Int, bool>();

            var checkPosition = position;
            logicPositions.Add(checkPosition, true);
            tilemap.SetTile(checkPosition, null);

            var chance = 1;
            var chanceStep = 16;
            var elapsedChanceStep = chanceStep;
            
            while (true)
            {
                if (!HorizontalPathFindingCaveTile(chance, tilemap, checkPosition, ref logicPositions))
                {
                    checkPosition = logicPositions.FirstOrDefault(l => l.Value == false).Key;
                    
                    if (checkPosition == default)
                        return;

                    elapsedChanceStep--;

                    if (elapsedChanceStep < 0)
                    {
                        elapsedChanceStep = chanceStep;
                        chance++;
                    }
                    
                    continue;
                }
                
                return;
            }
        }

        public static bool PathFindingCaveTile(int chance, Tilemap tilemap, Vector3Int checkPosition, ref Dictionary<Vector3Int, bool> logicPositions)
        {
            logicPositions[checkPosition] = true;
            
            for (var i = 0; i < DirectionUtils.EightDirections.Length; i++)
            {
                var position = checkPosition + DirectionUtils.EightDirections[i];
                
                if (!tilemap.HasTile(position))
                    continue;

                if (logicPositions.ContainsKey(position) && logicPositions[position])
                    continue;
                
                if (!logicPositions.ContainsKey(position))
                {
                    logicPositions.Add(position, false);
                }
                else
                {
                    logicPositions[position] = true;
                }

                if (Random.Range(0, chance) != 0) 
                    continue;
                
                for (var j = 0; j < DirectionUtils.EightDirections.Length; j++)
                {
                    if (tilemap.HasTile(position + DirectionUtils.EightDirections[j])) 
                        continue;
                    
                    tilemap.SetTile(position, null);
                    break;
                }
            }
            
            return false;
        }
        
        public static bool HorizontalPathFindingCaveTile(int chance, Tilemap tilemap, Vector3Int checkPosition, ref Dictionary<Vector3Int, bool> logicPositions)
        {
            logicPositions[checkPosition] = true;

            for (var i = 0; i < 2; i++)
            {
                var position = checkPosition + DirectionUtils.MoreDiagonalDirections[Random.Range(0, DirectionUtils.MoreDiagonalDirections.Length)];
                
                if (!tilemap.HasTile(position))
                    continue;

                if (logicPositions.ContainsKey(position) && logicPositions[position])
                    continue;
                
                if (!logicPositions.ContainsKey(position))
                {
                    logicPositions.Add(position, false);
                }
                else
                {
                    logicPositions[position] = true;
                }

                if (Random.Range(0, chance) != 0) 
                    continue;
                
                for (var j = 0; j < DirectionUtils.EightDirections.Length; j++)
                {
                    if (tilemap.HasTile(position + DirectionUtils.EightDirections[j])) 
                        continue;
                    
                    tilemap.SetTile(position, null);
                    break;
                }
            }
            
            return false;
        }
        */
        
        [Serializable]
        public class Timer
        {
            public float TimeDelay;
            public float ElapsedTime;
        }
    }
}