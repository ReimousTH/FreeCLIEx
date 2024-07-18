using MabTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace FreeCLI
{
    public class XboxFile : FFile
    {

     


        public XboxFile()
        {

        }







        public static FFile OpenFile(string path)
        {

            var c = FFile.OpenFile(path);

            var uncompressedSize = c.ReadTypeAt<uint>(4);
            var compressedSize = c.ReadTypeAt<uint>(8);



            try
            {
                return new FFile() { _localstream = new MemoryStream() };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new FFile();
            }


        }

    }
}
