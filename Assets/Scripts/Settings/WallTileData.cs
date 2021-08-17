using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(order = 1, menuName = "TileData/WallTileData", fileName = "WallTileData")]
    public class WallTileData : ScriptableObject
    {
        public RuleTile Wall;
    }
}