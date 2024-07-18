using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FreeCLI.Entries
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class EntryTypeAttribute : Attribute
    {

        public EntryTypeAttribute(uint TypeID)
        {
            Type = TypeID;
        }
        public EntryTypeAttribute()
        {
           
        }

        /// <summary>


        public uint Type { get; private set; }
    }
}
