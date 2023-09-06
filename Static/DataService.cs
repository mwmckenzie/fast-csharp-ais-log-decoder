// AisDataAggregator -- DataRef.cs
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

using System.Collections;
using AisDataProcessing.Models;
using Newtonsoft.Json;

namespace AisDataProcessing.Static;

public static class DataService
{

    public const string mmsiHeader = "mmsi,msgType,lon,lat,epoch";
    
    public const int epochFilterStart = 1640995200; //1646070000
    public const int epochFilterEnd = 1646075000;

    // public static FileInfo GetResourceFileInfo(string filename)
    // {
    //     var path = $@"{FileReferences.resourcesPath}\{filename}";
    //     return new FileInfo(path);
    // }
    //
    // public static FileInfo GetProcessedFileInfo(string filename)
    // {
    //     var path = $@"{FileReferences.processedPath}\{filename}";
    //     return new FileInfo(path);
    // }
    //
    // public static FileInfo GetFirstPassFileByDate(string year, string month, string day)
    // {
    //     var filename = $@"{FileReferences.firstPassFilenameBase}_{year}{month}{day}.txt";
    //     return GetProcessedFileInfo(filename);
    // }
    //
    // public static FileInfo GetFirstPassFileByDate(string fullDate)
    // {
    //     var filename = $@"{FileReferences.firstPassFilenameBase}_{fullDate}.txt";
    //     return GetProcessedFileInfo(filename);
    // }
    //
    // public static FileInfo GetProcessedAisFileByDate(string year, string month, string day)
    // {
    //     var filename = $@"{FileReferences.aisFilenameBase}_{year}{month}{day}.txt";
    //     return GetProcessedFileInfo(filename);
    // }
    //
    // public static FileInfo GetMmsi30MinByDate(string year, string month, string day)
    // {
    //     var filename = $@"{FileReferences.Mmsi30MinFilenameBase}_{year}{month}{day}.txt";
    //     return GetProcessedFileInfo(filename);
    // }
    //
    // public static FileInfo GetMmsi30MinByDate(string fullDate)
    // {
    //     var filename = $@"{FileReferences.Mmsi30MinFilenameBase}_{fullDate}.txt";
    //     return GetProcessedFileInfo(filename);
    // }
    //

    public static List<string> LoadLinesFromFile(string filepath)
    {
        var lines = new List<string>();
        using var sr = new StreamReader(filepath);
        while (sr.ReadLine() is {} line)
        {
            if (string.IsNullOrWhiteSpace(line)) { continue; }
            lines.Add(line);
        }
        return lines;
    }

    private static void WriteListToFile(string filepath, List<string> lineList)
    {
        using var streamWriter = new StreamWriter(filepath);
        foreach (var line in lineList)
        {
            streamWriter.WriteLine(line);
        }
        streamWriter.Close();
    }

    public static void WriteDictToFile(string filepath, Dictionary<int, int> dict, string header = "")
    {
        using var streamWriter = new StreamWriter(filepath);
        if (!string.IsNullOrWhiteSpace(header))
        {
            streamWriter.WriteLine(header);
        }
        foreach (var line in dict)
        {
            streamWriter.WriteLine($"{line.Key},{line.Value}");
        }
        streamWriter.Close();
    }

    public static void WriteListToDggsTrackCountsFolder(string filename, List<string> lineList)
    {
        var path = Path.Combine(FileReferences.dggsTrackCountsFldr, filename);
        WriteListToFile(path, lineList);
    }

    public static void WriteListToProcessed(string filename, List<string> lineList)
    {
        var path = Path.Combine(FileReferences.processedPath, filename);
        WriteListToFile(path, lineList);
    }
    
    public static void WriteListToMmsi30MinFolder(string filename, List<string> lineList)
    {
        var path = Path.Combine(FileReferences.mmsi30MinBinsPath, filename);
        WriteListToFile(path, lineList);
    }
    
    public static void WriteListToMappedMmsi30MinFolder(string filename, List<string> lineList)
    {
        var path = Path.Combine(FileReferences.mappedMmsi30MinPath, filename);
        WriteListToFile(path, lineList);
    }

    public static void SerializeHexGrid(HexGrid hexGrid, string filename)
    {
        var jsonHexGridOut = JsonConvert.SerializeObject(hexGrid, Formatting.Indented);
        var hexGridFileOut = FileReferences.ResourceFileInfo(filename).FullName;

        using var streamWriterHexGrids = new StreamWriter(hexGridFileOut);
        streamWriterHexGrids.Write(jsonHexGridOut);
    }
    
    public static string GetHexGeoJson(int resolution)
    {
        var info = FileReferences.HexGridGeoJson(resolution.ToString());
        return info.OpenText().ReadToEnd();
    }
    
    public static string GetHexJson(int resolution)
    {
        var info = FileReferences.HexGridJson(resolution.ToString());
        return info.OpenText().ReadToEnd();
    }
    
    public static HexGrid DeserializeGeoJsonHexGrid(int resolution)
    {
        return JsonConvert.DeserializeObject<HexGrid>(GetHexGeoJson(resolution)) ?? new HexGrid();
    }

    public static HexGrid DeserializeHexGrid(int resolution)
    {
        return JsonConvert.DeserializeObject<HexGrid>(GetHexJson(resolution)) ?? new HexGrid();
    }

    public static void SaveToStringListFile<T, T1>(string path, IEnumerable<T1> enumerable, string header = "") 
        where T : IEnumerable<T1> where T1 : struct
    {
        using var streamWriter = new StreamWriter(path);
        if (!string.IsNullOrWhiteSpace(header))
        {
            streamWriter.WriteLine(header);
        }
        foreach (var structItem in enumerable)
        {
            streamWriter.WriteLine(structItem.ToString());
        }
    }

    public static List<string> LoadMmsiRoiList(string filename = "Mmsi_RecordsGt10.csv")
    {
        return LoadLinesFromFile(Path.Combine(FileReferences.mmsiStatsPath, filename));
    }

    public static string BuildMmsiRecordPath(int mmsi)
    {
        var folder = mmsi / 1_000_000;
        var path = Path.Combine(FileReferences.mmsiRecordsPath, folder.ToString());
        if (!Path.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    //
    // public static FileInfo GetHexGrid(string resolution)
    // {
    //     var filename = $@"{FileReferences.hexGridFilenameBase}{resolution}.json";
    //     return GetResourceFileInfo(filename);
    // }
    
}