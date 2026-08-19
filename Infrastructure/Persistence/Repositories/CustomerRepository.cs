using Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CustomerRepository(AppDbContext dbContext) : ICustomerRepository
{
    public Task<Customer?> GetByLegacyIdAsync(string legacyCustomerId, CancellationToken cancellationToken = default)
    {
        return dbContext.Customers.FirstOrDefaultAsync(c => c.LegacyCustomerId == legacyCustomerId, cancellationToken);
    }

    public Task<Dictionary<string, Customer>> GetByLegacyIdsAsync(IEnumerable<string> legacyIds, CancellationToken cancellationToken = default)
    {
        return dbContext.Customers.Where(c => legacyIds.Contains(c.LegacyCustomerId)).ToDictionaryAsync(c => c.LegacyCustomerId, cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await dbContext.Customers.AddAsync(customer, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
