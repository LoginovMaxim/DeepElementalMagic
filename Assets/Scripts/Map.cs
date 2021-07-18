using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Utils;

public class Map : MonoBehaviour
{
    public Dictionary<Vector3Int, Chunk> Chunks => _chunks;
    
    public Tilemap Earth => _earth;
    public EarthTileData EarthTileData => _earthTileData;
    
    public Tilemap Water => _water;
    public WaterTileData WaterTileData => _waterTileData;
    
    public Tilemap Lava => _lava;
    public LavaTileData LavaTileData => _lavaTileData;

    [SerializeField] private MapUpdater _mapUpdater;
    
    [Header("Map Parameters")]
    [SerializeField] private int ChunkSize;
    [SerializeField] private int CountWidthChunks;
    [SerializeField] private int CountHeightChunks;
    
    [Header("Cave Parameters")]
    [Range(0, 1)]
    [SerializeField] private float _sizeCavePath;
    [SerializeField] private float _widthCaveHole;
    [SerializeField] private float _heightCaveHole;
    
    [Header("Earth Layer")]
    [SerializeField] private int _maxEarthHeight;
    [SerializeField] private Tilemap _earth;
    [SerializeField] private EarthTileData _earthTileData;

    [Header("Water Layer")] 
    [SerializeField] private int _maxWaterHeight;
    [SerializeField] private Tilemap _water;
    [SerializeField] private WaterTileData _waterTileData;
    
    [Header("Lava Layer")]
    [SerializeField] private int _minLavaHeight;
    [SerializeField] private Tilemap _lava;
    [SerializeField] private LavaTileData _lavaTileData;

    private Dictionary<Vector3Int, Chunk> _chunks;
    
    public bool IsDrawChunkBounds;
    public SpriteRenderer MarkPrefab;
    public Dictionary<Chunk, SpriteRenderer> ChunkMarks;

    private void Start()
    {
        CreateChunks();
        CreateGround();
        
        if (IsDrawChunkBounds)
            DrawChunkBounds();

        _mapUpdater.enabled = true;
    }

    private void CreateChunks()
    {
        _chunks = new Dictionary<Vector3Int, Chunk>();
        ChunkMarks = new Dictionary<Chunk, SpriteRenderer>();

        var currentPosition = new Vector3Int();
        
        for (var y = 0; y < CountHeightChunks; y++)
        {
            currentPosition.y = y;
            
            for (var x = 0; x < CountWidthChunks; x++)
            {
                currentPosition.x = x;
                
                _chunks.Add(currentPosition, new Chunk(ChunkSize, currentPosition, true));
                ChunkMarks.Add(_chunks[currentPosition], Instantiate(MarkPrefab, new Vector3(currentPosition.x * ChunkSize + ChunkSize/2, currentPosition.y * ChunkSize + ChunkSize/2, 0) * TileUtils.MapCellSize, Quaternion.identity));
            }
        }
    }

    private void CreateGround()
    {
        foreach (var chunk in _chunks)
        {
            if (chunk.Key.y > _maxEarthHeight)
                continue;

            var currentSizeCave = _sizeCavePath - chunk.Key.y / 64f;
            
            chunk.Value.TryProcessChunk(cellPosition =>
            {
                if (Mathf.PerlinNoise(cellPosition.x / _widthCaveHole, cellPosition.y / _heightCaveHole) > currentSizeCave)
                {
                    _earth.SetTile(cellPosition, _earthTileData.GreenGrass);
                }
                
                if (chunk.Key.y >= _maxWaterHeight)
                {
                    if (Mathf.PerlinNoise(cellPosition.x / _widthCaveHole, cellPosition.y / _heightCaveHole) <
                        currentSizeCave * (1.66f * currentSizeCave))
                    {
                        _water.SetTile(cellPosition, _waterTileData.BlueWater);
                    }
                }
                
                /*
                if (chunk.Key.y <= _minLavaHeight)
                {
                    if (Mathf.PerlinNoise(cellPosition.x / _widthCaveHole, cellPosition.y / _heightCaveHole) <
                        currentSizeCave - 0.1f)
                    {
                        _lava.SetTile(cellPosition, _lavaTileData.RedLava);
                    }
                }
                */
            });
            
            chunk.Value.SetEnabled(true);

            ChunkMarks[chunk.Value].color = chunk.Value.IsEnabled ? Color.green : Color.red;
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
