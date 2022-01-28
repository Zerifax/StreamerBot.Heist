using Zerifax.Heist;

namespace Zerifax
{
    public class PointManager : IPointManager {
        private IVariableProxy _variableProxy;
        
        public PointManager(IVariableProxy variableProxy)
        {
            _variableProxy = variableProxy;
        }

        
        public void AddPoints(string user, string pointsKey, int points)
        {
            var userPoints = _variableProxy.GetUserVariable<int>(user, pointsKey);
            _variableProxy.SetUserVariable(user, pointsKey, points + userPoints);
        }

        public int GetPoints(string user, string pointsKey)
        {
            return _variableProxy.GetUserVariable<int>(user, pointsKey);
        }
    }
}