namespace Domain.Customers;

public class Customer
{
	public int Id { get; private set; }
	public string LegacyCustomerId { get; private set; } = string.Empty;
	public string FullName { get; private set; } = string.Empty;
	public string Email { get; private set; } = string.Empty;
	public DateTime SignupDate { get; private set; }
	public Tier Tier { get; private set; }

	private Customer() { }

	public Customer(string legacyCustomerId, string fullName, string email, DateTime signupDate, Tier tier)
	{
		if (string.IsNullOrWhiteSpace(legacyCustomerId))
			throw new ArgumentException("Legacy customer ID cannot be empty.", nameof(legacyCustomerId));

		LegacyCustomerId = legacyCustomerId;
		FullName = fullName;
		Email = email;
		SignupDate = signupDate;
		Tier = tier;
	}

	public void UpdateDetails(string fullName, string email, DateTime signupDate, Tier tier)
	{
		FullName = fullName;
		Email = email;
		SignupDate = signupDate;
		Tier = tier;
	}
}
