namespace LeaveManagement.WebUI.ViewModels
{
    public class LeaveDistributionViewModel
    {
        public int TotalDays { get; set; }
        public int UsedCarryOver { get; set; }
        public int UsedCurrentYear { get; set; }
        public int YearCarryOver { get; set; }
        public int YearCurrent { get; set; }
        public List<LeaveDistributionDetail>? Details { get; set; }
    }

    public class LeaveDistributionDetail
    {
        public int Year { get; set; }
        public int Days { get; set; }
    }

}
