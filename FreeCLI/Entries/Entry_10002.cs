﻿using FreeCLI.FType;
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



    [EntryType(10002)]
    public class Entry_10002: Entry
    {
        public Entry_10002()
        {
            OnBaseInit();
        }
        public Entry_10002(uint EntryType,uint index)
        {
            OnInit(EntryType, index, -1);
        }



        public override RawFile OnFileDataUnpackProcess(FFile file, uint index, int yindex)
        {
            var gindex = index % 2;
        
            if (gindex == 0)
            {
               
            }
            else if (index == 1)
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
