using crud.Models;

namespace crud.Services
{
    public interface IExpenseService
    {
        IEnumerable<Expense> GetAllExpenses(int pageNumber,int pageSize);
        Expense? GetExpenseById(int id);
        void CreateExpense(Expense expense);
        Expense? UpdateExpense(Expense expense);
        void DeleteExpense(int id);
        decimal GetTotalExpenses();
        int GetTotal();
    }
}
