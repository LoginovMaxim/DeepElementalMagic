using System;
using UnityEngine;
using Utils;

namespace DefaultNamespace
{
    public class ChunkDetector : MonoBehaviour
    {
        [SerializeField] private float _detectRadius;
        
        private Map _map;

        private void Start()
        {
            _map = FindObjectOfType<Map>();
        }

        private void Update()
        {
            DetectChunk();
        }

        private void DetectChunk()
        {
            _map.DoActionChunk(chunk =>
            {
                var distance = chunk.GetChunkCenterPosition() - transform.position;
                
                if (distance.sqrMagnitude > _detectRadius * _detectRadius)
                    return;
                
                Debug.DrawRay(transform.position, distance, Color.green, 0.1f);
                
                chunk.ResetFrozen();
            });
        }
    }
}