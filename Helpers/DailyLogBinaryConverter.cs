// AisDataProcessing -- DailyLogBinaryConverter.cs
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

using System.Diagnostics;
using AisDataProcessing.General;
using AisDataProcessing.Models;
using AisDataProcessing.Static;
using AisDataProcessing.Utils;

namespace AisDataProcessing.Helpers;

public class DailyLogBinaryConverter
{

    public MmsiDailyLog LoadDailyLogFromBinary(string binaryFileName)
    {
        var dailyLog = new MmsiDailyLog();
        var storage = new PersistableStorage
        {
            savePath = FileReferences.mmsiDailyBinaryPath,
            filename = binaryFileName
        };
        storage.Load(dailyLog);
        return dailyLog;
       //Console.WriteLine($"Log Loaded, line count: {dailyLog.count} ({binaryFileName})");
        //dailyLog.DebugPrintDailyLog();
    }

    public Dictionary<int, MmsiRecord> BuildAllMmsiRecords()
    {
        var mmsiList = DataService.LoadMmsiRoiList();
        var mmsiDict = new Dictionary<int, MmsiRecord>();
        foreach (var mmsiString in mmsiList)
        {
            if (mmsiString.ToInt() < 0)
            {
                continue;
            }
            mmsiDict.TryAdd(mmsiString.ToInt(), new MmsiRecord());
        }
        
        Console.WriteLine($"MMSI Dictionary Size: {mmsiDict.Count}");
        
        var watch = new Stopwatch();

        foreach (var fileInfo in FileReferences.DailyLogBinaryDir().EnumerateFiles())
        {
            watch.Restart();
            var dailyLog = LoadDailyLogFromBinary(fileInfo.FullName);

            foreach (var logLine in dailyLog.logLines)
            {
                if (!mmsiDict.ContainsKey(logLine.mmsi)) continue;
                if (mmsiDict[logLine.mmsi] is not { } mmsiRecord) { continue; }

                var mmsiRecordLine = new MmsiRecordLine
                {
                    msgType = logLine.msgType,
                    epoch = logLine.epoch,
                    lat = logLine.lat,
                    lon = logLine.lon,
                    cellSeqVec = logLine.cellSeqVec
                };
                mmsiRecord.Add(mmsiRecordLine);
            }

            watch.Stop();
            Console.WriteLine("");
            Console.WriteLine($"BuildMmsiRecord ({fileInfo.Name}) Execution Time:");
            watch.PrintTime();
        }

        foreach (var mmsiRecord in mmsiDict)
        {
            SaveMmsiRecord(mmsiRecord.Value, mmsiRecord.Key);
        }

        return mmsiDict;
    }
    public MmsiRecord BuildMmsiRecord(int mmsi)
    {
        if (mmsi < 100_000_000)
        {
            Console.WriteLine($"Invalid MMSI: {mmsi}. Quitting App.");
            return new MmsiRecord();
        }
        
        var watch = new Stopwatch();
        
        var mmsiRecord = new MmsiRecord();
        foreach (var fileInfo in FileReferences.DailyLogBinaryDir().EnumerateFiles())
        {
            watch.Restart();
            var dailyLog = LoadDailyLogFromBinary(fileInfo.FullName);

            foreach (var logLine in dailyLog.logLines)
            {
                if (logLine.mmsi != mmsi)
                {
                    continue;
                }
                var mmsiRecordLine = new MmsiRecordLine
                {
                    msgType = logLine.msgType,
                    epoch = logLine.epoch,
                    lat = logLine.lat,
                    lon = logLine.lon,
                    cellSeqVec = logLine.cellSeqVec
                };
                mmsiRecord.Add(mmsiRecordLine);
            }
            
            watch.Stop();
            Console.WriteLine("");
            Console.WriteLine($"Record Count: {mmsiRecord.count}");
            Console.WriteLine($"BuildMmsiRecord ({fileInfo.Name}) Execution Time:");
            watch.PrintTime();
            // Console.WriteLine(
            //     $"{watch.Elapsed.TotalMinutes:0.000} min | {watch.Elapsed.TotalSeconds:0.00} s | {watch.ElapsedMilliseconds} ms");
        }

        return mmsiRecord;
    }

    public MmsiRecord SaveMmsiRecord(MmsiRecord mmsiRecord, int mmsi)
    {
        var storage = new PersistableStorage
        {
            savePath = DataService.BuildMmsiRecordPath(mmsi),
            filename = mmsi.ToString()
        };
        storage.Save(mmsiRecord);
        return mmsiRecord;
    }

    public Dictionary<int, int> GetMmsiCountFromAll()
    {
        var mmsiCountDict = new Dictionary<int, int>();
        foreach (var fileInfo in FileReferences.DailyLogBinaryDir().EnumerateFiles())
        {
            var dailyLog = LoadDailyLogFromBinary(fileInfo.FullName);
            mmsiCountDict = dailyLog.GetMmsiCountDict(mmsiCountDict);
        }
        return mmsiCountDict;
    }

    public void RemoveInvalidMmsisFromAll()
    {
        foreach (var fileInfo in FileReferences.DailyLogBinaryDir().EnumerateFiles())
        {
            var dailyLog = LoadDailyLogFromBinary(fileInfo.FullName);
            var storage = new PersistableStorage
            {
                savePath = FileReferences.mmsiDailyBinaryPath,
                filename = fileInfo.Name
            };
            storage.Save(dailyLog.RemoveInvalidMmsis());
        }
    }

    public void ConvertAllMmsiBinsToBinary()
    {
        var watch = new Stopwatch();
        foreach (var fileInfo in FileReferences.MappedMmsi30MinDir().EnumerateFiles())
        {
            watch.Restart();
            
            var path = fileInfo.FullName;
            var binaryName = fileInfo.Name.Replace(".txt", "");
            ConvertDailyLogToBinary(path, binaryName);
            
            watch.Stop();
            Console.WriteLine("");
            Console.WriteLine($"Binary Conversion Execution Time:");
            Console.WriteLine(
                $"{watch.Elapsed.TotalMinutes} min | {watch.Elapsed.TotalSeconds} s | {watch.ElapsedMilliseconds} ms");
        }
        
    }
    
    public void ConvertDailyLogToBinary(string logFilePath, string binaryFileName)
    {
        if (!Path.Exists(logFilePath))
        {
            Console.WriteLine($"No file found at: {logFilePath}; Exiting ConvertDailyLogToBinary");
            return;
        }

        var dailyLog = BuildDailyLog(DataService.LoadLinesFromFile(logFilePath));

        var storage = new PersistableStorage
        {
            savePath = FileReferences.mmsiDailyBinaryPath,
            filename = binaryFileName
        };
        storage.Save(dailyLog);

        //dailyLog.DebugPrintDailyLog();

    }

    

    private MmsiDailyLog BuildDailyLog(List<string> logText)
    {
        var dailyLog = new MmsiDailyLog();
        foreach (var logLineText in logText)
        {
            if (BuildLogLine(logLineText) is {} logLine)
            {
                dailyLog.Add(logLine);
            }
        }

        return dailyLog;
    }

    private MmsiDailyLogLine BuildLogLine(string logLineText)
    {
        if (string.IsNullOrWhiteSpace(logLineText))
        {
            return null;
        }
        var splitLine = logLineText.Split(',');
        if (splitLine.Length < 10 || splitLine[0].Equals("mmsi"))
        {
            return null;
        }

        //Console.WriteLine($"Split Line [6]: {splitLine[6]} ({splitLine[4]})");
        var seqVec = new HexCellSeqVec();

        for (int i = 4; i < 9; i++)
        {
            if (string.IsNullOrWhiteSpace(splitLine[i + 2]))
            {
                continue;
            }

            seqVec.UpdateSeqNum(i, int.Parse(splitLine[i + 2]));
        }
        // seqVec
        //     .UpdateSeqNum(4, int.Parse(splitLine[6]))
        //     .UpdateSeqNum(5, int.Parse(splitLine[7]))
        //     .UpdateSeqNum(6, int.Parse(splitLine[8]))
        //     .UpdateSeqNum(7, int.Parse(splitLine[9]))
        //     .UpdateSeqNum(8, int.Parse(splitLine[10]));
        
        var logLine = new MmsiDailyLogLine
        {
            mmsi = int.Parse(splitLine[0]),
            msgType = int.Parse(splitLine[1]),
            epoch = int.Parse(splitLine[4]),
            lat = double.Parse(splitLine[2]),
            lon = double.Parse(splitLine[3]),
            cellSeqVec = seqVec
        };

        return logLine;
    }
}