using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCLI.Entries
{
    public class SerializedEntry
    {
        public uint Type { get; set; }
        public string Name { get; set; }

        public List<SerializedEntryMember> Members { get; set; }= new List<SerializedEntryMember>();
    }
}
