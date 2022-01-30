using System;
using Zerifax.Heist;

namespace Zerifax.Proxies
{
    public class VariableProxy : IVariableProxy
    {
        private Plugins.InlineInvokeProxy _cph;
        
        private Random _random = new Random();
        
        public VariableProxy(Plugins.InlineInvokeProxy cph)
        {
            _cph = cph;
        }
        
        public T GetVariable<T>(string variable, bool persist = true)
        {
            try
            {
                return _cph.GetGlobalVar<T>(variable, persist);
            }
            catch (Exception ex)
            {
                Log($"There was an error getting the variable {variable}: {ex.Message}");
                return default(T);
            }
        }
        
        public T GetVariable<T>(string variable, T defaultValue, bool persist = true)
        {
            try
            {
                return _cph.GetGlobalVar<T>(variable, persist);
            }
            catch (Exception ex)
            {
                Log($"There was an error getting the variable {variable}: {ex.Message}");
                return defaultValue;
            }
        }

        public void SetVariable(string variable, object value, bool persist = true)
        {
            _cph.SetGlobalVar(variable, value, persist);
        }

        public T GetUserVariable<T>(string user, string variable)
        {
            try
            {
                return _cph.GetUserVar<T>(user, variable, true);
            }
            catch (Exception ex)
            {
                Log($"There was an error getting the user variable {variable}: {ex.Message}");
                return default(T);
            }
        }

        public void SetUserVariable(string user, string variable, object value)
        {
            _cph.SetUserVar(user, variable, value, true);
        }

        public void SendMessage(string message)
        {
            _cph.SendMessage(message);
        }

        public void Log(string message)
        {
            _cph.LogInfo(message);
        }

        public void Wait(int duration)
        {
            _cph.Wait(duration);
        }

        public int Roll(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}