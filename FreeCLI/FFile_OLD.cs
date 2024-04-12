using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FreeCLI
{

    public class FFile_OLD<T>
    {

        private Type StreamType;
        private Stream Stream;



        [XmlAttribute]
        public string FileName { get; set; }

        [XmlAttribute]
        public byte Tag;

    
        public string GetFileName()
        {
          
            return this.FileName;


        }

        public FFile_OLD()
        {

        }
        public FFile_OLD(string FileName)
        {
            this.FileName = FileName;
        }
    
        public byte[] GetEndFile()
        {
            this.ChangeAbsolutePosition(0);

            byte[] buffer = new byte[Stream.Length];
            Stream.Read(buffer, 0, (int)Stream.Length);
            return buffer;
        }

        public static FFile_OLD<MemoryStream> GetMemoryStreamFromFile(string path)
        {
            var u = File.ReadAllBytes(path);
           var r  = new FFile_OLD<MemoryStream>(path);
            r.Stream = new MemoryStream(u);
         
           return r;
        }
        public static string OpenFileAndGetHeader(string path,int length)
        {
            var F = File.Open(path,FileMode.Open);
            var r = new FFile_OLD<T>(path);
            r.Stream = F;
            var s = r.ReadFixedString(4);
            F.Close();
            return s;
        }
        public static FFile_OLD<MemoryStream> GetFromMemoryStream(string path, byte[] data)
        {
            var r = new FFile_OLD<MemoryStream>(path);
            r.Stream = new MemoryStream(data);
            return r;
        }
        public static FFile_OLD<MemoryStream> CreateFileAsMemoryStream(string path)
        {
            var r = new FFile_OLD<MemoryStream>(path);
            r.Stream = new MemoryStream();
            return r;
        }

        public BinaryWriter GetBinaryWritter()
        {
            return new BinaryWriter((Stream)Stream);
        }
        public BinaryReader GetBinaryReader()
        {
            return new BinaryReader((Stream)Stream);
        }

        public long GetStreamPosition()
        {
            return this.Stream.Position;
        }


        public void MovePosition(long direction)
        {
            this.Stream.Position += direction;
        }

        #region Readers

        public int ReadInt32()
        {
            return new BinaryReader(this.Stream).ReadInt32();
        }
        public uint ReadUInt32()
        {
            return new BinaryReader(this.Stream).ReadUInt32();
        }
        public int ReadInt16()
        {
            return new BinaryReader(this.Stream).ReadInt16();
        }
        public int ReadUInt16()
        {
            return new BinaryReader(this.Stream).ReadUInt16();

        }
        public int ReadInt32BE()
        {
            byte[] b = new byte[4]; Stream.Read(b, 0, 4);
            return (int)((b[0] << 24) | (b[1] << 16) | (b[2] << 8) | b[3]);
        }
        public uint ReadUInt32BE()
        {
            byte[] b = new byte[4]; Stream.Read(b, 0, 4);
            return (uint)((b[0] << 24) | (b[1] << 16) | (b[2] << 8) | b[3]);
        }

        public int ReadInt16BE()
        {
            byte[] b = new byte[2]; Stream.Read(b, 0, 2);
            return (short)((b[0] << 8) | b[1]);
        }
        public ushort ReadUInt16BE()
        {
            byte[] b = new byte[2]; Stream.Read(b, 0, 2);
            return (ushort)((b[0] << 8) | b[1]);
        }
        public float ReadFloatBE()
        {
            byte[] b = new byte[4]; Stream.Read(b, 0, 4);
            int po = ((b[0] << 24) | (b[1] << 16) | (b[2] << 8) | b[3]);
            float result = 0;
            unsafe
            {
                result  = * (float*)&po;
            }
            return result;
        }
        public string ReadFixedString(int count)
        {
            byte[] b = new byte[count]; Stream.Read(b, 0, count);
            return Encoding.UTF8.GetString(b);
        }
        public string ReadString()
        {
            string result = "";
            int ln = 0;
            while (true)
            {
                var c = (char)Stream.ReadByte();
                if (c == 0) break;
                result += (char)c;
             
            }
            return result;
        }
        public byte[] ReadBytes(int count)
        {
            byte[] b = new byte[count]; Stream.Read(b, 0, count);
            return b;
        }
        public byte ReadByte()
        {

            return (byte)Stream.ReadByte();
        }
        public byte[] ReadBytes(long count)
        {
            byte[] b = new byte[count]; Stream.Read(b, 0, (int)count);
            return b;
        }
        public void ChangeAbsolutePosition(long pos)
        {
            this.Stream.Position = pos;
        }

        #endregion

        #region Writers

        public void WriteInt32(int val)
        {
            byte[] save = new byte[4];
            unsafe
            {
                var x = (byte*)&val;
                save[0] = x[0];
                save[1] = x[1];
                save[2] = x[2];
                save[3] = x[3];


            }
            this.Stream.Write(save, 0, 4);
        }
        public void WriteInt32BE(int val)
        {
            byte[] save = new byte[4];
            unsafe
            {
                var x = (byte*)&val;
                save[0] = x[3];
                save[1] = x[2];
                save[2] = x[1];
                save[3] = x[0];
            }
            this.Stream.Write(save, 0, 4);
        }
        public void WriteUInt32BE(uint val)
        {
            byte[] save = new byte[4];
            unsafe
            {
                var x = (byte*)&val;
                save[0] = x[3];
                save[1] = x[2];
                save[2] = x[1];
                save[3] = x[0];
            }
            this.Stream.Write(save, 0, 4);
        }
        public void WriteUInt32(int val)
        {
            byte[] save = new byte[4];
            unsafe
            {
                var x = (byte*)&val;
                save[0] = x[0];
                save[1] = x[1];
                save[2] = x[2];
                save[3] = x[3];
            }
            this.Stream.Write(save, 0, 4);
        }
        public void WriteByte(byte val)
        {
            this.Stream.WriteByte(val);
            
        }
        public void WriteUInt16(byte val)
        {
            byte[] save = new byte[2];
            unsafe
            {
                var x = (byte*)&val;
                save[0] = x[0];
                save[1] = x[1];
       
            }
            this.Stream.Write(save, 0, 2);


        }
        public void WriteUInt16BE(UInt16 val)
        {
            byte[] save = new byte[2];
            unsafe
            {
                var x = (byte*)&val;
                save[0] = x[1];
                save[1] = x[0];
    
            }
            this.Stream.Write(save, 0, 2);


        }


        public void WriteString(string str)
        {
            var buff = System.Text.Encoding.ASCII.GetBytes(str);
            this.Stream.Write(buff, 0, str.Length);
        }
        public void WriteBytes(byte[] bytes)
        {
            this.Stream.Write(bytes, 0, bytes.Length);
        }

        public static long GetFilzeSie(string path)
        {
            return new FileInfo(path).Length;
        }

        public void WritePadding(int count)
        { 
            for (int i= 0; i < count; i++)
            {
                this.Stream.WriteByte(0);
            }
        }
        #endregion



        public void PadFFileByStreamPosition(int padding)
        {
            while (GetStreamPosition() % padding != 0) {
                WriteByte(0);
            }
        }

        public long GetLength()
        {
            return Stream.Length;
        }


        }
}
