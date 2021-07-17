using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

public class Map : MonoBehaviour
{
    public Dictionary<Vector3Int, Chunk> Chunks => _chunks;
    
    [SerializeField] private int ChunkSize;
    [SerializeField] private int MapSizeByChunks;
    
    [Header("Earth Layer")]
    [SerializeField] private Tilemap _earth;
    [SerializeField] private EarthTileData _earthTileData;
    
    [Header("Water Layer")]
    [SerializeField] private Tilemap _water;
    [SerializeField] private EarthTileData _waterTileData;

    private Dictionary<Vector3Int, Chunk> _chunks;
    
    public bool IsDrawChunkBounds;
    
    private void Start()
    {
        CreateChunks();
        CreateGround();
        
        if (IsDrawChunkBounds)
            DrawChunkBounds();
    }

    private void CreateChunks()
    {
        _chunks = new Dictionary<Vector3Int, Chunk>();

        var currentPosition = new Vector3Int();
        
        for (var y = 0; y < MapSizeByChunks; y++)
        {
            currentPosition.y = y;
            
            for (var x = 0; x < MapSizeByChunks; x++)
            {
                currentPosition.x = x;
                
                _chunks.Add(currentPosition, new Chunk(ChunkSize, currentPosition, true));
            }
        }
    }

    private void CreateGround()
    {
        foreach (var chunk in _chunks)
        {
            chunk.Value.TryProcessChunk(cellPosition =>
            {
                _earth.SetTile(cellPosition, _earthTileData.GreenGrass);
            });
        }
    }

    private void DrawChunkBounds()
    {
        foreach (var chunk in _chunks)
        {
            DebugDrawUtils.DrawSquare(
                chunk.Value.GetVertexPosition(0),
                chunk.Value.GetVertexPosition(1),
                chunk.Value.GetVertexPosition(2),
                chunk.Value.GetVertexPosition(3),
                Color.magenta);
        }
    }
}
