using crud.Models;
using Microsoft.EntityFrameworkCore;

namespace crud.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly SpendSmartDbContext _context;
        public ExpenseRepository(SpendSmartDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public IEnumerable<Expense> GetAllExpenses(int userId, int pageNumber, int pageSize)
        {

            return _context.Expenses
                .Where(e=> e.UserId == userId)
                .OrderBy(e=>e.Id)
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToList();
        }
        public async Task<IEnumerable<Expense>> GetFilteredExpensesAsync(int userId,string? Description,decimal? minValue,decimal? maxValue)
        {
            var query = _context.Expenses.Where(e=> e.UserId == userId);
            if (!string.IsNullOrEmpty(Description))
            {
                query = query.Where(p => p.Description == Description);
            }
            if (minValue.HasValue)
            {
                query = query.Where(e => e.Value >= minValue.Value);
            }

            if (maxValue.HasValue)
            {
                query = query.Where(e => e.Value <= maxValue.Value);
            }
            return await query.ToListAsync();

        }
        public Expense? GetExpenseById(int id)
        {
            return _context.Expenses.SingleOrDefault(ex => ex.Id == id);


        }
        public void AddExpense(Expense expense)
        {
            _context.Expenses.Add(expense);
        }
        public Expense UpdateExpense(Expense expense)
        {
            var existingExpense = _context.Expenses.SingleOrDefault(e => e.Id == expense.Id);
            if(existingExpense != null)
            {
                existingExpense.Value = expense.Value;
                existingExpense.Description = expense.Description;
            }
            return existingExpense;
        }
        public void DeleteExpense(int id)
        {
            var expense = _context.Expenses.SingleOrDefault(e => e.Id == id);
            if(expense != null)
            {
                _context.Expenses.Remove(expense);
            }
        }
        public void Save()
        {
            _context.SaveChanges();
        }
        public int GetTotal(int userId)
        {
            return _context.Expenses.Count(e => e.UserId == userId);
        }
    }
}
