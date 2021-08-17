using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace DefaultNamespace
{
    public abstract class LiquidMapUpdater : MonoBehaviour, IMapUpdater
    {
        public Queue<TileCode> LiquidTiles { get; set; }
        
        protected Map Map;
        
        [SerializeField] private TileUtils.Timer _simulateTimer;
        [SerializeField] private TileUtils.Timer _refreshTimer;
        
        private void Start()
        {
            Map = FindObjectOfType<Map>();
            LiquidTiles = new Queue<TileCode>();
        }

        public void DoUpdate()
        {
            RefreshTick();
            SimulateTick();
        }

        private void SimulateTick()
        {
            _simulateTimer.ElapsedTime -= Time.deltaTime;
        
            if (!(_simulateTimer.ElapsedTime < 0)) 
                return;
        
            var thread = new Thread(UpdateLiquidChunks);
            thread.Start();
            
            //UpdateLiquidChunks();
            _simulateTimer.ElapsedTime = _simulateTimer.TimeDelay;
        }

        private void RefreshTick()
        {
            _refreshTimer.ElapsedTime -= Time.deltaTime;
        
            if (!(_refreshTimer.ElapsedTime < 0)) 
                return;
        
            RegistryAllLiquidChunks();
            _refreshTimer.ElapsedTime = _refreshTimer.TimeDelay;
        }

        protected abstract void RegistryAllLiquidChunks();

        protected abstract void UpdateLiquidChunks();
    }
}