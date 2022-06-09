namespace AgendeApp.Models.DB
{
    public class SP_GetConsultantsProfitByDateRangeResult
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public double ReceitaLiquida { get; set; } = 0;
    }
}
