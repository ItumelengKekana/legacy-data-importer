namespace Domain.Customers;

public interface ICustomerRepository
{
    Task<Customer?> GetByLegacyIdAsync(string legacyCustomerId, CancellationToken cancellationToken = default);
    Task<Dictionary<string, Customer>> GetByLegacyIdsAsync(IEnumerable<string> legacyIds, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
