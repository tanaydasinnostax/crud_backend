using crud.Models;

namespace crud.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly SpendSmartDbContext _context;
        public ExpenseRepository(SpendSmartDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public IEnumerable<Expense> GetAllExpenses(int pageNumber,int pageSize)
        {
            return _context.Expenses
                .OrderBy(e=>e.Id)
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToList();
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
        public int GetTotal()
        {
            return _context.Expenses.Count();
        }
    }
}
