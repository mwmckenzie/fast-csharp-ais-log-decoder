// AisDataProcessing -- LogHelper.cs
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

namespace AisDataProcessing.Helpers;

public class LogController
{
    private FileStreamOptions _streamOptions;
    private List<string> _logLines;
    private FileInfo _fileInfo;
    
    private string _logPath;
    public string logPath
    {
        get => _logPath;
        set
        {
            _logPath = value;
            _fileInfo = new FileInfo(value);
        }
    }
    
    public LogController(string logName)
    {
        logPath = $"{FileReferences.logsPath}/{logName.Replace(".txt", "")}.txt";
        _streamOptions = new FileStreamOptions
        {
            Access = FileAccess.Write,
            BufferSize = 0,
            Mode = FileMode.Append,
            Options = FileOptions.None,
            PreallocationSize = 0,
            Share = FileShare.None
        };
        
        LoadLog();
    }

    public bool IsValid()
    {
        if (_fileInfo.Exists) return true;
        Console.WriteLine($"Log File Does Not Exist (Path: {_logPath})");
        return false;
    }

    private void LoadLog()
    {
        _logLines = new();
        if (!IsValid()) return;
        
        using var streamReader = new StreamReader(_logPath);
        while (streamReader.ReadLine() is { } lineIn)
        {
            _logLines.Add(lineIn);
        }
    }
    
    public void WriteToLog(string line)
    {
        //if (!IsValid()) return;
        
        using var streamWriter = new StreamWriter(_logPath, _streamOptions);
        streamWriter.WriteLine(line);
        _logLines.Add(line);
    }

    public void ClearLogFile(bool clearLogCache = true)
    {
        if (!IsValid()) return;
        _fileInfo.Delete();
        if (!clearLogCache) return;
        _logLines = new();
    }

    public bool LogLineExists(string line)
    {
        return _logLines.Any(x => x.Equals(line));
    }
   
}