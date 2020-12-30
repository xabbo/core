using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Xabbo.Core.Serialization
{
    public class AreaConverter : JsonConverter<Area>
    {
        public override Area ReadJson(JsonReader reader, Type objectType,
            Area existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            
        }

        public override void WriteJson(JsonWriter writer, Area value, JsonSerializer serializer)
        {
            
        }
    }
}
