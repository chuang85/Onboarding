﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Breeze.WebApi2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Onboarding.Models;
using Onboarding.Utils;
using System.Net.Mail;

namespace Onboarding.Controllers
{
    [BreezeController]
    public class BreezeController : ApiController
    {
        //readonly EFContextProvider<OnboardingDbContext> _contextProvider = new EFContextProvider<OnboardingDbContext>();
        readonly OnboardingRequestContextProvider _contextProvider = new OnboardingRequestContextProvider();

        // ~/breeze/Breeze/Metadata 
        [HttpGet]
        public string Metadata()
        {
            try
            {
                return _contextProvider.Metadata();
            }
            catch (Exception e)
            {
                return e.Message + System.Environment.NewLine + System.Environment.NewLine + e.StackTrace;
            }
            
        }

        // ~/breeze/Breeze/SaveChanges
        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _contextProvider.SaveChanges(saveBundle);
        }

        private static JsonSerializer CreateJsonSerializer()
        {
            var serializerSettings = BreezeConfig.Instance.GetJsonSerializerSettings();
            var jsonSerializer = JsonSerializer.Create(serializerSettings);
            return jsonSerializer;
        }

        // ~/breeze/Breeze/OnboardingRequests
        [HttpGet]
        public IQueryable<OnboardingRequest> OnboardingRequests()
        {
            return _contextProvider.Context.OnboardingRequest;
        }

        //private static void SendEmail(string from, string from_name, string to, string cc, string bcc, string subject, string body, bool isHtml)
        //{
        //    SmtpClient mailClient = new SmtpClient(Config.SmptSettings.Server);
        //    mailClient.Credentials = new NetworkCredential(Config.SmptSettings.UserName, Config.SmptSettings.Password);
        //    mailClient.Port = Config.SmptSettings.Port;

        //    MailMessage message = new MailMessage();
        //    if (!string.IsNullOrEmpty(from_name))
        //    {
        //        message.From = new MailAddress(from, from_name);
        //    }
        //    else
        //    {
        //        message.From = new MailAddress(Formatter.UnFormatSqlInput(from));
        //    }

        //    message.To.Add(new MailAddress(to));

        //    if (!string.IsNullOrEmpty(cc))
        //    {
        //        message.CC.Add(cc);
        //    }

        //    if (!string.IsNullOrEmpty(bcc))
        //    {
        //        message.Bcc.Add(bcc);
        //    }

        //    message.Subject = subject;
        //    message.Body = body;
        //    message.IsBodyHtml = isHtml;

        //    mailClient.EnableSsl = Config.SmptSettings.SSL;
        //    mailClient.Send(message); 
        //}
    }
}
