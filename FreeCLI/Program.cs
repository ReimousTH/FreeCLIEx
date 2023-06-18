using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCLI
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            //PackFile.Read<MemoryStream>(FFile<MemoryStream>.GetMemoryStreamFromFile("D:\\Games\\X360\\SonicFreeRiders\\SonicFreeRiders\\Game\\X\\Raw\\advE")).Unpack();
            //return;



         //   args = new string[] { "D:\\Games\\X360\\SonicFreeRiders\\SonicFreeRiders\\Game\\X\\pS_ST.paST" };

            for (int i = 0; i < args.Length; i++)
            {
                if (Directory.Exists(args[i]))
                {
                    var HU = args[i].Substring(args[i].LastIndexOf(".")+1);
                    switch (HU)
                    {
                        case PackFile.Header:
                            break;
                        case PastFile.Header:
                            PastFile.Pack(args[i]);
                            break;
                    }                    
                }
                else
                {

                    var H = FFile<FileStream>.OpenFileAndGetHeader(args[i], 4);
                    switch (H)
                    {
                        case PackFile.Header:
                            PackFile.Read<MemoryStream>(FFile<MemoryStream>.GetMemoryStreamFromFile(args[i])).Unpack();
                            break;
                        case PastFile.Header:
                            PastFile.Read<MemoryStream>(FFile<MemoryStream>.GetMemoryStreamFromFile(args[i])).Unpack();
                            break;
                    }
                }
            }
   


         //   if (args.Length == 0)
      //      PastFile.Read<MemoryStream>(FFile<MemoryStream>.GetMemoryStreamFromFile("D:\\Games\\X360\\SonicFreeRiders\\SonicFreeRiders\\Game\\X\\Raw\\pS_ST")).Unpack();
        
     
        }
    }
}
