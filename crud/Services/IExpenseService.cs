using crud.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace crud.Services
{
    public interface IExpenseService
    {
        IEnumerable<Expense> GetAllExpenses( int pageNumber,int pageSize);
        Task<IEnumerable<Expense>> GetFilteredExpensesAsync(string? Gender, string? City, string? Description, decimal? minValue, decimal? maxValue);
        Expense? GetExpenseById(int id);
        void CreateExpense(Expense expense);
        Expense? UpdateExpense(Expense expense);
        void DeleteExpense(int id);
        decimal GetTotalExpenses();
        int GetTotal();
    }
}
