using UnityEngine;

namespace Utils
{
    public static class DirectionUtils
    {
        public static Vector3Int[] EightDirections = 
        {
            Vector3Int.up, 
            Vector3Int.left, 
            Vector3Int.right,
            Vector3Int.down,
            
            Vector3Int.up + Vector3Int.left,
            Vector3Int.up + Vector3Int.right,
            Vector3Int.down + Vector3Int.left,
            Vector3Int.down + Vector3Int.right,
        };
        
        public static Vector3Int[] FourDirections = 
        {
            Vector3Int.up, 
            Vector3Int.left, 
            Vector3Int.right,
            Vector3Int.down
        };
    }
}