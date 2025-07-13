namespace LeaveManagement.WebUI.ViewModels
{
    public class LeaveTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int MaxDays { get; set; }
    }
}
