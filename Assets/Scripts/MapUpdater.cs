using System;
using UnityEngine;

public class MapUpdater : MonoBehaviour
{
    [SerializeField] private Map _map;

    private void Update()
    {
        
    }

    private void UpdateWaterChunks()
    {
        foreach (var chunk in _map.Chunks.Values)
        {
            chunk.TryProcessChunk(cellPosition =>
            {

            });
        }
    }
}
