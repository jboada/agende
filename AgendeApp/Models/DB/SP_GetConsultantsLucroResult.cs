namespace AgendeApp.Models.DB
{
    public class SP_GetConsultantsLucroResult
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public int OSYear { get; set; }

        public int OSMonth { get; set; }

        public string OSMonthName { get; set; }

        public string Periodo { get; set; }

        public double VALOR { get; set; } = 0;

        public double TotalTaxes { get; set; } = 0;

        public double ReceitaLiquida { get; set; } = 0;

        public double ConsultantComission { get; set; } = 0;

        public double Brutsalario { get; set; } = 0;

        public double Lucro { get; set; } = 0;

    }
}
