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


 

        public override RawFile OnFileDataUnpackProcess(FFile file, uint index,int yindex)
        {
            return new RawFile(file,$"File_{GetEntryName()}_{index}").Unpack();
        }

   
  
    }
}
