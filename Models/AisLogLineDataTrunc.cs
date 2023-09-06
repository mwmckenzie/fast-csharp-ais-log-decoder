// Created by: Matthew McKenzie, September 2022

namespace AisDataProcessing.Models; 

public struct AisLogLineDataTrunc {
    public string? recordType;
    public string? channel;
    public string? payload;
    public string? checkValue;
    public string? epoch;
    public bool isValid;

    public AisLogLineDataTrunc(string[] logLine) {
        if (logLine.Length < 8) {
            isValid = false;
            recordType = null;
            channel = null;
            payload = null;
            checkValue = null;
            epoch = null;
            return;
        }
        
        recordType = logLine[0];
        channel = logLine[4];
        payload = logLine[5];
        checkValue = logLine[6];
        epoch = logLine[7];

        isValid = payload.Length >= 28 && epoch.Length >= 10;

        // if (payload.Length < 28 || epoch.Length < 10) {
        //     isValid = false;
        //     return;
        // }
        //
        // isValid = true;
    }
    
    public override string ToString() {
        //return$"{recordType},{channel},{payload},{checkValue},{epoch}";
        return$"{payload},{epoch}";
    }
}