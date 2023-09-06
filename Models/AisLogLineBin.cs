// AisDataProcessing -- AisLogLineBin.cs
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

using System.Text;
using AisDataProcessing.Utils;

namespace AisDataProcessing.Models;

public struct AisLogLineBin
{
    private double _latLonMult = 0.00000166666d;
    private double _speedMult = 0.1d;
    
    public string decodedLine;
    
    public string decodedMsgId;
    public string decodedRepeat;
    public string decodedMmsi;
    
    // Navigation Status
    // 0 Under way using engine 
    // 1 At anchor
    // 2 Not under command
    // 3 Restricted manoeuverability
    // 4 Constrained by her draught
    // 5 Moored
    // 6 Aground
    // 7 Engaged in Fishing
    // 8 Under way sailing
    // 9 Reserved for future amendment of Navigational Status for HSC
    // 10 Reserved for future amendment of Navigational Status for WIG
    // 11 Reserved for future use
    // 12 Reserved for future use
    // 13 Reserved for future use
    // 14 AIS-SART is active
    // 15 Not defined (default)
    public string navStatus;
                
    // Turn rate is encoded as follows:
    // 0 = not turning
    // 1 to 126 = turning right at up to 708 degrees per minute or higher
    // 1 to -126 = turning left at up to 708 degrees per minute or higher
    // 127 = turning right at more than 5deg/30s (No TI available) 
    // -127 = turning left at more than 5deg/30s (No TI available)
    // 128 (80 hex) indicates no turn information available (default)
    // Values between 0 and 708 degrees/min coded by ROTAIS=4.733 * SQRT(ROTsensor) degrees/min where
    // ROTsensor is the Rate of Turn as input by an external Rate of Turn Indicator.
    // ROTAIS is rounded to the nearest integer value.
    // Thus, to decode the field value, divide by 4.733 and then square it.
    // Sign of the field value should be preserved when squaring it,
    // otherwise the left/right indication will be lost.
    public string rateOfTurn;
                
    // Speed over ground is in 0.1-knot resolution from 0 to 102 knots.
    // Value 1023 indicates speed is not available, value 1022 indicates 102.2 knots or higher.
    public string speedOverGround;
                
    // The position accuracy flag indicates the accuracy of the fix.
    // A value of 1 indicates a DGPS-quality fix with an accuracy of < 10ms.
    // 0, the default, indicates an unaugmented GNSS fix with accuracy > 10m.
    public string posAccuracy;
                
    // Longitude is given in in 1/10000 min; divide by 600000.0 to obtain degrees.
    // Values up to plus or minus 180 degrees, East = positive, West \= negative.
    // A value of 181 degrees (0x6791AC0 hex) indicates that longitude is not available and is the default.
    public string lon;
                
    // Latitude is given in in 1/10000 min; divide by 600000.0 to obtain degrees.
    // Values up to plus or minus 90 degrees, North = positive, South = negative.
    // A value of 91 degrees (0x3412140 hex) indicates latitude is not available and is the default.
    public string lat;
                
    // Course over ground will be 3600 (0xE10) if that data is not available.
    public string cog;
    public string hdg;
                
    // Seconds in UTC timestamp should be 0-59, except for these special values:
    // 60 if time stamp is not available (default)
    // 61 if positioning system is in manual input mode
    // 62 if Electronic Position Fixing System operates in estimated (dead reckoning) mode,
    // 63 if the positioning system is inoperative.
    public string timeStamp;
                
    // The Maneuver Indicator (143-144) may have these values:
    // 0, Not available (default)
    // 1, No special maneuver
    // 2, Special maneuver (such as regional passing arrangement)
    public string maneuverInd;
    public string remainingBinary;

    public string lonConverted;
    public string latConverted;
                
    
    public AisLogLineBin(string decoded)
    {
        decodedLine = decoded;
        decodedMsgId = decoded[..6];
        decodedRepeat = decoded[6..8];
        decodedMmsi = decoded[8..38];
        navStatus = decoded[38..42];
        rateOfTurn = decoded[42..50];
        speedOverGround = decoded[50..60];
        posAccuracy = decoded[60].ToString();
        lon = decoded[61..89];
        lat = decoded[89..116];
        cog = decoded[116..128];
        hdg = decoded[128..137];
        timeStamp = decoded[137..142];
        maneuverInd = decoded[143..145];
        remainingBinary = decoded[145..];
    }

    public AisLogLineBin ConvertInPlace()
    {
        decodedMsgId = AisTools.ConvertBinary(decodedMsgId);
        decodedMmsi = AisTools.ConvertBinary(decodedMmsi);
        //navStatus = ConvertBinary(navStatus);
        //rateOfTurn = ConvertBinary(rateOfTurn);
        //speedOverGround = ConvertBinary(speedOverGround);
        //posAccuracy = ConvertBinary(posAccuracy);
        //lon = ConvertBinary(lon);
        //lat = ConvertBinary(lat);
        //cog = ConvertBinary(cog);
        //hdg = ConvertBinary(hdg);
        timeStamp = AisTools.ConvertBinary(timeStamp);

        lon = AisTools.ConvertSigNumBinary(lon, _latLonMult);
        lat = AisTools.ConvertSigNumBinary(lat, _latLonMult);
        
        return this;
    }
    
    public override string ToString()
    {
        var sb = new StringBuilder()
            .Append(decodedMmsi).Append(',')
            .Append(decodedMsgId).Append(',')
            .Append(lon).Append(',')
            .Append(lat).Append(',')
            .Append(timeStamp);
        
        return sb.ToString();
        //return $"{decodedMmsi},{decodedMsgId},{lon},{lat},{timeStamp}";
    }
}