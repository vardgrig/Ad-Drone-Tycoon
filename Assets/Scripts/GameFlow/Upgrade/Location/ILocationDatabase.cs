namespace GameFlow.Upgrade.Location
{
    public interface ILocationDatabase
    {
        LocationData[] GetAllLocations();
        LocationData GetLocationById(string id);
    }
}