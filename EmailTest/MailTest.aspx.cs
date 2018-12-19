using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace EmailTest
{
    public partial class MailTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strConnectionString = "Data Source=sql4.haas.berkeley.edu;Encrypt=true;Initial Catalog=Admissions;user id=aspx_admissions;password=4dm1Th45$!;Connect Timeout=30;";
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(strConnectionString))
            using (var cmd = new SqlCommand(@"SELECT EmailMessageFrom, EmailMessageTo, EmailMessageSubject, EmailMessageBody
from  Email_SmtpLog where smtprequestid = 1000", con))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
            foreach (DataRow row in dt.Rows)
            {
                SmtpClient client = new SmtpClient();
                //client.Credentials = CredentialCache.DefaultNetworkCredentials;
                //MailAddress from = new MailAddress(row["EmailMessageFrom"].ToString());
                MailAddress from = new MailAddress("scheduler@haas.berkeley.edu");
                //MailAddress from = new MailAddress("bwaldman@haas.berkeley.edu");
                //MailAddress from = new MailAddress("ties@haas.berkeley.edu ");
                //MailAddress to = new MailAddress(row["EmailMessageTo"].ToString());                
                MailAddress to = new MailAddress("bwaldman@sonic.net");
                //MailAddress to = new MailAddress("bwaldman@berkeley.edu");   
                MailMessage message = new MailMessage();
                //message.ReplyToList.Add("bwaldman@sonic.net");
                message.From = from;
                message.To.Add(to);
                message.Body = row["EmailMessageBody"].ToString();
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = row["EmailMessageSubject"].ToString();
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                client.SendCompleted += MailDeliveryComplete;                                
                var status = "Failed";
                try
                {
                    client.Send(message);
                    status = "Success";
                }
                catch (Exception ex)
                {
                    status = status + ex.Message;
                }
                message.Dispose();
            }
        }

        public static SendCompletedEventHandler MailDeliveryComplete { get; set; }   
    }
}