namespace AppVecinos.API.Models
{
    public class Fee
    {
        public int Id { get; set; }
        public required string Concept { get; set; }
        public required string Year { get; set; }
        public required double Amount { get; set; }        
    }

}