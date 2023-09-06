// AisDataProcessing -- HexGridManager.cs
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

using AisDataProcessing.Models;
using AisDataProcessing.Static;

namespace AisDataProcessing.Helpers;

public class HexGridManager
{
    static HexGridManager() { }
    private HexGridManager() { }
    public static HexGridManager Instance { get; } = new HexGridManager();

    public bool initialized { get; private set; }

    public Dictionary<int, HexGrid> grids { get; set; } = new();

    public HexGrid Grid(int res) => grids[res];

    public void Add(HexGrid hexGrid, bool replaceExisting)
    {
        if (grids.TryAdd(hexGrid.res, hexGrid))
        {
            return;
        }
        if (!replaceExisting) return;
        grids[hexGrid.res] = hexGrid;
    }

    public void LoadGrids(int maxRes = 8)
    {
        grids = new Dictionary<int, HexGrid>();
        for (var i = 4; i < maxRes + 1; i++)
        {
            grids.Add(i, DataService.DeserializeHexGrid(i));
        }

        initialized = true;
    }

    public void SaveGrid(int res)
    {
        DataService.SerializeHexGrid(Grid(res), $"HexGrid_R{res}.json");
    }

    public void SaveAllGrids()
    {
        if (!initialized) LoadGrids();
        for (var i = 4; i < 9; i++)
        {
            SaveGrid(i);
        }
    }

    public HexCell GetCell(HexCellInfo cellInfo)
    {
        if (!initialized) LoadGrids();
        return Grid(cellInfo.res).Cell(cellInfo.seqNum);
    }
}