// Created by: Matthew McKenzie, September 2022

namespace AisDataProcessing; 

public struct AisLogLine {
    public string? recordType;
    public int? fragments;
    public int? fragment;
    public int? fragmentType;
    public string? channel;
    public string? payload;
    public string? checkValue;
    public string? epoch;
    public bool isValid;

    public AisLogLine(string logLine) {
        var lineElements = logLine.Split(',');
        if (lineElements.Length < 8) {
            isValid = false;
            recordType = null;
            fragments = null;
            fragment = null;
            fragmentType = null;
            channel = null;
            payload = null;
            checkValue = null;
            epoch = null;
            return;
        }
        
        recordType = lineElements[0];
        fragments = null; // int.Parse(lineElements[1]);
        fragment = null; // int.Parse(lineElements[2]);
        fragmentType = null; //int.Parse(lineElements[3]);
        channel = lineElements[4];
        payload = lineElements[5];
        checkValue = lineElements[6];
        epoch = lineElements[7]; //int.Parse(lineElements[7]);

        if (payload.Length < 28 || epoch.Length < 10) {
            isValid = false;
            return;
        }
        isValid = true;
    }

    public override string ToString() {
        return$"{recordType},{channel},{payload},{checkValue},{epoch}";
    }
}