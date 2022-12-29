namespace CompanyEmployees.Client.Services
{
    public interface ICompanyHttpClient
    {
        Task<HttpClient> GetClient();
    }
}
