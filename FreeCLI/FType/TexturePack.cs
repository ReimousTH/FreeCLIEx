using FreeCLI.Entries;
using MabTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCLI.FType
{
    public class TexturePack : RawFile
    {
        
        public TexturePack(FFile f):base(f) {
            this.Attributes[RawFile.PathAttribute] = "TexturePack";
            this.IsUnpackable = true;
        }
        public TexturePack(FFile f,string name) : base(f,name)
        {
            this.IsUnpackable=true;
        }

        public override RawFile OnUnpack()
        {
            Raw.Jump(0);
            ushort file_count = Raw.ReadTypeBEAt<ushort>(0x0);
            ushort file_tags = Raw.ReadTypeBEAt<ushort>(2);

            var offsets = Raw.ReadArrayBEAt<uint>(0x4, file_count);
            var sizes = Raw.ReadArrayBEAt<uint>((uint)(0x4 + (file_count * 4)), file_count);
            List<byte> tags = new List<byte>();
            if (file_tags == 1)
            {
                tags.AddRange(Raw.ReadArrayBEAt<byte>((uint)(0x4 + (file_count * 8)), file_count));
                Attributes[RawFile.Tagattribute] = "0";

            }


            List<string> names = new List<string>();
            Raw.Jump(0x4 + (file_count * 8) + tags.Count);
            for (int i = 0; i < file_count; i++)
            {
                names.Add(Raw.ReadString(System.IO.SeekOrigin.Current));
            }

            for (int i = 0; i < file_count; i++)
            {
                var offset = offsets[i];
                var size = sizes[i];
                var name = names[i];
                byte tag = 0;
                if (file_tags == 1) tag = tags[i];
                var sub_file_data = Raw.ReadBytesAt(offset, size);
                var sub_file = new TextureFile(new FFile(sub_file_data),name);
                sub_file.Attributes[RawFile.Tagattribute] = tag.ToString();


                RawFiles.Add(sub_file);

            }


            return this;
        }
        public override RawFile OnPack()
        {
            this.Raw = new FFile();


            ushort tags = (ushort)ushort.Parse(Attributes[Tagattribute]);
            Raw.WriteTypeBEAt<ushort>(0, (ushort)RawFiles.Count);
            Raw.WriteTypeBEAt<ushort>(2, (ushort)ushort.Parse(Attributes[Tagattribute]));

            uint Offsets_Offset = 4;
            uint Sizes_Offsets = (uint)(Offsets_Offset + (4 * (int)RawFiles.Count));
            uint Tags_Offsets = (uint)(Sizes_Offsets + (RawFiles.Count * 4));
            uint Strings_Offets = (uint)(Sizes_Offsets + (4 * (int)RawFiles.Count));    



            var data_offset =  (Sizes_Offsets + (4*RawFiles.Count) ) +  RawFiles.Select(zx => zx.Attributes[PathAttribute].Length + 1).Sum();
            if (tags == 1)
            {
                data_offset += RawFiles.Count;
                Strings_Offets += (uint)RawFiles.Count;
            }


            for (int i = 0; i < RawFiles.Count; i++)
            {
                var raw_file = RawFiles[i];
                var raw_file_name = raw_file.Attributes[PathAttribute];

                if (tags == 1)
                {
                    var raw_file_tag = ushort.Parse(raw_file.Attributes[Tagattribute]);
                    Raw.WriteTypeBEAt<ushort>(Tags_Offsets + i, raw_file_tag);
                    
                }


                Raw.Jump(Strings_Offets); Raw.WriteString(raw_file_name);
                Strings_Offets += (uint)raw_file_name.Length + 1;

                Raw.WriteTypeBEAt<uint>(Offsets_Offset + (i*4), (uint)data_offset);
                Raw.WriteTypeBEAt<uint>(Sizes_Offsets + (i*4), (uint)raw_file.Raw._localstream.Length);


                Raw.WriteBytesAt((uint)data_offset,raw_file.Raw.GetStreamRawData());
                data_offset += raw_file.Raw._localstream.Length;


            }
         

            return this;
        }

        public override string OnSaveFile(string path)
        {
            return path;
        }

    }
}
