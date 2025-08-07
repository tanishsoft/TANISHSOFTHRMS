using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    [RoutePrefix("api/Policy")]
    public class PolicyController : ApiController
    {
        [HttpGet]
        [Route("GetAllPolicies")]
        public List<string> GetAllPolicies()
        {
            List<string> list = new List<string>();
            list.Add("Leave Policy");
            list.Add("Grievance Policy");
            list.Add("Recruitment Policy");
            list.Add("Sexual Harassment Redressal Policy");
            list.Add("Employee Hand Book");
            list.Add("Training Policy");
            list.Add("Induction Policy");
            list.Add("Disciplinary Policy");
            list.Add("Annual Health Checkup");
            list.Add("Performance Appraisal");
            list.Add("Referral Bonus Policy");
            list.Add("Recognition and Reward Policy");                      
            return list;
        }
        [HttpGet]
        [Route("GetPolicyContent")]
        public List<string> GetPolicyContent(string name)
        {
            List<string> list = new List<string>();
            switch(name)
            {
                case "Leave Policy":
                    list.Add("/Documents/Images/Leave_policy_ver_2/Leave_policy_ver_2-01.jpg");
                    list.Add("/Documents/Images/Leave_policy_ver_2/Leave_policy_ver_2-02.jpg");
                    list.Add("/Documents/Images/Leave_policy_ver_2/Leave_policy_ver_2-03.jpg");
                    list.Add("/Documents/Images/Leave_policy_ver_2/Leave_policy_ver_2-04.jpg");
                    list.Add("/Documents/Images/Leave_policy_ver_2/Leave_policy_ver_2-05.jpg");
                    list.Add("/Documents/Images/Leave_policy_ver_2/Leave_policy_ver_2-06.jpg");
                    list.Add("/Documents/Images/Leave_policy_ver_2/Leave_policy_ver_2-07.jpg");
                    list.Add("/Documents/Images/Leave_policy_ver_2/Leave_policy_ver_2-08.jpg");
                    list.Add("/Documents/Images/Leave_policy_ver_2/Leave_policy_ver_2-09.jpg");
                    list.Add("/Documents/Images/Leave_policy_ver_2/Leave_policy_ver_2-10.jpg");
                    list.Add("/Documents/Images/Leave_policy_ver_2/Leave_policy_ver_2-11.jpg");
                    list.Add("/Documents/Images/Leave_policy_ver_2/Leave_policy_ver_2-12.jpg");
                    list.Add("/Documents/Images/Leave_policy_ver_2/Leave_policy_ver_2-13.jpg");
                    break;
                case "Grievance Policy":
                    list.Add("/Documents/Images/Grievance_Policy_2.0/Grievance_Policy_2.0-1.jpg");
                    list.Add("/Documents/Images/Grievance_Policy_2.0/Grievance_Policy_2.0-2.jpg");
                    list.Add("/Documents/Images/Grievance_Policy_2.0/Grievance_Policy_2.0-3.jpg");
                    list.Add("/Documents/Images/Grievance_Policy_2.0/Grievance_Policy_2.0-4.jpg");
                    list.Add("/Documents/Images/Grievance_Policy_2.0/Grievance_Policy_2.0-5.jpg");
                    list.Add("/Documents/Images/Grievance_Policy_2.0/Grievance_Policy_2.0-6.jpg");
                    list.Add("/Documents/Images/Grievance_Policy_2.0/Grievance_Policy_2.0-7.jpg");
                    break;
                case "Recruitment Policy":
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-01.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-02.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-03.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-04.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-05.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-06.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-07.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-08.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-09.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-10.jpg");

                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-11.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-12.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-13.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-14.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-15.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-16.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-17.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-18.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-19.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-20.jpg");

                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-21.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-22.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-23.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-24.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-25.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-26.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-27.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-28.jpg");
                    list.Add("/Documents/Images/Recruitment_Policy_ver2/Recruitment_Policy_ver2-29.jpg");
                    break;
                case "Sexual Harassment Redressal Policy":
                    list.Add("/Documents/Images/Sexual_Harassment_Redressal_Policy/1.jpg");
                    list.Add("/Documents/Images/Sexual_Harassment_Redressal_Policy/2.jpg");
                    list.Add("/Documents/Images/Sexual_Harassment_Redressal_Policy/3.jpg");
                    list.Add("/Documents/Images/Sexual_Harassment_Redressal_Policy/4.jpg");
                    list.Add("/Documents/Images/Sexual_Harassment_Redressal_Policy/5.jpg");
                    list.Add("/Documents/Images/Sexual_Harassment_Redressal_Policy/6.jpg");
                    list.Add("/Documents/Images/Sexual_Harassment_Redressal_Policy/7.jpg");
                    break;
                case "Employee Hand Book":
                    list.Add("/Documents/Images/Employee_Hand_Book/Employee_Hand_Book-01.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-02.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-03.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-04.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-05.jpg");
                    list.Add("/Documents/Images/Employee_Hand_Book/Employee_Hand_Book-06.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-07.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-08.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-09.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-10.jpg");
                    list.Add("/Documents/Images/Employee_Hand_Book/Employee_Hand_Book-11.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-12.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-13.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-14.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-15.jpg");
                    list.Add("/Documents/Images/Employee_Hand_Book/Employee_Hand_Book-16.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-17.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-18.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-19.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-20.jpg");
                    list.Add("/Documents/Images/Employee_Hand_Book/Employee_Hand_Book-21.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-22.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-23.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-24.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-25.jpg");
                    list.Add("/Documents/Images/Employee_Hand_Book/Employee_Hand_Book-26.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-27.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-28.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-29.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-30.jpg");
                    list.Add("/Documents/Images/Employee_Hand_Book/Employee_Hand_Book-31.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-32.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-33.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-34.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-35.jpg");
                    list.Add("/Documents/Images/Employee_Hand_Book/Employee_Hand_Book-36.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-37.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-38.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-39.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-40.jpg");
                    list.Add("/Documents/Images/Employee_Hand_Book/Employee_Hand_Book-41.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-42.jpg");
                    list.Add("/Documents/images/employee_hand_book/employee_hand_book-43.jpg");
                    //list.Add("/Documents/images/employee_hand_book/employee_hand_book-44.jpg");
                    //list.Add("/Documents/images/employee_hand_book/employee_hand_book-45.jpg");
                    //list.Add("/Documents/Images/Employee_Hand_Book/Employee_Hand_Book-46.jpg");
                    //list.Add("/Documents/images/employee_hand_book/employee_hand_book-47.jpg");
                    //list.Add("/Documents/Images/Employee_Hand_Book/Employee_Hand_Book-48.jpg");
                    break;
                case "Training Policy":
                    list.Add("/Documents/Images/Training_Policy_2.0/Training_Policy_2.0-01.jpg");
                    list.Add("/Documents/Images/Training_Policy_2.0/Training_Policy_2.0-02.jpg");
                    list.Add("/Documents/Images/Training_Policy_2.0/Training_Policy_2.0-03.jpg");
                    list.Add("/Documents/Images/Training_Policy_2.0/Training_Policy_2.0-04.jpg");
                    list.Add("/Documents/Images/Training_Policy_2.0/Training_Policy_2.0-05.jpg");
                    list.Add("/Documents/Images/Training_Policy_2.0/Training_Policy_2.0-06.jpg");
                    list.Add("/Documents/Images/Training_Policy_2.0/Training_Policy_2.0-07.jpg");
                    list.Add("/Documents/Images/Training_Policy_2.0/Training_Policy_2.0-08.jpg");
                    list.Add("/Documents/Images/Training_Policy_2.0/Training_Policy_2.0-09.jpg");
                    list.Add("/Documents/Images/Training_Policy_2.0/Training_Policy_2.0-10.jpg");
                    break;
                case "Induction Policy":
                    list.Add("/Documents/Images/Induction_Policy_2.0/Induction_Policy_2.0-1.jpg");
                    list.Add("/Documents/Images/Induction_Policy_2.0/Induction_Policy_2.0-2.jpg");
                    list.Add("/Documents/Images/Induction_Policy_2.0/Induction_Policy_2.0-3.jpg");
                    list.Add("/Documents/Images/Induction_Policy_2.0/Induction_Policy_2.0-4.jpg");
                    list.Add("/Documents/Images/Induction_Policy_2.0/Induction_Policy_2.0-5.jpg");
                    list.Add("/Documents/Images/Induction_Policy_2.0/Induction_Policy_2.0-6.jpg");
                    list.Add("/Documents/Images/Induction_Policy_2.0/Induction_Policy_2.0-7.jpg");
                    list.Add("/Documents/Images/Induction_Policy_2.0/Induction_Policy_2.0-8.jpg");
                    break;
                case "Disciplinary Policy":
                    list.Add("/Documents/Images/Disciplinary_Policy_2.0/Disciplinary_Policy_2.0-1.jpg");
                    list.Add("/Documents/Images/Disciplinary_Policy_2.0/Disciplinary_Policy_2.0-2.jpg");
                    list.Add("/Documents/Images/Disciplinary_Policy_2.0/Disciplinary_Policy_2.0-3.jpg");
                    list.Add("/Documents/Images/Disciplinary_Policy_2.0/Disciplinary_Policy_2.0-4.jpg");
                    list.Add("/Documents/Images/Disciplinary_Policy_2.0/Disciplinary_Policy_2.0-5.jpg");
                    list.Add("/Documents/Images/Disciplinary_Policy_2.0/Disciplinary_Policy_2.0-6.jpg");
                    list.Add("/Documents/Images/Disciplinary_Policy_2.0/Disciplinary_Policy_2.0-7.jpg");
                    list.Add("/Documents/Images/Disciplinary_Policy_2.0/Disciplinary_Policy_2.0-8.jpg");
                    break;
                case "Annual Health Checkup":
                    list.Add("/Documents/Images/Annual_Health_Checkup_2.0/Annual_Health_Checkup_2.0-1.jpg");
                    list.Add("/Documents/Images/Annual_Health_Checkup_2.0/Annual_Health_Checkup_2.0-2.jpg");
                    list.Add("/Documents/Images/Annual_Health_Checkup_2.0/Annual_Health_Checkup_2.0-3.jpg");
                    list.Add("/Documents/Images/Annual_Health_Checkup_2.0/Annual_Health_Checkup_2.0-4.jpg");
                    list.Add("/Documents/Images/Annual_Health_Checkup_2.0/Annual_Health_Checkup_2.0-5.jpg");
                    list.Add("/Documents/Images/Annual_Health_Checkup_2.0/Annual_Health_Checkup_2.0-6.jpg");
                    list.Add("/Documents/Images/Annual_Health_Checkup_2.0/Annual_Health_Checkup_2.0-7.jpg");
                    break;
                case "Performance Appraisal":
                    list.Add("/Documents/Images/PerformanceAppraisal/PerformanceAppraisal1.jpg");
                    list.Add("/Documents/Images/PerformanceAppraisal/PerformanceAppraisal2.jpg");
                    list.Add("/Documents/Images/PerformanceAppraisal/PerformanceAppraisal3.jpg");
                    list.Add("/Documents/Images/PerformanceAppraisal/PerformanceAppraisal4.jpg");
                    break;
            }
            return list;
        }

        [HttpGet]
        [Route("GetBirthDayImage")]
        public string GetBirthDayImage()
        {
            return "/Images/004.jpg";
        }
        [HttpGet]
        [Route("GetJoineesImage")]
        public string GetJoineesImage()
        {
            return "/Images/wc1.jpg";
        }
    }
}
