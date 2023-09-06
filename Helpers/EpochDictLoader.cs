﻿// Created by: Matthew McKenzie, September 2022

namespace AisDataProcessing.Helpers; 

public class EpochDictLoader {
    
    public Dictionary<string, string> epochDict;

    public EpochDictLoader(string dictionaryFilePath) {
        epochDict = new();
        string[] lineArr;
        
        var streamReader = new StreamReader($"{dictionaryFilePath}/epochDictionary.txt");
        
        while (streamReader.ReadLine() is { } line){
            lineArr = line.Split(',');
            if (lineArr.Length < 2) {
                continue;
            }
            epochDict.TryAdd(lineArr[0], lineArr[1]);
        }
    }
}