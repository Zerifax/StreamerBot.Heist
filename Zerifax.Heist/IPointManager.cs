namespace Zerifax.Heist
{
    public interface IPointManager
    {
        void AddPoints(string user, string pointKey, int points);
        int GetPoints(string user, string pointsKey);
    }
}