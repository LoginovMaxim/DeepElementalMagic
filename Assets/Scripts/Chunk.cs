using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct Chunk
{
    public int Size;
    public Vector3Int AnchorPosition;
    public bool IsEnabled;

    public Chunk(int size, Vector3Int anchorPosition, bool isEnabled)
    {
        Size = size;
        AnchorPosition = anchorPosition;
        IsEnabled = isEnabled;
    }

    public bool TryProcessChunk(Action<Vector3Int> actionByPosition)
    {
        if (!IsEnabled)
            return false;
        
        var currentPosition = new Vector3Int();
        
        for (var y = 0; y < Size; y++)
        {
            currentPosition.y = y + AnchorPosition.y * Size;
            
            for (var x = 0; x < Size; x++)
            {
                currentPosition.x = x + AnchorPosition.x * Size;
                
                actionByPosition?.Invoke(currentPosition);
            }
        }

        IsEnabled = false;
        return true;
    }

    public Vector3 GetVertexPosition(int index)
    {
        var vertexPosition = (Vector3) AnchorPosition * Size;
        
        switch (index)
        {
            case 0:
            {
                break;
            }
            case 1:
            {
                vertexPosition.y += Size;
                break;
            }
            case 2:
            {
                vertexPosition.x += Size;
                vertexPosition.y += Size;
                break;
            }
            case 3:
            {
                vertexPosition.x += Size;
                break;
            }
        }

        return vertexPosition * Utils.Utils.MapCellSize;
    }
}
