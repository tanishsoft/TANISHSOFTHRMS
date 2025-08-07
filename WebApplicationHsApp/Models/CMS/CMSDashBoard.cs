using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class CMSDashBoard
    {
    }

    public class CMSEmployeeDashBoard
    {
        public double? TotalAmount { get; set; }
        public List<OrdersDashBoardViewModel> myOrders = new List<OrdersDashBoardViewModel>();
        public List<PaymentsDashBoardViewModel> myPayments = new List<PaymentsDashBoardViewModel>();
    }

    public class CMSCanteenDashBoard
    {
        public double? TotalAmount { get; set; }
        public double? TotalPayment { get; set; }
        public int NonCooked { get; set; }
        public int Cooked { get; set; }
        public List<OrdersDashBoardViewModel> myOrders = new List<OrdersDashBoardViewModel>();
        public List<PaymentsDashBoardViewModel> myPayments = new List<PaymentsDashBoardViewModel>();
        public List<MealTpyeDashBoardViewModel> mealType = new List<MealTpyeDashBoardViewModel>();
    }
    public class CMSAdminDashBoard
    {
        public double? TotalAmount { get; set; }
        public double? TotalPayment { get; set; }
        public int NonCooked { get; set; }
        public int Cooked { get; set; }
        public List<OrdersDashBoardViewModel> myOrders = new List<OrdersDashBoardViewModel>();
        public List<PaymentsDashBoardViewModel> myPayments = new List<PaymentsDashBoardViewModel>();
        public List<MealTpyeDashBoardViewModel> mealType = new List<MealTpyeDashBoardViewModel>();
    }
    public class OrdersDashBoardViewModel
    {

        public int Id { get; set; }
        public string Item { get; set; }
        public DateTime Date { get; set; }
        public int Qty { get; set; }
        public string Comments { get; set; }
    }
    public class PaymentsDashBoardViewModel
    {

        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public double? Amount { get; set; }
        public string Comments { get; set; }
    }
    public class MealTpyeDashBoardViewModel
    {

        public int TotalReceived { get; set; }
        public int TotalSale { get; set; }
        public int WasteDamage { get; set; }
        public int Retrun { get; set; }
        public int Balance { get; set; }
        public string Type { get; set; }
    }
  
    public class EmployeeFoodDashboardCount
    {
        public string Date { get; set; }       
        public int BFEntitledCount { get; set; }
        public int BFUnEntitledCount { get; set; }

        public int LEntitledCount { get; set; }
        public int LUnEntitledCount { get; set; }

        public int DEntitledCount { get; set; }
        public int DUnEntitledCount { get; set; }
    }
}