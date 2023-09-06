// AisDataProcessing -- CsvCleaner.cs
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

namespace AisDataProcessing.Helpers;

public class CsvCleaner
{


    public static void CleanCsv(string path)
    {
        var lines = new List<string>();
        var i = 1;
        using var sr = new StreamReader(path);
        while (sr.ReadLine() is {} line)
        {
            if (i < 2)
            {
                i++;
                continue;
            }
            if (string.IsNullOrWhiteSpace(line)) { continue; }

            var split = line.Split(',');
            var num1 = (int) double.Parse(split[1]);
            var num2 = (int) double.Parse(split[2]);
            lines.Add($"{num1},{num2}");
        }
        sr.Close();
        
        using var streamWriter = new StreamWriter(path);
        streamWriter.WriteLine("src_seqnum,nbr_seqnum");
        foreach (var line in lines)
        {
            streamWriter.WriteLine(line);
        }
        streamWriter.Close();
    }
}