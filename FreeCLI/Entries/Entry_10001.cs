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



    [EntryType(10001)]
    public class Entry_10001 : Entry
    {
        public Entry_10001()
        {
            OnBaseInit();
        }
        public Entry_10001(uint EntryType,uint index)
        {
            OnInit(EntryType, index, 3);
        }



        public override RawFile OnFileDataUnpackProcess(FFile file, uint index, int yindex)
        {
   
        
            if (yindex == 0)
            {
               
            }
            else if (yindex == 1)
            {
                return new TexturePack(file, $"TexturePack_{index}").Unpack();
            }
         
            else
            {
                
            }
         
            return new RawFile(file, $"File_{GetEntryName()}_{index}").Unpack();
        }


        public override string GetEntryName()
        {
            return $"FontDTST_{Index}";
        }



    }
}
