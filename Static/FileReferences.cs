// AisDataAggregator -- FileConsts.cs
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

namespace AisDataProcessing.Static;

public static class FileReferences
{
    public const string basePath = "H:/Users/Matt/RiderProjects/AisDataProcessing/AisDataProcessing";
    public const string resourcesPath = basePath + "/Resources";
    public const string processedPath = basePath + "/Processed";
    public const string logsPath = basePath + "/Logs";

    //public const string aisFilenameBase = "DirectBinTranslation";
    //public const string aisFilenameBase = "DecodedAisMultiMsg";
    
    public const string firstPassFilenameBase = "firstPassResults";
    public const string aisFilenameBase = "AisMultiMsgStream";
    public const string shortDecodedFilenameBase = "MmsiBy30min";
    public const string mmsi30MinFilenameBase = "MmsiBy30min";
    public const string decodeProcessFilenameBase = "DecodeProcessLog_";

    public const string dggsFilenameBase = "Dggs_Isea4h_Res";
    public const string hexGridFilenameBase = "HexGrid_R";
    
    public const string currAisLogFilename = "20220102.log";
    
    public const string mmsi30MinLogFilename = "Mmsi30MinBinFiles_Created";
    
    public const string projectsPath = "C:/Users/Matt/Projects";
    public const string dataPath = projectsPath + "/AisDataDecoding";
    public const string firstPassPath = dataPath + "/FirstPass";
    public const string mmsi30MinBinsPath = dataPath + "/Mmsi30MinBins";
    public const string shortDecodedMsgPath = dataPath + "/ShortDecodedMsg";
    public const string aisBinMsgPath = dataPath + "/AisBinMsg";
    public const string mappedMmsi30MinPath = dataPath + "/MappedMmsi30Min";
    public const string dggsTrackCountsFldr = dataPath + "/DggsTrackCounts";
    public const string mmsiDailyBinaryPath = dataPath + "/Mmsi30MinDailyBinary";
    public const string mmsiStatsPath = dataPath + "/MmsiStats";
    public const string mmsiRecordsPath = dataPath + "/MmsiRecords";
    
    public const string zippedBasePath = "I:";

    public const string dggsDataPath = projectsPath + "/DggsHexMap";
    public const string hexChildTablesFldr = dggsDataPath + "/ChildIntersectTables";
    public const string hexNeighborsFldr = dggsDataPath + "/NeighborTables";
    
    
    
    public static FileInfo ResourceFileInfo(string filename) =>
        new ($@"{resourcesPath}\{filename}");
    
    public static FileInfo ProcessedFileInfo(string filename) =>
            new ($@"{processedPath}\{filename}");

    public static FileInfo FirstPassFileByDate(string year, string month, string day) =>
        ProcessedFileInfo($@"{firstPassFilenameBase}_{year}{month}{day}.txt");

    public static FileInfo FirstPassFileByDate(string fullDate) =>
        ProcessedFileInfo($@"{firstPassFilenameBase}_{fullDate}.txt");

    public static FileInfo ProcessedAisFileByDate(string year, string month, string day) =>
        ProcessedFileInfo($@"{aisFilenameBase}_{year}{month}{day}.txt");

    public static FileInfo Mmsi30MinByDate(string year, string month, string day) =>
        new ($@"{mmsi30MinBinsPath}/{mmsi30MinFilenameBase}_{year}{month}{day}.txt");

    public static FileInfo Mmsi30MinByDate(string fullDate) =>
        new ($@"{mmsi30MinBinsPath}/{mmsi30MinFilenameBase}_{fullDate}.txt");

    public static FileInfo HexGridGeoJson(string resolution) =>
        ResourceFileInfo($@"{dggsFilenameBase}{resolution}.geojson");

    public static FileInfo HexGridJson(string resolution) =>
        ResourceFileInfo($@"{hexGridFilenameBase}{resolution}.json");
    
    public static FileInfo HexGridChildList(string resolution) =>
        new ($@"{hexChildTablesFldr}\Dggs_Isea4h_ChildList_R{resolution}.csv");
    
    public static FileInfo HexGridNeighborList(string resolution) =>
        new ($@"{hexNeighborsFldr}\Dggs_Isea4h_NeighborsList_R{resolution}.csv");

    public static DirectoryInfo DggsTrackCountsDir() => new (dggsTrackCountsFldr);
    
    public static DirectoryInfo Mmsi30MinBinsDir() => new (mmsi30MinBinsPath);
    
    public static DirectoryInfo MappedMmsi30MinDir() => new (mappedMmsi30MinPath);
    
    public static DirectoryInfo DailyLogBinaryDir() => new (mmsiDailyBinaryPath);

    public static FileInfo DecodeProcessLog(string year) =>
        new($"{logsPath}/{decodeProcessFilenameBase}{year}.txt");
}