using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    public static class Utils
    {
        public static Vector3Int[] Directions = 
        {
            Vector3Int.up, 
            Vector3Int.left, 
            Vector3Int.right,
            Vector3Int.down, 
            Vector3Int.up + Vector3Int.left,
            Vector3Int.up + Vector3Int.right,
            Vector3Int.down + Vector3Int.left,
            Vector3Int.down + Vector3Int.right,
        };
        
        public static Vector3Int[] FourDirections = 
        {
            Vector3Int.up, 
            Vector3Int.left, 
            Vector3Int.right,
            Vector3Int.down
        };

        public static Color MultiplyColor(Color currentColor, Color anchorColor)
        {
            return Color.black;
        }

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

        public static void CalculateWaterPressure(Tilemap tilemap, Vector3Int position)
        {
            if (tilemap.HasTile(position + Vector3Int.up))
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
                
                tilemap.SetTile(position + Vector3Int.up, tilemap.GetTile(abovePosition));
                tilemap.SetTile(abovePosition, null);
                return;
            }
        }

        public static bool PathFindingAboveWaterTile(float minYPosition, Tilemap tilemap, Vector3Int checkPosition, out Vector3Int abovePosition, ref Dictionary<Vector3Int, bool> logicPositions)
        {
            abovePosition = checkPosition;
            logicPositions[checkPosition] = true;
            
            for (var i = 0; i < 4; i++)
            {
                var position = checkPosition + FourDirections[i];
                
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
        
        [Serializable]
        public class Timer
        {
            public float TimeDelay;
            public float ElapsedTime;
        }
    }
}