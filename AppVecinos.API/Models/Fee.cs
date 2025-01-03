namespace AppVecinos.API.Models
{
    public class Fee
    {
        public int Id { get; set; }
        public string Concept { get; set; }
        public string Year { get; set; }
        public decimal Amount { get; set; }        
    }

}