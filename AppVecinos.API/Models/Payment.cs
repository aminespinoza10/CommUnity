namespace AppVecinos.API.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public required int NeighborId { get; set; }
        public required int FeeId { get; set; }
        public required DateTime Date { get; set; }
        
    }

}