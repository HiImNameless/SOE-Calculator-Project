namespace SOE_Calculator_Project.Models
{
    public class CalculatorPageVM
    {
        // Brandon Lombaard 223021599
        // Sidebar lists
        public string? Username {  get; set; }
        public List<SavedCalculation> SessionHistory { get; set; } = new();
        public List<SavedCalculation> Saved { get; set; } = new();
    }
}
