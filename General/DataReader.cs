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


using Quaternion = System.Numerics.Quaternion;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace AisDataProcessing.General
{
    public class DataReader
    {
        private readonly BinaryReader _reader;

        public DataReader(BinaryReader reader)
        {
            _reader = reader;
        }

        public int ReadInt()
        {
            return _reader.ReadInt32();
        }
        public float ReadFloat()
        {
            return _reader.ReadSingle();
        }
        public double ReadDouble()
        {
            return _reader.ReadDouble();
        }
        public string ReadString()
        {
            return _reader.ReadString();
        }
        
        public Vector2 ReadVector2()
        {
            return new Vector2
            {
                X = _reader.ReadSingle(),
                Y = _reader.ReadSingle()
            };
        }
        public Vector3 ReadVector3()
        {
            return new Vector3
            {
                X = _reader.ReadSingle(),
                Y = _reader.ReadSingle(),
                Z = _reader.ReadSingle()
            };
        }
        public Quaternion ReadQuaternion()
        {
            return new Quaternion
            {
                W = _reader.ReadSingle(),
                X = _reader.ReadSingle(),
                Y = _reader.ReadSingle(),
                Z = _reader.ReadSingle()
            };
        }
        public byte ReadByte()
        {
            return _reader.ReadByte();
        }
    }
}