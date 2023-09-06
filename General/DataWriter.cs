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

using System.Numerics;

namespace AisDataProcessing.General
{
    public class DataWriter
    {
        private readonly BinaryWriter _writer;

        public DataWriter(BinaryWriter writer)
        {
            _writer = writer;
        }

        public void Write(int val)
        {
            _writer.Write(val);
        }
        public void Write(float val)
        {
            _writer.Write(val);
        }
        public void Write(double val)
        {
            _writer.Write(val);
        }
        public void Write(string val)
        {
            _writer.Write(val);
        }
        
        public void Write(Vector2 val)
        {
            _writer.Write(val.X);
            _writer.Write(val.Y);
        }
        public void Write(Vector3 val)
        {
            _writer.Write(val.X);
            _writer.Write(val.Y);
            _writer.Write(val.Z);
        }
        public void Write(Quaternion val)
        {
            _writer.Write(val.X);
            _writer.Write(val.Y);
            _writer.Write(val.Z);
            _writer.Write(val.W);
        }
        
    }
}