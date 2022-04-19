using System.Text.Json.Serialization;

namespace ConsoleApp2.Models
{
    [ProductContentJsonType("quote")]
    public class ProductContentQuote : ProductContentItem
    {
        [JsonPropertyName("subtype")]
        public string SubType { get; set; }
        
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }
        
        [JsonPropertyName("class_code")]
        public string ClassCode { get; set; }
    }
}