using crud.Models;

namespace crud.Repositories
{
    public interface IExpenseRepository
    {
        IEnumerable<Expense> GetAllExpenses(int pageNumber,int pageSize);
        Task<IEnumerable<Expense>> GetFilteredExpensesAsync(string? Gender, string? City, string? Description, decimal? minValue, decimal? maxValue);
        Expense? GetExpenseById(int id);
        void AddExpense(Expense expense);
        Expense UpdateExpense(Expense expense);
        void DeleteExpense(int id);
        void Save();
        int GetTotal();
    }
}
