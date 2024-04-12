using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FreeCLI
{

    public class PackFileExHeader: PFile
    {
        public List<byte> SavedHeader = new List<byte>();
        public List<PastFileBlockEx> Files { get => _Files; set => _Files = value; }
        private List<PastFileBlockEx> _Files = new List<PastFileBlockEx>();

    }
    public class PackFile: PFile
    {
        public bool Status = false;
        public const string Header = "pack";

        public string PackFileName = "";

        public PackFileExHeader Data = new PackFileExHeader();

        public PackFile()
        {

        }

        public List<PastFileBlockEx> Files { get => _Files; set => _Files = value; }
        private List<PastFileBlockEx> _Files = new List<PastFileBlockEx>();

        private List<uint> _Types  = new List<uint>();
        public static PackFile Read<T>(FFile_OLD<T> st)
        {
            var PackFile = new PackFile();
            PackFile.PackFileName = st.GetFileName();
            if (st.ReadFixedString(4) != Header)
            {
                Console.WriteLine($"{st.FileName} is not valid pack file");
                return PackFile;
            }

            var S1 = st.GetStreamPosition();
            //   var BlockSize = st.ReadInt16BE();
            //  var U1 = st.ReadInt16(); //always 3
            // st.MovePosition(BlockSize * 4);



            //  var Fuxed = Padding.FixPaddingFixedEX(BlockSize, 2);
            // st.MovePosition(Fuxed*2);

            var Unk01 = st.ReadUInt16BE();
            var Unk02 = st.ReadUInt16(); //always 3 , but i think it means empty offsets means min is 3\
            //8249C334

            Unk01 = (ushort)(Unk01 << 1);

            var Start = Unk01 + 8;
            var StartL = (Start + 3) & 0xFFFFFFFC;


            var S2 = st.GetStreamPosition();

            st.ChangeAbsolutePosition(S1);
            PackFile.Data.SavedHeader =  st.ReadBytes(S2 - S1).ToList();


            var offset_table = new List<uint>();
            while (true)
            {
                var s = st.ReadUInt32BE();              
                offset_table.Add(s);
                if (s == st.GetLength()) break;

            }
            for (var i = 0; i < offset_table.Count; i++)
            {
                int length = 0;


                if (offset_table[i] == st.GetLength())
                {
                    var a = new PastFileEndMark();
                    PackFile.Data.Files.Add(a);
                    break; //TODO
                }

                if (offset_table[i] == 0)
                {
                    var a = new PastFileListA();
                    a.File = FFile_OLD<MemoryStream>.GetFromMemoryStream($"UFile{i}", new byte[] {});
                    PackFile.Data.Files.Add(a);
                }
                else
                {
                    uint s1 = 0;
                    if (i+1 != offset_table.Count)
                    {
                        s1 = Enumerable.Range(i + 1, offset_table.Count - i - 1).Select(zx => offset_table[zx]).First(zx => zx != 0);
                    }
                    else
                    {
                        s1 = (uint)st.GetLength();
                    }
                    
                    PastFile.ReadPackedFile(st, PackFile.Data, offset_table[i], s1 - offset_table[i],0);
                }
     
            }

            //   Console.WriteLine($"{st.FileName}, offset : {st.GetStreamPosition().ToString("x8")}");




            PackFile.Status = true;
            return PackFile;


        }



        public void Unpack(string path = null)
        {

            var FileNameT = Path.GetFileNameWithoutExtension(PackFileName);
            var FileDir = Path.GetDirectoryName(PackFileName);
            var End_Dir = Path.Combine(FileDir, FileNameT + "." + Header);
            if (!Directory.Exists(End_Dir))
            {
                Directory.CreateDirectory(End_Dir);
            }
            var XML_S = new System.Xml.Serialization.XmlSerializer(typeof(PackFileExHeader));
            var FS = File.Create(Path.Combine(End_Dir, Header + ".xml"));
            XML_S.Serialize(FS, Data);
            FS.Close();

            for (int i = 0; i < Data.Files.Count; i++)
            {
                var F = Data.Files[i];
                if (F.GetType() == typeof(PastFileListB))
                {
                    var B = (PastFileListB)F;
                    for (int j = 0; j < B.Files.Count; j++)
                    {
                        var BF = B.Files[j];

                        var ENAME = BF.FileName;
                        if (ENAME.Contains(@"..\"))
                        {
                            ENAME=ENAME.Replace(@"..\", "");
                        }

                        var tu = ENAME.Split(Path.DirectorySeparatorChar);
                   
                        if (tu.Count() > 1)
                        {

                            var S = Path.Combine(End_Dir, String.Join("\\", Enumerable.Range(0, tu.Count() - 1).Select(zx => tu[zx])));
                            
                            if (!Directory.Exists(S))
                                Directory.CreateDirectory(S);   
                            }
                      

                        File.WriteAllBytes(Path.Combine(End_Dir, ENAME), BF.GetEndFile());
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

            var XML_FILE_NAME = Path.Combine(path, "pack.xml");
            var Serializer = new System.Xml.Serialization.XmlSerializer(typeof(PackFileExHeader));
            PackFileExHeader Header_Data = new PackFileExHeader();

            using (var S = File.OpenRead(XML_FILE_NAME))
            {
                Header_Data = (PackFileExHeader)Serializer.Deserialize(S);
            }


            var pu = path.IndexOf($".{Header}");
            if (pu == -1) return;

            var E = System.IO.Path.GetFileName(path);
            var FFILe = FFile_OLD<MemoryStream>.CreateFileAsMemoryStream(E);
            FFILe.WriteString(Header);

            FFILe.WriteBytes(Header_Data.SavedHeader.ToArray());


            var StartFileOffsets = FFILe.GetStreamPosition();

            var listining = new List<int>();

            for (int i = 0; i < Header_Data.Files.Count; i++)
            {
                FFILe.WriteUInt32BE(0); //Void Data
            }


            for (int i = 0; i < Header_Data.Files.Count; i++)
            {
                var F = Header_Data.Files[i];
                PastFile.ProcessSingleFileToPack(FFILe, F,ref listining, path);
            }


            FFILe.ChangeAbsolutePosition(StartFileOffsets);
            for (int i = 0; i < Header_Data.Files.Count; i++)
            {
                FFILe.WriteUInt32BE((uint)listining[i]);

            }

            File.WriteAllBytes(path.Replace($".{Header}", ""), FFILe.GetEndFile());

        }


    }
}
