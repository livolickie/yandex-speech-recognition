using System;
using System.IO;

namespace Google_Speech_API
{
    public class Buffer
    {
        MemoryStream memory_data;
        BinaryWriter write_data;
        BinaryReader read_data;
        public Buffer()
        {
            memory_data = new MemoryStream();
            write_data = new BinaryWriter(memory_data);
            read_data = new BinaryReader(memory_data);
        }
        public void Write(int value)
        {
            write_data.Write(value);
            write_data.Flush();
        }
        public void Write(float value)
        {
            write_data.Write(value);
            write_data.Flush();
        }
        public void Write(double value)
        {
            write_data.Write(value);
            write_data.Flush();
        }
        public void Write(long value)
        {
            write_data.Write(value);
            write_data.Flush();
        }
        public void Write(string value)
        {
            write_data.Write(value);
            write_data.Flush();
        }
        public void Write(bool value)
        {
            write_data.Write(value);
            write_data.Flush();
        }
        public void WriteFile(string path)
        {
            if (File.Exists(path))
            {
                FileStream fl = new FileStream(path, FileMode.Open);
                write_data.Write((int)fl.Length);
                write_data.Flush();
                fl.CopyTo(memory_data);
                memory_data.Flush();
                fl.Close();
            }
        }
        public void Seek(int pos)
        {
            if (pos > 0 && pos < memory_data.Length)
            {
                memory_data.Position = pos;
            }
        }
        public void SeekStart()
        {
            memory_data.Position = 0;
        }
        public short ReadShort()
        {
            return read_data.ReadInt16();
        }
        public int ReadInt()
        {
            return read_data.ReadInt32();
        }
        public double ReadDouble()
        {
            return read_data.ReadDouble();
        }
        public long ReadLong()
        {
            return read_data.ReadInt64();
        }
        public string ReadString()
        {
            return read_data.ReadString();
        }
        public bool ReadBool()
        {
            return read_data.ReadBoolean();
        }
        public byte[] ReadData(int size)
        {
            return read_data.ReadBytes(size);
        }
        public FileInfo ReadFile(string path)
        {
            FileStream fl = new FileStream(path, FileMode.Create);
            int size = read_data.ReadInt32();
            fl.Write(memory_data.ToArray(), (int)memory_data.Position, size);
            fl.Flush();
            fl.Close();
            fl.Dispose();
            GC.Collect();
            FileInfo fll = new FileInfo(path);
            return fll;
        }
        public byte[] ReturnData()
        {
            return memory_data.ToArray();
        }
        public void WriteData(byte[] data)
        {
            memory_data.Write(data, 0, data.Length);
            write_data.Flush();
            data = null;
        }
        public void Clear()
        {
            memory_data.Seek(0, SeekOrigin.Begin);
            memory_data.Flush();
            write_data.Close();
            read_data.Close();
            memory_data.Close();
            memory_data.Dispose();
            write_data.Dispose();
            read_data.Dispose();
            memory_data = null;
            write_data = null;
            read_data = null;
            GC.Collect();
        }
    }
}