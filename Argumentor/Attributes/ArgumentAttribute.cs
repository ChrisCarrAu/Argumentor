using System;

namespace ArgumentRes.Attributes
{
    public class ArgumentAttribute : Attribute
    {
        public string Key { get; set; }
        public string Description { get; set; }
    }
}
