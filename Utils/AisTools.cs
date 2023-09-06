// AisDataProcessing -- KenzTools.cs
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

using System.Globalization;
using System.Text;

namespace AisDataProcessing.Utils;

public static class AisTools
{
    // mult = 1 / 600000
    public const double latLonMult = 0.00000166666d;
    
    // mult = 1 / 10
    public const double speedMult = 0.1d;
    
    // Adapted from Java AIS Decoder; AISDecoder (class); 'getDecValueByBinStr' (method):
    // Repo: https://github.com/vlfig/ais-decoder/
    public static string ConvertSigNumBinary(string bin, double mult)
    {
        var decValue = Convert.ToInt32(bin, 2);
        if (bin[0] != '1') return Convert.ToString(decValue * mult, CultureInfo.InvariantCulture);
        
        char[] invert = new char[bin.Length];
        for (int i = 0; i < invert.Length; i++)
        {
            invert[i] = '1';
        }
        decValue ^= Convert.ToInt32(new string(invert), 2);
        decValue += 1;
        decValue = -decValue;
        return Convert.ToString(decValue * mult, CultureInfo.InvariantCulture);
    }
    
    public static string ConvertSigNumBinary(string bin)
    {
        var decValue = Convert.ToInt32(bin, 2);
        if (bin[0] != '1') return Convert.ToString(decValue, 10);
        
        char[] invert = new char[bin.Length];
        for (int i = 0; i < invert.Length; i++)
        {
            invert[i] = '1';
        }
        decValue ^= Convert.ToInt32(new string(invert), 2);
        decValue += 1;
        decValue = -decValue;
        return Convert.ToString(decValue, 10);
    }
    
    public static string ConvertBinary(string bin)
    {
        return Convert.ToString(Convert.ToInt32(bin, 2), 10);
    }
    
    public static int ConvertBinary2Int32(string bin)
    {
        return Convert.ToInt32(bin, 2);
    }
    
    public static string Ascii2Ais(string ascii)
    {
        var sb = new StringBuilder();
        foreach (var c in ascii.ToCharArray())
        {
            var i = Convert.ToInt32(c);
            i -= 48;
            if (i>40) 
            {
                i -= 8;
            }
            sb.Append(Convert.ToString(i, 2).PadLeft(6, '0'));
        }
        return sb.ToString();
    }
    
    public static string Ascii2Ais(string ascii, StringBuilder sb)
    {
        sb.Clear();
        foreach (var c in ascii.ToCharArray())
        {
            var i = Convert.ToInt32(c);
            i -= 48;
            if (i>40) 
            {
                i -= 8;
            }
            sb.Append(Convert.ToString(i, 2).PadLeft(6, '0'));
        }
        return sb.ToString();
    }
    
    public static double ConvertSigNumBin2Dbl(string bin, double mult)
    {
        var decValue = Convert.ToInt32(bin, 2);
        if (bin[0] != '1') return decValue * mult;
        
        char[] invert = new char[bin.Length];
        for (int i = 0; i < invert.Length; i++)
        {
            invert[i] = '1';
        }
        decValue ^= Convert.ToInt32(new string(invert), 2);
        decValue += 1;
        decValue = -decValue;
        return decValue * mult;
    }
}