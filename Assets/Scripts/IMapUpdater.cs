using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public interface IMapUpdater
    {
        Queue<TileCode> LiquidTiles { get; set; }
        
        void DoUpdate();
    }
}