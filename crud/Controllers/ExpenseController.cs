using Microsoft.AspNetCore.Mvc;
using crud.Models;
using crud.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace crud.Controllers
{
    [Route("api/v20/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        // Get all expenses
        [HttpGet]
        public ActionResult<IEnumerable<Expense>> GetAll(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize=2,
            [FromQuery] string sortOrder = "asc",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null
            )
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var expenses = _expenseService.GetAllExpenses(userId,pageNumber,pageSize,sortOrder,startDate,endDate);
            var totalCount = _expenseService.GetTotal(userId);

            return Ok(new
            {
                TotalCount=totalCount,
                PageNumber=pageNumber,
                PageSize=pageSize,
                Expenses=expenses
            });

        }
        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredProducts(
            [FromQuery] string? Description = null,
            [FromQuery] decimal? minValue = null,
            [FromQuery] decimal? maxValue = null)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var expenses = await _expenseService.GetFilteredExpensesAsync(userId, Description, minValue, maxValue);
            return Ok(expenses);
        }

        // Get an expense by ID
        [HttpGet("{id}")]
        public ActionResult<Expense> GetById(int id)
        {
            var expense = _expenseService.GetExpenseById(id);
            if (expense == null)
            {
                return NotFound(new { message = "Expense not found" });
            }
            return Ok(expense);
        }

        // Create a new expense
        [HttpPost]
        public ActionResult Create([FromBody] Expense expense)
        {
            if (expense == null)
            {
                return BadRequest(new { message = "Invalid data" });
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            expense.UserId = userId;
            _expenseService.CreateExpense(expense);
            return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
        }

        // Update an existing expense
        [HttpPatch("{id}")]
        public ActionResult Update(int id, [FromBody] Expense expense)
        {
            if (id != expense.Id)
            {
                return BadRequest(new { message = "Mismatched ID" });
            }

            try
            {
                var updatedExpense = _expenseService.UpdateExpense(expense);
                return Ok(updatedExpense);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // Delete an expense
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                _expenseService.DeleteExpense(id);
                return Ok(new { Message = "Deleted Sucessfully"});
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }

        }
    }
}
