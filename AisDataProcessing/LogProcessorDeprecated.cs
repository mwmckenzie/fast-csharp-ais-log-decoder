// Created by: Matthew McKenzie, September 2022

namespace AisDataProcessing; 

public class LogProcessorDeprecated {
    
    private List<AisLogLine> records = new();
    //private AisLogLine record;

    public bool TryProcessLog(string filename) {
        
        using (var sr = new StreamReader(
                   $"C:/Users/Matt/RiderProjects/AisDataProcessing/AisDataProcessing/Resources/{filename}")) {
            while (sr.ReadLine() is { } line) // && linesRead < 100)
            {
                if (!IsLineValid(line)) {
                    continue;
                }
                var record = new AisLogLine(line);
                if (!record.isValid) {
                    continue;
                }
                records.Add(record);
            }
        }

        Console.WriteLine($"Number of records collected: {records.Count}");

        using (var streamWriter = new StreamWriter(
                   $"C:/Users/Matt/RiderProjects/AisDataProcessing/AisDataProcessing/Processed/firstPassResults_{filename}")) {

            foreach (var record in records) {
                streamWriter.WriteLine(record.ToString());
            }
        }

        return true;
    }

    private bool IsLineValid(string line) {
        //Console.WriteLine($"{line.Length - line.Replace(",", "").Length}");
        return line.Length - line.Replace(",", "").Length == 7;
    }
}