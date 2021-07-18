using System;
using UnityEngine;

public class Chunk
{
    public Action<Chunk> ChangeEnabled;

    public Vector3Int AnchorPosition => _anchorPosition;
    public bool IsEnabled => _isEnabled;
    
    private int _size;
    private Vector3Int _anchorPosition;
    private bool _isEnabled;

    public Chunk(int size, Vector3Int anchorPosition, bool isEnabled)
    {
        _size = size;
        _anchorPosition = anchorPosition;
        _isEnabled = isEnabled;
    }

    public bool TryProcessChunk(Action<Vector3Int> actionByPosition)
    {
        if (!_isEnabled)
            return false;
        
        var currentPosition = new Vector3Int();
        
        for (var y = 0; y < _size; y++)
        {
            currentPosition.y = y + _anchorPosition.y * _size;
            
            for (var x = 0; x < _size; x++)
            {
                currentPosition.x = x + _anchorPosition.x * _size;
                
                actionByPosition?.Invoke(currentPosition);
            }
        }

        SetEnabled(false);
        return true;
    }

    public void SetEnabled(bool isEnabled)
    {
        _isEnabled = isEnabled;
        ChangeEnabled?.Invoke(this);
    }

    public bool CheckNeighborChunkForEnabled(Vector3Int cellPosition, out Vector3Int liquidDirection)
    {
        liquidDirection = Vector3Int.zero;
        
        if (cellPosition.y == _anchorPosition.y * _size)
        {
            liquidDirection = Vector3Int.down;
            return true;
        }
        
        if (cellPosition.x == _anchorPosition.x * _size)
        {
            liquidDirection = Vector3Int.left;
            return true;
        }
        
        if (cellPosition.x == _anchorPosition.x * _size + _size - 1)
        {
            liquidDirection = Vector3Int.right;
            return true;
        }

        return false;
    }

    public Vector3 GetVertexPosition(int index)
    {
        var vertexPosition = (Vector3) _anchorPosition * _size;
        
        switch (index)
        {
            case 0:
            {
                break;
            }
            case 1:
            {
                vertexPosition.y += _size;
                break;
            }
            case 2:
            {
                vertexPosition.x += _size;
                vertexPosition.y += _size;
                break;
            }
            case 3:
            {
                vertexPosition.x += _size;
                break;
            }
        }

        return vertexPosition * Utils.TileUtils.MapCellSize;
    }
}
