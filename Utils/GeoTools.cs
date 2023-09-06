// AisDataProcessing -- GeoTools.cs
// 
// Copyright (C) 2023 Matthew W. McKenzie and Kenz LLC
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Drawing;
using System.Numerics;
using AisDataProcessing.Helpers;
using AisDataProcessing.Models;
using AisDataProcessing.Static;
using Newtonsoft.Json;

namespace AisDataProcessing.Utils;

public static class GeoTools
{

    public static Root DeserializeGeoJson(string geoJson)
    {
        return JsonConvert.DeserializeObject<Root>(geoJson) ?? new Root();
    }

    public static List<Vector2> ToPath(Geometry geometry)
    {
        var innerList = geometry.coordinates[0];
        var path = new List<Vector2>();

        foreach (var coord in innerList)
        {
            path.Add(new Vector2((float) coord[0], (float) coord[1]));
        }

        return path;
    }
    
    public static List<Coord> ToCoordPath(Geometry geometry)
    {
        var innerList = geometry.coordinates[0];
        var path = new List<Coord>();

        foreach (var coord in innerList)
        {
            path.Add(new Coord
            {
                x = coord[0],
                y = coord[1]
            });
        }

        return path;
    }

    public static HexGrid ToHexGrid(GeoJson geoJson, int resolution)
    {
        var hexGrid = new HexGrid
        {
            res = resolution
        };
        if (!geoJson.isFeatureCollection) return hexGrid;

        foreach (var feature in geoJson.features)
        {
            var geo = feature.geometry;
            if (!geo.isPolygon) continue;
            
            var cell = new HexCell
            {
                info = new HexCellInfo
                {
                    res = resolution,
                    seqNum = feature.properties.seqnum
                },
                coords = ToCoordPath(geo),
                count = 0
            };
            hexGrid.AddHexCell(cell);
            //hexGrid.cells.Add(cell);
            //hexGrid.gridSummary.cellCount.Add(cell.info.seqNum, 0);
        }
        
        return hexGrid;
    }
    
    // Source: https://dominoc925.blogspot.com/2012/02/c-code-snippet-to-determine-if-point-is.html
    public static bool IsPointInPolygon(PointF[] polygon, PointF point)
    {
        var isInside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            if (polygon[i].Y > point.Y != polygon[j].Y > point.Y &&
                point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / 
                (polygon[j].Y - polygon[i].Y) + polygon[i].X)
            {
                isInside = !isInside;
            }
        }
        return isInside;
    }
    
    // Adapted from: https://dominoc925.blogspot.com/2012/02/c-code-snippet-to-determine-if-point-is.html
    public static bool IsPointInPolygon(IReadOnlyList<Coord> polygon, Coord point)
    {
        var isInside = false;
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        {
            if (polygon[i].y > point.y != polygon[j].y > point.y &&
                point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / 
                (polygon[j].y - polygon[i].y) + polygon[i].x)
            {
                isInside = !isInside;
            }
        }
        return isInside;
    }
    
    // Adapted from: https://dominoc925.blogspot.com/2012/02/c-code-snippet-to-determine-if-point-is.html
    public static bool IsPointInPolygon(IReadOnlyList<(double, double)> polygon, (double, double) point)
    {
        var isInside = false;
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        {
            if (polygon[i].Item2 > point.Item2 != polygon[j].Item2 > point.Item2 &&
                point.Item1 < (polygon[j].Item1 - polygon[i].Item1) * (point.Item2 - polygon[i].Item2) / 
                (polygon[j].Item2 - polygon[i].Item2) + polygon[i].Item1)
            {
                isInside = !isInside;
            }
        }
        return isInside;
    }

    public static HexCell FindContainingChild(HexCell parentCell, Coord point)
    {
        if (parentCell is null) return null;
        
        var hexManager = HexGridManager.Instance;
        foreach (var childInfo in parentCell.children)
        {
            var child = hexManager.GetCell(childInfo);
            if (child.AddToCell(point))
            {
                return child;
            }
        }

        return null;
    }

    public static void GenerateGridCountsFile(int res)
    {
        var hexManager = HexGridManager.Instance;
        hexManager.LoadGrids();

        var grid = hexManager.Grid(res);
        var counts = new List<string>();
        counts.Add($"seqNum,count");

        foreach (var line in grid.SeqNumCounts().Select(x => $"{x.a},{x.b}"))
        {
            counts.Add(line);
        }
        
        DataService.WriteListToDggsTrackCountsFolder($"DggsSeqNumCounts_R{res}.csv",counts);
    }
}