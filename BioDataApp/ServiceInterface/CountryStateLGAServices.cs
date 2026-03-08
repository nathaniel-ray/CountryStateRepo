using BioDataApp.Data;
using BioDataApp.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace BioDataApp.ServiceInterface
{
    public interface ICountryStateLGAServices
    {
        Task<List<CountryTab>> GetAllCountries();
        Task<List<StateTab>> GetStatesByCountry();
        Task<List<StateTab>> GetStatesByCountryId(int countryId);
        Task<List<LGATab>> GetLGAsByStateId(int stateId);
    }
    public class CountryStateLGAServices : ICountryStateLGAServices
    {
        private readonly BioDataDbContext context;

        public CountryStateLGAServices(BioDataDbContext context)
        {
            this.context = context;
        }
        public async Task<List<CountryTab>> GetAllCountries()
        {
            return await context.CountryTab.AsNoTracking().ToListAsync();
        }
        public async Task<List<StateTab>> GetStatesByCountry()
        {
            return await context.StateTab.ToListAsync();
        }
        public async Task<List<StateTab>> GetStatesByCountryId(int countryId)
        {
            return await context.StateTab.Where(s => s.CountryTabId == countryId).ToListAsync();
        }
        public async Task<List<LGATab>> GetLGAsByStateId(int stateId)
        {
            return await context.LGATab.Where(l => l.StateTabId == stateId).ToListAsync();
        }
    }
}
