namespace SOE_Calculator_Project.Models
{
    public class SavedCalculation
    {   
        // Brandon Lombaard 223021599 
        // Columns for the SavedCalculation table
        public int Id { get; set; }             //Primary Key
        public string Expression { get; set; }
        public string Result { get; set; } 

        // Brandon Lombaard 223021599
        // Foreign Key and Navigation properties

        public int UserId { get; set; }     // Foreign Key Column
        public User User { get; set; }      // Navigation Property
    }
}
