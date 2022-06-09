using AgendeApp.Models.DB;
using AgendeApp.Models.shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgendeApp.Services.DB.Performance
{
    public interface IPerformanceRepository
    {

        //public List<Consultants> GetGetConsultantsList();

        public Task<BaseResult> GetGetConsultantsListAsync();

        public Task<BaseResult> GetConsultantInfoAsync(string UserID);

        public Task<BaseResult> GetConsultantsLucroAsync(string[] consultants, int initYear, int initMonth, int EndYear, int EndMonth);

        public Task<BaseResult> GetConsultantsProfitsByRangeAsync(string[] consultants, int initYear, int initMonth, int EndYear, int EndMonth);

        public Task<BaseResult> GetConsultantsProfitsAsync(string[] consultants, int initYear, int initMonth, int EndYear, int EndMonth);

        public Task<BaseResult> GetConsultantsFixedCostsByRangeAsync(string[] consultants, int initYear, int initMonth, int EndYear, int EndMonth);
    }
}
