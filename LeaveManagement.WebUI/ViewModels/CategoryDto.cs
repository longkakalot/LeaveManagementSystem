namespace LeaveManagement.WebUI.ViewModels
{
    public class CategoryDto
    {
    }
    public class Countries
    {
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }
    }
    public class Provinces
    {
        public int Id { get; set; }
        public string? ProvinceCode { get; set; }
        public string? ProvinceName { get; set; }
    }
    public class Wards
    {
        public int Id { get; set; }
        public string? WardCode { get; set; }
        public string? WardName { get; set; }
        public int ProvinceId { get; set; }
    }
}
