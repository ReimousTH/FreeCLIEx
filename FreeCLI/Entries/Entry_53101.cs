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



    [EntryType(53101)]
    public class Entry_53101 : Entry
    {
        public Entry_53101()
        {
            OnBaseInit();
        }
        public Entry_53101(uint EntryType,uint index)
        {
            OnInit(EntryType, index, 3);
        }



        public override RawFile OnFileDataUnpackProcess(FFile file, uint index, int yindex)
        {
        
            //.eno
            if (yindex == 0)
            {
                return new NOFile(file, $"NOFILE_{index}").Unpack();
            }
            else if (yindex == 1)
            {
                return new TexturePack(file, $"TexturePack_{index}").Unpack();
            }
            //.env
            else if (yindex == 2)
            {
                return new NOFile(file, $"NOFILE_{index}").Unpack();
            }
         
            return new RawFile(file, $"File_{GetEntryName()}_{index}").Unpack();
        }

        public override string GetEntryName()
        {
            return $"GModelTextureMotion_{Index}";
        }




    }
}
