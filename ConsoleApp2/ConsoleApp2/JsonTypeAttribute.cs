using System;

namespace ConsoleApp2
{
    [System.AttributeUsage(System.AttributeTargets.Class |  
                           System.AttributeTargets.Struct,  
            AllowMultiple = true)
    ] 

    public class ProductContentJsonTypeAttribute : Attribute
    {
        public string JsonType { get; set; }  
 
        
        public ProductContentJsonTypeAttribute(string jsonType)  
        {  
            JsonType = jsonType;
        }  
    }
}