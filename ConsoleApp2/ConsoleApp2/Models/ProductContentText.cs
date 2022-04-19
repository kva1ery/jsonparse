using System.Text.Json.Serialization;

namespace ConsoleApp2.Models
{
    [ProductContentJsonType("text")]
    public class ProductContentText : ProductContentItem
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}