using FreeCLI.FType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace FreeCLI.Entries
{

    public class EntryRData
    {
        public EntryRData() { }

        public string Name { get; set; }
        public bool KeepEntryNameIndexed { get; set; } = false;

        public bool KeepEntryFilesNameIndexed { get; set; } = false;

        public uint Type { get; set; }

        public int GroupSize { get; set; } = -1;

        public List<string> Types { get; set; }
    }
    public class EntryTypeDataSerialize
    {
        public List<EntryRData> EntriesData { get; set; } = new List<EntryRData>();
        public List<Type> RegisteredTypes  = new List<Type>();

        public static EntryTypeDataSerialize Instance;


        public const string Name = "EntryData.json";

        public static EntryRData GetEntryDataByID(uint ID)
        {
            var FIND = Instance.EntriesData.Where(zx => zx.Type == ID).ToArray();
            if (FIND.Count() > 0)
            {
                return FIND[0];
            }
            else
            {
                return null;
            }

        }
        public static List<Type> GetTypeListByName(uint EntryType)
        {
            var f = GetEntryDataByID(EntryType);
            if (f == null) return new Type[] { typeof(RawFile) }.ToList();
            
            var Result = new List<Type>();
            for (int i = 0; i < f.Types.Count; i++)
            {
                var tps = f.Types[i];
                for (int j=0;j<Instance.RegisteredTypes.Count; j++)
                {
                    if (tps == Instance.RegisteredTypes[j].Name) Result.Add(Instance.RegisteredTypes[j]);
                }

            }
            return Result;


        }

        public static EntryTypeDataSerialize CreateNewData()
        {
            var d = new EntryTypeDataSerialize();
            /*
            d.EntriesData.Add(new EntryRData()
            {
                GroupSize = -1,
                Name = "Entry_53000",
                Type = 53000,
                Types = new List<string>() {
            typeof(RawFile).Name
            }
            });
            */

            return d;
        }

        public EntryTypeDataSerialize()
        {
            RegisteredTypes.AddRange(new Type[]
            {
                typeof(RawFile),
                typeof(TexturePack),
                typeof(TextFile),
                typeof(NOFile)

            });
        }
        public static void Serialize()
        {
            if (Instance == null) { Instance = CreateNewData(); }

            var jsonPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Name);
            string jsonData = JsonSerializer.Serialize(Instance, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Write the JSON data to the file
            File.WriteAllText(jsonPath, jsonData);

        }
        public static EntryTypeDataSerialize Deserialize()
        {
            var jsonPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,Name);


            if (File.Exists(jsonPath))
            {
                Instance = JsonSerializer.Deserialize<EntryTypeDataSerialize>(File.ReadAllText(jsonPath));

            }
            else
            {
                Instance = CreateNewData();
            }

            return Instance;
        }



    }
}
