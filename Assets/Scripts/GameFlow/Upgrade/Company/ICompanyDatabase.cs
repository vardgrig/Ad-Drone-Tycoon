namespace GameFlow.Upgrade.Company
{
    public interface ICompanyDatabase
    {
        CompanyData[] GetAllCompanies();
        CompanyData GetCompanyById(string id);
    }
}