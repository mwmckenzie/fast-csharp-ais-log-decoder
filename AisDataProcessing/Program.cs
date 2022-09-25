// Created by: Matthew McKenzie, September 2022

using System.Net;
using AisDataProcessing;

var watch = new System.Diagnostics.Stopwatch();

var processFirstPass = false;
var buildRefDicts = false;
var buildHelperDicts = false;
var processSecondPass = true;

const string basePath = "C:/Users/Matt/RiderProjects/AisDataProcessing/AisDataProcessing";
const string resourcesPath = basePath + "/Resources";
const string processedPath = basePath + "/Processed";

// const string filename = "20220301.log";
const string filename = "20220228.log";

const int epochFilterStart = 1646070000;
const int epochFilterEnd = 1646075000;

EpochHelper epochHelper;
MmsiHelper mmsiHelper;


watch.Start();

// Step 1: Filter Out Any Invalid Lines and Save Only Relevant Processing Info
// Note: Relevant processing info currently only 'payload' and 'epoch'

if (processFirstPass) {
    foreach (var filepath in Directory.GetFiles(resourcesPath)) {
        //Console.WriteLine($"Filepath: {filepath}");
        if (!filepath.Contains(".log")) { continue; }
        
        LogProcessor.TryFirstPassProcess(basePath, Path.GetFileName(filepath));
        //LogProcessor.TryFirstPassProcess(path, filename, epochFilterStart);
    }
}


// Step 2: Build Reference Dictionaries

if (buildRefDicts) {
    //LogProcessor.BuildEpochDictionary(processedPath, 1);
    LogProcessor.BuildMmsiDictionary(processedPath, 0);
}


// Step 3: Build Helper Dictionaries for Replacement Lookups (Only run for testing/timing)

if (buildHelperDicts) {
    
    epochHelper = new EpochHelper(processedPath);
    Console.WriteLine($"EpochHelper epoch key count: {epochHelper.epochDict.Keys.Count}, " +
                      $"first value: {epochHelper.epochDict.FirstOrDefault()}");

    mmsiHelper = new MmsiHelper(processedPath);
    Console.WriteLine($"MmsiHelper mmsi key count: {mmsiHelper.mmsiDict.Keys.Count}, " +
                      $"first value: {mmsiHelper.mmsiDict.Keys.FirstOrDefault()}, " +
                      $"{mmsiHelper.mmsiDict.Values.FirstOrDefault().FirstOrDefault()}");
    
}

// Step 4: Replace Values in First Pass Results with Dictionary Lookups and Save

if (processSecondPass) {
    epochHelper = new EpochHelper(processedPath);
    mmsiHelper = new MmsiHelper(processedPath);
    LogProcessor.TryProcessSecondPass(processedPath, epochHelper, mmsiHelper);
}

watch.Stop();
Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");