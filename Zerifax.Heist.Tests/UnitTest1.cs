// using System.Collections.Generic;
// using System.Threading;
// using NUnit.Framework;
// using Zerifax.Actions;
//
// namespace Zerifax.Heist.Tests
// {
//     public class Tests
//     {
//         private IVariableProxy _cph;
//         
//         [SetUp]
//         public void Setup()
//         {
//             _cph = new FakeVariableProxy();
//             _cph.SetVariable("pointsname", "Carts");
//         }
//         
//         [Test]
//         public void BasicTest()
//         {
//             _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, "Heists\\basic.json");
//             var manager = new HeistRunner(_cph);
//             var runner = new HeistRunner(_cph);
//             
//             runner.RunHeist();
//             Thread.Sleep(100);
//             
//             _cph.SetUserVariable("user1", "points", 200);
//
//             runner.RunHeist();
//             manager.EnrolUser("user1", "100");
//
//             Assert.AreEqual(100, _cph.GetUserVariable<int>("user1", "points"));
//             
//             Thread.Sleep(100);
//
//             runner.RunHeist();
//         }
//         
//         [Test]
//         public void ZapTest()
//         {
//             _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, "Heists\\advanced.json");
//             var manager = new HeistRunner(_cph);
//             var runner = new HeistRunner(_cph);
//             
//             _cph.SetUserVariable("user1", "points", 200);
//             _cph.SetUserVariable("user2", "points", 200);
//
//             runner.RunHeist();
//             manager.EnrolUser("user1", "100");
//             manager.EnrolUser("user2", "100");
//
//             Assert.AreEqual(100, _cph.GetUserVariable<int>("user1", "points"));
//             
//             Thread.Sleep(100);
//
//             runner.RunHeist();
//             
//             runner.ContinueHeist("user1", "!A");
//         }
//         
//         [Test]
//         public void FeedTest()
//         {
//             _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, "Heists\\advanced.json");
//             var manager = new HeistRunner(_cph);
//             var runner = new HeistRunner(_cph);
//             
//             _cph.SetUserVariable("user1", "points", 200);
//             _cph.SetUserVariable("user2", "points", 200);
//
//             runner.RunHeist();
//             manager.EnrolUser("user1", "100");
//             manager.EnrolUser("user2", "100");
//
//             Assert.AreEqual(100, _cph.GetUserVariable<int>("user1", "points"));
//             
//             Thread.Sleep(100);
//
//             runner.RunHeist();
//             
//             runner.ContinueHeist("user1", "!B");
//         }
//         
//         [Test]
//         public void CTest()
//         {
//             _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, "Heists\\advanced.json");
//             var manager = new HeistRunner(_cph);
//             var runner = new HeistRunner(_cph);
//             
//             _cph.SetUserVariable("user1", "points", 200);
//             _cph.SetUserVariable("user2", "points", 200);
//
//             runner.RunHeist();
//             manager.EnrolUser("user1", "100");
//             manager.EnrolUser("user2", "100");
//
//             Assert.AreEqual(100, _cph.GetUserVariable<int>("user1", "points"));
//             
//             Thread.Sleep(100);
//
//             runner.RunHeist();
//             
//             runner.ContinueHeist("user1", "!C");
//         }
//         
//         [Test]
//         public void NothingTest()
//         {
//             _cph.SetVariable(HeistConfiguration.VAR_ConfigFile, "Heists\\advanced.json");
//             var manager = new HeistRunner(_cph);
//             var runner = new HeistRunner(_cph);
//             
//             _cph.SetUserVariable("user1", "points", 200);
//             _cph.SetUserVariable("user2", "points", 200);
//
//             runner.RunHeist();
//             manager.EnrolUser("user1", "100");
//             manager.EnrolUser("user2", "100");
//
//             Assert.AreEqual(100, _cph.GetUserVariable<int>("user1", "points"));
//             
//             Thread.Sleep(100);
//
//             runner.RunHeist();
//             
//             Thread.Sleep(100);
//
//             runner.RunHeist();
//         }
//     }
// }