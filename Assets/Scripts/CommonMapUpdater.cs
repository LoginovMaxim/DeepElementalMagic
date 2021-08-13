using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace DefaultNamespace
{
    public class CommonMapUpdater : MonoBehaviour, IMapUpdater
    {
        protected Map Map;
        
        [SerializeField] private TileUtils.Timer _simulateTimer;
        [SerializeField] private TileUtils.Timer _refreshTimer;

        private void Start()
        {
            Map = FindObjectOfType<Map>();
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
        
            UpdateWaterChunks();
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

        protected virtual void RegistryAllLiquidChunks() { }

        protected virtual void UpdateWaterChunks() { }
    }
}