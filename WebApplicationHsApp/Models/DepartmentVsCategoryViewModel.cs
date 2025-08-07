namespace WebApplicationHsApp.Models
{
    public class DepartmentVsCategoryViewModel
    {
       public int Id { get; set; }
        public int CategoryId { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public bool IsActive { get; set; }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
        public string Name { get; set; }
    }
}