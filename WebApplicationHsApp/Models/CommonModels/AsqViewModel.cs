using System;
using System.Collections.Generic;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class AsqViewModel
    {
        public string header { get; set; }
        public List<AsqQuestionModel> questions { get; set; }
    }
    public class AsqQuestionModel
    {
        public int questionId { get; set; }
        public string question { get; set; }
        public string answners { get; set; }
        public string image { get; set; }
        public int orderNumber { get; set; }
        public bool IsNotesRequired { get; set; }
    }

    public class AsqFilledViewModel
    {
        public tbl_AsqForm asq { get; set; }
        public List<tbl_AsqSubForm> SubForm { get; set; }
    }
    public class AsqFilledDetailViewModel
    {
        public tbl_AsqForm asq { get; set; }
        public List<AsqSubFormFilledDetailViewModel> SubForm { get; set; }
    }
    public class AsqSubFormFilledDetailViewModel
    {
        public int Id { get; set; }
        public int AsqId { get; set; }
        public string FillingType { get; set; }
        public string Count { get; set; }
        public List<AsqSubFormFilledDetailViewModelQuestions> questions { get; set; }
    }
    public class AsqSubFormFilledDetailViewModelQuestions
    {
        public string Question { get; set; }
        public string Answner { get; set; }
        public string Remarks { get; set; }
    }
    public class AsqSubCountViewModel
    {
        public string FillingType { get; set; }
        public List<AsqSubCountTypeViewModel> model { get; set; }
    }
    public class AsqSubCountTypeViewModel
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}