using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDarkness : MonoBehaviour
{
    [SerializeField] private Vector3Int _tilePosition;
    [SerializeField] private Tilemap _tilemap;

    private void Start()
    {
        SetDarkness();
    }

    private void Update()
    {
    }

    private void SetDarkness()
    {
        Utils.CalculateAllTilemapDown(_tilemap, currentTile =>
        {
            _tilemap.SetColor(currentTile, GetAverageColorValue(currentTile));
        });
    }

    private Color GetAverageColorValue(Vector3Int targetTilePosition)
    {
        var averageColor = _tilemap.GetColor(targetTilePosition);

        for (var i = 0; i < Utils.Directions.Length; i++)
        {
            if (!_tilemap.HasTile(targetTilePosition + Utils.Directions[i]))
                return _tilemap.GetColor(targetTilePosition);

            var color = _tilemap.GetColor(targetTilePosition + Utils.Directions[i]);
            averageColor.r *= 0.96f * color.r;
            averageColor.g *= 0.96f * color.g;
            averageColor.b *= 0.96f * color.b;
        }

        return averageColor;
    }
}
