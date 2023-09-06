namespace AisDataProcessing.Models; 

public struct AisLogLineV3 {
    public string? recordType;
    public string? fragments;
    public string? fragment;
    public string? fragmentType;
    public string? channel;
    public string? payload;
    public string? checkValue;
    public string? epoch;
    public bool isValid;

    public AisLogLineV3(string logLine) {
        recordType = null;
        fragments = null;
        fragment = null;
        fragmentType = null;
        channel = null;
        payload = null;
        checkValue = null;
        epoch = null;
        isValid = false;
        
        recordType = logLine.Substring(0, logLine.IndexOf(",", StringComparison.Ordinal));
        var lineFrag = RestOfLine(logLine);

        fragments = lineFrag.Substring(0,lineFrag.IndexOf(",", StringComparison.Ordinal));
        lineFrag = RestOfLine(lineFrag);
      
        fragment = lineFrag.Substring(0,lineFrag.IndexOf(",", StringComparison.Ordinal));
        lineFrag = RestOfLine(lineFrag);
        
        fragmentType = lineFrag.Substring(0,lineFrag.IndexOf(",", StringComparison.Ordinal));
        lineFrag = RestOfLine(lineFrag);

        channel = lineFrag.Substring(0,lineFrag.IndexOf(",", StringComparison.Ordinal));
        lineFrag = RestOfLine(lineFrag);
        
        payload = lineFrag.Substring(0,lineFrag.IndexOf(",", StringComparison.Ordinal));
        lineFrag = RestOfLine(lineFrag);
        
        checkValue = lineFrag.Substring(0,lineFrag.IndexOf(",", StringComparison.Ordinal));
        epoch = RestOfLine(lineFrag);
        
        //epoch = lineFrag.Substring(0,lineFrag.IndexOf(",", StringComparison.Ordinal));
        
        // if (payload.Length < 28 || epoch.Length < 10) {
        //     isValid = false;
        //     return;
        // }
        isValid = true;
    }

    public override string ToString() {
        return$"{recordType},{channel},{payload},{checkValue},{epoch}";
    }

    private string RestOfLine(string line) {
        var loc = line.IndexOf(",", StringComparison.Ordinal) + 1;
        //Console.WriteLine($"{line.Substring(loc, line.Length - loc)}");
        return line.Length - loc > 0 ? line.Substring(loc, line.Length - loc) : "";
    }
    
}