using UnityEngine;

namespace GameFlow.Upgrade.Company
{
    public class CompanyDatabase : ICompanyDatabase
    {
        private CompanyData[] _allCompanies;
    
        public CompanyDatabase()
        {
            LoadCompanies();
        }
    
        private void LoadCompanies()
        {
            _allCompanies = Resources.LoadAll<CompanyData>("Companies");
        }
    
        public CompanyData[] GetAllCompanies() => _allCompanies;
    
        public CompanyData GetCompanyById(string id)
        {
            return System.Array.Find(_allCompanies, c => c.UniqueId == id);
        }
    }
}