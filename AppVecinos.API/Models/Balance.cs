
namespace AppVecinos.API.Models
{
    public class Balance
    {
        public int Id { get; set; }
        public required string Period { get; set; }
        public required int Year { get; set; }
        public required double Incomes { get; set; }
        public required double Outcomes { get; set; }
        public required double Remaining { get; set; }
        
    }

}