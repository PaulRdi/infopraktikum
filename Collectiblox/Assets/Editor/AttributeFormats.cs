using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
using Collectiblox.Model;

namespace Collectiblox.EditorExtensions
{

    public class AttributeFormats : IEnumerable<AttributeFormat>
    {
        public Dictionary<string, AttributeFormat> typeToFormat;

        public AttributeFormat this[string type]
        {
            get
            {
                if (typeToFormat == null)
                    throw new System.NullReferenceException("Attribute format dictionary was not loaded");
                //todo: implement default format...
                return typeToFormat[type];
            }
        }
        public AttributeFormats()
        {
            typeToFormat = new Dictionary<string, AttributeFormat>();
        }
        public AttributeFormats(List<AttributeFormat> formats)
        {
            typeToFormat = new Dictionary<string, AttributeFormat>();
            formats.ForEach(format => typeToFormat.Add(format.type, format));
        }
        
        public static AttributeFormats Load()
        {

            string path = Application.streamingAssetsPath + "/CardAttributeFormats.json";
            AttributeFormats formats = new AttributeFormats(JsonConvert.DeserializeObject<List<AttributeFormat>>(File.ReadAllText(path)));
            return formats;

        }

        public IEnumerator<AttributeFormat> GetEnumerator()
        {
            return new AttributeFormatEnumerator(typeToFormat);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new AttributeFormatEnumerator(typeToFormat);
        }
    }

    class AttributeFormatEnumerator : IEnumerator<AttributeFormat>
    {
        public AttributeFormat Current => typeToFormat.ElementAt(currentIndex).Value;
        object IEnumerator.Current => typeToFormat.ElementAt(currentIndex).Value;

        int currentIndex = -1;

        public Dictionary<string, AttributeFormat> typeToFormat;

        public AttributeFormatEnumerator(Dictionary<string, AttributeFormat> typeToFormat)
        {
            this.typeToFormat = typeToFormat;
        }

        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            ++currentIndex;
            return currentIndex < typeToFormat.Count;
        }

        public void Reset()
        {
            currentIndex = -1;
        }
    }

    [JsonObject]
    [JsonConverter(typeof(AttributeFormatConverter))]
    public class AttributeFormat
    {
        public string type;
        public Dictionary<string, AttributeType> nameToType;
        public AttributeFormat()
        {
            nameToType = new Dictionary<string, AttributeType>();
        }
        public AttributeFormat(string type, List<AttributeTuple> attributes)
        {
            nameToType = new Dictionary<string, AttributeType>();
            this.type = type;
            attributes.ForEach(tp => nameToType.Add(tp.name, tp.atrType));
        }

        internal Type[] GetTypes()
        {
            Type[] output = new Type[nameToType.Count];
            for (int i = 0; i < nameToType.Count; i++)
                output[i] = Convert.ToType(nameToType.ElementAt(i).Value);
            return output;
        }
    }
    [JsonObject]
    public class AttributeTuple
    {
        public string name;
        public AttributeType atrType;
        public AttributeTuple()
        {

        }

        public AttributeTuple(string name, string atrType)
        {
            this.name = name;
            this.atrType = (AttributeType)Enum.Parse(typeof(AttributeType),atrType);
        }
    }

    
}