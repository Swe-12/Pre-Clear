using System.Threading.Tasks;
using PreClear.Api.Models;

namespace PreClear.Api.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummary> GetSummaryAsync();
    }
}
