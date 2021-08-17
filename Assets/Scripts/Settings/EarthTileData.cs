using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(order = 1, menuName = "TileData/EarthTileData", fileName = "EarthTileData")]
    public class EarthTileData : ScriptableObject
    {
        public RuleTile GreenGrass;
    }
}