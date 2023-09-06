// Created by: Matthew McKenzie, September 2022
// Method 'Ascii2Ais' posted by user: https://www.codeproject.com/script/Membership/View.aspx?mid=3196973
// Method 'Ascii2Ais' posted on website: https://www.codeproject.com/Questions/288924/Convert-8bit-ASCII-to-6bit

using System.Text;
using AisDataProcessing.Helpers;
using AisDataProcessing.Models;
using AisDataProcessing.Static;
using AisDataProcessing.Utils;

namespace AisDataProcessing.Processing; 

public static class AisLogProcessor {
    
    public static bool TryFirstPassProcess(string filename, 
        int filterByEpochBefore, int filterByEpochAfter) {

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        
        var records = new List<AisLogLineDataTrunc>();

        using (var sr = new StreamReader(FileReferences.ResourceFileInfo(filename).FullName)) {
            while (sr.ReadLine() is { } line)
            {
                var lineArr = line.Split(',');
                var record = new AisLogLineDataTrunc(lineArr);
                if (!record.isValid) {
                    continue;
                }
                
                var epoch = record.epoch;
                if (long.TryParse(epoch, out var epochNum)) {
                    if(filterByEpochBefore > epochNum || filterByEpochAfter < epochNum)
                        continue;
                }
                records.Add(record);
            }
        }

        Console.WriteLine($"Number of valid records: {records.Count}");
        
        watch.Stop();
        Console.WriteLine($"TryFirstPassProcess Process Execution Time: {watch.ElapsedMilliseconds} ms");
            
        watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        using (var streamWriter = new StreamWriter(
                   $"{FileReferences.processedPath}/firstPassResults_{filename.Split('.')[0]}.txt")) {

            foreach (var record in records) {
                streamWriter.WriteLine(record.ToString());
            }
        }
        
        watch.Stop();
        Console.WriteLine($"TryFirstPassProcess Write Execution Time: {watch.ElapsedMilliseconds} ms");

        return true;
    }
    
    public static bool TryFirstPassProcess(string filename, 
        int filterByEpoch, bool filterIsBefore) {

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        
        var records = new List<AisLogLineDataTrunc>();

        using (var sr = new StreamReader(FileReferences.ResourceFileInfo(filename).FullName)) {
            while (sr.ReadLine() is { } line)
            {
                var lineArr = line.Split(',');
                var record = new AisLogLineDataTrunc(lineArr);
                if (!record.isValid) {
                    continue;
                }

                if (filterIsBefore) {
                    var epoch = record.epoch;
                    if (long.TryParse(epoch, out var epochNum)) {
                        if(filterByEpoch > epochNum)
                            continue;
                    }
                    
                }
                else
                {
                    var epoch = record.epoch;
                    if (long.TryParse(epoch, out var epochNum)) {
                        if(filterByEpoch < epochNum)
                            continue;
                    }
                }
                records.Add(record);
            }
        }

        Console.WriteLine($"Number of valid records: {records.Count}");
        
        watch.Stop();
        Console.WriteLine($"TryFirstPassProcess Process Execution Time: {watch.ElapsedMilliseconds} ms");
            
        //watch = new System.Diagnostics.Stopwatch();
        watch.Reset();
        watch.Start();

        using (var streamWriter = new StreamWriter(
                   $"{FileReferences.processedPath}/firstPassResults_{filename.Split('.')[0]}.txt")) {

            foreach (var record in records) {
                streamWriter.WriteLine(record.ToString());
            }
        }
        
        watch.Stop();
        Console.WriteLine($"TryFirstPassProcess Write Execution Time: {watch.ElapsedMilliseconds} ms");

        return true;
    }
    
    public static bool TryFirstPassProcess(string filename) {

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        
        var lineCount = File.ReadLines(FileReferences.ResourceFileInfo(filename).FullName).Count();
        
        watch.Stop();
        
        Console.WriteLine
            ($"TryFirstPassProcess Step 1 (Count Records) Process Time: {watch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Record Count: {lineCount}");
        watch.Reset();
        watch.Start();
        
        var i = 0;

        //var records = new HashSet<AisLogLineDataTrunc>();
        var records = new AisLogLineDataTrunc[lineCount];
        //var records = new List<AisLogLineDataTrunc>();

        using (var sr = new StreamReader(FileReferences.ResourceFileInfo(filename).FullName)) {
            while (sr.ReadLine() is { } line)
            {
                var lineArr = line.Split(',');
                var record = new AisLogLineDataTrunc(lineArr);
                if (!record.isValid) {
                    continue;
                }
                
                //records.Add(record);
                records[i] = record;
                i++;
            }
        }
        //Console.WriteLine($"Number of valid records: {records.Count}");
        Console.WriteLine($"Number of valid records: {i}");
        watch.Stop();
        Console.WriteLine($"TryFirstPassProcess Process Execution Time: {watch.ElapsedMilliseconds} ms");
            
        watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        using (var streamWriter = new StreamWriter(
                   $"{FileReferences.processedPath}/firstPassResults_{filename.Split('.')[0]}.txt")) {

            foreach (var record in records) {
                streamWriter.WriteLine(record.ToString());
            }
        }
        watch.Stop();
        Console.WriteLine($"TryFirstPassProcess Write Execution Time: {watch.ElapsedMilliseconds} ms");

        return true;
    }
    
    public static bool TryFirstPassProcess(string filepath, string currFilename, AisProcessInfo processInfo) {

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        //var records = new HashSet<AisLogLineDataTrunc>();
        var records = new List<AisLogLineDataTrunc>();

        using (var sr = new StreamReader(filepath)) {
            while (sr.ReadLine() is { } line)
            {
                var lineArr = line.Split(',');
                var record = new AisLogLineDataTrunc(lineArr);
                if (!record.isValid) {
                    continue;
                }
                
                records.Add(record);
            }
        }
        Console.WriteLine($"Number of valid records: {records.Count}");
        watch.Stop();
        Console.WriteLine($"TryFirstPassProcess Process Execution Time: {watch.ElapsedMilliseconds} ms");
        
        watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        using (var streamWriter = new StreamWriter(Path.Combine(processInfo.firstPassPath, currFilename))) {
            foreach (var record in records) {
                streamWriter.WriteLine(record.ToString());
            }
        }
        watch.Stop();
        Console.WriteLine($"TryFirstPassProcess Write Execution Time: {watch.ElapsedMilliseconds} ms");

        return true;
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
    
    public static bool BuildEpochDictionary(string path, int index) {

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        
        var epochDict = new Dictionary<string, string>();

        foreach (var filepath in Directory.GetFiles(path)) {
            if (!filepath.Contains("firstPassResults")) {
                continue;
            }
            using var sr = new StreamReader(filepath);
            while (sr.ReadLine() is { } line) {
                var epoch = line.Split(',')[index];
                if (long.TryParse(epoch, out var epochNum)) {
                    epochDict.TryAdd(epoch, DateTimeOffset.FromUnixTimeSeconds(epochNum).UtcDateTime.ToString());
                }
            }
        }
        Console.WriteLine($"Number of unique epochs: {epochDict.Count}");
        watch.Stop();
        
        Console.WriteLine($"BuildEpochDictionary Process Execution Time: {watch.ElapsedMilliseconds} ms");
        
        if (epochDict.Count < 1) { return false; }   

        watch.Reset();
        watch.Start();
        
        using var streamWriter = new StreamWriter($"{path}/epochDictionary.txt");
        foreach (var epochKvp in epochDict) {
            streamWriter.WriteLine($"{epochKvp.Key},{epochKvp.Value}");
        }
        watch.Stop();
        Console.WriteLine($"BuildEpochDictionary Write Execution Time: {watch.ElapsedMilliseconds} ms");
        
        return true;
    }

    public static void BuildDecodedLog(FileInfo file)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        
        var lineCount = File.ReadLines(file.FullName).Count();
        
        watch.Stop();
        Console.WriteLine($"BuildDecodedLog Step 1 (Count Lines) Process Time: {watch.ElapsedMilliseconds} ms");
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
            //logLines[i] = new AisLogLineBin(Ascii2Ais(payload)).ConvertInPlace().ToString();
            i++;
        }
        
        //watch.Reset();
        watch.Stop();
        Console.WriteLine($"BuildDecodedLog Step 2 (Decode) Total Process Time: {watch.ElapsedMilliseconds} ms");
        watch.Start();
        
        var filenameOut = file.FullName.Replace(
            "firstPassResults", "DirectBinTranslation");

        using var streamWriter = new StreamWriter(filenameOut);
        foreach (var logLine in logLines) {
            streamWriter.WriteLine(logLine);
        }
        
        watch.Stop();
        Console.WriteLine($"BuildDecodedLog Total Process Execution Time: {watch.ElapsedMilliseconds} ms");
        
    }
    
    public static bool BuildMmsiDictionary(string path, int index) {
        
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var payLoadHeadDict = new Dictionary<string, string>();
        var mmsiBinDict = new Dictionary<string, string>();
        var mmsiDict = new Dictionary<string, int>();
        var msgIdDict = new Dictionary<string, string>();
        // var mmsiRng = new Range(9, 38);
        // var repeatRng = new Range(7, 9);
        
        foreach (var filepath in Directory.GetFiles(path)) {
            if (!filepath.Contains("firstPassResults")) {
                continue;
            }
            using var sr = new StreamReader(filepath);
            while (sr.ReadLine() is { } line) {
                
                //ReadOnlySpan<char> spanString = line;
                
                var payloadHead = line[..7];
                //var payloadHead = spanString.Slice(0, 7);
                
                if (payLoadHeadDict.ContainsKey(payloadHead)) continue;
                
                // var decoded = Ascii2Ais(payloadHead).Replace(" ", "");
                //var decoded = Ascii2Ais(payloadHead);
                var decoded = Ascii2Ais(line);
                var decodedMsgId = decoded[..6];
                var decodedRepeat = decoded[6..8];
                var decodedMmsi = decoded[8..38];
                // var remainingBinary = decoded[38..];

                
                // Navigation Status
                // 0 Under way using engine 
                // 1 At anchor
                // 2 Not under command
                // 3 Restricted manoeuverability
                // 4 Constrained by her draught
                // 5 Moored
                // 6 Aground
                // 7 Engaged in Fishing
                // 8 Under way sailing
                // 9 Reserved for future amendment of Navigational Status for HSC
                // 10 Reserved for future amendment of Navigational Status for WIG
                // 11 Reserved for future use
                // 12 Reserved for future use
                // 13 Reserved for future use
                // 14 AIS-SART is active
                // 15 Not defined (default)
                var navStatus = decoded[38..42];
                
                // Turn rate is encoded as follows:
                // 0 = not turning
                // 1 to 126 = turning right at up to 708 degrees per minute or higher
                // 1 to -126 = turning left at up to 708 degrees per minute or higher
                // 127 = turning right at more than 5deg/30s (No TI available) 
                // -127 = turning left at more than 5deg/30s (No TI available)
                // 128 (80 hex) indicates no turn information available (default)
                // Values between 0 and 708 degrees/min coded by ROTAIS=4.733 * SQRT(ROTsensor) degrees/min where
                // ROTsensor is the Rate of Turn as input by an external Rate of Turn Indicator.
                // ROTAIS is rounded to the nearest integer value.
                // Thus, to decode the field value, divide by 4.733 and then square it.
                // Sign of the field value should be preserved when squaring it,
                // otherwise the left/right indication will be lost.
                var rateOfTurn = decoded[42..50];
                
                // Speed over ground is in 0.1-knot resolution from 0 to 102 knots.
                // Value 1023 indicates speed is not available, value 1022 indicates 102.2 knots or higher.
                var speedOverGround = decoded[50..60];
                
                // The position accuracy flag indicates the accuracy of the fix.
                // A value of 1 indicates a DGPS-quality fix with an accuracy of < 10ms.
                // 0, the default, indicates an unaugmented GNSS fix with accuracy > 10m.
                var posAccuracy = decoded[60];
                
                // Longitude is given in in 1/10000 min; divide by 600000.0 to obtain degrees.
                // Values up to plus or minus 180 degrees, East = positive, West \= negative.
                // A value of 181 degrees (0x6791AC0 hex) indicates that longitude is not available and is the default.
                var lon = decoded[61..89];
                
                // Latitude is given in in 1/10000 min; divide by 600000.0 to obtain degrees.
                // Values up to plus or minus 90 degrees, North = positive, South = negative.
                // A value of 91 degrees (0x3412140 hex) indicates latitude is not available and is the default.
                var lat = decoded[89..116]; 
                
                // Course over ground will be 3600 (0xE10) if that data is not available.
                var cog = decoded[116..128];
                var hdg = decoded[128..137];
                
                // Seconds in UTC timestamp should be 0-59, except for these special values:
                // 60 if time stamp is not available (default)
                // 61 if positioning system is in manual input mode
                // 62 if Electronic Position Fixing System operates in estimated (dead reckoning) mode,
                // 63 if the positioning system is inoperative.
                var timeStamp = decoded[137..142];
                
                // The Maneuver Indicator (143-144) may have these values:
                // 0, Not available (default)
                // 1, No special maneuver
                // 2, Special maneuver (such as regional passing arrangement)
                var maneuverInd = decoded[143..145];
                
                var remainingBinary = decoded[145..];

                msgIdDict.TryAdd(decodedMsgId, Convert.ToString(Convert.ToInt32(decodedMsgId, 2), 10));
                mmsiBinDict.TryAdd(decodedMmsi, Convert.ToString(Convert.ToInt32(decodedMmsi, 2), 10));
                
                // if (!msgIdDict.ContainsKey(decodedMsgId)) {
                //     msgIdDict.Add(decodedMsgId, Convert.ToString(Convert.ToInt32(decodedMsgId, 2), 10));
                // }
                var msgId =  msgIdDict[decodedMsgId];
                // if (!mmsiBinDict.ContainsKey(decodedMmsi)) {
                //     mmsiBinDict.Add(decodedMmsi, Convert.ToString(Convert.ToInt32(decodedMmsi, 2), 10));
                // }
                var mmsi = mmsiBinDict[decodedMmsi];
                    
                payLoadHeadDict.TryAdd(payloadHead, $"{mmsi},{msgId},{decodedRepeat},{remainingBinary}");

                if (!mmsiDict.TryAdd(mmsi, 1)) {
                    mmsiDict[mmsi]++;
                }
                // mmsiDict.Add(mmsi, 1);
            }
        }
        Console.WriteLine($"Number of unique mmsi 6-bit binary values: {payLoadHeadDict.Count}");
        watch.Stop();
        Console.WriteLine($"BuildMmsiDictionary Process Execution Time: {watch.ElapsedMilliseconds} ms");
        
        if (payLoadHeadDict.Count < 1) { return false; }

        watch.Reset();
        watch.Start();

        using var streamWriter = new StreamWriter($"{path}/mmsiBinDictionary.txt");
        foreach (var valuePair in payLoadHeadDict) {
            streamWriter.WriteLine($"{valuePair.Key},{valuePair.Value}");
        }
        Console.WriteLine($"Number of unique mmsi values: {mmsiDict.Count}");
        
        using var streamWriter2 = new StreamWriter($"{path}/mmsiDictionary.txt");
        foreach (var valuePair in mmsiDict) {
            streamWriter2.WriteLine($"{valuePair.Key},{valuePair.Value}"); // $"{valuePair.Key},{valuePair.Value}");
        }
        watch.Stop();
        Console.WriteLine($"BuildMmsiDictionary Write Execution Time: {watch.ElapsedMilliseconds} ms");
        
        return true;
    }
    
    // Method 'Ascii2Ais' posted by user: https://www.codeproject.com/script/Membership/View.aspx?mid=3196973
    // Method 'Ascii2Ais' posted on website: https://www.codeproject.com/Questions/288924/Convert-8bit-ASCII-to-6bit
    private static string Ascii2Ais(string p)
    {
        var sb = new StringBuilder();

        var chars = p.ToCharArray();
        foreach (var c in chars)
        {
            var i = Convert.ToInt32(c);
            i -= 48;
            if (i>40) {
                i -= 8;
            }

            var sixBit = Convert.ToString(i, 2).PadLeft(6, '0');
            // sb.Append(sixBit + " ");
            sb.Append(sixBit);
        }
        return sb.ToString();
    }

    public static bool TryProcessSecondPass(string processedFldrPath, EpochDictLoader epochDictLoader, MmsiDictLoader mmsiDictLoader) {

        var epochBins = epochDictLoader.epochDict;
        var mmsiBins = mmsiDictLoader.mmsiBinDict;
        var stringBuilder = new StringBuilder();
        
        foreach (var filepath in Directory.GetFiles(processedFldrPath)) {
            
            if (!filepath.Contains("firstPassResults")) { continue; }
            
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            //var strBldrList = new StringBuilder();
            var lineList = new List<string>();
            var filenameOut = Path.GetFileName(filepath).Replace(
                "firstPassResults", "secondPassResults");
            
            using var sr = new StreamReader(filepath);
            while (sr.ReadLine() is { } line) {
                //var stringbuilder = new StringBuilder();

                stringBuilder.Clear();
                var lineHead = line[..7];
                var epoch = line[^10..];

                // stringbuilder.Append(mmsiBins[lineHead]);
                // stringbuilder.Append(line[7..^10]);
                // // stringbuilder.Append(line.Replace(epoch, "").Replace(lineHead, ","));
                // if (epochBins.ContainsKey(epoch)) {
                //     stringbuilder.Append(epochBins[epoch]);
                // }
                
                if (mmsiBins.TryGetValue(lineHead, out var decodedMmsi)) {
                    stringBuilder.Append(decodedMmsi);
                    stringBuilder.Append(',');
                    stringBuilder.Append(line[7..^10]);
                }
                if (epochBins.TryGetValue(epoch, out var decodedEpoch)) {
                    stringBuilder.Append(decodedEpoch);
                }

                //strBldrList.AppendLine(stringbuilder.ToString());
                lineList.Add(stringBuilder.ToString());
            }
            watch.Stop();
            Console.WriteLine($"TryProcessSecondPass Process Execution Time: {watch.ElapsedMilliseconds} ms");
            
            watch.Reset();
            watch.Start();

            using (var streamWriter = new StreamWriter($"{processedFldrPath}/{filenameOut}"))
            {
                foreach (var newLine in lineList) 
                {
                    streamWriter.WriteLine(newLine);
                }
            }
            watch.Stop();
            Console.WriteLine($"TryProcessSecondPass Write Execution Time: {watch.ElapsedMilliseconds} ms");
        }
        return true;
    }
}