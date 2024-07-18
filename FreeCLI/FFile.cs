using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MabTool
{
 
    public class FFile
	{
		public MemoryStream _localstream;
		public List<FFile> sub_files;
		public string name;


	

			
   


        public FFile()
		{
			_localstream = new MemoryStream();
            sub_files = new List<FFile>();
         

        }
        public FFile(byte[] data)
        {
			_localstream = new MemoryStream(data);
			sub_files = new List<FFile> { };
        }	
        public static FFile OpenFile(string path)
		{

			try
			{
				return new FFile() { _localstream = new MemoryStream(File.ReadAllBytes(path))};
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return new FFile();
			}

		
		}

		public void SaveFile(string path)
		{
			var p = Path.Combine(path, name);


            if (name.Contains("\\"))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(p));
			
			}
		
            System.IO.File.WriteAllBytes(Path.Combine(path, name), ReadBytesAt(0, (int)_localstream.Length));
		}

        public void SaveFileF(string path)
        {
			var p = path;

			Directory.CreateDirectory(Path.GetDirectoryName(path));
            System.IO.File.WriteAllBytes(path, ReadBytesAt(0, (int)_localstream.Length));
        }
        public byte[] ReadBytes(int num)
		{

			byte[] loc = new byte[num];
			_localstream.Read(loc, 0, num);
			return loc;
		}
        public byte[] ReadBytesAt(uint addr,int num,SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            var _S = _localstream.Position;
            Jump(addr, seekOrigin);

            byte[] loc = new byte[num];
            _localstream.Read(loc, 0, num);

			Jump(_S, SeekOrigin.Begin);

            return loc;
        }
        public byte[] ReadBytesAt(uint addr, uint num, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            var _S = _localstream.Position;
            Jump(addr, seekOrigin);

            byte[] loc = new byte[num];
            _localstream.Read(loc, 0, (int)num);

            Jump(_S, SeekOrigin.Begin);

            return loc;
        }
        public unsafe T[] ReadArrayBEAt<T>(uint addr, int num, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            var _S = _localstream.Position;
            Jump(addr, seekOrigin);

			T[] ret = new T[num];


            var _tt = typeof(T).GetType();

			if (_tt.IsArray)
			{
				//var _array_size_mb = Marshal.SizeOf(typeof(T)); //
				//	var _elm_size_type = Marshal.SizeOf(_tt.GetElementType()); //	
			}
			else
			{
				var t = sizeof(T);
				if (t != null && t > 0)
				{
					unsafe
					{
						for (int i = 0; i < num; i++)
						{
							fixed (byte* ptr = ReadBytes(t).Reverse().ToArray())
							{
								ret[i] = *(T*)ptr;
							}
						}
					}
				}
			}



            Jump(_S, SeekOrigin.Begin);

            return ret;
        }

        public byte[] GetStreamRawData()
		{
			return _localstream.GetBuffer();
		}
        public byte[] GetArray()
        {
			return this.ReadBytesAt(0, (uint)_localstream.Length);
        }
        public string ReadArrayString(int count)
		{
			return System.Text.ASCIIEncoding.ASCII.GetString(ReadBytes(count));
		}
		public string ReadStringAt(uint addr, SeekOrigin seekOrigin = SeekOrigin.Begin)
		{
			var _S = _localstream.Position;
			Jump(addr, seekOrigin);

			var u = "";
			var b = (char)1;



			while ( b > 0)
			{
				b = (char)_localstream.ReadByte();
				if (b <= 0) break;
				u += b;
			}


            _localstream.Position = _S;
            return u;

		
			
		}

        public string ReadString(SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
   

            var u = "";
            var b = (char)1;

            while (b > 0)
            {
                b = (char)_localstream.ReadByte();
                if (b <= 0) break;
                u += b;
            }


            return u;

        }

		public void WriteString(string str)
		{
			_localstream.Write(str.ToArray().Select(zx=>(byte)zx).ToArray(), 0, str.Length);

		}

		public string ReadArrayStringAt(uint addr,int count,SeekOrigin seekOrigin = SeekOrigin.Begin)
		{
			var _S = _localstream.Position;
			Jump(addr,seekOrigin);
			var RE =  System.Text.ASCIIEncoding.ASCII.GetString(ReadBytes(count));
			_localstream.Position = _S;

			return RE;
		}
		public uint ReadUint32BE()
		{
			byte[] loc = ReadBytes(sizeof(uint));
			return 0;
			
		}
		public unsafe T ReadTypeBE<T>()
		{
			var _tt = typeof(T).GetType();

			if (_tt.IsArray)
			{
				//var _array_size_mb = Marshal.SizeOf(typeof(T)); //
			//	var _elm_size_type = Marshal.SizeOf(_tt.GetElementType()); //	
			}
			else
			{
				var t = sizeof(T);
				if (t != null && t > 0)
				{
					var _array = ReadBytes(t);
					_array = _array.Reverse().ToArray();
					unsafe
					{
						fixed (byte* ptr = _array)
						{
							T _t = *(T*)ptr;
							return _t;
						}
					}

				}

			}
			return (T)System.Convert.ChangeType(0, typeof(T));

		}
        public unsafe T ReadType<T>()
        {
            var _tt = typeof(T).GetType();

            if (_tt.IsArray)
            {
                //var _array_size_mb = Marshal.SizeOf(typeof(T)); //
                //	var _elm_size_type = Marshal.SizeOf(_tt.GetElementType()); //	
            }
            else
            {
                var t = sizeof(T);
                if (t != null && t > 0)
                {
                    var _array = ReadBytes(t);
                    _array = _array.ToArray();
                    unsafe
                    {
                        fixed (byte* ptr = _array)
                        {
                            T _t = *(T*)ptr;
                            return _t;
                        }
                    }

                }

            }
            return (T)System.Convert.ChangeType(0, typeof(T));

        }
		public unsafe void WriteBytes(byte[] data)
		{
            _localstream.Write(data,0, data.Length);	
        }
        public unsafe void WriteBytesAt(uint addr,byte[] data,SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
			var pos = GetCurrentPosition();
			Jump(addr, seekOrigin);
			_localstream.Write(data, 0, data.Length);
			Jump(pos);
		}




        public unsafe void WriteTypeBE<T>(T value)
		{
			var _tt = typeof(T).GetType();

			if (_tt.IsArray)
			{
				//var _array_size_mb = Marshal.SizeOf(typeof(T)); //
				//	var _elm_size_type = Marshal.SizeOf(_tt.GetElementType()); //	
			}
			else
			{
				var t = sizeof(T);
				var bt = new byte[t];
				if (t != null && t > 0)
				{

					unsafe
					{
						T _vl = value;

						var s = (byte*)&_vl;
						for (int i = 0; i < t; i++)
						{
							bt[i] = *(s+(t-1-i));
						}
					}
					_localstream.Write(bt,0,bt.Length);
				}
			}


		}
		public unsafe void WriteTypeBEAt<T>(long addr,T value , SeekOrigin origin = SeekOrigin.Begin)
		{

			var S = _localstream.Position;
			Jump(addr, origin);
			WriteTypeBE<T>(value);
			_localstream.Position = S;
		}


		public unsafe T ReadTypeBEAt<T>(uint addr, SeekOrigin origin = SeekOrigin.Begin)
		{
			var S1 = _localstream.Position;
			Jump(addr, origin);
			var S =  ReadTypeBE<T>();
			Jump(S1, SeekOrigin.Begin);
			return S;
		}

		public void Jump(int distance,SeekOrigin origin= SeekOrigin.Begin)
		{

			_localstream.Seek(distance, origin);
		}
		public void Jump(uint distance, SeekOrigin origin = SeekOrigin.Begin)
		{
			_localstream.Seek(distance, origin);
		}
		public void Jump(long distance, SeekOrigin origin = SeekOrigin.Begin)
		{
			_localstream.Seek(distance, origin);
		}


		public long GetCurrentPosition()
		{
			 return _localstream.Position;
		}

        internal T ReadTypeAt<T>(uint addr,SeekOrigin origin = SeekOrigin.Begin)
        {
            var S1 = _localstream.Position;
            Jump(addr, origin);
            var S = ReadType<T>();
            Jump(S1, SeekOrigin.Begin);
            return S;
        }
    }
}
