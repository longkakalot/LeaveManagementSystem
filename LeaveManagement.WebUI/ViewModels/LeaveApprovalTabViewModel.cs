namespace LeaveManagement.WebUI.ViewModels
{
    public class LeaveApprovalTabViewModel
    {
        public List<LeaveApprovalRequestDto> Pending { get; set; } = new();
        public int PendingTotal { get; set; }
        public List<LeaveApprovalRequestDto> Approved { get; set; } = new();
        public int ApprovedTotal { get; set; }
        public List<LeaveApprovalRequestDto> Rejected { get; set; } = new();
        public int RejectedTotal { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? CurrentTab { get; set; }
    }
}
