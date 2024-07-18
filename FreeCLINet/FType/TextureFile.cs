using FreeCLI.Entries;
using MabTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCLI.FType
{
    public class TextureFile : RawFile
    {
        
        public TextureFile()
        {

        }
        public TextureFile(FFile f) {

        }
        public TextureFile(FFile f,string name):base(f,name)
        {
            this.Attributes[RawFile.Tagattribute] = "0";
        }

        public override string OnSaveFile(string path)
        {
            return path+".dds";
        }

        public override string OnFileOpen(string path)
        {
            return path + ".dds";

        }

    }
}
