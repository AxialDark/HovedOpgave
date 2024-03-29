﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using System;
using Assets.Helpers;

/// <summary>
/// Handles the Tiles of the map, and dynamicly updates the map
/// </summary>
public class DynamicTileManager : TileManager {

    private Vector2 tileSize;
    [SerializeField]
    private Rect centerCollider;
    [SerializeField]
    private int removeAfter;

    /// <summary>
    /// Override of super class method
    /// Initializes the TileManager
    /// </summary>
    /// <param name="_buildFac">BuildingFactory class to build building on tiles</param>
    /// <param name="_roadFac">RoadFactory class to build roads on tiles</param>
    /// <param name="_settings">The settings from the WorldMap class</param>
    public override void Initialize(BuildingFactory _buildFac, RoadFactory _roadFac, WorldMap.Settings _settings)
    {
        base.Initialize(_buildFac, _roadFac, _settings);
        removeAfter = Math.Max(removeAfter, range * 2 + 1);
        tileSize = tiles.Values.First().rect.size;
        centerCollider = new Rect(Vector2.zero - tileSize / 2, tileSize);

        Observable.Interval(TimeSpan.FromSeconds(2)).Subscribe(x => { UpdateTiles(); }); //Runs UpdateTiles every 2 seconds
    }

    /// <summary>
    /// Updates the map when player leaves the Tile middle tile
    /// Is run once every 2 seconds
    /// </summary>
    private void UpdateTiles()
    {
        if(!centerCollider.Contains(User.Instance.transform.position.ToVector2xz(), true))
        {
            Vector2 tileDif = GetMovementVector();

            Centralize(tileDif);
            LoadTiles(centerTMS, centerInMercator);
            UnloadTiles(centerTMS);
        }
    }

    /// <summary>
    /// Moves the whole map so the player always is on the middle Tile
    /// </summary>
    /// <param name="_tileDif">The direction the player left central Tile</param>
    private void Centralize(Vector2 _tileDif)
    {
        //Routing
        foreach (GameObject routes in RouteManager.Instance.RoutePlanes)
        {
            routes.transform.position -= new Vector3(_tileDif.x * tileSize.x, 0, _tileDif.y * tileSize.y);
        }
        foreach (GameObject points in RouteManager.Instance.Points)
        {
            points.transform.position -= new Vector3(_tileDif.x * tileSize.x, 0, _tileDif.y * tileSize.y);
        }
        foreach (GameLocation gameLocations in RouteManager.Instance.Gamelocations)
        {
            gameLocations.gameObject.transform.position -= new Vector3(_tileDif.x * tileSize.x, 0, _tileDif.y * tileSize.y);
        }
        foreach (GameObject via in RouteManager.Instance.debugViaPoints)
        {
            via.gameObject.transform.position -= new Vector3(_tileDif.x * tileSize.x, 0, _tileDif.y * tileSize.y);
        }
        //End Routing

        foreach (Tile tile in tiles.Values)
        {
            tile.transform.position -= new Vector3(_tileDif.x * tileSize.x, 0, _tileDif.y * tileSize.y);
        }
        centerTMS += _tileDif;
        centerInMercator = GM.TileBounds(centerTMS, zoom).center;
        Vector3 difInUnity = new Vector3(_tileDif.x * tileSize.x, 0, _tileDif.y * tileSize.y);
        //user.transform.position -= difInUnity;
        User.Instance.Centralize(difInUnity, centerInMercator);
    }

    /// <summary>
    /// Removes all Tiles out of player range
    /// </summary>
    /// <param name="_currentTMS">The central Tile</param>
    private void UnloadTiles(Vector2 _currentTMS)
    {
        List<Vector2> rem = new List<Vector2>();
        foreach (Vector2 key in tiles.Keys.Where(x => x.ManhattanTo(_currentTMS) > removeAfter))
        {
            rem.Add(key);
            Destroy(tiles[key].gameObject);
        }

        foreach (Vector2 v in rem)
        {
            tiles.Remove(v);
        }
    }

    /// <summary>
    /// Finds which way the player left the middle Tile
    /// </summary>
    /// <returns>Direction vector</returns>
    private Vector2 GetMovementVector()
    {
        Vector2 dif = User.Instance.transform.position.ToVector2xz();
        Vector2 tileDif = Vector2.zero;
        if (dif.x < Math.Min(centerCollider.xMin, centerCollider.xMax))
            tileDif.x = -1;
        else if (dif.x > Math.Max(centerCollider.xMin, centerCollider.xMax))
            tileDif.x = 1;

        if (dif.y < Math.Min(centerCollider.yMin, centerCollider.yMax))
            tileDif.y = 1;
        else if (dif.y > Math.Max(centerCollider.yMin, centerCollider.yMax))
            tileDif.y = -1;

        return tileDif;
    }
}
