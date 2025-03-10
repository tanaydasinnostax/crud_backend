using crud.Models;
using crud.Repositories;

namespace crud.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }
        public IEnumerable<Expense> GetAllExpenses(int userId, int pageNumber,int pageSize)
        {
            return _expenseRepository.GetAllExpenses(userId,pageNumber,pageSize);
        }
        public async Task<IEnumerable<Expense>> GetFilteredExpensesAsync(int userId, string? Description,decimal? minValue,decimal? maxValue)
        {
            return await _expenseRepository.GetFilteredExpensesAsync(userId, Description, minValue, maxValue);
        }
        public Expense? GetExpenseById(int id)
        {
            return _expenseRepository.GetExpenseById(id);
        }
        public void CreateExpense(Expense expense)
        {
            _expenseRepository.AddExpense(expense);
            _expenseRepository.Save();
        }
        public Expense UpdateExpense(Expense expense)
        {
            var existingExpense = _expenseRepository.GetExpenseById(expense.Id);
            if(existingExpense == null)
            {
                throw new ArgumentException("Exception Not Found");
            }
            _expenseRepository.UpdateExpense(expense);
            _expenseRepository.Save();
            return existingExpense;
        }
        public void DeleteExpense(int id)
        {
            _expenseRepository.DeleteExpense(id);
            _expenseRepository.Save();
        }
        public decimal GetTotalExpenses(int userId)
        {
            return _expenseRepository.GetAllExpenses(userId,1,int.MaxValue).Sum(x => x.Value);
        }
        public int GetTotal(int userId)
        {
            return _expenseRepository.GetTotal(userId);
        }
    }
}
