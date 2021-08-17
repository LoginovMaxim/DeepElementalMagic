using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(order = 1, menuName = "TileData/RockTileData", fileName = "RockTileData")]
    public class RockTileData : ScriptableObject
    {
        public RuleTile GrayRock;
    }
}