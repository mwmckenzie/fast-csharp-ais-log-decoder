// AisDataProcessing -- AisLatLonMsg.cs
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

namespace AisDataProcessing.Models;

public readonly record struct AisLatLonMsg
{
    public int msgId { get; init; }
    public string payload { get; init; }
    public string epoch { get; init; }
    public double lon { get; init; }
    public double lat { get; init; }

    public bool IsInBoundingBox(BoundingBox bb)
    {
        if (lon < bb.xMin) return false;
        if (lon > bb.xMax) return false;
        if (lat < bb.yMin) return false;
        if (lat > bb.yMax) return false;

        return true;
    }
}