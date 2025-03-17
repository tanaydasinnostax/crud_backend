using crud.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace crud.Services
{
    public interface IExpenseService
    {
        IEnumerable<Expense> GetAllExpenses( int userId,int pageNumber,int pageSize,string sortOrder,DateTime? startDate,DateTime? endDate);
        Task<IEnumerable<Expense>> GetFilteredExpensesAsync(int userId, string? Description, decimal? minValue, decimal? maxValue);
        Expense? GetExpenseById(int id);
        void CreateExpense(Expense expense);
        Expense? UpdateExpense(Expense expense);
        void DeleteExpense(int id);
        decimal GetTotalExpenses(int userId,DateTime? startDate,DateTime? endDate);
        int GetTotal(int userId);
    }
}
