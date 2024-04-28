namespace API.StoreShared
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string ClientId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
