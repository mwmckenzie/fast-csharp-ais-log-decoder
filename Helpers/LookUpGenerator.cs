// AisDataProcessing -- LookUpGenerator.cs
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

namespace AisDataProcessing.Helpers;

public class LookUpGenerator
{

    public List<IntTupleAB> LoadData(string path)
    {
        var countList = new List<IntTupleAB>();
        var lineNum = 1;
        using var streamReader = new StreamReader(path);
        while (streamReader.ReadLine() is {} line)
        {
            if (lineNum == 1)
            {
                lineNum++;
                continue;
            }

            var split = line.Split(',');
            countList.Add(new IntTupleAB
            {
                a = int.Parse(split[0]),
                b = int.Parse(split[1])
            });
        }

        return countList;
    }
    
    
}