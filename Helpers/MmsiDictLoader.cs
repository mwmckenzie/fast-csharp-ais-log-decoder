// Created by: Matthew McKenzie, September 2022

namespace AisDataProcessing.Helpers; 

public class MmsiDictLoader {
    
    public Dictionary<string, Dictionary<string, string>> mmsiDict;
    public Dictionary<string, string> mmsiBinDict;

    public MmsiDictLoader(string dictionaryFilePath) {
        mmsiDict = new();
        mmsiBinDict = new();
        string mmsi;
        string binMmsi;
        string nonBinLine;
        string[] lineArr;
        

        var streamReader = new StreamReader($"{dictionaryFilePath}/mmsiDictionary.txt");
        
        while (streamReader.ReadLine() is { } line){
            mmsi = line.Split(',')[0];
            mmsiDict.TryAdd(mmsi, new Dictionary<string, string>());
        }
        
        streamReader = new StreamReader($"{dictionaryFilePath}/mmsiBinDictionary.txt");
        while (streamReader.ReadLine() is { } line){
            lineArr = line.Split(',');
            binMmsi = lineArr[0];
            mmsi = lineArr[1];
            nonBinLine = line.Split(',', 2)[1];
            mmsiBinDict.TryAdd(binMmsi, nonBinLine);
            if (mmsiDict.TryGetValue(mmsi, out var binDict)) {
                binDict.TryAdd(binMmsi, line);
            }
        }
    }
}