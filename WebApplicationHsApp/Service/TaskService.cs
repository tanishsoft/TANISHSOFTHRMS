using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Service
{
    public class TaskService
    {
        public async Task<(List<tbl_Task> Tasks, int TotalCount)> GetFilteredTasksAsync(JQueryDataTableParamModel param, string userId, string role)
        {
            string con = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            IDbConnection _db = new SqlConnection(con);
            var where = new StringBuilder("WHERE IsActive = 1");
            var parameters = new DynamicParameters();

            // Date range filter
            if (!string.IsNullOrEmpty(param.fromdate) && !string.IsNullOrEmpty(param.todate))
            {
                where.Append(" AND CAST(CallDateTime AS DATE) BETWEEN @FromDate AND @ToDate");
                parameters.Add("@FromDate", ProjectConvert.ConverDateStringtoDatetime(param.fromdate).Date);
                parameters.Add("@ToDate", ProjectConvert.ConverDateStringtoDatetime(param.todate).Date);
            }

            // Location filter
            if (param.locationid != null && param.locationid != 0)
            {
                where.Append(" AND CreatorLocationId = @LocationId");
                parameters.Add("@LocationId", param.locationid);
            }

            // Department filter
            if (param.departmentid != null && param.departmentid != 0)
            {
                where.Append(" AND CreatorDepartmentId = @DepartmentId");
                parameters.Add("@DepartmentId", param.departmentid);
            }

            // Employee filter
            if (Information.IsNumeric(param.Emp) && Convert.ToInt32(param.Emp) > 0)
            {
                where.Append(" AND AssignId = @AssignId");
                parameters.Add("@AssignId", Convert.ToInt32(param.Emp));
            }

            // Status filter
            if (!string.IsNullOrEmpty(param.status))
            {
                string status = param.status == "Pending" ? "In Progress" : param.status;
                where.Append(" AND AssignStatus = @Status");
                parameters.Add("@Status", status);
            }

            // Priority/FormType filter
            if (!string.IsNullOrEmpty(param.FormType))
            {
                where.Append(" AND Priority = @Priority");
                parameters.Add("@Priority", param.FormType);
            }

            // Expired filter
            if (!string.IsNullOrEmpty(param.Expired))
            {
                bool expired = param.Expired == "Yes";
                where.Append(" AND DocumentReceived = @DocumentReceived");
                parameters.Add("@DocumentReceived", expired);
            }

            // Category filter
            if (!string.IsNullOrEmpty(param.category))
            {
                where.Append(" AND CategoryOfComplaint = @Category");
                parameters.Add("@Category", param.category);
            }

            // Role-based access control
            string userSql = "SELECT TOP 1 * FROM tbl_User WHERE CustomUserId = @UserId";
            var dept = await _db.QueryFirstOrDefaultAsync<tbl_User>(userSql, new { UserId = userId });
            if (dept != null)
            {
                if (role == "LocationManager")
                {
                    where.Append(" AND (CreatorLocationId = @DeptLocationId OR AssignLocationId = @DeptLocationId OR AssignDepartmentName = @DeptDepartmentName)");
                    parameters.Add("@DeptLocationId", dept.LocationId);
                    parameters.Add("@DeptDepartmentName", dept.DepartmentName);
                }
                else if (role == "DepartmentManager" ||
                         new[] { "IT", "Information Technology", "Biomedical", "Maintenance", "Academics", "Purchase", "Finance & Accounts" }.Contains(dept.DepartmentName))
                {
                    switch (dept.DepartmentName)
                    {
                        case "IT":
                        case "Information Technology":
                            where.Append(" AND (CreatorId = @UserId OR AssignDepartmentName IN ('Information Technology'))");
                            break;
                        case "Biomedical":
                            where.Append(" AND (CreatorId = @UserId OR AssignDepartmentName = 'Biomedical')");
                            break;
                        case "Maintenance":
                            if (role == "DepartmentManager")
                                where.Append(" AND (CreatorId = @UserId OR AssignDepartmentName = 'Maintenance')");
                            else
                                where.Append(" AND AssignLocationId = @DeptLocationId AND (CreatorId = @UserId OR AssignDepartmentName = 'Maintenance')");
                            break;
                        case "Academics":
                            where.Append(" AND (CreatorId = @UserId OR AssignDepartmentName = 'Academics')");
                            break;
                        case "Purchase":
                            where.Append(" AND (CreatorId = @UserId OR AssignDepartmentName = 'Purchase')");
                            break;
                        case "Finance & Accounts":
                            where.Append(" AND (CreatorId = @UserId OR AssignDepartmentName = 'Finance & Accounts')");
                            break;
                        default:
                            where.Append(" AND (CreatorId = @UserId OR AssignDepartmentId = @DeptDepartmentId)");
                            parameters.Add("@DeptDepartmentId", dept.DepartmentId);
                            break;
                    }
                    parameters.Add("@UserId", dept.UserId);
                    parameters.Add("@DeptLocationId", dept.LocationId);
                }
                else if (role != "Admin")
                {
                    where.Append(" AND (CreatorId = @UserId OR AssignId = @UserId)");
                    parameters.Add("@UserId", dept.UserId);
                }
            }

            // Search keyword filter
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                string search = "%" + param.sSearch.ToLower() + "%";
                where.Append(@"
        AND (
            LOWER(CONVERT(VARCHAR, TaskId)) LIKE @Search OR
            LOWER(CONVERT(VARCHAR, CallDateTime, 120)) LIKE @Search OR
            LOWER(CreatorDepartmentName) LIKE @Search OR
            LOWER(CreatorName) LIKE @Search OR
            LOWER(CreatorLocationName) LIKE @Search OR
            LOWER(ExtensionNo) LIKE @Search OR
            LOWER(AssertEquipId) LIKE @Search OR
            LOWER(AssertEquipName) LIKE @Search OR
            LOWER(Description) LIKE @Search OR
            LOWER(AssignDepartmentName) LIKE @Search OR
            LOWER(AssignName) LIKE @Search OR
            LOWER(WorkDoneRemarks) LIKE @Search OR
            LOWER(CreatorStatus) LIKE @Search OR
            LOWER(CONVERT(VARCHAR, CallStartDateTime, 120)) LIKE @Search OR
            LOWER(CONVERT(VARCHAR, CallEndDateTime, 120)) LIKE @Search
        )");
                parameters.Add("@Search", search);
            }

            string countSql = $"SELECT COUNT(*) FROM tbl_Task {where}";

            string dataSql = $@"
        SELECT * FROM tbl_Task
        {where}
        ORDER BY ModifiedOn DESC
        OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY";

            parameters.Add("@Offset", param.iDisplayStart);
            parameters.Add("@Fetch", param.iDisplayLength);

            using (var multi = await _db.QueryMultipleAsync(countSql + ";" + dataSql, parameters))
            {
                int totalCount = multi.ReadFirst<int>();
                var tasks = multi.Read<tbl_Task>().ToList();
                return (tasks, totalCount);
            }
        }

        public async Task<List<tbl_TaskCallcenter>> GetFilteredTasksAsync(
      JQueryDataTableParamModel param,
      string userId,
      int dbUserId,
      int empId,
      int departmentId,
      bool isAdmin)
        {
            string con = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (IDbConnection _db = new SqlConnection(con))
            {
                var search = string.IsNullOrEmpty(param.sSearch) ? null : $"%{param.sSearch.ToLower()}%";

                var where = new StringBuilder("WHERE tc.IsActive = 1 AND tc.TaskType IS NOT NULL");
                var parameters = new DynamicParameters();
                bool isSearch = false;

                if (!isAdmin)
                {
                    where.Append(@"
        AND (
            tc.CreatorDepartmentId = @DepartmentId
            OR tc.AssignDepartmentId = @DepartmentId
            OR EXISTS (
                SELECT 1
                FROM tbl_TaskSLAUser sla
                WHERE sla.SLAUserId = @EmpId
                AND sla.TaskLocationId = tc.AssignLocationId
                AND sla.TaskDepartmentId = tc.AssignDepartmentId
            )
            OR tc.CreatorId = @DbUserId
        )");

                    parameters.Add("@EmpId", empId);
                    parameters.Add("@DbUserId", dbUserId);
                    parameters.Add("@DepartmentId", departmentId);
                }


                if (!string.IsNullOrEmpty(param.fromdate) && !string.IsNullOrEmpty(param.todate))
                {
                    isSearch = true;
                    where.Append(" AND CAST(tc.CallDateTime AS DATE) BETWEEN @FromDate AND @ToDate");
                    parameters.Add("@FromDate", ProjectConvert.ConverDateStringtoDatetime(param.fromdate).Date);
                    parameters.Add("@ToDate", ProjectConvert.ConverDateStringtoDatetime(param.todate).Date);
                }

                if (param.locationid != null && param.locationid != 0)
                {
                    isSearch = true;
                    where.Append(" AND tc.CreatorLocationId = @LocationId");
                    parameters.Add("@LocationId", param.locationid);
                }

                if (param.departmentid != null && param.departmentid != 0)
                {
                    isSearch = true;
                    where.Append(" AND tc.CreatorDepartmentId = @DepartmentId");
                    parameters.Add("@DepartmentId", param.departmentid);
                }

                if (!string.IsNullOrEmpty(param.Emp) && param.Emp != "0")
                {
                    isSearch = true;
                    where.Append(" AND tc.AssignId = @Emp");
                    parameters.Add("@Emp", int.Parse(param.Emp));
                }

                if (!string.IsNullOrEmpty(param.status))
                {
                    isSearch = true;
                    string status = param.status == "Pending" ? "In Progress" : param.status;
                    where.Append(" AND tc.AssignStatus = @Status");
                    parameters.Add("@Status", status);
                }

                if (!string.IsNullOrEmpty(param.FormType))
                {
                    isSearch = true;
                    where.Append(" AND tc.Priority = @FormType");
                    parameters.Add("@FormType", param.FormType);
                }

                if (!string.IsNullOrEmpty(param.Expired))
                {
                    isSearch = true;
                    where.Append(" AND tc.TaskType = @Expired");
                    parameters.Add("@Expired", param.Expired);
                }

                if (!string.IsNullOrEmpty(param.category))
                {
                    isSearch = true;
                    where.Append(" AND tc.CategoryOfComplaint = @Category");
                    parameters.Add("@Category", param.category);
                }

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    where.Append(@"
                AND (
                    LOWER(CONVERT(VARCHAR, tc.TaskId)) LIKE @Search OR
                    LOWER(CONVERT(VARCHAR, tc.CallDateTime, 120)) LIKE @Search OR
                    LOWER(tc.CreatorDepartmentName) LIKE @Search OR
                    LOWER(tc.CreatorName) LIKE @Search OR
                    LOWER(tc.CreatorLocationName) LIKE @Search OR
                    LOWER(tc.ExtensionNo) LIKE @Search OR
                    LOWER(tc.AssertEquipId) LIKE @Search OR
                    LOWER(tc.AssertEquipName) LIKE @Search OR
                    LOWER(tc.Description) LIKE @Search OR
                    LOWER(tc.AssignDepartmentName) LIKE @Search OR
                    LOWER(tc.AssignName) LIKE @Search OR
                    LOWER(tc.WorkDoneRemarks) LIKE @Search OR
                    LOWER(tc.CreatorStatus) LIKE @Search OR
                    LOWER(CONVERT(VARCHAR, tc.CallStartDateTime, 120)) LIKE @Search OR
                    LOWER(CONVERT(VARCHAR, tc.CallEndDateTime, 120)) LIKE @Search
                )");
                    parameters.Add("@Search", search);
                }

                if (!isSearch)
                {
                    where.Append(" AND LOWER(ISNULL(tc.AssignStatus, '')) <> 'done'");
                }

                // Paging
                int offset = param.iDisplayStart;
                int fetch = param.iDisplayLength;
                parameters.Add("@Offset", offset);
                parameters.Add("@Fetch", fetch);

                // SQL Queries
                string countSql = $"SELECT COUNT(*) FROM tbl_TaskCallcenter tc {where};";
                string dataSql = $@"
            SELECT *
            FROM tbl_TaskCallcenter tc
            {where}
            ORDER BY tc.ModifiedOn DESC
            OFFSET @Offset ROWS
            FETCH NEXT @Fetch ROWS ONLY;";

                using (var multi = await _db.QueryMultipleAsync(countSql + dataSql, parameters))
                {
                    int totalCount = multi.ReadFirst<int>();
                    var pagedTasks = multi.Read<tbl_TaskCallcenter>().ToList();
                    param.TotalCount = totalCount;
                    return pagedTasks;
                }
            }
        }

    }
}
