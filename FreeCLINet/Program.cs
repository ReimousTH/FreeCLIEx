﻿using FreeCLI.Entries;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCLI
{
    internal class Program
    {
     


        static void Main(string[] args)
        {
            EntryTypeDataSerialize.Deserialize();
            // EntryTypeDataSerialize.Serialize();

            

           // args = new string[] { "D:\\Games\\X360\\SonicFreeRiders\\SonicFreeRiders\\Game\\X360\\mods\\AntiDLL\\pS_ST" };

            for (int i = 0; i < args.Length; i++)
            {
                if (Directory.Exists(args[i]))
                {
                    var HU = args[i].Substring(args[i].LastIndexOf(".")+1);
                    switch (HU)
                    {
                        case PackFile.Header:
                            PackFileRework.PackFile(args[i]);
                            break;
                        case PastFile.Header:
                            PastFile.Pack(args[i]);
                            break;
                    }                    
                }
                else
                {

                   // try
                    {
                 
                    //    var Y = XboxFile.OpenFile(args[i]); ;
                        var H = FFile_OLD<FileStream>.OpenFileAndGetHeader(args[i], 4);
                        switch (H)
                        {
                            case PackFile.Header:
                                PackFileRework.UnpackFile(args[i]);
                                break;
                            case PastFile.Header:
                                PastFile.Read<MemoryStream>(FFile_OLD<MemoryStream>.GetMemoryStreamFromFile(args[i])).Unpack();
                                break;
                        }
                    }
                   // catch
                  //  {
                  //      Console.WriteLine($"Error on ${args[i]}");
                  //  }
                
                }
            }
   


         //   if (args.Length == 0)
      //      PastFile.Read<MemoryStream>(FFile<MemoryStream>.GetMemoryStreamFromFile("D:\\Games\\X360\\SonicFreeRiders\\SonicFreeRiders\\Game\\X\\Raw\\pS_ST")).Unpack();
        
     
        }
    }
}
