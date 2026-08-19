namespace Domain.Orders;

public class OrderItem
{
	public int Id { get; set; }
	public int OrderId { get; set; }
	public string Description { get; set; } = string.Empty;
	public string Sku { get; set; } = string.Empty;
	public float UnitPrice { get; set; }
	public int Quantity { get; set; }
}
