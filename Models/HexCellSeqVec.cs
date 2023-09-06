// AisDataProcessing -- HexCellSeqVec.cs
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

using AisDataProcessing.General;

namespace AisDataProcessing.Models;

public struct HexCellSeqVec : IPersistableData
{
    private int[] _seqVec = new int[6];
    
    public int R3 => _seqVec[0];
    public int R4 => _seqVec[1];
    public int R5 => _seqVec[2];
    public int R6 => _seqVec[3];
    public int R7 => _seqVec[4];
    public int R8 => _seqVec[5];
    public int R9 => _seqVec[6];

    public HexCellSeqVec()
    {
        _seqVec = new int[6];
    }

    public HexCellSeqVec UpdateSeqNum(int resolution, int seqNum)
    {
        if (resolution is > 2 and < 10)
        {
            _seqVec[resolution - 3] = seqNum;
        }
        return this;
    }

    public void Save(DataWriter writer)
    {
        var count = _seqVec.Length;
        writer.Write(count);
        for (int i = 0; i < count; i++)
        {
            writer.Write(_seqVec[i]);
        }
    }

    public void Load(DataReader reader)
    {
        _seqVec = new int[6];
        var count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            _seqVec[i] = reader.ReadInt();
        }
    }

    public override string ToString()
    {
        return $"{R4},{R5},{R6},{R7},{R8}";
    }
}