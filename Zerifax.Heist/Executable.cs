using System.Collections.Generic;

namespace Zerifax.Heist
{
    public abstract class Executable
    {
        protected Dictionary<string,object> args = new Dictionary<string, object>();
        
        public CPH CPH { get; set; }

        public abstract bool Execute();
        
        public bool ExecuteWithArgs(Dictionary<string, object> arguments)
        {
            args = arguments;
            return Execute();
        }
    }
}