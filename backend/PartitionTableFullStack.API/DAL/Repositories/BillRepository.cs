using Microsoft.EntityFrameworkCore;
using PartitionTableFullStack.API.Data;
using PartitionTableFullStack.API.Models;

namespace PartitionTableFullStack.API.DAL.Repositories;

public class BillRepository : Repository<BillDetail>, IBillRepository
{
    public BillRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<BillDetail?> GetBillWithComponentsAsync(long billId)
    {
        return await _dbSet
            .Include(b => b.BtDetails)
            .Include(b => b.GstDetails)
            .Include(b => b.EcsDetails)
            .Include(b => b.AllotmentDetails)
            .Include(b => b.Subvouchers)
            .FirstOrDefaultAsync(b => b.BillId == billId && !b.IsDeleted);
    }

    public async Task<List<BillDetail>> GetBillsByFinancialYearAsync(short financialYear)
    {
        return await _dbSet
            .Where(b => b.FinancialYear == financialYear && !b.IsDeleted)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }
}
