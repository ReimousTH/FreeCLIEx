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



    [EntryType(53000)]
    public class Entry_53000:Entry
    {
        public Entry_53000()
        {
            OnBaseInit();
        }
        public Entry_53000(uint EntryType,uint index)
        {
            OnInit(EntryType, index, -1);
        }



        public override RawFile OnFileDataUnpackProcess(FFile file, uint index, int yindex)
        {
            var gindex = index % 3;
            //FBG
            if (gindex == 0)
            {

            }
            else if (index == 1)
            {
                return new TexturePack(file, $"TexturePack_{index}").Unpack();
            }
            else if (index == 2)
            {
                return new TexturePack(file, $"TexturePack_{index}").Unpack();
            }

            return new RawFile(file, $"File_{GetEntryName()}_{index}").Unpack();
        }

        public override string GetEntryName()
        {
            return $"FBGTextureX2_{Index}";
        }



    }
}
