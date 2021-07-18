using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class MapUpdater : MonoBehaviour
{
    [SerializeField] private Map _map;
    [SerializeField] private TileUtils.Timer _waterTimer;
    [SerializeField] private TileUtils.Timer _waterGlobalTimer;

    private Dictionary<Vector3Int, Chunk> _liquidChunks;

    private void Start()
    {
        RegistryAllLiquidChunks();
    }

    private void Update()
    {
        WaterTimer();
        WaterGlobalTimer();
    }

    private void WaterTimer()
    {
        _waterTimer.ElapsedTime -= Time.deltaTime;
        
        if (!(_waterTimer.ElapsedTime < 0)) 
            return;
        
        UpdateWaterChunks();
        _waterTimer.ElapsedTime = _waterTimer.TimeDelay;
    }
    
    private void WaterGlobalTimer()
    {
        _waterGlobalTimer.ElapsedTime -= Time.deltaTime;
        
        if (!(_waterGlobalTimer.ElapsedTime < 0)) 
            return;
        
        RegistryAllLiquidChunks();
        _waterGlobalTimer.ElapsedTime = _waterGlobalTimer.TimeDelay;
    }

    private void RegistryAllLiquidChunks()
    {
        _liquidChunks = new Dictionary<Vector3Int, Chunk>();
        
        foreach (var chunk in _map.Chunks.Values)
        {
            chunk.SetEnabled(true);
            var isEnabledChunk = false;
            chunk.TryProcessChunk(cellPosition =>
            {
                if (_liquidChunks.ContainsKey(chunk.AnchorPosition))
                    return;
                
                if (!_map.Water.HasTile(cellPosition) && !_map.Lava.HasTile(cellPosition)) 
                    return;
                
                _liquidChunks.Add(chunk.AnchorPosition, chunk);
                isEnabledChunk = true;
            });
            
            chunk.SetEnabled(isEnabledChunk);
            
            _map.ChunkMarks[chunk].color = chunk.IsEnabled ? Color.green : Color.red;
        }
    }
    
    private void UpdateWaterChunks()
    {
        var addedChunks = new Dictionary<Vector3Int, Chunk>();
        var removedChunks = new Dictionary<Vector3Int, Chunk>();
        
        foreach (var chunk in _liquidChunks.Values)
        {
            var isEnabledChunk = false;
            
            chunk.TryProcessChunk(cellPosition =>
            {
                if (!_map.Water.HasTile(cellPosition))
                    return;

                isEnabledChunk = true;
                
                if (_map.Earth.HasTile(cellPosition + Vector3Int.down))
                    return;

                if (HasPossibleWaterMovement(cellPosition, Vector3Int.down))
                {
                    if (!chunk.CheckNeighborChunkForEnabled(cellPosition, out var liquidDirection))
                        return;

                    var neighborChunkPosition = chunk.AnchorPosition + liquidDirection;
                    
                    if (!_map.Chunks.ContainsKey(neighborChunkPosition))
                        return;
                    
                    if (_map.Chunks[neighborChunkPosition].IsEnabled)
                        return;
                    
                    if (addedChunks.ContainsKey(neighborChunkPosition))
                        return;
                    
                    addedChunks.Add(neighborChunkPosition, _map.Chunks[neighborChunkPosition]);
                }

                TileUtils.CalculateCountNearLateralVoid(_map.Water, _map.Earth, cellPosition, out var isRight);
                
                HasPossibleWaterMovement(cellPosition, isRight ? Vector3Int.right : Vector3Int.left);

                TileUtils.CalculateWaterPressure(_map.Water, cellPosition, Vector3Int.up);
                
            });
            
            chunk.SetEnabled(isEnabledChunk);
            
            _map.ChunkMarks[chunk].color = chunk.IsEnabled ? Color.green : Color.red;
            
            if (isEnabledChunk)
                continue;

            if (removedChunks.ContainsKey(chunk.AnchorPosition))
                continue;
            
            removedChunks.Add(chunk.AnchorPosition, chunk);
        }

        foreach (var addedChunk in addedChunks)
        {
            if (_liquidChunks.ContainsKey(addedChunk.Key))
                continue;
            
            _liquidChunks.Add(addedChunk.Key, addedChunk.Value);
            _liquidChunks[addedChunk.Key].SetEnabled(true);
        }
        
        foreach (var removedChunk in removedChunks)
        {
            _liquidChunks.Remove(removedChunk.Key);
        }
    }
    
    private bool HasPossibleWaterMovement(Vector3Int currentPosition, Vector3Int direction)
    {
        if (_map.Water.HasTile(currentPosition + direction)) 
            return false;
        
        _map.Water.SetTile(currentPosition + direction, _map.Water.GetTile(currentPosition));
        _map.Water.SetTile(currentPosition, null);
        
        return true;
    }
}
