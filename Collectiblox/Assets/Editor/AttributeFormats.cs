using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
namespace Collectiblox
{
    public class AttributeFormats
    {
        public Dictionary<CardType, AttributeFormat> typeToFormat;

        public AttributeFormat this[CardType type]
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
            typeToFormat = new Dictionary<CardType, AttributeFormat>();
        }
        public AttributeFormats(List<AttributeFormat> formats)
        {
            typeToFormat = new Dictionary<CardType, AttributeFormat>();
            formats.ForEach(format => typeToFormat.Add(format.type, format));
        }
        
        public static AttributeFormats Load()
        {

            string path = Application.streamingAssetsPath + "/CardAttributeFormats.json";
            AttributeFormats formats = new AttributeFormats(JsonConvert.DeserializeObject<List<AttributeFormat>>(File.ReadAllText(path)));
            return formats;

        }
    }
    [JsonObject]
    [JsonConverter(typeof(AttributeFormatConverter))]
    public class AttributeFormat
    {
        public CardType type;
        public Dictionary<string, AttributeType> nameToType;
        public AttributeFormat()
        {
            nameToType = new Dictionary<string, AttributeType>();
        }
        public AttributeFormat(CardType type, List<AttributeTuple> attributes)
        {
            nameToType = new Dictionary<string, AttributeType>();
            this.type = type;
            attributes.ForEach(tp => nameToType.Add(tp.name, tp.atrType));
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

    public enum AttributeType
    {
        Integer,
        String
    }
}
