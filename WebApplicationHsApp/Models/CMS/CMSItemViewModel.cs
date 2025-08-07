using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class CMSItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemImage { get; set; }
        public Nullable<int> UnitTypeId { get; set; }
        public string AvailableStock { get; set; }
        public string Category { get; set; }
        public string MealType { get; set; }
        public string ItemType { get; set; }
        public Nullable<bool> IsRecipe { get; set; }
        public string HeatingInstructions { get; set; }
        public string Packaging { get; set; }
        public string PackagingInstructions { get; set; }
        public string Notes { get; set; }
        public decimal TotalCalories { get; set; }
        public decimal Serves { get; set; }
        public string PreparationMethod { get; set; }
        public decimal LabourCost { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal PackagingCost { get; set; }
        public decimal OtherCost { get; set; }
        public decimal IngredientTotalCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalCostStaff { get; set; }
        public decimal CostPerServe { get; set; }
        public string KitchenNotes { get; set; }
        public string ServingInstructions { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}