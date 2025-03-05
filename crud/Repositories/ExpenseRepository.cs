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
        public IEnumerable<Expense> GetAllExpenses(int pageNumber, int pageSize)
        {

            return _context.Expenses
                .OrderBy(e=>e.Id)
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToList();
        }
        public async Task<IEnumerable<Expense>> GetFilteredExpensesAsync(string? Gender,string? City,string? Description,decimal? minValue,decimal? maxValue)
        {
            var query = _context.Expenses.AsQueryable();
            if (!string.IsNullOrEmpty(Gender))
            {
                query = query.Where(p => p.Gender == Gender);
            }
            if (!string.IsNullOrEmpty(City))
            {
                query = query.Where(p => p.City == City);
            }
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
                existingExpense.City = expense.City;
                existingExpense.Gender = expense.Gender;
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
        public int GetTotal()
        {
            return _context.Expenses.Count();
        }
    }
}
