using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Chunk
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

    public void SetEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }

    public bool CheckNeighborChunkForEnabled(Vector3Int cellPosition, out Vector3Int liquidDirection)
    {
        liquidDirection = Vector3Int.zero;
        
        if (cellPosition.y == AnchorPosition.y * Size)
        {
            liquidDirection = Vector3Int.down;
            return true;
        }
        
        if (cellPosition.x == AnchorPosition.x * Size)
        {
            liquidDirection = Vector3Int.left;
            return true;
        }
        
        if (cellPosition.x == AnchorPosition.x * Size + Size - 1)
        {
            liquidDirection = Vector3Int.right;
            return true;
        }

        return false;
    }

    public Vector3Int GetRandomCellPosition()
    {
        return new Vector3Int(
            AnchorPosition.x * Size + Random.Range(0, Size),
            AnchorPosition.y * Size + Random.Range(0, Size), 
            0);
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

        return vertexPosition * Utils.TileUtils.MapCellSize;
    }
}
