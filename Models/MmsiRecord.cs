// AisDataProcessing -- MmsiRecord.cs
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

public class MmsiRecord : IPersistableData
{
    private List<MmsiRecordLine> _recordLines = new();

    public List<MmsiRecordLine> recordLines => _recordLines;
    public int count => _recordLines.Count;

    public void Add(MmsiRecordLine recordLine)
    {
        _recordLines.Add(recordLine);
    }
    
    public void Save(DataWriter writer)
    {
        writer.Write(_recordLines.Count);
        foreach (var logLine in _recordLines)
        {
            logLine.Save(writer);
        }
    }

    public void Load(DataReader reader)
    {
        _recordLines = new List<MmsiRecordLine>();
        var recordCount = reader.ReadInt();
        for (int i = 0; i < recordCount; i++)
        {
            var recordLine = new MmsiRecordLine();
            recordLine.Load(reader);
            _recordLines.Add(recordLine);
        }
    }
    

    public void DebugPrintMmsiRecord()
    {
        foreach (var recordLine in _recordLines)
        {
            Console.WriteLine(recordLine.ToString());
        }
    }
}