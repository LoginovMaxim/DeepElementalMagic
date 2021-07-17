using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(order = 1, menuName = "TileData/WaterTileData", fileName = "WaterTileData")]
    public class WaterTileData : ScriptableObject
    {
        public RuleTile BlueWater;
    }
}