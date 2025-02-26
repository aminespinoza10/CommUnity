namespace CommonData.Models
{
    public class Neighbor
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int Number { get; set; }
        public required string User { get; set; }
        public required string Password { get; set; }
        public required string Level { get; set; }
        public required string Status { get; set; }
    }
}