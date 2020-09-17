using System;
using System.Threading.Tasks;

namespace SpravkiFirstDraft.Services.Regions
{
    public interface IRegionsService
    {
        Task<string> UploadRegion(string regionName);

        Task<bool> CheckRegionByName(string regionName);

        Task<int> IdByName(string regionName);
    }
}
