using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace FreeCLI
{

    public struct PastFileMini
    {
        public uint Relative;
        public uint Length;
        public PastFileMini(uint R,uint L)
        {
            this.Relative = R;
            this.Length = L;
        }
    }

    [Serializable]
    [XmlInclude(typeof(PastFileListA))]
    [XmlInclude(typeof(PastFileListB))]
    [XmlInclude(typeof(PastFileEndMark))]

    public class PastFileBlockEx {
    public PastFileBlockEx()
        {

        }
    }
    public class PastFileListB:PastFileBlockEx
    {
        public PastFileListB()
        {

        }
        public List<FFile<MemoryStream>> Files { get; set; } = new List<FFile<MemoryStream>>();

        [XmlAttribute]
        public bool HasTags { get; set; }

        
        public byte[] FilesTags;
    


    }
    public class PastFileListA:PastFileBlockEx
    {
        public PastFileListA()
        {

        }
        public FFile<MemoryStream> File = new FFile<MemoryStream>();
    }
    public class PastFileEndMark : PastFileBlockEx
    {

    }




    public interface PFile
    {
     
        List<PastFileBlockEx> Files { get; set; }
    }

    public class PastFile: PFile
    {
        public bool Status;

        public string PastFileName ="";

        public const string Header = "paST";

        public List<PastFileBlockEx> Files { get => _Files; set => _Files = value; }
        private List<PastFileBlockEx> _Files = new List<PastFileBlockEx>();

        public static void ReadPackedFile<T>(FFile<T> st,PFile rt,long offset,long length = 0)
        {
            st.ChangeAbsolutePosition(offset);
            var F = st.ReadBytes(4);
            var FHead = System.Text.Encoding.ASCII.GetString(F);
            if (IsCommonMiniPackFile(F))
            {
                var Mini_Count = F[1];
                var uknown_1 = F[2];
                var uknown_2 = F[3]; //AddExtraPad 0x20???????///
                var Base_New = st.GetStreamPosition();
                long[] second_table_from = new long[Mini_Count];
                long[] second_table_length = new long[Mini_Count];
                string[] PastFileMiniNames = new string[Mini_Count];
                for (int j = 0; j < Mini_Count; j++)
                {
                    second_table_from[j] = st.ReadUInt32BE();

                }
                for (int j = 0; j < Mini_Count; j++)
                {
                    second_table_length[j] = st.ReadUInt32BE();

                }
                var PT = new PastFileListB();
                // Pad 0xB
                if (uknown_2 != 0)
                {
                    var getter = st.GetStreamPosition();
                    PT.FilesTags = st.ReadBytes(Mini_Count);
                    PT.HasTags = true;
                    //st.MovePosition(Mini_Count);
                }
                for (int j = 0; j < Mini_Count; j++)
                {
                    PastFileMiniNames[j] = st.ReadString();

                }

                for (int j = 0; j < Mini_Count; j++)
                {
                    long pos = offset + second_table_from[j];
                    st.ChangeAbsolutePosition(pos);
                    var buffer = st.ReadBytes(second_table_length[j]);
                    var ext = ".dds";
                    var FF = FFile<MemoryStream>.GetFromMemoryStream(PastFileMiniNames[j] + ext, buffer);
                    if (PT.HasTags)
                        FF.Tag = PT.FilesTags[j];
                    PT.Files.Add(FF);
                }
                rt.Files.Add(PT);


            }
            else if (IsENO(F))
            {
                var ENO_Relative = st.ReadUInt32();
                var ENO_U1 = st.ReadUInt32BE();
                var ENO_BASE_OFFSET = st.ReadUInt32BE();
                //////////////////////////////////////////
                var UENO_1 = st.ReadUInt32BE();
                var UENO_2 = st.ReadUInt32BE();
                var UENO_3 = st.ReadUInt32BE();
                var UENO_4 = st.ReadUInt32BE();
                st.ChangeAbsolutePosition(offset + UENO_2); //+4 because padding or eno
                var NOFO = st.ReadFixedString(4);
                if (NOFO != "NOF0")
                {

                    Console.WriteLine("WARNING : UNSOPORTED ENO");
                    return;
                }
                var NOFO_SIZE = st.ReadUInt32();
                st.MovePosition(NOFO_SIZE);
                var NFNO = st.ReadFixedString(4);
                if (NFNO != "NFN0")
                {
                    Console.WriteLine("WARNING : UNSOPORTED ENO");
                    return;
                }

                var NFNO_SIZE = st.ReadUInt32();
                var Move_TO = st.GetStreamPosition() + NFNO_SIZE + 0x10; //Last Section
                var SIZE = Move_TO - offset;
                st.MovePosition(8);
                var FileName = st.ReadString();
                st.ChangeAbsolutePosition(offset);
                var buffer = st.ReadBytes(SIZE);
                var a = new PastFileListA();
                a.File = FFile<MemoryStream>.GetFromMemoryStream(FileName, buffer);
                rt.Files.Add(a);

            }
            else
            {
                st.MovePosition(-4);
                var a = new PastFileListA();
                var buffer = st.ReadBytes(length);
                a.File = FFile<MemoryStream>.GetFromMemoryStream($"FRiders{offset.ToString("x2")}", buffer);
                rt.Files.Add(a);
                Console.WriteLine($"WARNING : UNSOPORTED FILE OR ARCHIVE TO EXTRACT : {offset.ToString("x8")}");
            }
        }
        

        public static PastFile Read<T>(FFile<T> st)
        {
      
            var rt = new PastFile();
            rt.PastFileName = st.GetFileName();
            var Header = st.ReadFixedString(4);
            if (Header != PastFile.Header) return rt;
            var FFCount = st.ReadInt32BE();
            long[] offset_table = new long[FFCount];
            for (int i = 0; i < FFCount; i++)
            {
                offset_table[i] = st.ReadInt32BE();
            }
            List<FFile<MemoryStream>> Files_To_Extract = new List<FFile<MemoryStream>>();
            for (int i = 0; i < offset_table.Length; i++)
            {
                ReadPackedFile(st, rt, offset_table[i]);
            }
            for (int i = 0; i < Files_To_Extract.Count; i++)
            {
                //var s = Files_To_Extract[i];
                //string Folder_Save = "D:\\Games\\X360\\SonicFreeRiders\\SonicFreeRiders\\Game\\X\\Tump\\";
              //  var FN = s.GetFileName();
            //    string Full = Path.Combine(Folder_Save,FN );
          //      File.WriteAllBytes(Full,s.GetEndFile());
               

            }


            rt.Status = true;

            return rt;

        }

        public void Unpack(string path = null)
        {

            var FileNameT = Path.GetFileNameWithoutExtension(PastFileName);
            var FileDir = Path.GetDirectoryName(PastFileName);
            var End_Dir = Path.Combine(FileDir, FileNameT+"."+Header);
            if (!Directory.Exists(End_Dir))
            {
                Directory.CreateDirectory(End_Dir);
            }
            var XML_S = new System.Xml.Serialization.XmlSerializer(typeof(List<PastFileBlockEx>));
            var FS = File.Create(Path.Combine(End_Dir , Header + ".xml"));
            XML_S.Serialize(FS,Files);
            FS.Close();
            
            for (int i = 0; i < Files.Count; i++)
            {
                var F = Files[i];
                if (F.GetType() == typeof(PastFileListB))
                {
                    var B = (PastFileListB)F;
                    for (int j = 0; j < B.Files.Count; j++)
                    {
                        var BF = B.Files[j];
                        File.WriteAllBytes(Path.Combine(End_Dir,BF.FileName), BF.GetEndFile());
                    }

                }
                else if (F.GetType() == typeof(PastFileListA))
                {
                    var A = (PastFileListA)F;
                    File.WriteAllBytes(Path.Combine(End_Dir, A.File.FileName), A.File.GetEndFile());

                }

            }


        }



        public static void Pack(string path)
        {


            var pu = path.IndexOf($".{Header}");
            if (pu == -1) return;

            var E = System.IO.Path.GetFileName(path); 
            var FFILe = FFile<MemoryStream>.CreateFileAsMemoryStream(E);
            FFILe.WriteString(Header);


       

            var XML_S = new System.Xml.Serialization.XmlSerializer(typeof(List<PastFileBlockEx>));
            var FS = File.Open(Path.Combine(path, Header + ".xml"),FileMode.Open);
            var x = (List<PastFileBlockEx>)XML_S.Deserialize(FS);

            FFILe.WriteInt32BE(x.Count);

            var StartFileOffsets = FFILe.GetStreamPosition();

            var listining = new List<int>();

            for (int i = 0; i < x.Count; i++)
            {
                FFILe.WriteUInt32BE(0);
            }



            for (int i = 0; i < x.Count; i++)
            {
                var F = x[i];
                if (F.GetType() == typeof(PastFileListB))
                {
                    var pos = FFILe.GetStreamPosition();
                    listining.Add((int)FFILe.GetStreamPosition());
                    var B = (PastFileListB)F;
                    FFILe.WriteUInt16BE((byte)B.Files.Count);
                    FFILe.WriteUInt16BE((byte)System.Convert.ToInt16(B.HasTags));

                    //Calculate Full SizeBefore
                    int size = 4;
                    size += B.Files.Count * 8; //OffsetsTables
                    for (int j = 0; j < B.Files.Count; j++)
                    {
                        var BF = B.Files[j];
                        var N = Path.GetFileNameWithoutExtension(BF.FileName);
                        size += N.Length+1;
                    }
                    if (B.HasTags)
                    {
                        size += B.Files.Count;
                    }
            
                    var pad = Padding.FixPaddingFixedX(size, 0x10)-size;
                    //FFILe.WriteInt32BE(size+pad);
                    var AfterPos = size  + pad;
                    for (int j = 0; j < B.Files.Count; j++)
                    {
                        var BF = B.Files[j];
                        FFILe.WriteUInt32BE((uint)AfterPos);
                        AfterPos += (int)FFile<MemoryStream>.GetFilzeSie(Path.Combine(path, BF.GetFileName()));
                    }
                    for (int j = 0; j < B.Files.Count; j++)
                    {
                        var BF = B.Files[j];
                        FFILe.WriteUInt32BE((uint)FFile<MemoryStream>.GetFilzeSie(Path.Combine(path, BF.GetFileName())));
                    }
                    for (int j = 0; j < B.Files.Count; j++)
                    {
                        FFILe.WriteString(Path.GetFileNameWithoutExtension(B.Files[j].FileName));
                        FFILe.WriteByte(0);
                    
                    }

                    FFILe.WritePadding(pad);
                    for (int j = 0; j < B.Files.Count; j++)
                    {
                        FFILe.WriteBytes(FFile<MemoryStream>.GetMemoryStreamFromFile(Path.Combine(path, B.Files[j].FileName)).GetEndFile());

                    }

                }
                //NEIF ONLY
                else if (F.GetType() == typeof(PastFileListA))
                {
                    listining.Add((int)FFILe.GetStreamPosition());
                    var A = (PastFileListA)F;
                    FFILe.WriteBytes(FFile<MemoryStream>.GetMemoryStreamFromFile(Path.Combine(path, A.File.FileName)).GetEndFile());


                }
                else
                {
                    Console.WriteLine("game may CRASH LOL");
                }
             



            }


            FFILe.ChangeAbsolutePosition(8);
            for (int i = 0; i < x.Count; i++)
            {
                FFILe.WriteUInt32BE((uint)listining[i]);

            }

            File.WriteAllBytes(path.Replace($".{Header}",""), FFILe.GetEndFile());
          
        }



       
        public static bool IsCommonMiniPackFile(byte[] bt)
        {
            bool result = false;

            result = bt[0] == 0 & bt[1] > 0 & bt[2] == 0 && bt[1] < 26;

            return result;
        }
        public static bool IsENO(byte[] bt)
        {
            if (System.Text.Encoding.ASCII.GetString(bt) == "NEIF")
            {
                return true;
            }
            return false;
        }


      

    }
}
