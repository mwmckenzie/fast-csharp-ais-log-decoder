// AisDataProcessing -- MmsiDailyLogLine.cs
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

public class MmsiDailyLogLine : IPersistableData
{
    public int mmsi;
    public int msgType;
    public int epoch;
    public double lat;
    public double lon;
    public HexCellSeqVec cellSeqVec;
    
    public void Save(DataWriter writer)
    {
        writer.Write(mmsi);
        writer.Write(msgType);
        writer.Write(epoch);
        writer.Write(lat);
        writer.Write(lon);
        cellSeqVec.Save(writer);
    }

    public void Load(DataReader reader)
    {
        mmsi = reader.ReadInt();
        msgType = reader.ReadInt();
        epoch = reader.ReadInt();
        lat = reader.ReadDouble();
        lon = reader.ReadDouble();
        cellSeqVec.Load(reader);
    }

    public override string ToString()
    {
        return $"{mmsi},{msgType},{lat},{lon},{epoch},{cellSeqVec.ToString()}";
    }
}