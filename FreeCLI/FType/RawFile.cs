using FreeCLI.Entries;
using MabTool;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FreeCLI.FType
{



    [JsonDerivedType(typeof(RawFile), typeDiscriminator: "RawFile")]
    [JsonDerivedType(typeof(TexturePack), typeDiscriminator: "TexturePack")]
    [JsonDerivedType(typeof(TextureFile), typeDiscriminator: "TextureFile")]
    public class RawFile
    {

        [JsonIgnore]
        public static string PathAttribute = "path";

        [JsonIgnore]
        public static string Tagattribute = "tag";

        [JsonIgnore]
        public static string IsUnpackedAttribute = "IsUnpacked";


        [JsonIgnore]
        public bool IsUnpackable;


   
        public List<RawFile> RawFiles { get; set; } = new List<RawFile>();


        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();





        [JsonIgnore]
        public FFile Raw;

        public RawFile()
        {
            IsUnpackable = false;
            Attributes = new Dictionary<string, string>();
        }
        public RawFile(FFile file) : this()
        {
            Raw = file;
        }
        public RawFile(FFile file, string name) : this()
        {
            Raw = file;
            Attributes[PathAttribute] = name;
            Attributes[IsUnpackedAttribute] = "false";
        }

        public RawFile Unpack()
        {
            if (IsUnpackable) { return OnUnpack(); }
            return this;
        }
        public virtual RawFile OnUnpack()
        {
            return this;
        }


        public RawFile Pack()
        {
            return this;
        }

        public virtual RawFile OnPack()
        {
            if (IsUnpackable) return OnUnpack();
            return this;
        }


        public void SaveFile(string path)
        {
      
            Raw.SaveFileF(OnSaveFile(path));

        }
        public virtual string OnSaveFile(string path)
        {
            return path;
        }






    }
}
