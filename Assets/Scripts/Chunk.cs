using System;
using UnityEngine;
using Utils;

public class Chunk
{
    public Action<Chunk> ChangeEnabled;

    public Vector3Int AnchorPosition => _anchorPosition;
    public bool IsEnabled => _isEnabled;
    public bool IsFrozen => _isFrozen;
    
    private int _size;
    private Vector3Int _anchorPosition;
    private bool _isEnabled;
    private int _initialFrozenSteps;
    private int _currentFrozenSteps;
    
    private bool _isFrozen;

    public Chunk(int size, Vector3Int anchorPosition, bool isEnabled, int frozenSteps)
    {
        _size = size;
        _anchorPosition = anchorPosition;
        _isEnabled = isEnabled;
        _initialFrozenSteps = frozenSteps;

        _isFrozen = false;
    }

    public bool TryProcessChunk(Action<Vector3Int> actionByPosition)
    {
        if (!_isEnabled)
            return false;

        if (_isFrozen)
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

        CalculateFrozenSteps();

        SetEnabled(false);
        return true;
    }
    
    public bool TryProcessChunkForceEnable(Action<Vector3Int> actionByPosition)
    {
        if (_isFrozen)
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
        
        return true;
    }

    public void SetEnabled(bool isEnabled)
    {
        _isEnabled = isEnabled;
        //ChangeEnabled?.Invoke(this);
    }

    public void ResetFrozen()
    {
        _currentFrozenSteps = 0;
        _isFrozen = false;
    }

    private void CalculateFrozenSteps()
    {
        _currentFrozenSteps++;

        if (_currentFrozenSteps <= _initialFrozenSteps) 
            return;
        
        _currentFrozenSteps = 0;
        _isFrozen = true;
    }

    public Vector3 GetChunkCenterPosition()
    {
        var offset = Vector3.one * _size / 2f;
        offset.z = 0;
        
        return (_anchorPosition * _size + offset) * TileUtils.MapCellSize;
    }

    public bool CheckNeighborChunkForEnabled(Vector3Int cellPosition, out Vector3Int liquidDirection)
    {
        liquidDirection = Vector3Int.zero;
        
        if (cellPosition.y == _anchorPosition.y * _size)
        {
            liquidDirection = Vector3Int.down;
            return true;
        }
        
        if (cellPosition.y == _anchorPosition.y * _size + _size - 1)
        {
            liquidDirection = Vector3Int.up;
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
