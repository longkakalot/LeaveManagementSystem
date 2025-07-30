namespace LeaveManagement.WebUI.ViewModels
{
    public class UserLeaveBalancesViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Year { get; set; }
        public double LeaveDaysGranted { get; set; }
        public double LeaveDaysTaken { get; set; }
        public double LeaveDaysRemain { get; set; }
        public double DaysToDeduct { get; set; }
        public double DaysToReturn { get; set; }
    }
}
