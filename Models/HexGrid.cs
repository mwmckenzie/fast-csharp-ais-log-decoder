// AisDataProcessing -- HexGrid.cs
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


namespace AisDataProcessing.Models;

public class HexGrid
{
    public int res { get; set; }
    public string name { get; set; }
    
    public Dictionary<int, HexCell> hexCells { get; set; } = new();
    public List<int> cellSearchOrder { get; set; } = new();

    public HexCell Cell(int seqNum) => hexCells.ContainsKey(seqNum) ? hexCells[seqNum] : null;

    public void ReorderByCount()
    {
        cellSearchOrder = hexCells
            .OrderByDescending(x => x.Value.count)
            .Select(x => x.Key).ToList();
        
        Console.WriteLine($"Updated Sort Order {name}: High Count {Cell(cellSearchOrder[0]).count}");
    }

    public void AddHexCell(HexCell hexCell)
    {
        var seqNum = hexCell.info.seqNum;
        if (hexCells.ContainsKey(seqNum))
        {
            hexCells[seqNum] = hexCell;
            return;
        }
        hexCells.Add(seqNum, hexCell);
    }

    public HexCell FindContainingCell(Coord point)
    {
        if (cellSearchOrder.Count < 1)
        {
            ReorderByCount();
        }
        
        foreach (var cellInfo in cellSearchOrder)
        {
            if (!hexCells.ContainsKey(cellInfo)) continue;
            
            var cell = hexCells[cellInfo];
            if (!cell.AddToCell(point)) continue;
            
            return cell;
        }

        return null;
    }

    public List<IntTupleAB> SeqNumCounts()
    {
        var seqNumCounts = new List<IntTupleAB>();
        foreach (var cell in hexCells.Values)         
        {
            seqNumCounts.Add(new IntTupleAB
            {
                a = cell.info.seqNum, 
                b = cell.count
            });
        }

        return seqNumCounts;
    }
}