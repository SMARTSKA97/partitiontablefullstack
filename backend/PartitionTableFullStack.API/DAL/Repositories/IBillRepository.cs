using PartitionTableFullStack.API.Models;

namespace PartitionTableFullStack.API.DAL.Repositories;

public interface IBillRepository : IRepository<BillDetail>
{
    Task<BillDetail?> GetBillWithComponentsAsync(long billId);
    Task<List<BillDetail>> GetBillsByFinancialYearAsync(short financialYear);
}
