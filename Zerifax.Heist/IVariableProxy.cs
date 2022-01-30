using System.Collections.Generic;

namespace Zerifax.Heist
{
    public interface IVariableProxy
    {
        T GetVariable<T>(string variable, bool persist = true);
        
        T GetVariable<T>(string variable, T defaultValue, bool persist = true);

        void SetVariable(string variable, object value, bool persist = true);

        T GetUserVariable<T>(string user, string variable);
        
        void SetUserVariable(string user, string variable, object value);

        void SendMessage(string message);

        void Log(string message);

        void Wait(int duration);

        int Roll(int min, int max);
    }
}