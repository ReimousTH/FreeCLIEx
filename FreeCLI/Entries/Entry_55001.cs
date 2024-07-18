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



    [EntryType(55001)]
    public class Entry_55001 : Entry
    {
        public Entry_55001()
        {
            OnBaseInit();
        }
        public Entry_55001(uint EntryType,uint index)
        {
            OnInit(EntryType, index, -1);
        }



        public override RawFile OnFileDataUnpackProcess(FFile file, uint index, int yindex)
        {
       
                return new TexturePack(file, $"TexturePack_{index}").Unpack();
        
        }


        public override string GetEntryName()
        {
            return $"TexturePack_{Index}";
        }



    }
}
