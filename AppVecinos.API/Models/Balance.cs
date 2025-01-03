
namespace AppVecinos.API.Models
{
    public class Balance
    {
        public int Id { get; set; }
        public string Period { get; set; }
        public int Year { get; set; }
        public decimal Incomes { get; set; }
        public decimal Outcomes { get; set; }
        public decimal Remaining { get; set; }
        
    }

}