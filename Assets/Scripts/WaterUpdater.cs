using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace DefaultNamespace
{
    public class WaterUpdater : CommonMapUpdater
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
                    if (_waterChunks.ContainsKey(chunk.AnchorPosition))
                        return;
                
                    if (!Map.Water.HasTile(cellPosition) && !Map.Lava.HasTile(cellPosition)) 
                        return;
                
                    _waterChunks.Add(chunk.AnchorPosition, chunk);
                    isEnabledChunk = true;
                });
            
                chunk.SetEnabled(isEnabledChunk);
            }
        }
        
        protected override void UpdateWaterChunks()
        {
            var addedChunks = new Dictionary<Vector3Int, Chunk>();
            var removedChunks = new Dictionary<Vector3Int, Chunk>();
            
            foreach (var chunk in _waterChunks.Values)
            {
                var isEnabledChunk = false;

                chunk.TryProcessChunk(cellPosition =>
                {
                    if (!Map.Water.HasTile(cellPosition))
                        return;

                    isEnabledChunk = true;
                    
                    if (!Map.Earth.HasTile(cellPosition + Vector3Int.down))
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

                    TileUtils.CalculateCountNearLateralVoid(Map.Water, Map.Earth, cellPosition, out var directionSign);
                    
                    switch (directionSign)
                    {
                        case 1:
                            HasPossibleWaterMovement(cellPosition, Vector3Int.right);
                            break;
                        case -1:
                            HasPossibleWaterMovement(cellPosition, Vector3Int.left);
                            break;
                    }

                    TileUtils.CalculateWaterPressure(Map.Water, Map.Earth, cellPosition, Vector3Int.up);
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
            if (Map.Water.HasTile(currentPosition + direction)) 
                return false;
        
            if (Map.Earth.HasTile(currentPosition + direction))
            {
                /*
                var tileBase = Map.Earth.GetTile(currentPosition + direction);
                if (tileBase != Map.EarthTileData.Absidian) 
                    return false;
                
                var lavaTileBase = Map.Lava.GetTile(currentPosition + direction + direction);
                if (lavaTileBase == null)
                    return false;

                var waterTileBase = Map.Water.GetTile(currentPosition - direction);
                if (waterTileBase == null)
                    return false;
                
                Map.Water.SetTile(currentPosition - direction, null);
                Map.Lava.SetTile(currentPosition + direction + direction, null);
                Map.Earth.SetTile(currentPosition + direction + direction, Map.EarthTileData.Absidian);
                */
                
                return false;
            }

            if (Map.Lava.HasTile(currentPosition + direction))
            {
                Map.Water.SetTile(currentPosition, null);
                Map.Lava.SetTile(currentPosition + direction, null);
                Map.Earth.SetTile(currentPosition + direction, Map.EarthTileData.Absidian);
                return false;
            }
        
            Map.Water.SetTile(currentPosition + direction, Map.Water.GetTile(currentPosition));
            Map.Water.SetTile(currentPosition, null);
        
            return true;
        }
    
        private bool HasNotDestroyWaterMovement(Vector3Int currentPosition, Vector3Int direction)
        {
            if (Map.Water.HasTile(currentPosition + direction)) 
                return false;
        
            if (Map.Earth.HasTile(currentPosition + direction)) 
                return false;
        
            Map.Water.SetTile(currentPosition + direction, Map.Water.GetTile(currentPosition));
        
            return true;
        }
    }
}