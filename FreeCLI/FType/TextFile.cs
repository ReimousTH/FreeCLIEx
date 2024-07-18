using FreeCLI.Entries;
using MabTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCLI.FType
{
    public class TextFile : RawFile
    {
        public TextFile()
        {
    
        }
        
        public TextFile(FFile f):base(f) {

            this.Attributes[RawFile.IsUnpackedAttribute] = "true";
       
        }
        public TextFile(FFile f,string name) : base(f,name)
        {

            this.Attributes[RawFile.IsUnpackedAttribute] = "true";
    
        }

  
        public override RawFile OnUnpack()
        {
            Raw.Jump(0);
            uint file_size = Raw.ReadTypeAt<uint>(0);
            uint HEADER = Raw.ReadTypeBEAt<uint>(4);
            uint HEADER_1 = Raw.ReadTypeBEAt<uint>(8);  
            uint HEADER_2 = Raw.ReadTypeBEAt<uint>(0xC);
            uint HEADER_3 = Raw.ReadTypeBEAt<uint>(0x10);
            if (HEADER != 0x6D92DC4D) return this;



            return this;
        }
        public override RawFile OnPack()
        {
          

            return this;
        }

        public override string OnSaveFile(string path)
        {
            return path;
        }

    }
}
