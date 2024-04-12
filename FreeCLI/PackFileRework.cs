
using FreeCLI.Entries;
using FreeCLI.FType;
using MabTool;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using System.Xml;

namespace FreeCLI
{

    public class PackFileJSON
    {
        public static PackFileRework Unpack(string path)
        {
            var jsonPath = Path.Combine(path, "save.json");

            // Read the JSON data from the file
            string jsonData = File.ReadAllText(jsonPath);

            // Deserialize the JSON data to PackFileRework object
            return JsonSerializer.Deserialize<PackFileRework>(jsonData);
        }

        public static void Pack(string path, PackFileRework data)
        {
            var jsonPath = Path.Combine(path, "save.json");

            // Serialize the PackFileRework object to JSON
            string jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Write the JSON data to the file
            File.WriteAllText(jsonPath, jsonData);
        }
    }
    public class PackFileRework
    {


        public List<Entries.Entry> entries { get; set; } = new List<Entries.Entry>();
        public const uint _HEADER_ = 0x7061636B;

        public FFile _file;
        public bool _Result;


        public PackFileRework()
        {

        }
        public PackFileRework(FFile file)
        {
            _file = file;

        }


        public bool Pack()
        {
            _file = new FFile();
            _file.WriteTypeBEAt<uint>(0,_HEADER_);
            _file.WriteTypeBEAt<ushort>(4, (ushort)entries.Count);


            var EntryCount = entries.Count;
            var FEntryCount = (ushort)(EntryCount << 1);

            var DataOffset0 = (8);
            var DataOffset1 = (FEntryCount + 0 + 8 + 3) & 0xFFFFFFFC;
            var DataOffset2 = (FEntryCount + DataOffset1);
            var DataOffset3 = (DataOffset2 + FEntryCount);

            uint index = 0;

            uint DATA_OFFSET = (uint)(DataOffset3 + entries.Select(zx => zx.Groups.Select(zx => zx.Members.Count).Sum()).Sum() * 4) + 4;
            uint LAST_OFFSET = DATA_OFFSET - 4;

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                var entries_elements_count = entry.Groups.Select(zx => zx.Members.Count).Sum();

                _file.Jump(DataOffset0 + (i * 2));
                _file.WriteTypeBE<ushort>((ushort)entries_elements_count);

                _file.Jump(DataOffset1 + (i * 2));
                _file.WriteTypeBE<ushort>((ushort)index);

                //type
                _file.Jump(DataOffset2 + (i * 2));
                _file.WriteTypeBE<ushort>((ushort)entry.GetEntryType());
                uint SubEntryStartFileOffset = (uint)((index * 4) + DataOffset3);

                entry.Pack(_file,SubEntryStartFileOffset, ref DATA_OFFSET);


                index += (uint)entries_elements_count;

            }
            _file.WriteTypeBEAt<uint>(LAST_OFFSET,(uint)_file._localstream.Length);





            return true;
        }

        public bool Unpack()
        {
            #region CheckBasic
            var HEADER = _file.ReadTypeBEAt<uint>(0);
            if (_HEADER_ != HEADER)
            {
                _Result = false;
                return _Result;
            }
            #endregion

            #region DataProcess



            var EntryCount = _file.ReadTypeBEAt<ushort>(4); // last offset not included here, and most files
            var FEntryCount = (ushort)(EntryCount << 1);

            var DataOffset0 = (8);
            var DataOffset1 = (FEntryCount + 0 + 8 + 3) & 0xFFFFFFFC;
            var DataOffset2 = (FEntryCount + DataOffset1);
            var DataOffset3 = (DataOffset2 + FEntryCount);


            Console.WriteLine($"D0 {DataOffset0} D1 {DataOffset1} D2 {DataOffset2} D3 {DataOffset3}");



            for (int i = 0; i < EntryCount; i++)
            {
                var SubEntryType = _file.ReadTypeBEAt<ushort>((uint)((2 * i) + DataOffset2));
                var SubEntryCount = _file.ReadTypeBEAt<ushort>((uint)((2 * i) + DataOffset0));
                var SubEntryStartIndex = _file.ReadTypeBEAt<ushort>((uint)(2 * i + DataOffset1));
                var SubEntryStartFileOffset = (SubEntryStartIndex * 4) + DataOffset3;

                Entry entry;


                entry = new Entry(SubEntryType, (uint)i);
                var attrs = typeof(Entry).GetCustomAttributes<JsonDerivedTypeAttribute>();
                foreach (var attr in attrs)
                {

                    if (attr.DerivedType.GetCustomAttribute<EntryTypeAttribute>().Type == SubEntryType)
                    {
                        entry = (Entry)attr.DerivedType.GetConstructor(new Type[] { typeof(uint), typeof(uint) }).Invoke(new object[] { SubEntryType, (uint)i });
                        break;
                    }
                }



                entry.Unpack(_file, SubEntryCount, (uint)SubEntryStartFileOffset, null);

                entries.Add(entry);

            }






            #endregion



            #region FinishBasic
            _Result = true;
            return _Result;
            #endregion

        }


        public static PackFileRework PackFile(string path)
        {

            var json = PackFileJSON.Unpack(path);

            //Help Process
            for (int i = 0; i < json.entries.Count; i++)
            {
                var entry = json.entries[i];
                var entry_path = Path.Combine(path, entry.Name);

                for (int j = 0; j < entry.Groups.Count; j++)
                {
                    var group = entry.Groups[j];
                    var group_path = Path.Combine(entry_path, group.path);
                    for (int k = 0; k < group.Members.Count; k++)
                    {
                        var member = group.Members[k];
                        var member_path = Path.Combine(group_path, member.Attributes[RawFile.PathAttribute]);
                        if (member.IsUnpackable)
                        {
                            for (int z = 0; z < member.RawFiles.Count; z++)
                            {
                                var submember = member.RawFiles[z];
                                var submember_path = Path.Combine(member_path, submember.Attributes[RawFile.PathAttribute]);
                                submember.Raw = FFile.OpenFile(submember_path);

                            }

                        }
                        else
                        {
                            member.Raw = FFile.OpenFile(member_path);
                        }

                    }

                }
            }

       
            json.Pack();
            path += ".packed";
            json._file.SaveFileF(path);



            return null;
        }

        public static PackFileRework UnpackFile(string path)
        {
            var F = new PackFileRework(FFile.OpenFile(path));
            F.Unpack();


            path += ".pack";
            Directory.CreateDirectory(path);

            PackFileJSON.Pack(path, F);


            for (int i = 0; i < F.entries.Count; i++)
            {
                var entry = F.entries[i];
                entry.Index = (uint)i;

                string Entry_Path = Path.Combine(path, $"{entry.GetEntryName()}");
                Directory.CreateDirectory(Entry_Path);

                for (int j = 0; j < entry.Groups.Count; j++)
                {
                    var group = entry.Groups[j];
                    string Group_path = Path.Combine(Entry_Path, group.path);
                    Directory.CreateDirectory(Group_path);
                    for (int k = 0; k < group.Members.Count; k++)
                    {
                        var member = group.Members[k];
                        if (member.IsUnpackable)
                        {
                            var member_path = member.Attributes[RawFile.PathAttribute];
                            var member_path_end = Path.Combine(Group_path, member_path);
                            Directory.CreateDirectory(member_path_end);
                            for (int z = 0; z < member.RawFiles.Count; z++)
                            {

                                var rawFile = member.RawFiles[z];
                                var raw_path = rawFile.Attributes[RawFile.PathAttribute];
                                var raw_path_end = Path.Combine(member_path_end, raw_path);
                                rawFile.SaveFile(raw_path_end);

                            }

                        }
                        else
                        {
                            var member_path = member.Attributes[RawFile.PathAttribute];
                            var member_path_end = Path.Combine(Group_path, member_path);
                            member.SaveFile(member_path_end);
                        }



                    }

                }
            }




            return null;
        }







    }
}
