// Created by: Matthew McKenzie, September 2022
// Method 'Ascii2Ais' posted by user: https://www.codeproject.com/script/Membership/View.aspx?mid=3196973
// Method 'Ascii2Ais' posted on website: https://www.codeproject.com/Questions/288924/Convert-8bit-ASCII-to-6bit

using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AisDataProcessing; 

public static class LogProcessor {
    
    public static bool TryFirstPassProcess(string path, string filename, 
        int? filterByEpochBefore = null, int? filterByEpochAfter = null) {

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        
        var records = new List<AisLogLineDataTrunc>();
        string[] lineArr;
        
        using (var sr = new StreamReader(
                   $"{path}/Resources/{filename}")) {
            while (sr.ReadLine() is { } line)
            {
                lineArr = line.Split(',');
                var record = new AisLogLineDataTrunc(lineArr);
                if (!record.isValid) {
                    continue;
                }

                if (filterByEpochBefore != null) {
                    var epoch = record.epoch;
                    if (Int64.TryParse(epoch, out var epochNum)) {
                        if(filterByEpochBefore > epochNum)
                            continue;
                    }
                    
                }
                if (filterByEpochAfter != null){
                    var epoch = record.epoch;
                    if (Int64.TryParse(epoch, out var epochNum)) {
                        if(filterByEpochAfter < epochNum)
                            continue;
                    }
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
                   $"{path}/Processed/firstPassResults_{filename.Split('.')[0]}.txt")) {

            foreach (var record in records) {
                streamWriter.WriteLine(record.ToString());
            }
        }
        
        watch.Stop();
        Console.WriteLine($"TryFirstPassProcess Write Execution Time: {watch.ElapsedMilliseconds} ms");

        return true;
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
                if (Int64.TryParse(epoch, out var epochNum)) {
                    epochDict.TryAdd(epoch, DateTimeOffset.FromUnixTimeSeconds(epochNum).UtcDateTime.ToString());
                }
            }
        }

        Console.WriteLine($"Number of unique epochs: {epochDict.Count}");
        watch.Stop();
        Console.WriteLine($"BuildEpochDictionary Process Execution Time: {watch.ElapsedMilliseconds} ms");
        
        if (epochDict.Count < 1) { return false; }   
        
        watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        
        using var streamWriter = new StreamWriter($"{path}/epochDictionary.txt");
        foreach (var epochKvp in epochDict) {
            streamWriter.WriteLine($"{epochKvp.Key},{epochKvp.Value}");
        }
        
        watch.Stop();
        Console.WriteLine($"BuildEpochDictionary Write Execution Time: {watch.ElapsedMilliseconds} ms");
        
        return true;
    }
    
    public static bool BuildMmsiDictionary(string path, int index) {
        
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        var mmsiBinDict = new Dictionary<string, string>();
        var mmsiDict = new Dictionary<string, string>();
        // var mmsiRng = new Range(9, 38);
        // var repeatRng = new Range(7, 9);
        
        foreach (var filepath in Directory.GetFiles(path)) {
            if (!filepath.Contains("firstPassResults")) {
                continue;
            }
            using var sr = new StreamReader(filepath);
            while (sr.ReadLine() is { } line) {
                var payloadHead = line[..7];
                
                if (mmsiBinDict.ContainsKey(payloadHead)) continue;
                
                var decoded = Ascii2Ais(payloadHead).Replace(" ", "");
                var decodedMsgId = decoded[..6];
                var decodedRepeat = decoded[6..8];
                var decodedMmsi = decoded[8..38];
                var remainingBinary = decoded[38..];
                var msgId = Convert.ToString(Convert.ToInt32(decodedMsgId, 2), 10);
                var mmsi = Convert.ToString(Convert.ToInt32(decodedMmsi, 2), 10);
                    
                mmsiBinDict.TryAdd(payloadHead, $"{mmsi},{msgId},{decodedRepeat},{remainingBinary}");
                
                if (mmsiDict.ContainsKey(mmsi)) { continue; }
                
                mmsiDict.Add(mmsi, "");
            }
        }

        Console.WriteLine($"Number of unique mmsi 6-bit binary values: {mmsiBinDict.Count}");
        watch.Stop();
        Console.WriteLine($"BuildMmsiDictionary Process Execution Time: {watch.ElapsedMilliseconds} ms");
        
        if (mmsiBinDict.Count < 1) { return false; }
        
        watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        using var streamWriter = new StreamWriter($"{path}/mmsiBinDictionary.txt");
        foreach (var valuePair in mmsiBinDict) {
            streamWriter.WriteLine($"{valuePair.Key},{valuePair.Value}");
        }
        
        Console.WriteLine($"Number of unique mmsi values: {mmsiDict.Count}");
        using var streamWriter2 = new StreamWriter($"{path}/mmsiDictionary.txt");
        foreach (var valuePair in mmsiDict) {
            streamWriter2.WriteLine(valuePair.Key); // $"{valuePair.Key},{valuePair.Value}");
        }
        
        watch.Stop();
        Console.WriteLine($"BuildMmsiDictionary Write Execution Time: {watch.ElapsedMilliseconds} ms");
        
        return true;
    }
    
    // Method 'Ascii2Ais' posted by user: https://www.codeproject.com/script/Membership/View.aspx?mid=3196973
    // Method 'Ascii2Ais' posted on website: https://www.codeproject.com/Questions/288924/Convert-8bit-ASCII-to-6bit
    public static string Ascii2Ais(string p)
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
            sb.Append(sixBit + " ");
        }
        return sb.ToString();
    }

    public static bool TryProcessSecondPass(string processedFldrPath, EpochHelper epochHelper, MmsiHelper mmsiHelper) {

        var epochBins = epochHelper.epochDict;
        var mmsiBins = mmsiHelper.mmsiBinDict;

        foreach (var filepath in Directory.GetFiles(processedFldrPath)) {
            
            if (!filepath.Contains("firstPassResults")) { continue; }
            
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            
            var lineList = new List<string>();
            var filenameOut = Path.GetFileName(filepath).Replace(
                "firstPassResults", "secondPassResults");
            
            using var sr = new StreamReader(filepath);
            while (sr.ReadLine() is { } line) {
                var stringbuilder = new StringBuilder();
                var lineHead = line[..7];
                var epoch = line[^10..];
                if (mmsiBins.TryGetValue(lineHead, out var decodedMmsi)) {
                    stringbuilder.Append(decodedMmsi);
                    stringbuilder.Append(',');
                    stringbuilder.Append(line[7..^10]);
                    //line = $"{decodedMmsi},{line[7..^10]}"
                    //line = line.Replace(lineHead, decodedMmsi);
                }
                if (epochBins.TryGetValue(epoch, out var decodedEpoch)) {
                    stringbuilder.Append(decodedEpoch);
                    //line = $"{line},{decodedEpoch}";
                    //line = line.Replace(epoch, decodedEpoch);
                }
                lineList.Add(stringbuilder.ToString());
            }
            
            watch.Stop();
            Console.WriteLine($"TryProcessSecondPass Process Execution Time: {watch.ElapsedMilliseconds} ms");
            
            watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            
            using var streamWriter = new StreamWriter($"{processedFldrPath}/{filenameOut}");
            foreach (var newLine in lineList) {
                streamWriter.WriteLine(newLine);
            }
            
            watch.Stop();
            Console.WriteLine($"TryProcessSecondPass Write Execution Time: {watch.ElapsedMilliseconds} ms");
        }

        return true;
    }
}
