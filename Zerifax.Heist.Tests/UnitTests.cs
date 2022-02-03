using System;
using System.Collections.Generic;
using NUnit.Framework;
using Zerifax.Actions;

namespace Zerifax.Heist.Tests
{
    public class LoadingTests
    {
        private FakeVariableProxy _cph;
        private const string User1 = "user1";
        private const string User2 = "user2";
        private const string User3 = "user3";
        private const string User4 = "user4";
        private const string User5 = "user5";
        private const string User99 = "user99";
        
        [SetUp]
        public void Setup()
        {
            _cph = new FakeVariableProxy();
            _cph.SetVariable("pointsname", "Points");
        }
        
        [Test]
        public void ConfigurationLoadsSuccessfully()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\config.json");
            
            var builder = new HeistRunner(_cph, null);
            var config = builder.Configuration;
            
            Assert.AreEqual(1, config.Cooldown);
            Assert.AreEqual(2, config.MinPoints);
            Assert.AreEqual(3, config.MaxPoints);
            Assert.AreEqual(4, config.MessageWait);
            Assert.AreEqual(5, config.MinUsers);
            Assert.AreEqual(6, config.PrepTime);
            Assert.AreEqual("points", config.PointsVariable);
            Assert.AreEqual("pointsName", config.PointsNameVariable);
            Assert.AreEqual("cooldown", config.CooldownMessage);
            Assert.AreEqual("ready", config.ReadyMessage);
            Assert.AreEqual("createTeam", config.CreateTeamMessage);
            Assert.AreEqual("userEntry", config.UserEntryMessage);
            Assert.AreEqual("notEnoughUsers", config.NotEnoughUsersMessage);
        }
        
        [Test]
        public void HeistCanStart()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\basic.json");
            
            var runner = new HeistRunner(_cph, null);
            
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.Cooldown);
            _cph.SetVariable(HeistConfiguration.VAR_TIME, DateTime.Now.Subtract(TimeSpan.FromSeconds(10)));
            runner.RunHeist();
            
            Assert.AreEqual((int)HeistStatus.Pending, _cph.GetVariable<int>(HeistConfiguration.VAR_STATUS));
            Assert.AreEqual(1, _cph.Messages.Count);
            Assert.AreEqual(runner.Configuration.ReadyMessage, _cph.Messages[0]);
        }
        
        [Test]
        public void UserCanEnterHeist()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\basic.json");
            
            var runner = new HeistRunner(_cph, new PointManager(_cph));
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.Pending);

            runner.EnrolUser(User1, "0");

            var users = _cph.GetVariable<Dictionary<string, int>>(HeistConfiguration.VAR_USERS);
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual(0, users[User1]);
            Assert.AreEqual(2, _cph.Messages.Count);
            Assert.AreEqual(runner.Configuration.CreateTeamMessage, _cph.Messages[0]);
            Assert.AreEqual(runner.Configuration.UserEntryMessage, _cph.Messages[1]);
        }
        
        [Test]
        public void HeistCanSucceed()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\basic.json");
            
            var runner = new HeistRunner(_cph, new PointManager(_cph));
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.Preparing);
            _cph.SetVariable(HeistConfiguration.VAR_TIME, DateTime.Now.Subtract(TimeSpan.FromSeconds(10)));

            _cph.SetVariable(HeistConfiguration.VAR_USERS, new Dictionary<string, int>() {{User1, 1}});
            _cph.SetNextRoll(0, 0); // event 0, force success roll < 50 success, >=50 fail

            runner.RunHeist();
            
            Assert.AreEqual(1, _cph.GetUserVariable<int>(User1,runner.Configuration.PointsVariable));
            Assert.AreEqual(3, _cph.Messages.Count);
            Assert.AreEqual(runner.Configuration.Events[0].StartMessage, _cph.Messages[0]);
            Assert.AreEqual(runner.Configuration.Events[0].SuccessMessage, _cph.Messages[1]);
            Assert.AreEqual((int)HeistStatus.Cooldown, _cph.GetVariable<int>(HeistConfiguration.VAR_STATUS));
            Assert.AreEqual(null, _cph.GetVariable<Dictionary<string, int>>(HeistConfiguration.VAR_USERS));
            
        }
        
        [Test]
        public void HeistCanFail()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\basic.json");
            
            var runner = new HeistRunner(_cph, null);
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.Preparing);
            _cph.SetVariable(HeistConfiguration.VAR_TIME, DateTime.Now.Subtract(TimeSpan.FromSeconds(10)));

            _cph.SetVariable(HeistConfiguration.VAR_USERS, new Dictionary<string, int>() {{User1, 1}});
            _cph.SetNextRoll(0, 100); // event 0, force success roll < 50 success, >=50 fail

            runner.RunHeist();
            
            Assert.AreEqual(0, _cph.GetUserVariable<int>(User1,runner.Configuration.PointsVariable));
            Assert.AreEqual(2, _cph.Messages.Count);
            Assert.AreEqual(runner.Configuration.Events[0].StartMessage, _cph.Messages[0]);
            Assert.AreEqual(runner.Configuration.Events[0].FailMessage, _cph.Messages[1]);
            Assert.AreEqual((int)HeistStatus.Cooldown, _cph.GetVariable<int>(HeistConfiguration.VAR_STATUS));
            Assert.AreEqual(null, _cph.GetVariable<Dictionary<string, int>>(HeistConfiguration.VAR_USERS));
        }

        [Test]
        public void HeistCanPartiallySucceed()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\basic.json");
            
            var runner = new HeistRunner(_cph, new PointManager(_cph));
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.Preparing);
            _cph.SetVariable(HeistConfiguration.VAR_TIME, DateTime.Now.Subtract(TimeSpan.FromSeconds(10)));

            _cph.SetVariable(HeistConfiguration.VAR_USERS, new Dictionary<string, int>() {{User1, 1}, {User2, 1}});
            _cph.SetNextRoll(0, 0, 100); // event 0, force success and fail roll < 50 success, >=50 fail

            runner.RunHeist();
            
            Assert.AreEqual(1, _cph.GetUserVariable<int>(User1,runner.Configuration.PointsVariable));
            Assert.AreEqual(0, _cph.GetUserVariable<int>(User2,runner.Configuration.PointsVariable));
            Assert.AreEqual(3, _cph.Messages.Count);
            Assert.AreEqual(runner.Configuration.Events[0].StartMessage, _cph.Messages[0]);
            Assert.AreEqual(runner.Configuration.Events[0].PartialSuccessMessage, _cph.Messages[1]);
            Assert.AreEqual((int)HeistStatus.Cooldown, _cph.GetVariable<int>(HeistConfiguration.VAR_STATUS));
            Assert.AreEqual(null, _cph.GetVariable<Dictionary<string, int>>(HeistConfiguration.VAR_USERS));
        }

        [Test]
        public void WinningsAreCalculatedCorrectly()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\reward.json");
            
            var runner = new HeistRunner(_cph, new PointManager(_cph));
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.Preparing);
            _cph.SetVariable(HeistConfiguration.VAR_TIME, DateTime.Now.Subtract(TimeSpan.FromSeconds(10)));

            _cph.SetVariable(HeistConfiguration.VAR_USERS, new Dictionary<string, int>() {{User1, 100}});
            _cph.SetNextRoll(0, 0); // event 0, force success roll < 50 success, >=50 fail

            runner.RunHeist();
            
            Assert.AreEqual(150, _cph.GetUserVariable<int>(User1,runner.Configuration.PointsVariable));
            Assert.AreEqual(3, _cph.Messages.Count);
            Assert.AreEqual(runner.Configuration.Events[0].StartMessage, _cph.Messages[0]);
            Assert.AreEqual(runner.Configuration.Events[0].SuccessMessage, _cph.Messages[1]);
        }

        [Test]
        public void UserCantEnterHeistTwice()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\basic.json");
            
            var runner = new HeistRunner(_cph, new PointManager(_cph));
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.Pending);

            runner.EnrolUser(User1, "0");
            
            var users = _cph.GetVariable<Dictionary<string, int>>(HeistConfiguration.VAR_USERS);
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual(0, users[User1]);
            Assert.AreEqual(2, _cph.Messages.Count);
            Assert.AreEqual(runner.Configuration.CreateTeamMessage, _cph.Messages[0]);
            Assert.AreEqual(runner.Configuration.UserEntryMessage, _cph.Messages[1]);
            
            runner.EnrolUser(User1, "0");
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual(0, users[User1]);
            Assert.AreEqual(3, _cph.Messages.Count);
        }

        [Test]
        public void UserCantEnterHeistOnCooldown()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\basic.json");
            
            var runner = new HeistRunner(_cph, new PointManager(_cph));
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.Cooldown);
            _cph.SetVariable(HeistConfiguration.VAR_TIME, DateTime.Now.Subtract(TimeSpan.FromSeconds(10)));
         
            runner.EnrolUser(User1, "0");

            var users = _cph.GetVariable<Dictionary<string, int>>(HeistConfiguration.VAR_USERS);
            Assert.AreEqual(null, users);
            Assert.AreEqual(1, _cph.Messages.Count);
        }

        [Test]
        public void FeeDeductedWhenUserEnters()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\basic.json");
            
            var runner = new HeistRunner(_cph, new PointManager(_cph));
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.Preparing);
            _cph.SetUserVariable(User1, runner.Configuration.PointsVariable, 300);

            runner.EnrolUser(User1, "100");

            var users = _cph.GetVariable<Dictionary<string, int>>(HeistConfiguration.VAR_USERS);
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual(100, users[User1]);
            Assert.AreEqual(200, _cph.GetUserVariable<int>(User1,runner.Configuration.PointsVariable));
        }
        
        [Test]
        public void UserChoiceEventCanTrigger()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\userchoice.json");
            
            var runner = new HeistRunner(_cph, new PointManager(_cph));
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.Preparing);
            _cph.SetVariable(HeistConfiguration.VAR_TIME, DateTime.Now.Subtract(TimeSpan.FromSeconds(10)));

            _cph.SetVariable(HeistConfiguration.VAR_USERS, new Dictionary<string, int>() {{User1, 1}});
            _cph.SetNextRoll(0, 0, 0); // event 0, force subevent roll, subevent 0

            runner.RunHeist();
            
            Assert.AreEqual(runner.Configuration.Events[0].StartMessage, _cph.Messages[0]);
            Assert.AreEqual(runner.Configuration.Events[0].Events[0].StartMessage, _cph.Messages[1]);
            Assert.AreEqual((int)HeistStatus.InProgress, _cph.GetVariable<int>(HeistConfiguration.VAR_STATUS));
            Assert.AreEqual(new List<int>{0,0}, _cph.GetVariable<List<int>>(HeistConfiguration.VAR_EVENTTREE));
        }

        [Test]
        public void UserChoiceCanSucceed()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\userchoice.json");
            
            var runner = new HeistRunner(_cph, new PointManager(_cph));
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.InProgress);
            _cph.SetVariable(HeistConfiguration.VAR_TIME, DateTime.Now.Subtract(TimeSpan.FromSeconds(10)));

            _cph.SetVariable(HeistConfiguration.VAR_USERS, new Dictionary<string, int>() {{User1, 100}});
            _cph.SetVariable(HeistConfiguration.VAR_EVENTTREE, new List<int> {0, 0});
                
            _cph.SetNextRoll(0); // force success

            runner.ContinueHeist(User1, "!A");

            var expectedEvent = runner.Configuration.Events[0].Events[0].Events[0];
            
            Assert.AreEqual(180, _cph.GetUserVariable<int>(User1,runner.Configuration.PointsVariable));
            Assert.AreEqual(3, _cph.Messages.Count);
            Assert.AreEqual(expectedEvent.StartMessage, _cph.Messages[0]);
            Assert.AreEqual(expectedEvent.SuccessMessage, _cph.Messages[1]);
            Assert.AreEqual((int)HeistStatus.Cooldown, _cph.GetVariable<int>(HeistConfiguration.VAR_STATUS));
            Assert.AreEqual(null, _cph.GetVariable<Dictionary<string, int>>(HeistConfiguration.VAR_USERS));
        }
        
        [Test]
        public void NonVotingUserDoesntCount()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\voting.json");
            
            var runner = new HeistRunner(_cph, new PointManager(_cph));
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.InProgress);
            _cph.SetVariable(HeistConfiguration.VAR_TIME, DateTime.Now.Subtract(TimeSpan.FromSeconds(10)));

            _cph.SetVariable(HeistConfiguration.VAR_USERS, new Dictionary<string, int>() {{User1, 100}});
            _cph.SetVariable(HeistConfiguration.VAR_EVENTTREE, new List<int> {0, 0});
                
            _cph.SetNextRoll(0); // force success

            runner.ContinueHeist(User1, "!A");
            runner.ContinueHeist(User2, "!B");
            runner.ContinueHeist(User3, "!B");

            runner.RunHeist();

            var expectedEvent = runner.Configuration.Events[0].Events[0].Events[0]; // second event (B)
            
            Assert.AreEqual(180, _cph.GetUserVariable<int>(User1,runner.Configuration.PointsVariable));
            Assert.AreEqual(3, _cph.Messages.Count);
            Assert.AreEqual(expectedEvent.StartMessage, _cph.Messages[0]);
            Assert.AreEqual(expectedEvent.SuccessMessage, _cph.Messages[1]);
            Assert.AreEqual((int)HeistStatus.Cooldown, _cph.GetVariable<int>(HeistConfiguration.VAR_STATUS));
            Assert.AreEqual(null, _cph.GetVariable<Dictionary<string, int>>(HeistConfiguration.VAR_USERS));
        }
        
        [Test]
        public void MostPopularVoteWins()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\voting.json");
            
            var runner = new HeistRunner(_cph, new PointManager(_cph));
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.InProgress);
            _cph.SetVariable(HeistConfiguration.VAR_TIME, DateTime.Now.Subtract(TimeSpan.FromSeconds(10)));

            _cph.SetVariable(HeistConfiguration.VAR_USERS, new Dictionary<string, int>() {{User1, 100}, {User2, 100}, {User3, 100}, {User4, 100}, {User99, 100}});
            _cph.SetVariable(HeistConfiguration.VAR_EVENTTREE, new List<int> {0, 0});
                
            _cph.SetNextRoll(0,0,0,0,0); // force success

            runner.ContinueHeist(User1, "!A");
            runner.ContinueHeist(User2, "!C");
            runner.ContinueHeist(User3, "!B");
            runner.ContinueHeist(User4, "!B");
            // user 99 doesn't vote

            runner.RunHeist();

            var expectedEvent = runner.Configuration.Events[0].Events[0].Events[1]; // second event (B)
            
            Assert.AreEqual(125, _cph.GetUserVariable<int>(User1,runner.Configuration.PointsVariable));
            Assert.AreEqual(3, _cph.Messages.Count);
            Assert.AreEqual(expectedEvent.StartMessage, _cph.Messages[0]);
            Assert.AreEqual(expectedEvent.SuccessMessage, _cph.Messages[1]);
            Assert.AreEqual((int)HeistStatus.Cooldown, _cph.GetVariable<int>(HeistConfiguration.VAR_STATUS));
            Assert.AreEqual(null, _cph.GetVariable<Dictionary<string, int>>(HeistConfiguration.VAR_USERS));
        }
        
        [Test]
        public void FirstOptionResolvesTie()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\voting.json");
            
            var runner = new HeistRunner(_cph, new PointManager(_cph));
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.InProgress);
            _cph.SetVariable(HeistConfiguration.VAR_TIME, DateTime.Now.Subtract(TimeSpan.FromSeconds(10)));

            _cph.SetVariable(HeistConfiguration.VAR_USERS, new Dictionary<string, int>() {{User1, 100}, {User2, 100}, {User3, 100}, {User4, 100}, {User5, 100}, {User99, 100}});
            _cph.SetVariable(HeistConfiguration.VAR_EVENTTREE, new List<int> {0, 0});
                
            _cph.SetNextRoll(0,0,0,0,0,0); // force success

            runner.ContinueHeist(User1, "!A");
            runner.ContinueHeist(User2, "!C");
            runner.ContinueHeist(User3, "!B");
            runner.ContinueHeist(User4, "!B");
            runner.ContinueHeist(User5, "!C");
            // user 99 doesn't vote

            runner.RunHeist();

            var expectedEvent = runner.Configuration.Events[0].Events[0].Events[2]; // second event (B)
            
            Assert.AreEqual(150, _cph.GetUserVariable<int>(User1,runner.Configuration.PointsVariable));
            Assert.AreEqual(3, _cph.Messages.Count);
            Assert.AreEqual(expectedEvent.StartMessage, _cph.Messages[0]);
            Assert.AreEqual(expectedEvent.SuccessMessage, _cph.Messages[1]);
            Assert.AreEqual((int)HeistStatus.Cooldown, _cph.GetVariable<int>(HeistConfiguration.VAR_STATUS));
            Assert.AreEqual(null, _cph.GetVariable<Dictionary<string, int>>(HeistConfiguration.VAR_USERS));
        }
        
        [Test]
        public void HeistContinuesWhenAllUsersVote()
        {
            _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, $"Heists\\voting.json");
            
            var runner = new HeistRunner(_cph, new PointManager(_cph));
            _cph.SetVariable(HeistConfiguration.VAR_STATUS, (int)HeistStatus.InProgress);
            _cph.SetVariable(HeistConfiguration.VAR_TIME, DateTime.Now.Subtract(TimeSpan.FromSeconds(10)));

            _cph.SetVariable(HeistConfiguration.VAR_USERS, new Dictionary<string, int>() {{User1, 100}});
            _cph.SetVariable(HeistConfiguration.VAR_EVENTTREE, new List<int> {0, 0});
                
            _cph.SetNextRoll(0); // force success

            runner.ContinueHeist(User1, "!A");

            var expectedEvent = runner.Configuration.Events[0].Events[0].Events[0];
            
            Assert.AreEqual(180, _cph.GetUserVariable<int>(User1,runner.Configuration.PointsVariable));
            Assert.AreEqual(3, _cph.Messages.Count);
            Assert.AreEqual(expectedEvent.StartMessage, _cph.Messages[0]);
            Assert.AreEqual(expectedEvent.SuccessMessage, _cph.Messages[1]);
            Assert.AreEqual((int)HeistStatus.Cooldown, _cph.GetVariable<int>(HeistConfiguration.VAR_STATUS));
            Assert.AreEqual(null, _cph.GetVariable<Dictionary<string, int>>(HeistConfiguration.VAR_USERS));
        }
        
        // TODO: More tests to come
    }
}