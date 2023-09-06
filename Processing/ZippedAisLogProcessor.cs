// AisDataProcessing -- ZippedAisLogProcessor.cs
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

using System.IO.Compression;
using AisDataProcessing.Models;

namespace AisDataProcessing.Processing;


public static class ZippedAisLogProcessor
{
    // Adapted from https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.zipfile.openread?view=net-7.0
    public static void ProcessZippedAisLogs(AisProcessInfo processInfo)
    {
        var count = 1;

        using ZipArchive archive = ZipFile.OpenRead(processInfo.zipFilePath);
        
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            
            if (count > processInfo.fileProcessCount) return;
            
            if (!entry.FullName.EndsWith(".log", StringComparison.OrdinalIgnoreCase)) continue;

            var currFilename = entry.Name.Replace(".log", ".txt");
            
            Console.WriteLine($"Starting Zipped File Processing for: {currFilename}");
            var stepWatch = new System.Diagnostics.Stopwatch();
            stepWatch.Start();
            
            if (!processInfo.completedFirstPassLog.LogLineExists(currFilename))
            {
                Console.WriteLine($"Starting Extraction for: {currFilename}");
                if (!ExtractToFirstPassFile(processInfo, currFilename, entry)) return;
            }
            
            stepWatch.Stop();
            Console.WriteLine($"First Pass Execution Time: {stepWatch.Elapsed.TotalSeconds} s [{stepWatch.ElapsedMilliseconds} ms]");
            stepWatch.Restart();


            var firstPass = Path.Combine(processInfo.firstPassPath, currFilename);
            var shortDecoded = Path.Combine(processInfo.shortDecodedMsgPath, currFilename);

            if (!processInfo.completedShortDecodedMsgLog.LogLineExists(currFilename))
            {
                AisLogProcessor.WriteShortDecodedLog(firstPass, shortDecoded, processInfo.boundingBox);
                
                File.Delete(firstPass);
                processInfo.completedShortDecodedMsgLog.WriteToLog(currFilename);
            }
            
            stepWatch.Stop();
            Console.WriteLine($"Short Decode Execution Time: {stepWatch.Elapsed.TotalSeconds} s [{stepWatch.ElapsedMilliseconds} ms]");
            stepWatch.Restart();

            var mmsiFilteredBin = Path.Combine(processInfo.mmsiFilteredBinPath, currFilename);
           
            if (!processInfo.completedMmsiFilteredBinLog.LogLineExists(currFilename))
            {
                var aggregator = new AggregateMmsi();
                aggregator.AggregateByHalfHour(shortDecoded, mmsiFilteredBin);
                
                File.Delete(shortDecoded);
                processInfo.completedMmsiFilteredBinLog.WriteToLog(currFilename);
            }
            
            stepWatch.Stop();
            Console.WriteLine($"MMSI Filter Execution Time: {stepWatch.Elapsed.TotalSeconds} s [{stepWatch.ElapsedMilliseconds} ms]");
            stepWatch.Restart();
            
            watch.Stop();
            Console.WriteLine("");
            Console.WriteLine($" -- Completed AIS Log {currFilename}, Log #{count}");
            Console.WriteLine($" -- Total Execution Time: {watch.Elapsed.TotalSeconds} s [{watch.ElapsedMilliseconds} ms]");
            Console.WriteLine("");
            count++;
        }
    }

    private static bool ExtractToFirstPassFile(AisProcessInfo processInfo, string currFilename, ZipArchiveEntry entry)
    {
        var destinationPath = Path.GetFullPath(Path.Combine(processInfo.aisBinMsgPath, currFilename));

        if (!File.Exists(destinationPath))
        {
            Console.WriteLine($"Extraction Does Not Exist -- Extracting: {destinationPath}");
            entry.ExtractToFile(destinationPath);
        }
        
        Console.WriteLine($"Trying First Pass for: {currFilename}");
        if (!AisLogProcessor.TryFirstPassProcess(destinationPath, currFilename, processInfo))
        {
            Console.WriteLine($"First Pass Failed: {currFilename}, {destinationPath}");
            return false;
        }

        Console.WriteLine($"Deleting: {destinationPath}");
        File.Delete(destinationPath);

        processInfo.completedFirstPassLog.WriteToLog(currFilename);
        return true;
    }
}