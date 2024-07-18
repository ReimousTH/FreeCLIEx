using FreeCLI.FType;
using MabTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FreeCLI.Entries
{


    public class Group
    {
        public Group()
        {

        }
        public string path { get; set; }

        public List<RawFile> Members { get; set; } = new List<RawFile>();
    }

    [EntryType(0xFFFFFFFF)]
    [JsonDerivedType(typeof(Entry), typeDiscriminator: "Entry")]
    [JsonDerivedType(typeof(Entry_10001), typeDiscriminator: "Entry_10001")]
    [JsonDerivedType(typeof(Entry_10002), typeDiscriminator: "Entry_10002")]
    [JsonDerivedType(typeof(Entry_32000), typeDiscriminator: "Entry_32000")]
    [JsonDerivedType(typeof(Entry_53000), typeDiscriminator: "Entry_53000")]
    [JsonDerivedType(typeof(Entry_53001), typeDiscriminator: "Entry_53001")]
    [JsonDerivedType(typeof(Entry_53050), typeDiscriminator: "Entry_53050")]
    [JsonDerivedType(typeof(Entry_53051), typeDiscriminator: "Entry_53051")]
    [JsonDerivedType(typeof(Entry_53101), typeDiscriminator: "Entry_53101")]
    [JsonDerivedType(typeof(Entry_53200), typeDiscriminator: "Entry_53200")]
    [JsonDerivedType(typeof(Entry_53201), typeDiscriminator: "Entry_53201")]
    [JsonDerivedType(typeof(Entry_55001), typeDiscriminator: "Entry_55001")]
    [JsonDerivedType(typeof(Entry_55002), typeDiscriminator: "Entry_55002")]
    public class Entry
    {
        public Entry()
        {
            OnBaseInit();
        }
        public Entry(uint EntryType, uint index)
        {
            OnInit(EntryType, index, -1);
        }
        public void OnBaseInit()
        {

            Index = 0;
            GroupSize = -1;
            Groups = new List<Group>();
        }
        public virtual void OnInit(uint EntryType, uint index, int group_size)
        {
            OnBaseInit();
            Type = EntryType;
            Index = index;
            GroupSize = group_size;
        }


        public virtual string GetEntryName()
        {
            return $"Entry_{GetEntryType()}_{Index}";
        }
        public virtual uint GetEntryType()
        {
            EntryTypeAttribute attr = GetType().GetCustomAttribute<EntryTypeAttribute>();
            if (attr != null && attr.Type != 0xFFFFFFFF) return attr.Type;
            return this.Type;
        }



        public List<Group> Groups { get; set; }




        public string Name { get; set; }

        public uint Index { get; set; }
        public uint Type { get; set; }

        public int GroupSize { get; set; }




        //For pack only
        public FFile RawEntry;


        public virtual RawFile OnFileDataUnpackProcess(FFile file, uint index, int yindex)
        {
            return new RawFile(file, $"File_{GetEntryName()}_{index}").Unpack();
        }

        public void Unpack(FFile file, uint count, uint offset)
        {
            this.Name = GetEntryName();
            if (this.GroupSize != -1)
            {
                Groups.Clear();
                Groups.AddRange(Enumerable.Range(0, (int)(count /this.GroupSize)).Select(zx => new Group() { path = $"Group_{zx}" }));
            }
            else
            {
                Groups.Add(new Group() { path = $"Group_{0}" });
            }


            for (uint i = 0; i < count; i++)
            {
                uint soffset = ((uint)((uint)offset + (i * 4)));
                var EntryFileOffset = file.ReadTypeBEAt<uint>(soffset);
                var NextEntryFileOffset = file.ReadTypeBEAt<uint>(soffset + 4);
                FFile FileData;
                if (EntryFileOffset == 0)
                {
                    FileData = new FFile(new byte[] { });
                }
                else
                {
                    if (NextEntryFileOffset == 0)
                    {
                        for (uint k = 1; ; k++)
                        {
                            NextEntryFileOffset = file.ReadTypeBEAt<uint>(soffset + (k * 4));
                            if (NextEntryFileOffset != 0) break;
                        }
                    }
                    var EntryFileSize = NextEntryFileOffset - EntryFileOffset;
                    FileData = new FFile(file.ReadBytesAt(EntryFileOffset, EntryFileSize));

                }


                int gindex = (int)(i / GroupSize);
                if (this.GroupSize != -1)
                {
                    Groups[gindex].Members.Add(OnFileDataUnpackProcess(FileData, i, (int)(i % GroupSize)));
                }
                else
                {
                    Groups[0].Members.Add(OnFileDataUnpackProcess(FileData, i, (int)(i % GroupSize)));
                }


            }
        }


        //if file is packed have sub_files:)
        public virtual FFile OnFileDataPackProcess(FFile file)
        {


            return file;
        }

        //returns Packed Entry
        public void Pack(FFile file, uint EntryStartOffset, ref uint DataOffset)
        {

            DataOffset = Padding.FixPaddingFixedX(DataOffset, 0x10);
            file.Jump(DataOffset);

            for (int i = 0; i < Groups.Count; i++)
            {
                var group = Groups[i];
                if (Groups.Count > 1)
                {

                }

                for (int j = 0; j < group.Members.Count; j++)
                {
                

                    var member = group.Members[j];
                    if (bool.Parse(member.Attributes[RawFile.IsUnpackedAttribute]))
                    {
                        member.Pack();
                    }
                    var findex = j;
                    if (GroupSize != -1)
                    {
                         findex= (i * this.GroupSize) + j;
                    }
                
                    if (member.Raw._localstream.Length != 0) file.WriteTypeBEAt<uint>(EntryStartOffset + ((findex) * 4), DataOffset);
                    file.WriteBytes(member.Raw.GetArray());
                    DataOffset += (uint)member.Raw._localstream.Length; 

                }
            }

        }
    }
}
