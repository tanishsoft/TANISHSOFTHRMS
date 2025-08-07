namespace WebApplicationHsApp.Models
{
    public class JQueryDataTableParamModel
    {
        /// <summary>
        /// Request sequence number sent by DataTable, same value must be returned in response
        /// </summary>       
        public string sEcho { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }

        public string fromdate { get; set; }
        public string todate { get; set; }
        public string Dotor { get; set; }
        public string Department { get; set; }
        public string Emp { get; set; }
        public string status { get; set; }
        public string floor { get; set; }
        public string room { get; set; }
        public string alldaybooking { get; set; }
        public string accessrights { get; set; }
        public string typeofitem { get; set; }
        public int locationid { get; set; }
        public int departmentid { get; set; }
        public int subdepartmentid { get; set; }
        public string equipment { get; set; }
        public int PmDue { get; set; }
        public int Year { get; set; }
        public int LeaveTypeid { get; set; }
        public string locationname { get; set; }
        public string departmentname { get; set; }
        public string assignname { get; set; }
        public string category { get; set; }
        public string SubCategory { get; set; }
        public int categoryid { get; set; }
        public int SubCategoryid { get; set; }
        public string renewal { get; set; }
        public string sfromdate { get; set; }
        public string stodate { get; set; }
        public string efromdate { get; set; }
        public string etodate { get; set; }
        public string rfromdate { get; set; }
        public string rtodate { get; set; }
        public string Expired { get; set; }
        public string FormType { get; set; }
        public int? MealType { get; set; }
        public string ModeOfPayment { get; set; }
        public int Canteen { get; set; }
        public int OrderRequestId { get; set; }
        public int AssetTypeId { get; set; }
        public int Brand { get; set; }
        public int Model { get; set; }
        public int TotalCount { get; set; }
    }
}