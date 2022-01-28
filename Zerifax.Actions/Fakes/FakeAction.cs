using System.Collections.Generic;
using Zerifax.Heist;

namespace Zerifax.Actions
{
    public abstract class FakeAction
    {
        protected Plugins.InlineInvokeProxy CPH { get; }
        protected Dictionary<string,object> args { get; }
    }
}