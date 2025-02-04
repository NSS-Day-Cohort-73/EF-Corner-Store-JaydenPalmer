namespace CornerStore.Models;

public class Order
{
    public int Id { get; set; }
    public int CashierId { get; set; }
    public Cashier Cashier { get; set; }
    public List<OrderProduct> OrderProducts { get; set; }
    public decimal Total
    {
        //(decimal) for the explicit cast cus we're doing an int times a decimal
        get { return (decimal)(OrderProducts?.Sum(op => op.Product.Price * op.Quantity)); }
    }
    public DateTime? PaidOnDate { get; set; }
}
