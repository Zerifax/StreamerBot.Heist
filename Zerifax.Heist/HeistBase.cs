using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using Newtonsoft.Json;

namespace Zerifax.Heist
{
    public abstract class HeistBase
    {
        private readonly IVariableProxy _variable;
        private readonly IPointManager _pointsManager;
        private HeistConfiguration _configuration;

        private string _pointsName;

        public HeistBase(IVariableProxy variable, IPointManager pointsManager)
        {
            _variable = variable;
            _pointsManager = pointsManager;
        }
		
        public HeistConfiguration Configuration 
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = _variable.GetVariable<HeistConfiguration>(HeistConfiguration.VAR_Config, false);

                    if (_configuration == null)
                    {
                        var configFile = _variable.GetVariable<string>(HeistConfiguration.VAR_ConfigFile);
                        var file = File.ReadAllText(configFile, Encoding.Default);
                        _configuration = JsonConvert.DeserializeObject<HeistConfiguration>(file);
                        _variable.SetVariable(HeistConfiguration.VAR_Config, _configuration, false);
                    }
                }

                return _configuration;
            }
        }
        
        protected Dictionary<string, List<string>> Votes
        {
            get
            {
                return _variable.GetVariable<Dictionary<string, List<string>>>(HeistConfiguration.VAR_VOTES) ?? new Dictionary<string, List<string>>();
            }
            private set
            {
                _variable.SetVariable(HeistConfiguration.VAR_VOTES, value);
            }
        }
        
        protected List<string> VoteOrder
        {
            get
            {
                return _variable.GetVariable<List<string>>(HeistConfiguration.VAR_VOTE_ORDER) ?? new List<string>();
            }
            private set
            {
                _variable.SetVariable(HeistConfiguration.VAR_VOTE_ORDER, value);
            }
        }

        internal HeistStatus Status
        {
            get
            {
                return (HeistStatus)_variable.GetVariable<int>(HeistConfiguration.VAR_STATUS, (int)HeistStatus.Cooldown);
            }
            set
            {
                _variable.SetVariable(HeistConfiguration.VAR_STATUS, (int)value);
                _variable.SetVariable(HeistConfiguration.LAST_EVENT, value.ToString());
            }
        }
        
        protected DateTime LastRun
        {
            get
            {
                var lastRun = _variable.GetVariable<DateTime?>(HeistConfiguration.VAR_TIME);
                if (lastRun == null)
                {
                    lastRun = DateTime.Now;
                    _variable.SetVariable(HeistConfiguration.VAR_TIME, lastRun);                    
                }

                return lastRun.Value;
            }
            set
            {
                _variable.SetVariable(HeistConfiguration.VAR_TIME, value);
            }
        }

        protected Dictionary<string, int> Users
        {
            get
            {
                return _variable.GetVariable<Dictionary<string, int>>(HeistConfiguration.VAR_USERS) ?? new Dictionary<string, int>();
            }
            set
            {
                _variable.SetVariable(HeistConfiguration.VAR_USERS, value);
            }
        }

        protected string PointsName
        {
            get
            {
                if (_pointsName == null)
                {
                    _pointsName = _variable.GetVariable<string>(Configuration.PointsNameVariable) ?? "Points";
                }

                return _pointsName;
            }
        }

        protected void SendMessage(string message, Dictionary<string,object> args = null)
        {
            var messageBuilder = new StringBuilder (message);
            messageBuilder.Replace("[pointsName]", PointsName);

            if (args != null)
            {
                foreach (var kvp in args)
                {
                    messageBuilder.Replace($"[{kvp.Key}]", kvp.Value?.ToString());
                }
            }

            _variable.SendMessage(messageBuilder.ToString());
        }

        protected void AddPoints(string user, int points)
        {
            _pointsManager.AddPoints(user, Configuration.PointsVariable, points);
        }

        protected int GetPoints(string user)
        {
            return _pointsManager.GetPoints(user, Configuration.PointsVariable);
        }

        protected Event CurrentEvent
        {
            get
            {
                if (Configuration?.Events == null)
                {
                    return null;
                }

                var eventTree = _variable.GetVariable<List<int>>(HeistConfiguration.VAR_EVENTTREE);
                if (!(eventTree?.Count > 0))
                {
                    return null;
                }

                try
                {
                    var currentEvent = Configuration.Events[eventTree[0]];

                    for (var i = 1; i < eventTree.Count; i++)
                    {
                        currentEvent = currentEvent.Events[eventTree[i]];
                    }

                    return currentEvent;
                }
                catch (IndexOutOfRangeException)
                {
                    _variable.SetVariable(HeistConfiguration.VAR_EVENTTREE, null);
                }
                
                return null;
            }
        }
        
        public void ClearHeist()
        {
            Status = HeistStatus.Cooldown;
            Users = null;
            LastRun = DateTime.Now;
            _variable.SetVariable(HeistConfiguration.VAR_EVENTTREE, null);
        }
        
        protected void AppendEvent(int index)
        {
            var eventTree = _variable.GetVariable<List<int>>(HeistConfiguration.VAR_EVENTTREE) ?? new List<int>();
            eventTree.Add(index);
            _variable.SetVariable(HeistConfiguration.VAR_EVENTTREE, eventTree);
        }

        protected void Log(string msg)
        {
            _variable.Log(msg);
        }

        protected void Wait()
        {
            _variable.Wait(Configuration.MessageWait);
        }

        protected int Roll(int min, int max)
        {
            return _variable.Roll(min, max);
        }

        protected int Vote(string option, string user)
        {
            var votes = Votes;
            List<string> users = null;
            
            var voteCount = votes.Values.Sum(v => v.Count);

            if (votes.Any(v => v.Value.Contains(user, StringComparer.CurrentCultureIgnoreCase)))
            {
                return voteCount; // no double voting
            }

            if (votes.ContainsKey(option))
            {
                users = votes[option];
                
                if (!users.Contains(user))
                {
                    users.Add(user);
                    voteCount++;
                }
            } else {
                users = new List<string>() { user };
                votes.Add(option, users);
                voteCount++;

                var order = VoteOrder;
                order.Add(option);
                VoteOrder = order;
            }
            
            Votes = votes;

            return voteCount;
        }

        protected void ClearVote()
        {
            Votes = null;
            VoteOrder = null;
        }
    }
}