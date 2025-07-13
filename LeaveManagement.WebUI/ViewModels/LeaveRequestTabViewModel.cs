namespace LeaveManagement.WebUI.ViewModels
{
    public class LeaveRequestTabViewModel
    {
        public List<LeaveRequestDto> Drafts { get; set; } = new();
        public int DraftTotal { get; set; }
        public List<LeaveRequestDto> Submitted { get; set; } = new();
        public int SubmittedTotal { get; set; }
        public List<LeaveRequestDto> Approved { get; set; } = new();
        public int ApprovedTotal { get; set; }
        public List<LeaveRequestDto> Rejected { get; set; } = new();
        public int RejectedTotal { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? CurrentTab { get; set; } // Để biết đang ở tab nào, dùng cho partial hoặc url
    }

}

