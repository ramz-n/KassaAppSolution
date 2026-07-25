namespace Kassa.Domain.Entities
{
    public class Cashier
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string PinCode { get; set; }
        public bool IsManager { get; set; }
        public bool IsActive { get; set; }
    }
}
