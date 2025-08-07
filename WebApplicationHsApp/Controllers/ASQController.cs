using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    public class ASQController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: ASQ
        [Route("ASQ/{name}")]
        public ActionResult Index(string name = "")
        {
            ViewBag.AsqName = name;
            return View();
        }
        public ActionResult BindAsq(string name)
        {
            var dbQuestions = myapp.tbl_AsqQuestions.Where(l => l.FillingMonth == name).ToList();
            List<AsqViewModel> model = new List<AsqViewModel>();
            var headers = dbQuestions.OrderBy(l => l.FillingPageTypeOrder).Select(l => l.FillingPageType).Distinct();
            foreach (var h in headers)
            {
                var hques = dbQuestions.Where(l => l.FillingPageType == h).ToList();
                AsqViewModel asq = new AsqViewModel();
                asq.header = h;
                asq.questions = (from q in hques
                                 select new AsqQuestionModel
                                 {
                                     answners = q.QuestionOptions,
                                     question = q.Question,
                                     questionId = q.AsqQuestionId,
                                     image = q.QuestionImage,
                                     orderNumber = q.OrderNumber.HasValue ? q.OrderNumber.Value : 0,
                                     IsNotesRequired = q.IsNotesRequired.HasValue ? q.IsNotesRequired.Value : false
                                 }).ToList();
                model.Add(asq);
            }
            return PartialView(model);
        }
        public ActionResult TwentyFourMonth()
        {
            return PartialView();
        }
        public ActionResult ManageResponses()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult AjaxGetAsqForm(JQueryDataTableParamModel param)
        {
            List<tbl_AsqForm> query = myapp.tbl_AsqForm.OrderByDescending(a => a.AsqId).ToList();
            IEnumerable<tbl_AsqForm> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.AsqId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.MotherName != null && c.MotherName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.BabyMRNumber != null && c.BabyMRNumber.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_AsqForm> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[] {
                                              c.AsqId.ToString(),
                                              c.FillingMonth,
                                              c.MotherName,
                                              c.BabyMRNumber,
                                              c.BabyGender,
                                              c.PhoneNumber,
                                              c.PersonFilling,
                                              c.CreatedOn!=null?c.CreatedOn.Value.ToString("dd/MM/yyyy hh:mm tt"):"",
                                              c.RelationshipChild,
                                              c.AsqId.ToString(),
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewDetails(int id)
        {
            AsqFilledDetailViewModel model = new AsqFilledDetailViewModel();
            model.asq = myapp.tbl_AsqForm.Where(l => l.AsqId == id).SingleOrDefault();
            var subforms = myapp.tbl_AsqSubForm.Where(l => l.AsqId == id).ToList();
            var fillingtypes = subforms.Select(l => l.FillingType).Distinct().ToList();
            model.SubForm = new List<AsqSubFormFilledDetailViewModel>();
            foreach (var f in fillingtypes)
            {
                AsqSubFormFilledDetailViewModel m = new AsqSubFormFilledDetailViewModel();
                m.AsqId = id;
                m.FillingType = f;
                string filllingtype = m.FillingType.ToLower().Replace(" ", "");
                switch (filllingtype)
                {
                    case "communication":
                        m.Count = model.asq.CommunicationScore.Value.ToString("0");
                        break;
                    case "grossmotor":
                    case "emotionalfunctioning":
                        m.Count = model.asq.GrossMotorScore.Value.ToString("0");
                        break;
                    case "finemotor":
                    case "cognitivefunctioning":
                        m.Count = model.asq.FineMotorScore.Value.ToString("0");
                        break;
                    case "problemsolving":
                    case "physicalfunctioning":
                        m.Count = model.asq.ProblemSolvingScore.Value.ToString("0");
                        break;
                    case "personal-social":
                    case "socialfunctioning":
                        m.Count = model.asq.PersonalSocialScore.Value.ToString("0");
                        break;
                    case "overall":
                    case "worry":
                        m.Count = "0";
                        break;
                }
                m.Id = 1;
                m.questions = new List<AsqSubFormFilledDetailViewModelQuestions>();
                var ssubform = subforms.Where(l => l.FillingType == f).SingleOrDefault();
                AsqSubFormFilledDetailViewModelQuestions q = new AsqSubFormFilledDetailViewModelQuestions();
                if (ssubform.QuestionId1 != null && ssubform.QuestionId1 != 0)
                {
                    q.Question = myapp.tbl_AsqQuestions.Where(l => l.FillingMonth == model.asq.FillingMonth && l.FillingPageType == f && l.AsqQuestionId == ssubform.QuestionId1).SingleOrDefault().Question;
                    q.Answner = ssubform.Answner1;
                    q.Remarks = ssubform.Remarks1;
                    m.questions.Add(q);
                }
                q = new AsqSubFormFilledDetailViewModelQuestions();
                if (ssubform.QuestionId2 != null && ssubform.QuestionId2 != 0)
                {
                    q.Question = myapp.tbl_AsqQuestions.Where(l => l.FillingMonth == model.asq.FillingMonth && l.FillingPageType == f && l.AsqQuestionId == ssubform.QuestionId2).SingleOrDefault().Question;
                    q.Answner = ssubform.Answner2;
                    q.Remarks = ssubform.Remarks2;
                    m.questions.Add(q);
                }
                q = new AsqSubFormFilledDetailViewModelQuestions();
                if (ssubform.QuestionId3 != null && ssubform.QuestionId3 != 0)
                {
                    q.Question = myapp.tbl_AsqQuestions.Where(l => l.FillingMonth == model.asq.FillingMonth && l.FillingPageType == f && l.AsqQuestionId == ssubform.QuestionId3).SingleOrDefault().Question;
                    q.Answner = ssubform.Answner3;
                    q.Remarks = ssubform.Remarks3;
                    m.questions.Add(q);
                }
                q = new AsqSubFormFilledDetailViewModelQuestions();
                if (ssubform.QuestionId4 != null && ssubform.QuestionId4 != 0)
                {
                    q.Question = myapp.tbl_AsqQuestions.Where(l => l.FillingMonth == model.asq.FillingMonth && l.FillingPageType == f && l.AsqQuestionId == ssubform.QuestionId4).SingleOrDefault().Question;
                    q.Answner = ssubform.Answner4;
                    q.Remarks = ssubform.Remarks4;
                    m.questions.Add(q);
                }
                q = new AsqSubFormFilledDetailViewModelQuestions();
                if (ssubform.QuestionId5 != null && ssubform.QuestionId5 != 0)
                {
                    q.Question = myapp.tbl_AsqQuestions.Where(l => l.FillingMonth == model.asq.FillingMonth && l.FillingPageType == f && l.AsqQuestionId == ssubform.QuestionId5).SingleOrDefault().Question;
                    q.Answner = ssubform.Answner5;
                    q.Remarks = ssubform.Remarks5;
                    m.questions.Add(q);
                }
                q = new AsqSubFormFilledDetailViewModelQuestions();
                if (ssubform.QuestionId6 != null && ssubform.QuestionId6 != 0)
                {
                    q.Question = myapp.tbl_AsqQuestions.Where(l => l.FillingMonth == model.asq.FillingMonth && l.FillingPageType == f && l.AsqQuestionId == ssubform.QuestionId6).SingleOrDefault().Question;
                    q.Answner = ssubform.Answner6;
                    q.Remarks = ssubform.Remarks6;
                    m.questions.Add(q);
                }
                q = new AsqSubFormFilledDetailViewModelQuestions();
                if (ssubform.QuestionId7 != null && ssubform.QuestionId7 != 0)
                {
                    q.Question = myapp.tbl_AsqQuestions.Where(l => l.FillingMonth == model.asq.FillingMonth && l.FillingPageType == f && l.AsqQuestionId == ssubform.QuestionId7).SingleOrDefault().Question;
                    q.Answner = ssubform.Answner7;
                    q.Remarks = ssubform.Remarks7;
                    m.questions.Add(q);
                }
                q = new AsqSubFormFilledDetailViewModelQuestions();
                if (ssubform.QuestionId8 != null && ssubform.QuestionId8 != 0)
                {
                    q.Question = myapp.tbl_AsqQuestions.Where(l => l.FillingMonth == model.asq.FillingMonth && l.FillingPageType == f && l.AsqQuestionId == ssubform.QuestionId8).SingleOrDefault().Question;
                    q.Answner = ssubform.Answner8;
                    q.Remarks = ssubform.Remarks8;
                    m.questions.Add(q);
                }
                q = new AsqSubFormFilledDetailViewModelQuestions();
                if (ssubform.QuestionId9 != null && ssubform.QuestionId9 != 0)
                {
                    q.Question = myapp.tbl_AsqQuestions.Where(l => l.FillingMonth == model.asq.FillingMonth && l.FillingPageType == f && l.AsqQuestionId == ssubform.QuestionId9).SingleOrDefault().Question;
                    q.Answner = ssubform.Answner9;
                    q.Remarks = ssubform.Remarks9;
                    m.questions.Add(q);
                }
                q = new AsqSubFormFilledDetailViewModelQuestions();
                if (ssubform.QuestionId10 != null && ssubform.QuestionId10 != 0)
                {
                    q.Question = myapp.tbl_AsqQuestions.Where(l => l.FillingMonth == model.asq.FillingMonth && l.FillingPageType == f && l.AsqQuestionId == ssubform.QuestionId10).SingleOrDefault().Question;
                    q.Answner = ssubform.Answner10;
                    q.Remarks = ssubform.Remarks10;
                    m.questions.Add(q);
                }
                q = new AsqSubFormFilledDetailViewModelQuestions();
                if (ssubform.QuestionId11 != null && ssubform.QuestionId11 != 0)
                {
                    q.Question = myapp.tbl_AsqQuestions.Where(l => l.FillingMonth == model.asq.FillingMonth && l.FillingPageType == f && l.AsqQuestionId == ssubform.QuestionId11).SingleOrDefault().Question;
                    q.Answner = ssubform.Answner11;
                    q.Remarks = ssubform.Remarks11;
                    m.questions.Add(q);
                }
                q = new AsqSubFormFilledDetailViewModelQuestions();
                if (ssubform.QuestionId12 != null && ssubform.QuestionId12 != 0)
                {
                    q.Question = myapp.tbl_AsqQuestions.Where(l => l.FillingMonth == model.asq.FillingMonth && l.FillingPageType == f && l.AsqQuestionId == ssubform.QuestionId12).SingleOrDefault().Question;
                    q.Answner = ssubform.Answner12;
                    q.Remarks = ssubform.Remarks12;
                    m.questions.Add(q);
                }
                model.SubForm.Add(m);
            }

            return View(model);
        }

        public ActionResult SaveAsq(AsqFilledViewModel model)
        {
            tbl_AsqForm m = model.asq;
            m.AdminComments = "";
            m.CreatedOn = DateTime.Now;
            m.OverallScore = 0;
            m.CommunicationScore = 0;
            m.FineMotorScore = 0;
            m.GrossMotorScore = 0;
            m.PersonalSocialScore = 0;
            m.ProblemSolvingScore = 0;
            m.Column1 = "0";
            m.Column2 = "0";

            myapp.tbl_AsqForm.Add(m);
            myapp.SaveChanges();
            foreach (var sm in model.SubForm)
            {
                string filllingtype = sm.FillingType.ToLower().Replace(" ", "");
                if (model.asq.FillingMonth == "12Month")
                {
                    if (sm.FillingType == "FINE MOTOR")
                    {
                        if (sm.Answner4 != "NOT YET")
                        {
                            sm.Answner2 = "YES";
                        }
                    }
                    if (sm.FillingType == "PROBLEM SOLVING")
                    {
                        if (sm.Answner5 != "NOT YET")
                        {
                            sm.Answner4 = "YES";
                        }
                    }
                }
                if (model.asq.FillingMonth == "18Month")
                {
                    if (sm.FillingType == "PROBLEM SOLVING")
                    {
                        if (sm.Answner6 != "NOT YET")
                        {
                            sm.Answner3 = "YES";
                        }
                    }
                }
                if (model.asq.FillingMonth == "24Month")
                {
                    if (sm.FillingType == "GROSS MOTOR")
                    {
                        if (sm.Answner6 != "NOT YET")
                        {
                            sm.Answner2 = "YES";
                        }
                    }
                }
                switch (filllingtype)
                {
                    case "communication":
                        m.CommunicationScore = GetScore(sm);
                        break;
                    case "grossmotor":
                    case "emotionalfunctioning":
                        m.GrossMotorScore = GetScore(sm);
                        break;
                    case "finemotor":
                    case "cognitivefunctioning":
                        m.FineMotorScore = GetScore(sm);
                        break;
                    case "problemsolving":
                    case "physicalfunctioning":
                        m.ProblemSolvingScore = GetScore(sm);
                        break;
                    case "personal-social":
                    case "socialfunctioning":
                        m.PersonalSocialScore = GetScore(sm);
                        break;
                    case "overall":
                    case "worry":
                        m.OverallScore = GetScore(sm);
                        break;
                    case "dailyactivities":
                        m.Column1 = GetScore(sm).ToString();
                        break;
                    case "familyrelationships":
                        m.Column2 = GetScore(sm).ToString();
                        break;
                }
                sm.AsqId = m.AsqId;
                sm.CreatedOn = DateTime.Now;
                myapp.tbl_AsqSubForm.Add(sm);
                myapp.SaveChanges();
            }
            return Json("Successfully Created", JsonRequestBehavior.AllowGet);
        }
        private int GetScore(tbl_AsqSubForm sm)
        {
            int score = 0;
            if (sm.Answner1 != null)
                score = score + FindAnswer(sm.Answner1);
            if (sm.Answner2 != null)
                score = score + FindAnswer(sm.Answner2);
            if (sm.Answner3 != null)
                score = score + FindAnswer(sm.Answner3);
            if (sm.Answner4 != null)
                score = score + FindAnswer(sm.Answner4);
            if (sm.Answner5 != null)
                score = score + FindAnswer(sm.Answner5);
            if (sm.Answner6 != null)
                score = score + FindAnswer(sm.Answner6);
            if (sm.Answner7 != null)
                score = score + FindAnswer(sm.Answner7);
            if (sm.Answner8 != null)
                score = score + FindAnswer(sm.Answner8);
            if (sm.Answner9 != null)
                score = score + FindAnswer(sm.Answner9);
            if (sm.Answner10 != null)
                score = score + FindAnswer(sm.Answner10);
            if (sm.Answner11 != null)
                score = score + FindAnswer(sm.Answner11);
            if (sm.Answner12 != null)
                score = score + FindAnswer(sm.Answner12);
            return score;
        }
        public ActionResult GetAvgScoreforEntries(string fillingmonth = "")
        {
            List<tbl_AsqForm> listmodel = new List<tbl_AsqForm>();
            if (fillingmonth != null && fillingmonth != "")
            {
                listmodel = myapp.tbl_AsqForm.Where(l => l.FillingMonth == fillingmonth).ToList();
            }
            else
            {
                listmodel = myapp.tbl_AsqForm.ToList();
            }
            tbl_AsqForm m = new tbl_AsqForm();
            if (listmodel.Count > 0)
            {
                m.AdminComments = listmodel.Count().ToString();
                m.OverallScore = listmodel.Sum(l => l.OverallScore) / listmodel.Count();
                m.CommunicationScore = listmodel.Sum(l => l.CommunicationScore) / listmodel.Count();
                m.FineMotorScore = listmodel.Sum(l => l.FineMotorScore) / listmodel.Count();
                m.GrossMotorScore = listmodel.Sum(l => l.GrossMotorScore) / listmodel.Count();
                m.PersonalSocialScore = listmodel.Sum(l => l.PersonalSocialScore) / listmodel.Count();
                m.ProblemSolvingScore = listmodel.Sum(l => l.ProblemSolvingScore) / listmodel.Count();
                m.Column1 = (listmodel.Sum(l => int.Parse(l.Column1)) / listmodel.Count()).ToString();
                m.Column2 = (listmodel.Sum(l => int.Parse(l.Column2)) / listmodel.Count()).ToString();
            }
            return Json(m, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllFormsScores(string fillingmonth)
        {
            var model = (from a in myapp.tbl_AsqForm
                         join asub in myapp.tbl_AsqSubForm on a.AsqId equals asub.AsqId
                         where a.FillingMonth == fillingmonth
                         select asub).ToList();
            var fillingtypes = model.Select(l => l.FillingType).Distinct().ToList();
            List<AsqSubCountViewModel> MainModel = new List<AsqSubCountViewModel>();
            List<string> optionselect = new List<string>();
            if (fillingmonth == "PedsQL")
            {
                optionselect.Add("Never");
                optionselect.Add("Almost Never");
                optionselect.Add("Sometimes");
                optionselect.Add("Often");
                optionselect.Add("Almost Always");
            }
            else
            {
                optionselect.Add("YES");
                optionselect.Add("SOMETIMES");
                optionselect.Add("NOT YET");

            }
            foreach (var filltype in fillingtypes)
            {
                var checkforms = model.Where(l => l.FillingType == filltype).ToList();
                AsqSubCountViewModel m = new AsqSubCountViewModel();
                m.FillingType = filltype;
                m.model = new List<AsqSubCountTypeViewModel>();
                foreach (var m2 in optionselect)
                {
                    AsqSubCountTypeViewModel m3 = new AsqSubCountTypeViewModel();
                    m3.Name = m2;
                    m3.Count = FindCountSubforms(checkforms.Where(l => l.Answner1 == m2).Select(l => l.Answner1).ToList()) +
                        FindCountSubforms(checkforms.Where(l => l.Answner2 == m2).Select(l => l.Answner2).ToList()) +
                        FindCountSubforms(checkforms.Where(l => l.Answner3 == m2).Select(l => l.Answner3).ToList()) +
                        FindCountSubforms(checkforms.Where(l => l.Answner4 == m2).Select(l => l.Answner4).ToList()) +
                        FindCountSubforms(checkforms.Where(l => l.Answner5 == m2).Select(l => l.Answner5).ToList()) +
                        FindCountSubforms(checkforms.Where(l => l.Answner6 == m2).Select(l => l.Answner6).ToList());
                    m.model.Add(m3);
                }
                MainModel.Add(m);
            }
            return Json(MainModel, JsonRequestBehavior.AllowGet);
        }
        private int FindCountSubforms(List<string> submodel)
        {
            int count = 0;
            foreach (var v in submodel)
            {
                count = count + FindAnswer(v);
            }
            return count;
        }
        private int FindAnswer(string answner)
        {
            if (answner != null && answner != "")
            {
                answner = answner.Trim(' ');
                int s = 0;
                switch (answner)
                {
                    case "Never":
                        s = 0;
                        break;
                    case "Almost Never":
                        s = 1;
                        break;
                    case "Sometimes":
                        s = 2;
                        break;
                    case "Often":
                        s = 3;
                        break;
                    case "Almost Always":
                        s = 4;
                        break;
                    case "YES":
                        s = 10;
                        break;
                    case "SOMETIMES":
                        s = 5;
                        break;
                    case "NOT YET":
                    case "NO":
                        s = 0;
                        break;
                }
                return s;
            }
            else
                return 0;
        }

        public ActionResult SelectMonth()
        {
            return View();
        }
    }
}