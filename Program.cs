// Created by: Matthew McKenzie, September 2022

using System.Text;
using AisDataProcessing.General;
using AisDataProcessing.Helpers;
using AisDataProcessing.Models;
using AisDataProcessing.Processing;
using AisDataProcessing.Static;
using AisDataProcessing.Utils;
using Newtonsoft.Json;

var watch = new System.Diagnostics.Stopwatch();

const bool processFirstPass = false;
var buildRefDicts = false;
var buildHelperDicts = false;
const bool processSecondPass = false;
const bool processBinDirectly = false;
const bool convertGeoJson = false;
const bool processZippedLogs = false;
const bool loadHexGrids = false;
const bool buildNeighborLists = false;
const bool buildChildrenLists = false;

const bool mapMmsiToR4 = false;


EpochDictLoader epochDictLoader;
MmsiDictLoader mmsiDictLoader;

watch.Start();

// Step 1: Filter Out Any Invalid Lines and Save Only Relevant Processing Info
// Note: Relevant processing info currently only 'payload' and 'epoch'
if (processFirstPass) {
    foreach (var filepath in Directory.GetFiles(FileReferences.resourcesPath)) {
        //Console.WriteLine($"Filepath: {filepath}");
        if (!filepath.Contains(".log")) { continue; }
        
        AisLogProcessor.TryFirstPassProcess(Path.GetFileName(filepath));
        //LogProcessor.TryFirstPassProcess(path, filename, epochFilterStart);
    }
}

// Step 2: Build Reference Dictionaries
if (buildRefDicts) {
    AisLogProcessor.BuildEpochDictionary(FileReferences.processedPath, 1);
    AisLogProcessor.BuildMmsiDictionary(FileReferences.processedPath, 0);
}

// Step 3: Build Helper Dictionaries for Replacement Lookups (Only run for testing/timing)
if (buildHelperDicts) {
    
    epochDictLoader = new EpochDictLoader(FileReferences.processedPath);
    Console.WriteLine($"EpochHelper epoch key count: {epochDictLoader.epochDict.Keys.Count}, " +
                      $"first value: {epochDictLoader.epochDict.FirstOrDefault()}");

    mmsiDictLoader = new MmsiDictLoader(FileReferences.processedPath);
    Console.WriteLine($"MmsiHelper mmsi key count: {mmsiDictLoader.mmsiDict.Keys.Count}, " +
                      $"first value: {mmsiDictLoader.mmsiDict.Keys.FirstOrDefault()}, " +
                      $"{mmsiDictLoader.mmsiDict.Values.FirstOrDefault().FirstOrDefault()}");
    
}

// Step 4: Replace Values in First Pass Results with Dictionary Lookups and Save
if (processSecondPass) {
    epochDictLoader = new EpochDictLoader(FileReferences.processedPath);
    mmsiDictLoader = new MmsiDictLoader(FileReferences.processedPath);
    AisLogProcessor.TryProcessSecondPass(FileReferences.processedPath, epochDictLoader, mmsiDictLoader);
}

// Step Optional: Process Binary Payload Directly via Single Step
if (processBinDirectly)
{
    // var info = new FileInfo(
    //     @"H:\Users\Matt\RiderProjects\AisDataProcessing\AisDataProcessing\Processed\firstPassResults_20220102.txt");
    
    var date = "20220102";
    //var info = new FileInfo(DataRef.GetFirstPassFileByDate(date).FullName);
    
    var bb = new BoundingBox
    {
        // xMin = 27.4437,
        // xMax = 42.355,
        // yMin = 40.9088,
        // yMax = 47.3103
        
        xMin = 100,
        xMax = 70,
        yMin = 18,
        yMax = 82
    };
    
    //FastDecoder.BuildDecodedLog(info, bb);
    //FastDecoder.BuildMultiMessageDecodedLog(info, bb);
    //FastDecoder.WriteMultiMsgDecodedLog(info, bb);
}

if (convertGeoJson)
{
    const int resolution = 8;
    
    var jsonText = DataService.GetHexGeoJson(resolution);
    var json = JsonConvert.DeserializeObject<GeoJson>(jsonText);

    if (json is null) return;

    Console.WriteLine($"GeoJson: {json.type} (FeatureCollection: {json.isFeatureCollection})");

    var hexGrid = GeoTools.ToHexGrid(json, resolution);
    hexGrid.res = resolution;
    hexGrid.name = $"R{resolution.ToString()}";

    Console.WriteLine($"HexGrid: Cell Count {hexGrid.hexCells.Count}");
    
    DataService.SerializeHexGrid(hexGrid, $"HexGrid_{hexGrid.name}.json");
    
    // var jsonHexGridOut = JsonConvert.SerializeObject(hexGrid, Formatting.Indented);
    // var hexGridFileOut = DataService.GetResourceFileInfo($"HexGrid_{hexGrid.name}.json").FullName;
    //
    // using var streamWriterHexGrids = new StreamWriter(hexGridFileOut);
    // streamWriterHexGrids.Write(jsonHexGridOut);

    
    var processBelow = false;
    
}

if (processZippedLogs)
{
    
    var firstPassLogHelper = new LogController("FirstPassFiles");
    var shortDecodedMsgLogHelper = new LogController("ShortDecodedMsgs");
    var mmsiFilteredBinLogHelper = new LogController("MmsiFilteredBinFiles");
    
    var processInfo = new AisProcessInfo
    {
        zipFilePath = Path.Combine(FileReferences.zippedBasePath,
            "2019.zip"),
        aisBinMsgPath = FileReferences.aisBinMsgPath,
        firstPassPath = FileReferences.firstPassPath,
        shortDecodedMsgPath = FileReferences.shortDecodedMsgPath,
        mmsiFilteredBinPath = FileReferences.mmsi30MinBinsPath,
        completedFirstPassLog = firstPassLogHelper,
        completedShortDecodedMsgLog = shortDecodedMsgLogHelper,
        completedMmsiFilteredBinLog = mmsiFilteredBinLogHelper,
        boundingBox = new BoundingBox()
        {
            xMin = -100,
            xMax = 70,
            yMin = 18,
            yMax = 82
        },
        fileProcessCount = 365,
    };
    ZippedAisLogProcessor.ProcessZippedAisLogs(processInfo);
}

if (loadHexGrids)
{
    const int resolution = 7;
    
    // var info = DataService.GetHexGrid(resolution.ToString());
    // var text = info.OpenText().ReadToEnd();
    // var hexGrid = JsonConvert.DeserializeObject<HexGrid>(text);

    var hexGrid = DataService.DeserializeHexGrid(resolution);
    
    Console.WriteLine($"HexGrid: {hexGrid.name} (Cell Count: {hexGrid.hexCells.Count})");
    if (hexGrid.res == 0)
    {
        return;
    }

    var saveTestJson = false;
    if (saveTestJson)
    { 
        DataService.SerializeHexGrid(hexGrid, $"HexGrid_{hexGrid.name}_FromJson.json");
        
        //  var jsonHexGridOut = JsonConvert.SerializeObject(hexGrid, Formatting.Indented);
        // var hexGridFileOut = DataService.GetResourceFileInfo($"HexGrid_{hexGrid.name}_FromJson.json").FullName;
        //
        // using var streamWriterHexGrids = new StreamWriter(hexGridFileOut);
        // streamWriterHexGrids.Write(jsonHexGridOut); 
    }
    
}

if (buildNeighborLists)
{
    var res = 4;
    var file = FileReferences.HexGridNeighborList(res.ToString());
    var hexGrid = DataService.DeserializeHexGrid(res);

    using var streamReader = new StreamReader(file.FullName);
    var i = 0;
    while (streamReader.ReadLine() is { } lineIn)
    {
        i++;
        if (i < 2) continue;
        
        var line = lineIn.Split(',');
        if (line.Length < 2)
        {
            continue;
        }
        
        var cell = hexGrid.Cell(int.Parse(line[0]));
        var neighbor = hexGrid.Cell(int.Parse(line[1]));

        if (cell is null || neighbor is null)
        {
            Console.WriteLine($"Cell {int.Parse(line[0])} or Neighbor {int.Parse(line[1])} Is Null");
            Console.WriteLine($"HexGrid Cell Count: {hexGrid.hexCells.Count}");
            continue;
        }
        
        cell.neighbors.Add(neighbor.info);
    }
    
    DataService.SerializeHexGrid(hexGrid, $"HexGrid_{hexGrid.name}_WithN.json");
}

if (buildChildrenLists)
{
    var res = 4;
    var file = FileReferences.HexGridChildList(res.ToString());

    var hexManager = HexGridManager.Instance;
    hexManager.LoadGrids(5);

    var hexGrid = hexManager.Grid(res);
    var childrenHexGrid = hexManager.Grid(res + 1);

    using var streamReader = new StreamReader(file.FullName);
    var i = 0;
    while (streamReader.ReadLine() is { } lineIn)
    {
        i++;
        if (i < 2) continue;
        
        var line = lineIn.Split(',');
        if (line.Length < 2)
        {
            continue;
        }
        
        var cell = hexGrid.Cell(int.Parse(line[0]));
        var child = childrenHexGrid.Cell(int.Parse(line[1]));

        if (cell is null || child is null)
        {
            Console.WriteLine($"Cell {int.Parse(line[0])} or Neighbor {int.Parse(line[1])} Is Null");
            Console.WriteLine($"HexGrid Cell Count: {hexGrid.hexCells.Count}");
            continue;
        }
        
        cell.children.Add(child.info);
    }
    streamReader.Close();
    hexManager.SaveGrid(res);
}

if (mapMmsiToR4)
{
    var lapWatch = new System.Diagnostics.Stopwatch();
    lapWatch.Start();
    
    var res = 4;
    var mapCount = 400;   
    
    var mappedMmsiLogController = new LogController("mmsiFilesMapped");

    var hexManager = HexGridManager.Instance;
    hexManager.LoadGrids();
    var hexGrid = hexManager.Grid(res);
    
    Console.WriteLine($"Grids Loaded; Time: {lapWatch.Elapsed.TotalSeconds} s [{lapWatch.ElapsedMilliseconds} ms]");
    lapWatch.Restart();
    
    var i = 1;
    foreach (var fileInfo in FileReferences.Mmsi30MinBinsDir().GetFiles())
    {
        if (i > mapCount) break;

        var filename = fileInfo.Name;
        
        
        if (mappedMmsiLogController.LogLineExists(filename)) { continue; }
        
        //var date = filename.Replace(fileInfo.Extension, "");
        Console.WriteLine($"Starting Name: {filename}; FullName: {fileInfo.FullName}");

        var newLines = new List<string>();
        
        var lineCount = 0;
        using var streamReader = new StreamReader(fileInfo.FullName);
        while (streamReader.ReadLine() is { } line)
        {
            lineCount++;
            if (lineCount < 2)
            {
                newLines.Add($"{line},R4,R5,R6,R7,R8");
                continue;
            }
            
            ReadOnlySpan<char> span = line.AsSpan();

            var firstComma = line.IndexOf(',', 0);
            var secondComma = line.IndexOf(',', firstComma + 1);
            var thirdComma = line.IndexOf(',', secondComma + 1);
            var fourthComma = line.IndexOf(',', thirdComma + 1);
            
            ReadOnlySpan<char> slice1 = span.Slice(secondComma + 1, thirdComma - secondComma - 1);
            var lineX = double.Parse(slice1);
            
            ReadOnlySpan<char> slice2 = span.Slice(thirdComma + 1, fourthComma - thirdComma - 1);
            var lineY = double.Parse(slice2);
            
            // var values = line.Split(',');
            // if (values.Length < 5)
            // {
            //     continue;
            // }

            // var lineX = double.Parse(values[2]);
            // var lineY = double.Parse(values[3]);
            
            var point = new Coord() { x = lineX, y = lineY };
            var r4Cell = hexGrid.FindContainingCell(point);

            var r5Cell = GeoTools.FindContainingChild(r4Cell, point);
            var r6Cell = GeoTools.FindContainingChild(r5Cell, point);
            var r7Cell = GeoTools.FindContainingChild(r6Cell, point);
            var r8Cell = GeoTools.FindContainingChild(r7Cell, point);

            var sb = new StringBuilder(line)
                .Append(r4Cell is null ? "," : $",{r4Cell.info.seqNum}")
                .Append(r5Cell is null ? "," : $",{r5Cell.info.seqNum}")
                .Append(r6Cell is null ? "," : $",{r6Cell.info.seqNum}")
                .Append(r7Cell is null ? "," : $",{r7Cell.info.seqNum}")
                .Append(r8Cell is null ? "," : $",{r8Cell.info.seqNum}");
            
            newLines.Add(sb.ToString());
            //newLines.Add($"{line},{r4Cell.info.seqNum},{r5Cell.info.seqNum},{r6Cell.info.seqNum},{r7Cell.info.seqNum},{r8Cell.info.seqNum}");
            //Console.WriteLine(newLines[^1]);
        }
        streamReader.Close();
        
        Console.WriteLine($"Mapping {i} Complete; Time: {lapWatch.Elapsed.TotalSeconds} s [{lapWatch.ElapsedMilliseconds} ms]");
        
        //DataService.WriteListToProcessed($"TestMappedMmsiList_{date}.txt", newLines);
        DataService.WriteListToMappedMmsi30MinFolder(filename, newLines);
        mappedMmsiLogController.WriteToLog(filename);
        
        Console.WriteLine($"File {i} Saved; Time: {lapWatch.Elapsed.TotalSeconds} s [{lapWatch.ElapsedMilliseconds} ms]");
        Console.WriteLine("");
        lapWatch.Restart();
        i++;
    }
    
    //hexGrid.ReorderByCount();
    //hexManager.SaveAllGrids();

    // using var streamReader = new StreamReader(file.FullName);
    // var i = 0;
    // var reorderIndex = 100;
    // while (streamReader.ReadLine() is { } lineIn)
    // {
    //     // example line
    //     // 237594800,1,25.32548869764,37.46520347192,1593561600
    //     i++;
    //     if (i < 2) continue;
    //
    //     var values = lineIn.Split(',');
    //     if (values.Length < 5)
    //     {
    //         continue;
    //     }
    //
    //     if (Math.DivRem(i, reorderIndex).Remainder == 0)
    //     {
    //         reorderIndex *= 2;
    //         hexGrid.ReorderByCount();
    //         Console.WriteLine($"Total Execution Time [{i}]: {watch.ElapsedMilliseconds} ms");
    //     }
    //
    //     hexGrid.FindContainingCell(new Coord
    //     {
    //         x = double.Parse(values[2]),
    //         y = double.Parse(values[3])
    //     });
    //
    // }
    //
    // var jsonOut = JsonConvert.SerializeObject(hexGrid, Formatting.Indented);
    // var fileOut = FileReferences.ResourceFileInfo("HexGrid_Coords_R7_Summary_20220101.json").FullName;
    //
    // using var streamWriter = new StreamWriter(fileOut);
    // using var reader = new StringReader(jsonOut);
    //
    // while (reader.ReadLine() is { } line)
    // {
    //     streamWriter.WriteLine(line);
    // }
    //
    // fileOut = FileReferences.ResourceFileInfo("HexGrid_Coords_R7_Summary_20220101.csv").FullName;
    // using var sw = new StreamWriter(fileOut);
    //
    // sw.WriteLine("seqnum,COUNT");
    // foreach (var cellCounts in hexGrid.hexCells)
    // {
    //     sw.WriteLine($"{cellCounts.Key},{cellCounts.Value.count}");
    // }
}

//StatsProcessor.GenerateDecodeProcessStats();
//GeoTools.GenerateGridCountsFile(8);

var binaryFilename = "20190101";

var logBinaryConverter = new DailyLogBinaryConverter();

// var path = Path.Combine(FileReferences.mmsiStatsPath, "MmsiCountsTotal.csv");
// var countList = new LookUpGenerator().LoadData(path);
//
// Console.WriteLine($"Total Count: {countList.Sum(x => x.b)}");
// countList = countList.Where(x => x.b > 9).ToList();
// Console.WriteLine($"Total Count: {countList.Sum(x => x.b)}");
//
// path = Path.Combine(FileReferences.mmsiStatsPath, "MmsiCountsTotal_RecordsGt10.csv");
// DataService.SaveToStringListFile<List<IntTupleAB>, IntTupleAB>(path, countList, "mmsi,count");

//logBinaryConverter.BuildAllMmsiRecords();

//mmsiRecord.DebugPrintMmsiRecord();


var csvPath = Path.Combine(FileReferences.hexNeighborsFldr, "Dggs_Isea4h_NeighborsList_R9.csv");
CsvCleaner.CleanCsv(csvPath);


watch.Stop();
Console.WriteLine("");
Console.WriteLine($"Execution Time (Totals):");
watch.PrintTime();