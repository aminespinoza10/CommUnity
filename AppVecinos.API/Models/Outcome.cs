namespace AppVecinos.API.Models
{
    public class Outcome
    {
        public int Id { get; set; }
        public required string Month { get; set; }
        public required int Year { get; set; }
        public required string Concept { get; set; }
        public required double Amount { get; set; }
        
    }
}