using FreeCLI.Entries;
using MabTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCLI.FType
{
    public class NOFile : RawFile
    {
        
        public NOFile()
        {

        }
        public NOFile(FFile f) {

        }
        public NOFile(FFile f,string name):base(f,name)
        {
          
        }

        public override RawFile OnUnpack()
        {
            Raw.Jump(0);
            uint HEADER =  Raw.ReadTypeBEAt<uint>(0);
            if (HEADER != 0x4E454946) return this;
            uint HEADER_SIZE = Raw.ReadTypeAt<uint>(4);
            uint HEADER_COUNT = Raw.ReadTypeAt<uint>(8);
            uint NEIF_DATA = Raw.ReadTypeAt<uint>(0xC);
            uint OFFSET_1 = Raw.ReadTypeBEAt<uint>(0x10);
            uint NOFO_OFFSET = Raw.ReadTypeBEAt<uint>(0x14);

            if (Raw.ReadTypeBEAt<uint>(NOFO_OFFSET) != 0x4E4F4630) return this;
            uint NOFO_SIZE = Raw.ReadTypeAt<uint>(NOFO_OFFSET + 4);

            uint NFN0_OFFSET = NOFO_OFFSET + NOFO_SIZE + 8;
            if (Raw.ReadTypeBEAt<uint>(NFN0_OFFSET) != 0x4E464E30) return this;
            uint NFN0_SIZE = Raw.ReadTypeAt<uint>(NFN0_OFFSET + 4);
         
            string name =  Raw.ReadArrayStringAt(NFN0_OFFSET + 0xC, (int)(NFN0_SIZE - 4));
            name= name.Replace("\0", "");


            this.Attributes[PathAttribute] = $"{this.Attributes[PathAttribute].Split('_')[1]}_{name}";

            Raw.Jump(0);




            return this;
        }

        public override RawFile OnPack()
        {
            return this;
        }



    }
}
