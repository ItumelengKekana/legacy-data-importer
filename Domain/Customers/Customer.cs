namespace Domain.Customers;

public class Customer
{
    public int Id { get; set; }
    public string LegacyCustomerId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly SignupDate { get; set; }
    public Tier Tier { get; set; }

    public Customer() { }

    public Customer(string legacyCustomerId, string fullName, string email, DateOnly signupDate, Tier tier)
    {
        if (string.IsNullOrWhiteSpace(legacyCustomerId))
            throw new ArgumentException("Legacy customer ID cannot be empty.", nameof(legacyCustomerId));

        LegacyCustomerId = legacyCustomerId;
        FullName = fullName;
        Email = email;
        SignupDate = signupDate;
        Tier = tier;
    }

    public void UpdateDetails(string fullName, string email, DateOnly signupDate, Tier tier)
    {
        FullName = fullName;
        Email = email;
        SignupDate = signupDate;
        Tier = tier;
    }
}
