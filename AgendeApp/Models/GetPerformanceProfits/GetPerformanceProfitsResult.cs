using AgendeApp.Models.DB;
using System.Collections.Generic;

namespace AgendeApp.Models.GetPerformanceProfits
{
    public class GetPerformancePerConsultantProfitsResult
    {
        public string UserID { get; set; }

        public string Name { get; set; }

        public List<SP_GetConsultantsLucroResult> Profits { get; set; } = new List<SP_GetConsultantsLucroResult>();

        public double AverageFixedCost { get; set; }
    }

    public class GetConsultantProfitsResult
    {
        public double AverageFixedCost { get; set; } = 25000;

        public List<GetPerformancePerConsultantProfitsResult> Consultants { get; set; } = new List<GetPerformancePerConsultantProfitsResult>(); 
}
}
