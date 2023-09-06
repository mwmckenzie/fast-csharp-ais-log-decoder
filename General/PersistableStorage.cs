// AisDataProcessing -- PersistableStorage.cs
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

namespace AisDataProcessing.General;

public class PersistableStorage
{
    public string filename
    {
        get => _filename;
        set => _filename = value;
    }

    public string savePath
    {
        get => _savePath;
        set => _savePath = value;
    }

    private string _filename;
    private string _savePath;
    

    public void Save (IPersistableData persistableData)
    {
        var path = Path.Combine(_savePath, _filename);
        using var writer = new BinaryWriter(File.Open(path, FileMode.Create));
        persistableData.Save(new DataWriter(writer));
    }

    public void Load (IPersistableData persistableData)
    {
        var path = Path.Combine(_savePath, _filename);
        using var reader = new BinaryReader(File.Open(path, FileMode.Open));
        persistableData.Load(new DataReader(reader));
    }
}