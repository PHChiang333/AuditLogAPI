namespace AuditLogAPI.Model.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductPutDto
    {
        //public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        //public DateTime CreatedAt { get; set; }
    }

    public class ProductPostDto
    {
        //public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        //public DateTime CreatedAt { get; set; }
    }
}
