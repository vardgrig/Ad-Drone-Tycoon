using UnityEngine;

namespace GameFlow.Upgrade.Location
{
    public class LocationDatabase : ILocationDatabase
    {
        private LocationData[] _allLocations;
    
        public LocationDatabase()
        {
            LoadLocations();
        }
    
        private void LoadLocations()
        {
            _allLocations = Resources.LoadAll<LocationData>("Locations");
        }
    
        public LocationData[] GetAllLocations() => _allLocations;
    
        public LocationData GetLocationById(string id)
        {
            return System.Array.Find(_allLocations, l => l.UniqueId == id);
        }
    }
}