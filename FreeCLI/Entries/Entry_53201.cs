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



    [EntryType(53201)]
    public class Entry_53201:Entry
    {
        public Entry_53201()
        {
            OnBaseInit();
        }
        public Entry_53201(uint EntryType,uint index)
        {
            OnInit(EntryType, index, -1);
        }



        public override RawFile OnFileDataUnpackProcess(FFile file, uint index, int yindex)
        {
            var gindex = index % 2;
            //.eno
            if (gindex == 0)
            {
                return new NOFile(file, $"NOFILE_{index}").Unpack();
            }
            else if (index == 1)
            {
                return new TexturePack(file, $"TexturePack_{index}").Unpack();
            }
            //Motions
            else
            {
                return new NOFile(file, $"NOFILE_{index}").Unpack();
            }
         
            return new RawFile(file, $"File_{GetEntryName()}_{index}").Unpack();
        }


        public override string GetEntryName()
        {
            return $"ModelTextureX_{Index}";
        }



    }
}
