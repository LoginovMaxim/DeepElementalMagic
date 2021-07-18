using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(order = 1, menuName = "TileData/LavaTileData", fileName = "LavaTileData")]
    public class LavaTileData : ScriptableObject
    {
        public RuleTile RedLava;
    }
}