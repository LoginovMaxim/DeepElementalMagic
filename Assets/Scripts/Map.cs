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
    public Tilemap Rock => _rock;
    public RockTileData RockTileData => _rockTileData;
    public Tilemap Absidian => _absidian;
    public AbsidianTileData AbsidianTileData => _absidianTileData;
    
    public Tilemap Water => _water;
    public WaterTileData WaterTileData => _waterTileData;
    
    public Tilemap Lava => _lava;
    public LavaTileData LavaTileData => _lavaTileData;
    public MatrixTilemap MatrixTilemap => _matrixTilemap;

    [SerializeField] private MapUpdater _mapUpdater;
    
    [Header("Map Parameters")]
    [SerializeField] private int _chunkSize;
    [SerializeField] private int _chunkFrozenSteps;
    [SerializeField] private int _countWidthChunks;
    [SerializeField] private int _countHeightChunks;
    
    [Header("Cave Parameters")]
    [Range(0, 1)]
    [SerializeField] private float _sizeCavePath;
    [SerializeField] private float _dampingValue;
    [SerializeField] private float _widthCaveHole;
    [SerializeField] private float _heightCaveHole;
    
    [Header("Earth")]
    [SerializeField] private int _maxEarthHeight;
    [SerializeField] private Tilemap _earth;
    [SerializeField] private EarthTileData _earthTileData;
    
    [Header("Rock")]
    [SerializeField] private int _maxRockHeight;
    [SerializeField] private Tilemap _rock;
    [SerializeField] private RockTileData _rockTileData;
    
    [Header("Absidian")]
    [SerializeField] private int _maxAbsidianHeight;
    [SerializeField] private Tilemap _absidian;
    [SerializeField] private AbsidianTileData _absidianTileData;

    [Header("Water")] 
    [SerializeField] private int _maxWaterHeight;
    [SerializeField] private Tilemap _water;
    [SerializeField] private WaterTileData _waterTileData;
    [SerializeField] [Range(0.1f, 1.9f)]private float _waterCoefficient;
    
    [Header("Lava")]
    [SerializeField] private int _minLavaHeight;
    [SerializeField] private Tilemap _lava;
    [SerializeField] private LavaTileData _lavaTileData;
    
    [Header("Wall")]
    [SerializeField] private Tilemap _wall;
    [SerializeField] private WallTileData _wallTileData;

    private Dictionary<Vector3Int, Chunk> _chunks;

    [Header("Tools")] 
    public bool IsCreateMode;
    public bool IsDrawChunkBounds;

    public ChunkMark ChunkMark;
    public bool IsDrawChunkMarks;

    private bool _isValidate;

    private MatrixTilemap _matrixTilemap;
    private void OnValidate()
    {
        if (!_isValidate)
            return;
        
        ChunkMark.SetVisibleMarks(IsDrawChunkMarks);
    }

    private void Start()
    {
        InitMatrixTilemap();
        
        if (!IsCreateMode)
        {
            GenerateChunks();
            EnableAdjustFunctions();
            return;
        }
        
        GenerateChunks();
        GenerateGround();
        GenerateBounds();

        UpdateTilemap();
        
        EnableAdjustFunctions();
    }

    private void Update()
    {
        //UpdateTilemap();

        if (Input.GetKeyDown(KeyCode.A))
            SetAbsidianChunk();
    }

    private void InitMatrixTilemap()
    {
        _matrixTilemap = new MatrixTilemap();
        
        var countMatrixCell = _chunkSize * _countWidthChunks * _countHeightChunks;
        MatrixTilemap.Codes = new byte[countMatrixCell, countMatrixCell];
        
        _matrixTilemap.CodeByTiles = new Dictionary<RuleTile, byte>
        {
            {_earthTileData.GreenGrass, 1},
            {_waterTileData.BlueWater, 2},
            {_rockTileData.GrayRock, 3},
            {_lavaTileData.RedLava, 4},
            {_absidianTileData.PurpleAbsidian, 5},
        };
        
        _matrixTilemap.TileByCodes = new Dictionary<byte, RuleTile>
        {
            {1, _earthTileData.GreenGrass},
            {2, _waterTileData.BlueWater},
            {3, _rockTileData.GrayRock},
            {4, _lavaTileData.RedLava},
            {5, _absidianTileData.PurpleAbsidian},
        };
        
        _matrixTilemap.TilemapByCodes = new Dictionary<byte, Tilemap>
        {
            {1, _earth},
            {2, _water},
            {3, _rock},
            {4, _lava},
            {5, _absidian},
        };
    }

    private void EnableAdjustFunctions()
    {
        if (IsDrawChunkBounds)
            DrawChunkBounds();

        _mapUpdater.enabled = true;

        _isValidate = true;
        ChunkMark.SetVisibleMarks(IsDrawChunkMarks);
    }

    private void GenerateChunks()
    {
        _chunks = new Dictionary<Vector3Int, Chunk>();

        var currentPosition = new Vector3Int();
        
        for (var y = 0; y < _countHeightChunks; y++)
        {
            currentPosition.y = y;
            
            for (var x = 0; x < _countWidthChunks; x++)
            {
                currentPosition.x = x;
                
                _chunks.Add(currentPosition, new Chunk(_chunkSize, currentPosition, true, _chunkFrozenSteps));
                
                ChunkMark.CreateMark(_chunks[currentPosition]);
            }
        }
        
        ChunkMark.AddedListeners();
    }

    private void GenerateBounds()
    {
        var maxHeight = _countHeightChunks * ChunkSize;
        var maxWidth = _countWidthChunks * ChunkSize;
        
        for (var x = 0; x < _countWidthChunks * ChunkSize; x++)
        {
            _wall.SetTile(new Vector3Int(x, -1, 0), _wallTileData.Wall);
        }
        for (var x = 0; x < _countWidthChunks * ChunkSize; x++)
        {
            _wall.SetTile(new Vector3Int(x, maxHeight, 0), _wallTileData.Wall);
        }
        for (var y = 0; y < _countHeightChunks * ChunkSize; y++)
        {
            _wall.SetTile(new Vector3Int(-1, y, 0), _wallTileData.Wall);
        }
        for (var y = 0; y < _countHeightChunks * ChunkSize; y++)
        {
            _wall.SetTile(new Vector3Int(maxWidth, y, 0), _wallTileData.Wall);
        }
    }

    private void GenerateGround()
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
                    _matrixTilemap.Codes[cellPosition.x, cellPosition.y] = _matrixTilemap.CodeByTiles[LavaTileData.RedLava];
                    return;
                }
                
                if (Mathf.PerlinNoise(cellPosition.x / _widthCaveHole, cellPosition.y / _heightCaveHole) > currentSizeCave)
                {
                    _matrixTilemap.Codes[cellPosition.x, cellPosition.y] = _matrixTilemap.CodeByTiles[_earthTileData.GreenGrass];
                }
                
                if (Mathf.PerlinNoise(cellPosition.x / _widthCaveHole, cellPosition.y / _heightCaveHole) - currentSizeCave > 0.3f)
                {
                    _matrixTilemap.Codes[cellPosition.x, cellPosition.y] = _matrixTilemap.CodeByTiles[_rockTileData.GrayRock];
                }
                
                if (chunk.Key.y >= _maxWaterHeight)
                {
                    if (Mathf.PerlinNoise(cellPosition.x / _widthCaveHole, cellPosition.y / _heightCaveHole) <
                        currentSizeCave * (_waterCoefficient * currentSizeCave))
                    {
                        _matrixTilemap.Codes[cellPosition.x, cellPosition.y] = _matrixTilemap.CodeByTiles[_waterTileData.BlueWater];
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

    public void UpdateTilemap()
    {
        foreach (var chunk in _chunks.Values)
        {
            chunk.TryProcessChunk(cellPosition =>
            {
                var index = _matrixTilemap.Codes[cellPosition.x, cellPosition.y];
                
                if (index == 0)
                {
                    foreach (var tilemap in _matrixTilemap.TilemapByCodes.Values)
                    {
                        tilemap.SetTile(cellPosition, null);
                    }
                    return;
                }

                _matrixTilemap.TilemapByCodes[index].SetTile(cellPosition, _matrixTilemap.TileByCodes[index]);
            });
        }
    }

    private void SetAbsidianChunk()
    {
        var chunks = new List<Chunk>();
        chunks.Add(_chunks[new Vector3Int(1, 0, 0)]);
        chunks.Add(_chunks[new Vector3Int(1, 1, 0)]);
        chunks.Add(_chunks[new Vector3Int(3, 3, 0)]);

        foreach (var chunk in chunks)
        {
            chunk.SetEnabled(true);
            chunk.TryProcessChunk(cellPosition =>
            {
                MatrixTilemap.Codes[cellPosition.x, cellPosition.y] =
                    MatrixTilemap.CodeByTiles[_absidianTileData.PurpleAbsidian];
            });
            chunk.SetEnabled(true);
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
