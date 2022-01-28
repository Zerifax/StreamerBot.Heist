using System;
using System.Collections.Generic;

namespace Zerifax.Heist
{
    public class HeistManager : Executable
    {
        // ## Configuration ##
        // MAX points a user can wager
        private const int MAX_POINTS = 500;

        // Don't change these
        private const string STATUS_VAR = "heist_status";
        private const string USERS_VAR = "heist_users";
        private const string POINTSNAME_VAR = "pointsname";
        private const string POINTS_VAR = "points";
        private const string TIME_VAR = "heist_time";

        public override bool Execute()
        {
            var status = (HeistStatus?) CPH.GetGlobalVar<int?>(STATUS_VAR, true) ?? HeistStatus.Pending;

            if (status == HeistStatus.Cooldown)
            {
                CPH.SendMessage($"The area is still too hot. Best to wait a while");
                return true;
            }

            var heistUsers = CPH.GetGlobalVar<Dictionary<string, int>>(USERS_VAR, true) ??
                             new Dictionary<string, int>();
            var user = args["user"].ToString();

            if (heistUsers.ContainsKey(user))
            {
                CPH.SendMessage($"{user} you have already entered the heist");
                return true;
            }

            var inputRaw = args["rawInput"].ToString();
            var charSeparators = new char[] {' '};
            var inputArgs = inputRaw.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            var pointsName = CPH.GetGlobalVar<string>(POINTSNAME_VAR, true);

            if (inputArgs.Length > 0 && int.TryParse(inputRaw, out var points))
            {
                var currentPoints = CPH.GetUserVar<int>(user, POINTS_VAR, true);
                if (currentPoints < points)
                {
                    CPH.SendMessage($"{user} You do not have enough {pointsName}");
                    return true;
                }

                if (points < 1 || points > MAX_POINTS)
                {
                    CPH.SendMessage($"{user} please enter an amount between 1 and {MAX_POINTS}");
                    return true;
                }

                CPH.SetUserVar(user, POINTS_VAR, currentPoints - points, true);
                heistUsers.Add(user, points);

                if (status == HeistStatus.Pending)
                {
                    CPH.SetGlobalVar(STATUS_VAR, (int) HeistStatus.Preparing, true);
                    CPH.SetGlobalVar(TIME_VAR, DateTime.Now, true);
                    CPH.SendMessage(
                        $"{user} is putting a team together for the heist. Type !heist and a wager amount to join");
                }

                CPH.SendMessage($"{user} has entered the heist with {points} {pointsName}");
                CPH.SetGlobalVar(USERS_VAR, heistUsers, true);
            }
            else
            {
                CPH.SendMessage("{user} please enter a valid wager amount");
            }

            return true;

        }

    }

    
}