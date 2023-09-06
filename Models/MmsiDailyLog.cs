// AisDataProcessing -- MmsiDailyLog.cs
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

public class MmsiDailyLog : IPersistableData
{
    private List<MmsiDailyLogLine> _logLines = new();

    public List<MmsiDailyLogLine> logLines => _logLines;
    public int count => _logLines.Count;

    public void Add(MmsiDailyLogLine logLine)
    {
        _logLines.Add(logLine);
    }
    
    public void Save(DataWriter writer)
    {
        writer.Write(_logLines.Count);
        foreach (var logLine in _logLines)
        {
            logLine.Save(writer);
        }
    }

    public void Load(DataReader reader)
    {
        var count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            var logLine = new MmsiDailyLogLine();
            logLine.Load(reader);
            _logLines.Add(logLine);
        }
    }

    public MmsiDailyLog RemoveInvalidMmsis()
    {
        _logLines = _logLines.Where(x => x.mmsi >= 100_000_000).ToList();
        return this;
    }

    public HashSet<int> GetUniqueMmsis()
    {
        var uniqueMmsis = new HashSet<int>();
        foreach (var logLine in _logLines)
        {
            uniqueMmsis.Add(logLine.mmsi);
        }

        return uniqueMmsis;
    }

    public Dictionary<int, int> GetMmsiCountDict()
    {
        var mmsiCountDict = new Dictionary<int, int>();
        foreach (var logLine in _logLines)
        {
            if (mmsiCountDict.ContainsKey(logLine.mmsi))
            {
                mmsiCountDict[logLine.mmsi]++;
                continue;
            }
            mmsiCountDict.Add(logLine.mmsi, 1);
        }

        return mmsiCountDict;
    }
    
    public Dictionary<int, int> GetMmsiCountDict(Dictionary<int, int> mmsiCountDict)
    {
        foreach (var logLine in _logLines)
        {
            if (mmsiCountDict.ContainsKey(logLine.mmsi))
            {
                mmsiCountDict[logLine.mmsi]++;
                continue;
            }
            mmsiCountDict.Add(logLine.mmsi, 1);
        }

        return mmsiCountDict;
    }

    public void DebugPrintDailyLog()
    {
        foreach (var logLine in _logLines)
        {
            Console.WriteLine(logLine.ToString());
        }
    }
}