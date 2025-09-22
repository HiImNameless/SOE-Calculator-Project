using Microsoft.AspNetCore.Mvc;

namespace SOE_Calculator_Controller.Controllers
{
    public class CalculatorController : Controller
    {
        public IActionResult Index()  //This is the default view
        {
            return View();
        }

        [HttpGet]
        public IActionResult Save() // This view saves the users calculations to the repo
        {
           return View();
        }

        [HttpPost]
        public IActionResult Save()
        {
          if(ModelState.IsValid)  //Checks to see if the same calculation is already saved (Prevents duplicate saved calulations)
          { 
          
          }
          else
          {
            return View();
          }
        }
         public IActionResult ShowHistory() // This view displays the history of saved calculations
        {
           return View();
        }

         [HttpGet]  // The GET Delete() retrieves the chosen saved calculations to be deleted
         public IActionResult Delete()
         {
         
            return View()
         }


         [HttpPost]
         [ValidateAntiForgeryToken]
         public IActionResult Delete() //The POST Delete() deletes saved calculations from the repo
        {
        
        if(ModelState.IsValid)  //Validates whether the chosen calculation is already deleted or not
        {
           
        }        
           return View();
        }

        [HttpGet]
        public IActionResult EditHistory()  //This GET displays chosen saved calculations
        {
        
           return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditHistory()  //This view edits saved calculations
        {
          if(ModelState.IsValid)  //This statement will notify the user of the updated change in the repo
          {
            
          }
          else
          {
            return View();
          }
        }

        
    }
}
