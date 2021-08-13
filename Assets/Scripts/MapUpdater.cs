using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class MapUpdater : MonoBehaviour
{
    private List<IMapUpdater> _mapUpdaters;

    private void Start()
    {
        _mapUpdaters = new List<IMapUpdater>();
        _mapUpdaters.AddRange(GetComponents<IMapUpdater>());
    }

    private void Update()
    {
        foreach (var mapUpdater in _mapUpdaters)
        {
            mapUpdater.DoUpdate();
        }
    }
}
