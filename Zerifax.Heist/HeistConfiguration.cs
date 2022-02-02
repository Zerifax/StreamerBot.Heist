using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Zerifax.Heist
{
    [Serializable]
    public class HeistConfiguration
    {
        public const string VAR_ConfigFile = "Heist_Config_File";
        public const string VAR_Config = "Heist_Config";
        public const string VAR_STATUS = "heist_status";
        public const string VAR_USERS = "heist_users";
        public const string VAR_TIME = "heist_time";
        public const string VAR_EVENTTREE = "heist_event_tree";
        public const string LAST_EVENT = "heist_event";
        public const string VAR_VOTES = "heist_votes";
        public const string VAR_VOTE_ORDER = "heist_voteorder";
        
        public int Cooldown { get; set; }
        public int PrepTime { get; set; }
        public int MinUsers { get; set; }
        
        public string CreateTeamMessage { get; set; }
        public string UserEntryMessage { get; set; }
        public string NotEnoughUsersMessage { get; set; }
        public string CooldownMessage { get; set; }
        
        public string ReadyMessage { get; set; }
        public int MinPoints { get; set; }
        public int MaxPoints { get; set; }
        public int MessageWait { get; set; }
        
        public bool UseEventVoting { get; set; }

        public string PointsNameVariable = "pointsname";
        public string PointsVariable = "points";
        
        public List<Event> Events { get; set; }
    }
    
    [Serializable]
    public class Event
    {
        public string StartMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string PartialSuccessMessage { get; set; }
        public string FailMessage { get; set; }
        public string SoloSuccessMessage { get; set; }
        public string SoloFailMessage { get; set; }
        
        public int SuccessChance { get; set; }
        public double PointsMultiplier { get; set; } // calculated as (1+BonusPoints) * investment
        public int EventChance { get; set; }
        
        public List<ActionEvent> Events { get; set; }
    }
    
    [Serializable]
    public class ActionEvent : Event
    {
        public string Command { get; set; }
        public string Description { get; set; }
    }
    

}