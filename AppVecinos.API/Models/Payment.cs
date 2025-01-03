namespace AppVecinos.API.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int NeighborId { get; set; }
        public int FeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        
    }

}