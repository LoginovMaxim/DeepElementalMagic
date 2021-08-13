using System;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

public class Map : MonoBehaviour
{
    public int ChunkSize => _chunkSize;
    
    public Dictionary<Vector3Int, Chunk> Chunks => _chunks;
    
    public Tilemap Earth => _earth;
    public EarthTileData EarthTileData => _earthTileData;
    
    public Tilemap Water => _water;
    public WaterTileData WaterTileData => _waterTileData;
    
    public Tilemap Lava => _lava;
    public LavaTileData LavaTileData => _lavaTileData;

    [SerializeField] private MapUpdater _mapUpdater;
    
    [Header("Map Parameters")]
    [SerializeField] private int _chunkSize;
    [SerializeField] private int _chunkFrozenSteps;
    [SerializeField] private int CountWidthChunks;
    [SerializeField] private int CountHeightChunks;
    
    [Header("Cave Parameters")]
    [Range(0, 1)]
    [SerializeField] private float _sizeCavePath;
    [SerializeField] private float _dampingValue;
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
    [SerializeField] [Range(0.1f, 1.9f)]private float _waterCoefficient;
    
    [Header("Lava Layer")]
    [SerializeField] private int _minLavaHeight;
    [SerializeField] private Tilemap _lava;
    [SerializeField] private LavaTileData _lavaTileData;

    private Dictionary<Vector3Int, Chunk> _chunks;

    [Header("Tools")] 
    public bool IsCreateMode;
    public bool IsDrawChunkBounds;

    public ChunkMark ChunkMark;
    public bool IsDrawChunkMarks;

    private bool _isValidate;

    private void OnValidate()
    {
        if (!_isValidate)
            return;
        
        ChunkMark.SetVisibleMarks(IsDrawChunkMarks);
    }

    private void Start()
    {
        if (!IsCreateMode)
        {
            CreateChunks();
            EnableAdjustFunctions();
            return;
        }
        
        CreateChunks();
        CreateGround();

        EnableAdjustFunctions();
    }

    private void EnableAdjustFunctions()
    {
        if (IsDrawChunkBounds)
            DrawChunkBounds();

        _mapUpdater.enabled = true;

        _isValidate = true;
        ChunkMark.SetVisibleMarks(IsDrawChunkMarks);
    }

    private void CreateChunks()
    {
        _chunks = new Dictionary<Vector3Int, Chunk>();

        var currentPosition = new Vector3Int();
        
        for (var y = 0; y < CountHeightChunks; y++)
        {
            currentPosition.y = y;
            
            for (var x = 0; x < CountWidthChunks; x++)
            {
                currentPosition.x = x;
                
                _chunks.Add(currentPosition, new Chunk(_chunkSize, currentPosition, true, _chunkFrozenSteps));
                
                ChunkMark.CreateMark(_chunks[currentPosition]);
            }
        }
        
        ChunkMark.AddedListeners();
    }

    private void CreateGround()
    {
        foreach (var chunk in _chunks)
        {
            if (chunk.Key.y > _maxEarthHeight)
                continue;

            var currentSizeCave = _sizeCavePath + TileUtils.GetNormalizeValue(_chunkSize, chunk.Key.y) * _dampingValue;
            
            chunk.Value.TryProcessChunk(cellPosition =>
            {
                if (chunk.Key.y == 0)
                {
                    _lava.SetTile(cellPosition, _lavaTileData.RedLava);
                    return;
                }
                
                if (Mathf.PerlinNoise(cellPosition.x / _widthCaveHole, cellPosition.y / _heightCaveHole) > currentSizeCave)
                {
                    _earth.SetTile(cellPosition, _earthTileData.GreenGrass);
                }
                
                if (Mathf.PerlinNoise(cellPosition.x / _widthCaveHole, cellPosition.y / _heightCaveHole) - currentSizeCave > 0.3f)
                {
                    _earth.SetTile(cellPosition, _earthTileData.Rock);
                }
                
                if (chunk.Key.y >= _maxWaterHeight)
                {
                    if (Mathf.PerlinNoise(cellPosition.x / _widthCaveHole, cellPosition.y / _heightCaveHole) <
                        currentSizeCave * (_waterCoefficient * currentSizeCave))
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
        }
    }

    public void DoActionChunk(Action<Chunk> actionChunk)
    {
        foreach (var chunk in _chunks.Values)
        {
            actionChunk?.Invoke(chunk);
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
