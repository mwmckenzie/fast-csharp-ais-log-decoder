// AisDataProcessing -- HexGridSummary.cs
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

public class HexGridSummary
{
    public int res { get; set; }
    public string name => $"R{res}";

    public Dictionary<int, int> countBySeqNum { get; set; } = new ();
    public Dictionary<string, int> countByDate { get; set; } = new();

    private string _currDate = string.Empty;

    public void AddToCount(int seqNum)
    {
        if (!countBySeqNum.ContainsKey(seqNum))
        {
            return;
        }
        countBySeqNum[seqNum]++;
        if (!countByDate.ContainsKey(_currDate))
        {
            return;
        }
        countByDate[_currDate]++;
    }

    public void StartProcessingDate(string date)
    {
        countByDate.TryAdd(date, 0);
        _currDate = date;
    }
}