// AisDataAggregator -- AggregateMmsi.cs
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

using AisDataProcessing.Static;

namespace AisDataProcessing.Processing;

public class AggregateMmsi
{

    public bool writeHeader = true;
    public bool addTime = true;
    
    public void AggregateByHour(string filepath, string fileOutPath)
    {
        Aggregate(filepath, fileOutPath, 1f / 3600f);
    }
    
    public void AggregateByHalfHour(string filepath, string fileOutPath)
    {
        Aggregate(filepath, fileOutPath, 1f / 1800f);
    }
    
    private void Aggregate(string filepath, string fileOutPath, float epochMult)
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        
        var records = new Dictionary<string, string>();
        
        using var streamReader = new StreamReader(filepath);
        while (streamReader.ReadLine() is { } line)
        {
            var values = line.Split(',');
            if (values.Length < 5)
            {
                continue;
            }

            int epochHours = (int) (int.Parse(values[4]) * epochMult);
            var hash = $"{values[0]}_{epochHours}";
            if (records.ContainsKey(hash))
            {
                continue;
            }

            if (addTime)
            {
                line = 
                    $"{line},{DateTimeOffset.FromUnixTimeSeconds(int.Parse(values[4])).UtcDateTime.ToString("s")}";
            }
            records.Add(hash, line);
        }
        watch.Stop();
        Console.WriteLine
            ($"AggregateByHour Step 2 (Decode & Filter) Process Time: {watch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Filtered Record Count: {records.Count}");
        watch.Reset();
        watch.Start();

        using var streamWriter = new StreamWriter(fileOutPath);

        if (writeHeader)
        {
            streamWriter.WriteLine(addTime ? $"{DataService.mmsiHeader},DateTime" : DataService.mmsiHeader);
        }
        
        foreach (var record in records)
        {
            streamWriter.WriteLine(record.Value);
        }
        watch.Stop();
        Console.WriteLine
            ($"AggregateByHour Step 3 (Write) Process Time: {watch.ElapsedMilliseconds} ms");
    }
}