using System;
using System.Collections.Generic;
using Zerifax.Heist;

namespace Zerifax.Actions
{

    public class FakeVariableProxy : IVariableProxy
    {
        public Dictionary<string, object> _globalVariables = new Dictionary<string, object>();
        public Dictionary<string,Dictionary<string,object>> _userVariables = new Dictionary<string, Dictionary<string, object>>();

        public List<string> Messages { get; } = new List<string>();

        private Queue<int> _nextRoll = new Queue<int>();
        
        public T GetVariable<T>(string name, bool persist = true)
        {
            if (!_globalVariables.ContainsKey(name))
            {
                return default(T);
            }
            
            return (T) _globalVariables[name];
        }

        public T GetVariable<T>(string name, T defaultValue, bool persist = true)
        {
            if (!_globalVariables.ContainsKey(name))
            {
                return defaultValue;
            }
            
            return (T) _globalVariables[name];
        }

        public void SetVariable(string name, object var, bool persist = true)
        {
            _globalVariables[name] = var;
        }
        
        public T GetUserVariable<T>(string user, string name)
        {
            if (!_userVariables.ContainsKey(user))
            {
                _userVariables[user] = new Dictionary<string, object>();
            }
            
            if (!_userVariables[user].ContainsKey(name))
            {
                return default(T);
            }

            return (T)_userVariables[user][name];
        }

        public void SetUserVariable(string user, string name, object val)
        {if (!_userVariables.ContainsKey(user))
            {
                _userVariables[user] = new Dictionary<string, object>();
            }

            _userVariables[user][name] = val;
        }
        
        public void SendMessage(string message)
        {
            Messages.Add(message);
            Console.WriteLine(message);
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Wait(int duration)
        {
            
        }

        public int Roll(int min, int max)
        {
            return _nextRoll.Dequeue();
        }

        public void SetNextRoll(params int[] value)
        {
            foreach (var i in value)
            {
                _nextRoll.Enqueue(i);                
            }
        }
    }
}