using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace DefaultNamespace
{
    public class WaterUpdater : LiquidMapUpdater
    {
        private Dictionary<Vector3Int, Chunk> _waterChunks;

        protected override void RegistryAllLiquidChunks()
        {
            _waterChunks = new Dictionary<Vector3Int, Chunk>();
        
            foreach (var chunk in Map.Chunks.Values)
            {
                chunk.SetEnabled(true);
                var isEnabledChunk = false;
                
                chunk.TryProcessChunk(cellPosition =>
                {
                    if (isEnabledChunk)
                        return;
                    
                    if (_waterChunks.ContainsKey(chunk.AnchorPosition))
                        return;
                
                    if (Map.MatrixTilemap.Codes[cellPosition.x, cellPosition.y] != 
                        Map.MatrixTilemap.CodeByTiles[Map.WaterTileData.BlueWater])
                        return;
                
                    _waterChunks.Add(chunk.AnchorPosition, chunk);
                    isEnabledChunk = true;
                });
            
                chunk.SetEnabled(isEnabledChunk);
            }
        }
        
        protected override void UpdateLiquidChunks()
        {
            var addedChunks = new Dictionary<Vector3Int, Chunk>();
            var removedChunks = new Dictionary<Vector3Int, Chunk>();
            
            foreach (var chunk in _waterChunks.Values)
            {
                var isEnabledChunk = false;
                chunk.SetEnabled(true);

                chunk.TryProcessChunk(cellPosition =>
                {
                    if (Map.MatrixTilemap.Codes[cellPosition.x, cellPosition.y] !=
                        Map.MatrixTilemap.CodeByTiles[Map.WaterTileData.BlueWater])
                        return;

                    isEnabledChunk = true;
                    
                    if (cellPosition.y - 1 >=0 && 
                        Map.MatrixTilemap.Codes[cellPosition.x, cellPosition.y - 1] != 
                        Map.MatrixTilemap.CodeByTiles[Map.EarthTileData.GreenGrass])
                    {
                        if (HasPossibleWaterMovement(cellPosition, Vector3Int.down))
                        {
                            if (!chunk.CheckNeighborChunkForEnabled(cellPosition, out var liquidDirection))
                                return;
                            
                            var neighborChunkPosition = chunk.AnchorPosition + liquidDirection;

                            if (!Map.Chunks.ContainsKey(neighborChunkPosition))
                                return;

                            if (Map.Chunks[neighborChunkPosition].IsEnabled)
                                return;

                            if (addedChunks.ContainsKey(neighborChunkPosition))
                                return;

                            addedChunks.Add(neighborChunkPosition, Map.Chunks[neighborChunkPosition]);
                        }
                    }
                    
                    TileUtils.CalculateCountNearLateralVoid(
                        Map.MatrixTilemap, 
                        Map.MatrixTilemap.CodeByTiles[Map.WaterTileData.BlueWater], 
                        cellPosition, 
                        out var directionSign);
                    
                    switch (directionSign)
                    {
                        case 1:
                            HasPossibleWaterMovement(cellPosition, Vector3Int.right);
                            break;
                        case -1:
                            HasPossibleWaterMovement(cellPosition, Vector3Int.left);
                            break;
                    }

                    //TileUtils.CalculateWaterPressure(MatrixTilemap.CodeByTiles[Map.WaterTileData.BlueWater], cellPosition, Vector3Int.up);
                });
                
                chunk.SetEnabled(isEnabledChunk);
                
                if (isEnabledChunk)
                    continue;

                if (removedChunks.ContainsKey(chunk.AnchorPosition))
                    continue;
                
                removedChunks.Add(chunk.AnchorPosition, chunk);
            }

            foreach (var addedChunk in addedChunks)
            {
                if (_waterChunks.ContainsKey(addedChunk.Key))
                    continue;
                
                _waterChunks.Add(addedChunk.Key, addedChunk.Value);
                _waterChunks[addedChunk.Key].SetEnabled(true);
            }
            
            foreach (var removedChunk in removedChunks)
            {
                _waterChunks.Remove(removedChunk.Key);
            }
        }
        
        private bool HasPossibleWaterMovement(Vector3Int currentPosition, Vector3Int direction)
        {
            var flowPosition = currentPosition + direction;
            
            if (Map.MatrixTilemap.Codes[flowPosition.x, flowPosition.y] == 
                Map.MatrixTilemap.CodeByTiles[Map.WaterTileData.BlueWater])
                return false;
            
            if (Map.MatrixTilemap.Codes[flowPosition.x, flowPosition.y] == 
                Map.MatrixTilemap.CodeByTiles[Map.EarthTileData.GreenGrass])
                return false;
            
            if (Map.MatrixTilemap.Codes[flowPosition.x, flowPosition.y] == 
                Map.MatrixTilemap.CodeByTiles[Map.AbsidianTileData.PurpleAbsidian])
                return false;

            if (Map.MatrixTilemap.Codes[flowPosition.x, flowPosition.y] == 
                Map.MatrixTilemap.CodeByTiles[Map.LavaTileData.RedLava])
            {
                Map.MatrixTilemap.Codes[flowPosition.x, flowPosition.y] = 
                    Map.MatrixTilemap.CodeByTiles[Map.AbsidianTileData.PurpleAbsidian];
                LiquidTiles.Enqueue(new TileCode(flowPosition, Map.MatrixTilemap.Codes[flowPosition.x, flowPosition.y]));
                //CodeChanged?.Invoke(flowPosition, Map.MatrixTilemap.Codes[flowPosition.x, flowPosition.y]);
                
                Map.MatrixTilemap.Codes[currentPosition.x, currentPosition.y] = 0;
                LiquidTiles.Enqueue(new TileCode(currentPosition, Map.MatrixTilemap.Codes[currentPosition.x, currentPosition.y]));
                //CodeChanged?.Invoke(currentPosition, Map.MatrixTilemap.Codes[currentPosition.x, currentPosition.y]);
                return false;
            }
        
            Map.MatrixTilemap.Codes[flowPosition.x, flowPosition.y] = 
                Map.MatrixTilemap.CodeByTiles[Map.WaterTileData.BlueWater];
            LiquidTiles.Enqueue(new TileCode(flowPosition, Map.MatrixTilemap.Codes[flowPosition.x, flowPosition.y]));
            //CodeChanged?.Invoke(flowPosition, Map.MatrixTilemap.Codes[flowPosition.x, flowPosition.y]);
            
            Map.MatrixTilemap.Codes[currentPosition.x, currentPosition.y] = 0;
            LiquidTiles.Enqueue(new TileCode(currentPosition, Map.MatrixTilemap.Codes[currentPosition.x, currentPosition.y]));
            //CodeChanged?.Invoke(currentPosition, Map.MatrixTilemap.Codes[currentPosition.x, currentPosition.y]);
        
            return true;
        }
    }
}