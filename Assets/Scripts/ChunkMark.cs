using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[RequireComponent(typeof(Map))]
public class ChunkMark : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _markPrefab;
    [SerializeField] private Transform _markContainer;
    
    private Dictionary<Chunk, SpriteRenderer> _chunkMarks;

    private Map _map;
    
    private void OnEnable()
    {
        if (_map == null)
            _map = GetComponent<Map>();
    }

    private void Start()
    {
        _chunkMarks = new Dictionary<Chunk, SpriteRenderer>();
    }

    public void CreateMark(Chunk chunk)
    {
        var anchorPosition = chunk.AnchorPosition;
        var markPosition = new Vector3(
            anchorPosition.x * _map.ChunkSize + _map.ChunkSize / 2,
            anchorPosition.y * _map.ChunkSize + _map.ChunkSize / 2, 
            0);
        
        _chunkMarks.Add(
            chunk, 
            Instantiate(
                _markPrefab, 
                markPosition * TileUtils.MapCellSize, 
                Quaternion.identity, 
                _markContainer));
    }

    public void SetVisibleMarks(bool isShowMarks)
    {
        if (_chunkMarks == null)
            return;
        
        foreach (var chunkMark in _chunkMarks.Values)
        {
            chunkMark.enabled = isShowMarks;
        }
    }

    public void AddedListeners()
    {
        foreach (var chunk in _map.Chunks.Values)
        {
            chunk.ChangeEnabled += OnChangeChunkEnabled;
        }
    }
    
    private void OnChangeChunkEnabled(Chunk chunk)
    {
        _chunkMarks[chunk].color = chunk.IsEnabled ? Color.green : Color.red;
    }

    private void OnDisable()
    {
        foreach (var chunk in _map.Chunks.Values)
        {
            chunk.ChangeEnabled -= OnChangeChunkEnabled;
        }
    }
}
