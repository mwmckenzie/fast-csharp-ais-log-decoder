// AisDataProcessing -- FastDecoder.cs
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

using System.Text;
using AisDataProcessing.Models;
using AisDataProcessing.Utils;

namespace AisDataProcessing.Processing;

public class FastDecoder
{
    public static void BuildDecodedLog(FileInfo file)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        
        var lineCount = File.ReadLines(file.FullName).Count();
        
        watch.Stop();
        Console.WriteLine
            ($"FastCascadingBinaryDecoder Step 1 (Count Lines) Process Time: {watch.ElapsedMilliseconds} ms");
        watch.Start();
        
        //var logLines = new AisLogLineBin[lineCount];
        var logLines = new string[lineCount];
        var i = 0;

        var sb = new StringBuilder();
        using var sr = new StreamReader(file.FullName);
        while (sr.ReadLine() is { } line)
        {
            var comma = line.IndexOf(',');
            var payload = line[..comma];
            //var epoch = line[comma..];

            var ais = AisTools.Ascii2Ais(payload, sb);

            var msgIdRecord = new AisMsgId
            {
                msgId = AisTools.ConvertBinary2Int32(ais[..6]),
                payload = ais,
                epoch = line[(comma + 1)..]
            };

            if (!msgIdRecord.isMsgOfInterest)
            {
                continue;
            }
            logLines[i] = new AisLogLineBin(msgIdRecord.payload).ConvertInPlace().ToString();
            i++;
        }
        
        //watch.Reset();
        watch.Stop();
        Console.WriteLine
            ($"FastCascadingBinaryDecoder Step 2 (Decode) Total Process Time: {watch.ElapsedMilliseconds} ms");
        watch.Start();
        
        var filenameOut = file.FullName.Replace(
            "firstPassResults", "DirectBinTranslation");

        using var streamWriter = new StreamWriter(filenameOut);
        foreach (var logLine in logLines) {
            streamWriter.WriteLine(logLine);
        }
        
        watch.Stop();
        Console.WriteLine($"FastCascadingBinaryDecoder Total Process Execution Time: {watch.ElapsedMilliseconds} ms");
        
    }
    
    public static void BuildDecodedLog(FileInfo file, BoundingBox boundingBox)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        
        var lineCount = File.ReadLines(file.FullName).Count();
        
        watch.Stop();
        
        Console.WriteLine
            ($"FastCascadingBinaryDecoder Step 1 (Count Records) Process Time: {watch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Record Count: {lineCount}");
        watch.Reset();
        watch.Start();
        
        var logLines = new string[lineCount];
        var i = 0;

        var sb = new StringBuilder();
        using var sr = new StreamReader(file.FullName);
        while (sr.ReadLine() is { } line)
        {
            var comma = line.IndexOf(',');
            var payload = line[..comma];
            var ais = AisTools.Ascii2Ais(payload, sb);

            var msgIdRecord = new AisMsgId
            {
                msgId = AisTools.ConvertBinary2Int32(ais[..6]),
                payload = ais,
                epoch = line[(comma + 1)..]
            };

            if (!msgIdRecord.isMsgOfInterest)
            {
                continue;
            }

            var msgLatLonRecord = new AisLatLonMsg
            {
                msgId = msgIdRecord.msgId,
                payload = msgIdRecord.payload,
                epoch = msgIdRecord.epoch,
                lon = AisTools.ConvertSigNumBin2Dbl(msgIdRecord.payload[61..89], AisTools.latLonMult),
                lat = AisTools.ConvertSigNumBin2Dbl(msgIdRecord.payload[89..116], AisTools.latLonMult)
            };

            if (!msgLatLonRecord.IsInBoundingBox(boundingBox))
            {
                continue;
            }

            sb.Clear()
                .Append(AisTools.ConvertBinary(msgLatLonRecord.payload[8..38])).Append(',')
                .Append(msgLatLonRecord.msgId).Append(',')
                .Append(msgLatLonRecord.lon).Append(',')
                .Append(msgLatLonRecord.lat).Append(',')
                .Append(msgLatLonRecord.epoch);

            logLines[i] = sb.ToString();
            //logLines[i] = new AisLogLineBin(msgIdRecord.payload).ConvertInPlace().ToString();
            i++;
        }
        
        watch.Stop();
        Console.WriteLine
            ($"FastCascadingBinaryDecoder Step 2 (Decode & Filter) Process Time: {watch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Filtered Record Count: {i}");
        watch.Reset();
        watch.Start();
        
        var filenameOut = file.FullName.Replace(
            "firstPassResults", "DirectBinTranslation");
        
        using var streamWriter = new StreamWriter(filenameOut);
        for (int j = 0; j < i; j++)
        {
            streamWriter.WriteLine(logLines[j]);
        }
        
        // foreach (var logLine in logLines) {
        //     streamWriter.WriteLine(logLine);
        // }
        
        watch.Stop();
        Console.WriteLine
            ($"FastCascadingBinaryDecoder Step 3 (Write) Process Time: {watch.ElapsedMilliseconds} ms");
    }
    
    public static void BuildMultiMessageDecodedLog(FileInfo file, BoundingBox boundingBox)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        
        var lineCount = File.ReadLines(file.FullName).Count();
        
        watch.Stop();
        
        Console.WriteLine
            ($"BuildMultiMessageDecodedLog Step 1 (Count Records) Process Time: {watch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Record Count: {lineCount}");
        watch.Reset();
        watch.Start();
        
        var logLines = new string[lineCount];
        var i = 0;

        var sb = new StringBuilder();
        using var sr = new StreamReader(file.FullName);
        while (sr.ReadLine() is { } line)
        {
            var comma = line.IndexOf(',');
            var payload = line[..comma];
            var ais = AisTools.Ascii2Ais(payload, sb);
            var msgId = AisTools.ConvertBinary2Int32(ais[..6]);
            
            int typeNum;
            double lon;
            double lat;

            switch (msgId)
            {
                case < 4:
                    typeNum = 1;
                    break;
                case > 17 and < 20:
                    typeNum = 2;
                    break;
                default:
                    continue;
            }
            
            if (typeNum == 1)
            {
                lon = AisTools.ConvertSigNumBin2Dbl(ais[61..89], AisTools.latLonMult);
            }
            else
            {
                lon = AisTools.ConvertSigNumBin2Dbl(ais[57..85], AisTools.latLonMult);
            }
            
            if (!IsValidLon(lon, boundingBox)) continue;
            
            if (typeNum == 1)
            {
                lat = AisTools.ConvertSigNumBin2Dbl(ais[89..116], AisTools.latLonMult);
            }
            else
            {
                lat = AisTools.ConvertSigNumBin2Dbl(ais[85..112], AisTools.latLonMult);
            }
            
            if (!IsValidLat(lat, boundingBox)) continue;
            
            var epoch = line[(comma + 1)..];
            
            sb.Clear()
                .Append(AisTools.ConvertBinary(ais[8..38])).Append(',')
                .Append(msgId).Append(',')
                .Append(lon).Append(',')
                .Append(lat).Append(',')
                .Append(epoch);

            logLines[i] = sb.ToString();
            i++;
        }
        
        watch.Stop();
        Console.WriteLine
            ($"BuildMultiMessageDecodedLog Step 2 (Decode & Filter) Process Time: {watch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Filtered Record Count: {i}");
        watch.Reset();
        watch.Start();
        
        var filenameOut = file.FullName.Replace(
            "firstPassResults", "DecodedAisMultiMsg");
        
        using var streamWriter = new StreamWriter(filenameOut);
        for (int j = 0; j < i; j++)
        {
            streamWriter.WriteLine(logLines[j]);
        }
        
        watch.Stop();
        Console.WriteLine
            ($"BuildMultiMessageDecodedLog Step 3 (Write) Process Time: {watch.ElapsedMilliseconds} ms");
        
        bool IsValidLon(double lon, BoundingBox bb)
        {
            if (lon < bb.xMin) return false;
            return !(lon > bb.xMax);
        }
    
        bool IsValidLat(double lat, BoundingBox bb)
        {
            if (lat < bb.yMin) return false;
            return !(lat > bb.yMax);
        }
    }
    
    public static void WriteMultiMsgDecodedLog(FileInfo file, BoundingBox boundingBox)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var filenameOut = file.FullName.Replace(
            "firstPassResults", "AisMultiMsgStream");
        
        using var streamWriter = new StreamWriter(filenameOut);

        //var sb = new StringBuilder();
        using var sr = new StreamReader(file.FullName);
        while (sr.ReadLine() is { } line)
        {
            var comma = line.IndexOf(',');
            var payload = line[..comma];
            //var ais = AisTools.Ascii2Ais(payload, sb);
            var ais = AisTools.Ascii2Ais(payload);
            var msgId = AisTools.ConvertBinary2Int32(ais[..6]);
            
            var typeNum = 1;
            switch (msgId)
            {
                case < 4:
                    //typeNum = 1;
                    break;
                case > 17 and < 20:
                    typeNum = 2;
                    break;
                default:
                    continue;
            }
            
            var lon = 
                AisTools.ConvertSigNumBin2Dbl(typeNum == 1 ? ais[61..89] : ais[57..85], AisTools.latLonMult);
            if (!IsValidLon(lon, boundingBox)) continue;
  
            var lat = 
                AisTools.ConvertSigNumBin2Dbl(typeNum == 1 ? ais[89..116] : ais[85..112], AisTools.latLonMult);
            if (!IsValidLat(lat, boundingBox)) continue;
            
            // var epoch = line[(comma + 1)..];
            //
            // sb.Clear()
            //     .Append(AisTools.ConvertBinary(ais[8..38])).Append(',')
            //     .Append(msgId).Append(',')
            //     .Append(lon).Append(',')
            //     .Append(lat).Append(',')
            //     .Append(epoch);

            //streamWriter.WriteLine(sb.ToString());
            
            streamWriter.WriteLine(
                $"{AisTools.ConvertBinary(ais[8..38])},{msgId},{lon},{lat},{line[(comma + 1)..]}");
        }
        
        watch.Stop();
        Console.WriteLine
            ($"BuildMultiMessageDecodedLog Step 2 (Decode & Filter) Process Time: {watch.ElapsedMilliseconds} ms");

        bool IsValidLon(double lon, BoundingBox bb)
        {
            if (lon < bb.xMin) return false;
            return !(lon > bb.xMax);
        }
    
        bool IsValidLat(double lat, BoundingBox bb)
        {
            if (lat < bb.yMin) return false;
            return !(lat > bb.yMax);
        }
    }
    
    public static void WriteShortDecodedLog(string firstPassFilePath, string decodedFilePath,  BoundingBox boundingBox)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        using var streamWriter = new StreamWriter(decodedFilePath);

        //var sb = new StringBuilder();
        using var sr = new StreamReader(firstPassFilePath);
        while (sr.ReadLine() is { } line)
        {
            var comma = line.IndexOf(',');
            var payload = line[..comma];
            //var ais = AisTools.Ascii2Ais(payload, sb);
            var ais = AisTools.Ascii2Ais(payload);
            var msgId = AisTools.ConvertBinary2Int32(ais[..6]);
            
            var typeNum = 1;
            switch (msgId)
            {
                case < 4:
                    //typeNum = 1;
                    break;
                case > 17 and < 20:
                    typeNum = 2;
                    break;
                default:
                    continue;
            }

            double lon;
            if (typeNum == 1)
            {
                lon = AisTools.ConvertSigNumBin2Dbl(ais[61..89], AisTools.latLonMult);
            }
            else
            {
                lon = AisTools.ConvertSigNumBin2Dbl(ais[57..85], AisTools.latLonMult);
            }
            if (!IsValidLon(lon, boundingBox)) continue;

            double lat;
            if (typeNum == 1)
            {
                lat = AisTools.ConvertSigNumBin2Dbl(ais[89..116], AisTools.latLonMult);
            }
            else
            {
                lat = AisTools.ConvertSigNumBin2Dbl(ais[85..112], AisTools.latLonMult);
            }
            if (!IsValidLat(lat, boundingBox)) continue;
            
            // var epoch = line[(comma + 1)..];
            //
            // sb.Clear()
            //     .Append(AisTools.ConvertBinary(ais[8..38])).Append(',')
            //     .Append(msgId).Append(',')
            //     .Append(lon).Append(',')
            //     .Append(lat).Append(',')
            //     .Append(epoch);

            //streamWriter.WriteLine(sb.ToString());
            
            streamWriter.WriteLine(
                $"{AisTools.ConvertBinary(ais[8..38])},{msgId},{lon},{lat},{line[(comma + 1)..]}");
        }
        
        watch.Stop();
        Console.WriteLine
            ($"BuildMultiMessageDecodedLog Step 2 (Decode & Filter) Process Time: {watch.ElapsedMilliseconds} ms");

        bool IsValidLon(double lon, BoundingBox bb)
        {
            if (lon < bb.xMin) return false;
            return !(lon > bb.xMax);
        }
    
        bool IsValidLat(double lat, BoundingBox bb)
        {
            if (lat < bb.yMin) return false;
            return !(lat > bb.yMax);
        }
    }
    

    
}