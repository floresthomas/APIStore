namespace API.StoreShared
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal Price { get; set; }
        public int ProductCategory { get ; set; }
        public List<OrderDetail>? OrderDetail { get; set; }
    }
}
