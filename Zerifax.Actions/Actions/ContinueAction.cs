using Zerifax.Actions;


namespace Zerifax.Actions.Continue
{
    using Zerifax;
    using Zerifax.Heist;
    using Zerifax.Proxies;

    public partial class CPHInline
    {
        private HeistRunner _heistRunner = null;

        public HeistRunner Runner 
        { 
            get 
            {
                if (_heistRunner == null) {
                    var variableProxy = new VariableProxy(CPH);
                    _heistRunner = new HeistRunner(variableProxy, new PointManager(variableProxy));
                }

                return _heistRunner;
            }
        }

        public void Dispose()
        {
            _heistRunner = null;
        }

        public bool Execute()
        {

            Runner.ContinueHeist(args["user"].ToString(), args["rawInput"].ToString());
            return true;
        }
    }
}