using System;
using System.Collections.Generic;
using System.Linq;

namespace Zerifax.Heist
{
	public class HeistRunner : HeistBase
    {
        public HeistRunner(IVariableProxy variable, IPointManager pointsManager) : base(variable, pointsManager) { }

        public bool RunHeist()
        {
	        try
	        {
		        Log($"Heist Status is {Status}");
		        
		        switch (Status)
		        {
			        case HeistStatus.Cooldown:
				        HeistCooldown();
				        break;

			        case HeistStatus.Preparing:
				        StartHeist();
				        break;

			        case HeistStatus.InProgress:
				        ContinueHeist();
				        break;
		        }
	        }
	        catch (Exception ex)
	        {
		        Log($"There was an error running the heist: {ex.Message}");
	        }

	        return true;
		}
        
        public bool EnrolUser(string user, string input)
        {
            try
            {
                var status = Status;

                if (status == HeistStatus.Cooldown)
                {
                    SendMessage(Configuration.CooldownMessage);
                    return true;
                }

                var heistUsers = Users;

                if (heistUsers.ContainsKey(user))
                {
                    SendMessage($"{user} you have already entered the heist");
                    return true;
                }

                var inputRaw = input;
                var charSeparators = new[] {' '};
                var inputArgs = inputRaw.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);

                if (inputArgs.Length > 0 && int.TryParse(inputRaw, out var points))
                {
                    var currentPoints = GetPoints(user);
                    if (currentPoints < points)
                    {
                        SendMessage($"[user] You do not have enough [pointsName]", Args.ForUser(user));
                        return true;
                    }

                    if (points < Configuration.MinPoints || points > Configuration.MaxPoints)
                    {
                        SendMessage(
                            $"[user] please enter an amount between {Configuration.MinPoints} and {Configuration.MaxPoints}",
                            Args.ForUser(user));
                        return true;
                    }

                    AddPoints(user, -points);

                    heistUsers.Add(user, points);

                    Users = heistUsers;

                    if (status == HeistStatus.Pending)
                    {
                        Status = HeistStatus.Preparing;
                        LastRun = DateTime.Now;
                        SendMessage(Configuration.CreateTeamMessage, Args.ForUser(user));
                    }

                    SendMessage(Configuration.UserEntryMessage, Args.ForUser(user, points));
                }
                else
                {
                    SendMessage("[user] please enter a valid wager amount", Args.ForUser(user));
                }
            } catch (Exception ex)
            {
                Log($"Error adding user: {ex.Message} - {ex.StackTrace}");
            }

            return true;
        }

		// This assumes event is in progress with some user options
		private void ContinueHeist()
		{
			var lastRun = LastRun;
			var prepTime = TimeSpan.FromSeconds(Configuration.PrepTime);

			if (DateTime.Now.Subtract(prepTime) > lastRun)
			{
				var currentEvent = CurrentEvent;

				if (currentEvent == null)
				{
					ClearHeist();
					return;
				}

				var subEvents = currentEvent.Events.Where(se => string.IsNullOrWhiteSpace(se.Command)).ToArray();

				if (subEvents.Length > 0)
				{
					
					var eventToRunIndex = Roll(0, subEvents.Length);
					AppendEvent(eventToRunIndex);
					RunEvent(subEvents[eventToRunIndex]);
				}
				else
				{
					CompleteEvent(currentEvent, true);
				}
			}
		}

		public bool ContinueHeist(string user, string command)
		{
			if (Status == HeistStatus.InProgress)
			{
				if (!Users.ContainsKey(user))
				{
					return true; // user not in heist
				}
				
				if (!command.StartsWith("!"))
				{
					return true;
				}
				
				var currentEvent = CurrentEvent;
				
				if (currentEvent == null)
				{
					ClearHeist();
					return true;
				}

				if (currentEvent.Events.Count > 0)
				{
					var selectedEvent = currentEvent.Events.FirstOrDefault(se =>
						string.Equals(se.Command, command.Substring(1), StringComparison.CurrentCultureIgnoreCase));

					if (selectedEvent != null)
					{
						AppendEvent(currentEvent.Events.IndexOf(selectedEvent));
						RunEvent(selectedEvent, user);
					}
				}
				else
				{
					CompleteEvent(currentEvent);
				}
			}

			return true;
		}

		private void HeistCooldown()
		{
			var cooldown = TimeSpan.FromSeconds(Configuration.Cooldown);

			if (DateTime.Now.Subtract(cooldown) > LastRun)
			{
				Status = HeistStatus.Pending;
				
				SendMessage(Configuration.ReadyMessage);
			}
		}
		
		private void StartHeist()
		{
			var lastRun = LastRun;
			var prepTime = TimeSpan.FromSeconds(Configuration.PrepTime);
			
			if (DateTime.Now.Subtract(prepTime) > lastRun)
			{
				var heistUsers = Users;

				if (heistUsers.Count == 0)
				{
					Status = HeistStatus.Pending;
					return;
				}
				
				LastRun = DateTime.Now;
				Status = HeistStatus.InProgress;
				
				if (Configuration.Events.Count > 0)
				{
					var eventToRunIndex = Roll(0, Configuration.Events.Count);
					AppendEvent(eventToRunIndex);
					RunEvent(Configuration.Events[eventToRunIndex]);
				}
			}
		}

		private void RunEvent(Event eventToRun, string user = null)
		{
			Wait();
			
			if (eventToRun == null)
			{
				return;
			}
			
			SendMessage(eventToRun.StartMessage, Args.ForUser(user));

			var actionEvents = eventToRun.Events?.Where(se => !string.IsNullOrWhiteSpace(se.Command)).ToArray();
			
			if (actionEvents?.Length > 0)
			{
				SendMessage("What do you do?: " + string.Join(", ", actionEvents.Select(av => $"!{av.Command} ({av.Description})")));
				return;
			}

			if (eventToRun.Events?.Count > 0)
			{
				if (Roll(0, 100) < eventToRun.EventChance)
				{
					var eventIdx = Roll(0, eventToRun.Events.Count);
					var nextEvent = eventToRun.Events[eventIdx];

					AppendEvent(eventIdx);

					RunEvent(nextEvent, user);

					return;
				}
			}

			CompleteEvent(eventToRun, user: user);
		}

		private void CompleteEvent(Event eventToComplete, bool forceFail = false, string user = null)
		{
			Wait();
			
			var users = Users;

			ClearHeist();
			
			if (users.Count < Configuration.MinUsers)
			{
				SendMessage(Configuration.NotEnoughUsersMessage);
				return;
			}

			var successful = new List<string>();
			var failed = new List<string>();

			if (forceFail)
			{
				failed.AddRange(users.Keys);
			}
			else
			{
				foreach (var heistUser in users.Keys.ToList())
				{
					if (Roll(0, 100) < eventToComplete.SuccessChance)
					{
						successful.Add(heistUser);
						var reward = (int) Math.Ceiling(users[heistUser] * eventToComplete.PointsMultiplier);
					
						users[heistUser] = reward;
					
						AddPoints(heistUser, reward);
					}
					else
					{
						failed.Add(heistUser);
					}
				}
			}
			
			if (successful.Count == 0)
			{
				SendMessage(eventToComplete.FailMessage, Args.ForUser(user));
				return;
			}

			if (failed.Count == 0)
			{
				SendMessage(eventToComplete.SuccessMessage, Args.ForUser(user));
			}
			else
			{
				SendMessage(eventToComplete.PartialSuccessMessage, Args.ForUser(user));
			}

			var result = string.Join(", ",
				users.Where(hu => 
					successful.Contains(hu.Key))
					.Select(hu => $"{hu.Key} ({hu.Value})")
					.ToArray());
			
			SendMessage($"Winnings: {result}");
		}
        
    }

    
}