namespace Domain.Orders;

public class Order
{
	public int Id { get; set; }
	public int CustomerId { get; set; }
	public DateTime OrderDate { get; set; }
	public string Currency { get; set; } = string.Empty;
	public string Status { get; set; } = string.Empty;
}
