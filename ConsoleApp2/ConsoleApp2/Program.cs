using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ConsoleApp2.Models;

namespace ConsoleApp2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await DynamicParse();
            Console.WriteLine("--------------------------");
            await CustomConverterParse();
        }

        private static async Task DynamicParse()
        {
            await using FileStream openStream = File.OpenRead("data.json");

            dynamic productContent = await JsonSerializer.DeserializeAsync<ExpandoObject>(openStream);

            string header = string.Empty;
            string text = string.Empty;
            bool afterHeader = false;

            using (var enumerator = productContent.content.EnumerateArray())
            {
                while (enumerator.MoveNext() && (header == string.Empty || text == string.Empty))
                {
                    var contentItem = enumerator.Current;
                    if (contentItem.TryGetProperty("type", out JsonElement typeValue))
                    {
                        var type = typeValue.GetString();
                        if (type == "header")
                        {
                            if (contentItem.TryGetProperty("text", out JsonElement textValue))
                            {
                                header = textValue.GetString();
                            }

                            afterHeader = true;
                        }
                        else if (type == "text" && afterHeader)
                        {
                            if (contentItem.TryGetProperty("text", out JsonElement textValue))
                            {
                                text = textValue.GetString();
                            }
                        }
                    }
                }
            }
            
            Console.WriteLine($"{header} - {text}");
        }

        private static async Task CustomConverterParse()
        {
            await using FileStream openStream = File.OpenRead("data.json");
            
            var serializeOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new ProductContentItemConverter()
                }
            };
            ProductContent productContent = await JsonSerializer.DeserializeAsync<ProductContent>(openStream, serializeOptions);

            var header = productContent.Content.FirstOrDefault(x => x is ProductContentHeader) as ProductContentHeader;
            var text = productContent.Content.FirstOrDefault(x => x is ProductContentText) as ProductContentText;
            
            Console.WriteLine($"{header.Text} - {text.Text}");
        }
    }

    public class ProductContentItemConverter : JsonConverter<ProductContentItem>
    {
        public override bool CanConvert(Type type)
        {
            return typeof(ProductContentItem).IsAssignableFrom(type);
        }

        public override ProductContentItem Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != "type")
            {
                throw new JsonException();
            }

            reader.Read();
            
            var jsonType = reader.GetString();
            
            var subclassTypes = Assembly
                .GetAssembly(typeof(ProductContentItem))
                ?.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ProductContentItem)));

            var itemType = subclassTypes?.FirstOrDefault(t => IsContentItemType(t, jsonType));

            ProductContentItem item = (ProductContentItem)Activator.CreateInstance(itemType);

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return item;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    var propertyValue = reader.GetString();

                    SetPropertyValue(item, propertyName, propertyValue);
                }
            }
            
            throw new JsonException();
        }

        private static bool IsContentItemType(Type type, string jsonType)
        {
            var attrs = type.GetCustomAttributes(typeof(ProductContentJsonTypeAttribute), false);
            return attrs.Any(a => ((ProductContentJsonTypeAttribute)a).JsonType == jsonType);
        }

        private static void SetPropertyValue(Object obj, string jsonPropertyName, string value)
        {
            var property = obj.GetType().GetProperties().FirstOrDefault(p => IsJsonProperty(p, jsonPropertyName));
            if (property != null)
            {
                property.SetValue(obj, value);                
            }
        }
        
        private static bool IsJsonProperty(PropertyInfo property, string jsonPropertyName)
        {
            var attrs = property.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false);
            return attrs.Any(a => ((JsonPropertyNameAttribute)a).Name == jsonPropertyName);
        }
        
        public override void Write(
            Utf8JsonWriter writer,
            ProductContentItem value,
            JsonSerializerOptions options)
        {
            
        }
    }
}