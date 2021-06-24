using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace WebTemplate.Helpers
{
    public static class EmailUtil
    {
        public static IEnumerable<EmailResult> SendEmail(EmailCredential credential, int userid, IEnumerable<string> emails, string category, long? itemid, string file, string body = "", string subject = null)
        {
            if (subject == null)
                subject = ConfigurationManager.AppSettings["Subject"];

            var res = new List<EmailResult>();
            using (var emailhelper = new EmailHelper())
            {
                try
                {
                    foreach (var email in emails)
                    {

                        emailhelper.EmailCredential = credential;
                        emailhelper.ReceiverEmail = email;
                        emailhelper.Category = category;
                        emailhelper.ItemId = itemid;
                        emailhelper.Subject = subject;
                        emailhelper.Body = body;
                        emailhelper.UserId = userid;
                        emailhelper.FileName = file;

                        emailhelper.Send();
                        res.Add(true);
                    }
                }
                catch (Exception ex)
                {
                    res.Add(ex.GetActualMessage(), false);
                }
            }
            
            return res;
        }

        public static EmailResult SendEmail(EmailCredential credential, int userid, string email, string category, long? itemid, string file, string body = "", string subject = null)
        {
            if (subject == null)
                subject = ConfigurationManager.AppSettings["Subject"];

            return SendEmail(credential, userid, new List<string> { email }, category, itemid, file, body, subject).FirstOrDefault();
        }
    }

    public class EmailResult
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
    public static class EmailResultExtension
    {
        public static void Add(this List<EmailResult> emailResults, string message, bool isSuccess)
        {
            emailResults.Add(new EmailResult { Message = message, IsSuccess = isSuccess });
        }
        public static void Add(this List<EmailResult> emailResults, bool isSuccesss)
        {
            emailResults.Add(new EmailResult { IsSuccess = isSuccesss });
        }
    }
    public class EmailHelper : IDisposable
    {
        public EmailHelper()
        {
            Client = new SmtpClient();
            Credential = new NetworkCredential();
        }

        public int UserId { get; set; }
        public string Category { get; set; }
        public string Remarks { get; set; }
        public long? ItemId { get; set; }
        public string FileName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ReceiverEmail { get; set; }
        public EmailCredential EmailCredential { get; set; }

        private SmtpClient Client { get; }
        private NetworkCredential Credential { get; }
        private MailMessage MailMessage { get; set; }

        private MailAddress EmailTo { get { return new MailAddress(ReceiverEmail); } }
        private MailAddress EmailFrom { get { return new MailAddress(EmailCredential.Email); } }
        //private string Password { get { return ConfigurationManager.AppSettings["Password"]; } }
        private int Port { get { return Int32.TryParse(ConfigurationManager.AppSettings["Port"], out int port) ? port : 587; } }
        private string Host { get { return ConfigurationManager.AppSettings["Host"]; /*smtp.gmail.com*/} }


        private void InitializeCredential()
        {
            if (EmailCredential == null)
                throw new Exception("Invalid Credential");

            Credential.UserName = EmailCredential.Email;
            Credential.Password = EmailCredential.Password.Decrypt();
        }

        private void InitializeComponents()
        {
            Client.Port = Port;
            Client.Host = Host;
            Client.EnableSsl = true;
            Client.DeliveryMethod = SmtpDeliveryMethod.Network;
            Client.UseDefaultCredentials = false;
            Client.Credentials = Credential;
            Client.Timeout = 60000;
        }


        private void ConstructMessage()
        {
            MailMessage = new MailMessage(EmailFrom, EmailTo);
            MailMessage.Attachments.Add(new Attachment(FileName));
            MailMessage.Subject = Subject;
            MailMessage.Body = Body;
            MailMessage.BodyEncoding = UTF8Encoding.UTF8;
            MailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
        }

        public bool Send()
        {
            var retval = true;
            InitializeCredential();
            InitializeComponents();
            ConstructMessage();
            try
            {
                Client.Send(MailMessage);
            }
            catch (Exception e)
            {
                retval = false;
                Remarks = e.Message; // "Failed to send email.";
            }

            var emailLog = new EmailLogs {
                Category = Category,
                ItemID = ItemId,
                Email = ReceiverEmail,
                File = FileName,
                Subject = Subject,
                Body = Body,
                Status = retval,
                Remarks = Remarks
            };

            EmailLogsProcess.Insert(emailLog, UserId);
            if (retval)
                return true;
            else
                throw new Exception(Remarks);
        }

        public void Dispose()
        {
            Client.Dispose();
            MailMessage.Dispose();
        }
    }
}