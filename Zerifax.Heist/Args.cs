using System.Collections.Generic;

namespace Zerifax.Heist
{
    internal static class Args
    {
        private const string VAR_USER = "user";
        private const string VAR_POINTS = "points";
        
        public static Dictionary<string, object> ForUser(string user)
        {
            return new Dictionary<string, object> {{VAR_USER, user}};
        }
        
        public static Dictionary<string, object> ForUser(string user, int points)
        {
            return new Dictionary<string, object> {{VAR_USER, user}, {VAR_POINTS, points}};
        }
    }
}