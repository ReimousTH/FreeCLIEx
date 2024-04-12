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



    [EntryType(32000)]
    public class Entry_32000:Entry
    {
        public Entry_32000()
        {
            OnBaseInit();

        }
        public Entry_32000(uint EntryType,uint index)
        {
            OnInit(EntryType, index, -1);
        }


        public override RawFile OnFileDataUnpackProcess(FFile file, uint index,int yindex)
        {
            var gindex = index % 2;
            if (gindex == 0)
            {
              
            }
            else 
            {
                return new TexturePack(file, $"TexturePack_{index}").Unpack();
            }

            return new RawFile(file,$"File_{GetEntryName()}_{index}").Unpack();
        }

   
  
    }
}
