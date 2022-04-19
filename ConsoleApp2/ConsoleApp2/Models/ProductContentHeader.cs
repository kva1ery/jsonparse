using System.Text.Json.Serialization;

namespace ConsoleApp2.Models
{
    [ProductContentJsonType("header")]
    public class ProductContentHeader : ProductContentItem
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}