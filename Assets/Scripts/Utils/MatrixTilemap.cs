using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utils
{
    public class MatrixTilemap
    {
        public byte[,] Codes;

        public Dictionary<RuleTile, byte> CodeByTiles;
        public Dictionary<byte, RuleTile> TileByCodes;
        public Dictionary<byte, Tilemap> TilemapByCodes;

        public bool TryGetCode(Vector3Int position, out byte code)
        {
            code = 0;
            if (position.x < 0 || position.y < 0 || position.x > Codes.GetLength(0) - 1 ||
                position.y > Codes.GetLength(1) - 1)
                return false;
            
            code = Codes[position.x, position.y];
            return true;
        }
    }
}