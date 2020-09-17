namespace SpravkiFirstDraft.Services.Cities
{
    using System.Threading.Tasks;

    public interface ICitiesService
    {
        Task<string> UploadCity(string city);

        Task<bool> CheckCityName(string companyName);

        Task<int> IdByName(string companyName);
    }
}
