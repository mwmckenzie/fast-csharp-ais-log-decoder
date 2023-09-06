// AisDataProcessing -- StatProcessor.cs
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

using AisDataProcessing.Models;
using AisDataProcessing.Static;

namespace AisDataProcessing.Processing;

public class StatsProcessor
{

    public static List<IntTupleAB> FindDecodedTrackCounts(string filepath, List<IntTupleAB> trackCounts)
    {
        var totalTracks = new List<int>();
        var filteredTracks = new List<int>();

        var lines = DataService.LoadLinesFromFile(filepath);
        
        const char splitOn = ':';
        const string totalString = "Number of valid records"; // Number of valid records: 31626991
        const string filteredString = "Filtered Record Count"; // Filtered Record Count: 1029897
        
        foreach (var line in lines)
        {
            if (line.StartsWith(totalString))
            {
                var count = int.Parse(line.Split(splitOn)[1]);
                totalTracks.Add(count);
            }
            if (line.StartsWith(filteredString))
            {
                var count = int.Parse(line.Split(splitOn)[1]);
                filteredTracks.Add(count);
            }
        }

        var minCount = Math.Min(totalTracks.Count, filteredTracks.Count);
        for (int i = 0; i < minCount; i++)
        {
            trackCounts.Add(new IntTupleAB
            {
                name = "Total/Filtered",
                a = totalTracks[i],
                b = filteredTracks[i]
            });
        }
        
        return trackCounts;
    }

    public static void GenerateDecodeProcessStats()
    {
        var results = new List<IntTupleAB>();
        var years = new[] { "2019", "2020", "2021" };
        foreach (var year in years)
        {
            results = FindDecodedTrackCounts(FileReferences.DecodeProcessLog(year).FullName, results);
        }
        
        foreach (var result in results)
        {
            Console.WriteLine($"Results => {result.ToString()}");
        }

        var avgTotal = results.Average(x => x.a);
        var avgFiltered = results.Average(x => x.b);
        var avgPercentage = results.Average(x => x.PercentageBA()); // avgFiltered / avgTotal;

        Console.WriteLine("");
        Console.WriteLine($"Results Count: {results.Count}");
        Console.WriteLine($"Avgs: Total: {avgTotal}, Filtered: {avgFiltered}");
        Console.WriteLine($"Avg Percentage: {avgPercentage:P}");
    }

    public static Dictionary<int, int> FindUniqueMmsis()
    {
        var mmsiDict = new Dictionary<int, int>();

        return mmsiDict;
    }
}