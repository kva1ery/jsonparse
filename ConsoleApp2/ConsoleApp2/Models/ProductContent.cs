using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConsoleApp2.Models
{
    public class ProductContent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("subtitle")]
        public string Subtitle { get; set; }
        
        [JsonPropertyName("image")]
        public string Image { get; set; }
        
        [JsonPropertyName("content")]
        public List<ProductContentItem> Content { get; set; } 
    }
}