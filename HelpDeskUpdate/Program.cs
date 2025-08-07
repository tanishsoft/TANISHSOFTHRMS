using HelpDeskUpdate.Common;
using ScheduleApplication.HelpDesk;
using System;

namespace HelpDeskUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Start Help Desk app");
                UpdateTicketAuto model1 = new UpdateTicketAuto();
                model1.Updateticketevery15minutes();
                Console.WriteLine("Help Desk Update ticket every 15 minutes");
                HelpDeskRemainderSend model2 = new HelpDeskRemainderSend();
                model2.Sendtickets();
                Console.WriteLine("Help Desk Send Tickets");
                HelpDeskautoClose model3 = new HelpDeskautoClose();
                model3.UpdateHelpdesk();
                Console.WriteLine("Update Help Desk Tickets");
                DateTime dt = DateTime.Now;
                if (dt.Hour == 7 && dt.Minute < 30)
                {
                    SendAnEmail cm1 = new SendAnEmail();
                    MailModel mailmodel1 = new MailModel();
                    mailmodel1.fromemail = "helpdesk@fernandez.foundation";
                    mailmodel1.toemail = "vamsirm26@gmail.com";
                    mailmodel1.ccemail = "vamsi@microarctech.com";

                    mailmodel1.subject = "Job running Success on " + DateTime.Now;
                    mailmodel1.body = "Help Desk Job Success";
                    mailmodel1.filepath = "";
                    mailmodel1.username = "Help Desk";
                    mailmodel1.fromname = "Help Desk";
                    string emailstatus = cm1.SendEmail(mailmodel1);

                    Console.WriteLine("Send Email " + emailstatus);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Occured " + ex.Message);
                SendAnEmail cm1 = new SendAnEmail();
                MailModel mailmodel1 = new MailModel();
                mailmodel1.fromemail = "helpdesk@fernandez.foundation";
                mailmodel1.toemail = "vamsirm26@gmail.com";
                mailmodel1.ccemail = "vamsi@microarctech.com";

                mailmodel1.subject = "Job running Error on " + DateTime.Now;
                mailmodel1.body = ex.Message;
                mailmodel1.filepath = "";
                mailmodel1.username = "Help Desk";
                mailmodel1.fromname = "Help Desk";
                string emailstatus = cm1.SendEmail(mailmodel1);
                Console.WriteLine("Send email status " + emailstatus);
            }
        }
    }
}
