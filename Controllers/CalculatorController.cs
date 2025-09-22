using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOE_Calculator_Project.Data;
using SOE_Calculator_Project.Models;

namespace SOE_Calculator_Controller.Controllers
{

    // Collaborators: Kamohelo Phatsonae 224090026, Brandon Lombaard 223021599

    public class CalculatorController : Controller
    {
        // Brandon Lombaard 223021599
        // Reference to the DbContext, Session Keys, Constructor, Register Methods,
        // Login Methods, Logout Method, SetUserSession, GetSessionHistory, SaveSessionHistory,
        // GetCurrentUserId
        private readonly CalculatorDbContext _db;

        private const string SessionUserIdKey = "user_id";
        private const string SessionUsernameKey = "username";
        private const string SessionHistoryKey = "calc_history";

        public CalculatorController(CalculatorDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Verifies user input and check whether a username already exists in the database,
        // if not a new user is added to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register (string username, string password)
        {
            // Verifies that both fields are filled in
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Username and password are required");
                return View();
            }

            // Creates a bool that contains whether a username exists in the database.
            bool exists = await _db.Users.AnyAsync(u => u.Username == username);

            // Returns an error to the user if exists is true.
            if (exists)
            {
                ModelState.AddModelError(string.Empty, "This username already exists");
                return View();
            }

            // Adds user input into an object
            var user = new User
            {
                Username = username.Trim(),
                Password = password,
                CreatedAt = DateTime.Now,
            };

            // Adds user data to the database
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            SetUserSession(user.UserId, user.Username);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Verifies user input and returns an error if the login information is incorrect
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username,  string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Credentials");
                return View();
            }

            SetUserSession(user.UserId, user.Username);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove(SessionUserIdKey);
            HttpContext.Session.Remove(SessionUsernameKey);
            return RedirectToAction(nameof(Login));
        }

        // Helpers (SetUserSession, GetSessionHistory, SaveSessionHistory, GetCurrentUserId)
        private void SetUserSession(int userId, string username)
        {
            HttpContext.Session.SetInt32(SessionUserIdKey, userId);
            HttpContext.Session.SetString(SessionUserIdKey, username);
        }

        // The session history is stored as a JSON file
        private List<SavedCalculation> GetSessionHistory()
        {
            var json = HttpContext.Session.GetString(SessionHistoryKey);
            if (string.IsNullOrEmpty(json))
            {
                return new List<SavedCalculation>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<SavedCalculation>>(json) ?? new List<SavedCalculation>();
            }
            catch
            {
                return new List<SavedCalculation>();
            }
        }

        private void SaveSessionHistory(List<SavedCalculation> history)
        {
            var json = JsonSerializer.Serialize(history);
            HttpContext.Session.SetString(SessionHistoryKey, json);
        }

        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32(SessionUserIdKey);
        }

        // Kamohelo Phatsonae 224090026
        // Added the Initial Calculator Controller methods
        //------------------------------------------------------------------------------------
        // Brandon Lombaard 223021599
        // Added Functionality to Calculator Controller methods based on database connections:
        // Index, AddToSessionHistory, ShowHistory, ClearSessionHistory, Saved, Save, EditCalculation
        public IActionResult Index()  //This is the default view
        {
            ViewBag.Username = HttpContext.Session.GetString(SessionUsernameKey);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToSessionHistory(string expression, string result)
        {
            // Checks whether the expression and result are present
            if (string.IsNullOrWhiteSpace(expression) || string.IsNullOrWhiteSpace(result))
            {
                return BadRequest("Expression and Result are required");
            }

            var history = GetSessionHistory();
            history.Insert(0, new SavedCalculation
            {
                Expression = expression.Trim(),
                Result = result.Trim(),
                CreatedAt = DateTime.Now
            });

            // Keeps the session history a certain length (last 20 calculations)
            if (history.Count > 20)
            {
                history = history.Take(20).ToList();
            }

            SaveSessionHistory(history);
            return RedirectToAction(nameof(ShowHistory));
        }

        // This view displays the history of calculations done in the current session
        [HttpGet]
         public IActionResult ShowHistory() 
        {
            var history = GetSessionHistory();
            return View(history);
        }
        
        // Clears all calculations for the current session (Not saved calculations)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearSessionHistory()
        {
            HttpContext.Session.Remove(SessionHistoryKey);
            return RedirectToAction(nameof(ShowHistory));
        }

        // Displays all saved calculations
        [HttpGet]
        public async Task<IActionResult> Saved()
        {
            int? userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var list = await _db.SavedCalculations.Where(c => c.UserId == userId.Value).OrderByDescending(c => c.CreatedAt).ToListAsync();

            return View(list);
        }

        // Used to save a calculation directly into the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string expression, string result)
        {
            int? userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // This should redirect the user back the the calculator view
            if(!ModelState.IsValid || string.IsNullOrWhiteSpace(expression) || string.IsNullOrWhiteSpace(result))
            {
                TempData["Error"] = "Expression and Result are required.";
                return RedirectToAction(nameof(Index));
            }

            expression = expression.Trim();
            result = result.Trim();

            // Prevents duplicate calculations being stored for the same user
            bool duplicate = await _db.SavedCalculations.AnyAsync(c => c.UserId == userId.Value && c.Expression == expression && c.Result == result);

            // Redirects the user if they attempt to store the same calculation
            if (duplicate)
            {
                TempData["Info"] = "This calculation is already saved";
                return RedirectToAction(nameof(Saved));
            }

            var saved = new SavedCalculation
            {
                Expression = expression,
                Result = result,
                CreatedAt = DateTime.Now,
                UserId = userId.Value,
            };

            _db.SavedCalculations.Add(saved);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Calculation saved.";
            return RedirectToAction(nameof(Saved));
        }

        // Returns the selected calculation to the user for editing
        [HttpGet]
        public async Task<IActionResult> EditHistory(int id) 
        {
            int? userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var calc = await _db.SavedCalculations.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId.Value);

            if (calc == null)
            {
                return NotFound();
            }

           return View(calc);
        }
        
        // Alters the selected calculation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHistory(int id, string expression, string result) 
        {
            int? userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

          if(!ModelState.IsValid || string.IsNullOrWhiteSpace(expression) || string.IsNullOrWhiteSpace(result)) 
          {
                ModelState.AddModelError(string.Empty, "Expression and Result are Required.");

                // Reloads the current record to show the form again
                var existing = await _db.SavedCalculations.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId.Value);
                return View(existing);
          }
          
            var calc = await _db.SavedCalculations.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId.Value);

            if (calc == null)
            {
                return NotFound();
            }

            calc.Expression = expression.Trim();
            calc.Result = result.Trim();
            await _db.SaveChangesAsync();

            TempData["Success"] = "Saved calculation updated";
            return RedirectToAction(nameof(Saved));
        }

        // Returns the selected calculation for deletion
        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            int? userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var calc = await _db.SavedCalculations.FirstOrDefaultAsync(c => c.Id == Id && c.UserId == userId.Value);

            if (calc == null)
            {
                return NotFound();
            }

            return View(calc);
        }

        // Deletes the selected calculation from the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int? userId = GetCurrentUserId();

            if (userId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var calc = await _db.SavedCalculations.FirstOrDefaultAsync(c=> c.Id == id && c.UserId == userId.Value);

            if (calc == null)
            {
                TempData["Info"] = "That calculation was already deleted or not found.";
                return RedirectToAction(nameof(Saved));
            }

            _db.SavedCalculations.Remove(calc);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Saved calculation deleted";
            return RedirectToAction(nameof(Saved));
        }
    }
}
