using System;

namespace CompositeConsole
{
    public class DebugMethodAttribute : Attribute
    {
        public string Info { get; set; }
    }
}