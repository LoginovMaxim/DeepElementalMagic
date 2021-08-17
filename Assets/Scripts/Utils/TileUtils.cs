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

        public static void CalculateCountNearLateralVoid(
            MatrixTilemap matrixTilemap, 
            byte selfCode,
            Vector3Int position, 
            out int directionSign)
        {
            var rightOffset = position + Vector3Int.right;
            var leftOffset = position + Vector3Int.left;

            for (var x = 0; x < MapCellSize * 100; x++)
            {
                if (matrixTilemap.TryGetCode(rightOffset, out var codeRight))
                {
                    if (codeRight == 0)
                    {
                        directionSign = 1;
                        return;
                    }
                }
                
                if (matrixTilemap.TryGetCode(leftOffset, out var codeLeft))
                {
                    if (codeLeft == 0)
                    {
                        directionSign = -1;
                        return;
                    }
                }
        
                rightOffset += Vector3Int.right;
                leftOffset += Vector3Int.left;
            }

            directionSign = 0;
        }

        public static void CalculateWaterPressure(MatrixTilemap matrixTilemap, byte codeTile, Vector3Int position, Vector3Int direction)
        {
            var flowDirection = position + direction;
            
            if (matrixTilemap.Codes[flowDirection.x, flowDirection.y] % 2 != 0)
                return;
            
            if (matrixTilemap.Codes[flowDirection.x, flowDirection.y] == codeTile)
                return;
            
            var logicPositions = new Dictionary<Vector3Int, bool>();
            var minYPosition = position.y;

            var checkPosition = position;
            logicPositions.Add(checkPosition, true);
            
            while (true)
            {
                if (!PathFindingAboveWaterTile(matrixTilemap, minYPosition, codeTile, checkPosition, out var abovePosition,
                    ref logicPositions))
                {
                    checkPosition = logicPositions.FirstOrDefault(l => l.Value == false).Key;
                    
                    if (checkPosition == default)
                        return;
                    
                    continue;
                }
                
                matrixTilemap.Codes[flowDirection.x, flowDirection.y] = codeTile;
                matrixTilemap.Codes[abovePosition.x, abovePosition.y] = 0;
                return;
            }
        }

        private static bool PathFindingAboveWaterTile(
            MatrixTilemap matrixTilemap, 
            float minYPosition, 
            byte codeTile, 
            Vector3Int checkPosition, 
            out Vector3Int abovePosition, 
            ref Dictionary<Vector3Int, bool> logicPositions)
        {
            abovePosition = checkPosition;
            logicPositions[checkPosition] = true;
            
            for (var i = 0; i < DirectionUtils.FourDirections.Length; i++)
            {
                var position = checkPosition + DirectionUtils.FourDirections[i];
                
                if (matrixTilemap.Codes[position.x, position.y] != codeTile)
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

        public static Vector3Int GetFlowDirectionByMagneticPosition(Vector3Int magneticPosition, Vector3Int currentPosition)
        {
            var direction = Vector3Int.zero;

            if (magneticPosition.y > currentPosition.y)
            {
                direction += Vector3Int.up;
            }
            else if (magneticPosition.y < currentPosition.y)
            {
                direction += Vector3Int.down;
            }
            
            if (magneticPosition.x > currentPosition.x)
            {
                direction += Vector3Int.right;
            }
            else if (magneticPosition.x < currentPosition.x)
            {
                direction += Vector3Int.left;
            }

            return direction;
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