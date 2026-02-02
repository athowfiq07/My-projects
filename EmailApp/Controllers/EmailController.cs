
//using MailKit.Security;
//using MimeKit;
//using MimeKit.Text;
//using System.Data;

//private async Task SendEquipmentLogEmail()
//{
//    DataTable dt = new DataTable();

//    using (SqlConnection con = new SqlConnection(_conn))
//    using (SqlCommand cmd = new SqlCommand("GetEquipmentLogs", con))
//    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
//    {
//        cmd.CommandType = CommandType.StoredProcedure;
//        da.Fill(dt);
//    }

//    // Build HTML Table
//    string html = "<h2>Equipment Log Report</h2><table border='1'>";
//    html += "<tr>";
//    foreach (DataColumn col in dt.Columns)
//        html += $"<th>{col.ColumnName}</th>";
//    html += "</tr>";

//    foreach (DataRow row in dt.Rows)
//    {
//        html += "<tr>";
//        foreach (var cell in row.ItemArray)
//            html += $"<td>{cell}</td>";
//        html += "</tr>";
//    }
//    html += "</table>";

//    // Create Email
//    var email = new MimeMessage();
//    email.From.Add(MailboxAddress.Parse("yourgmail@gmail.com"));

//    // 📌 Add multiple recipients here
//    email.To.Add(MailboxAddress.Parse("person1@gmail.com"));
//    email.To.Add(MailboxAddress.Parse("person2@gmail.com"));
//    email.To.Add(MailboxAddress.Parse("person3@gmail.com"));

//    email.Subject = "Scheduled Equipment Log";
//    email.Body = new TextPart(TextFormat.Html) { Text = html };

//    using var smtp = new SmtpClient();

//    // Gmail SMTP Server
//    smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

//    // IMPORTANT: Use Gmail App Password here
//    smtp.Authenticate("yourgmail@gmail.com", "YOUR-APP-PASSWORD-HERE");

//    await smtp.SendAsync(email);
//    smtp.Disconnect(true);
//}



//using MailKit.Net.Smtp;
//using MailKit.Security;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using MimeKit;
//using MimeKit.Text;

//namespace EmailApp.Controllers
//{
//    //molly.sipes64@ethereal.email
//    //3ebhTA1zDYSPBUnMXR
//    [Route("api/[controller]")]
//    [ApiController]
//    public class EmailController : ControllerBase
//    {
//        [HttpPost]
//        public IActionResult SendEmail(string body)
//        {
//            var email = new MimeMessage ();
//            email.From.Add(MailboxAddress.Parse("molly.sipes64@ethereal.email"));
//            email.To.Add(MailboxAddress.Parse("molly.sipes64@ethereal.email"));
//            email.Subject = "Text Email Subject";
//            email.Body = new TextPart(TextFormat.Html) { Text = body };

//            using var smtp =new SmtpClient();
//            smtp.Connect("smtp.ethereal.email",587, SecureSocketOptions.StartTls);
//            smtp.Authenticate("molly.sipes64@ethereal.email", "3ebhTA1zDYSPBUnMXR");
//            smtp.Send(email);
//            smtp.Disconnect(true);

//            return Ok();
//        }
//    }
//}


//using MailKit.Net.Smtp;
//using MailKit.Security;
//using Microsoft.AspNetCore.Mvc;
//using MimeKit;
//using MimeKit.Text;
//using System.Data;
//using System.Data.SqlClient;

//[Route("api/[controller]")]
//[ApiController]
//public class EmailController : ControllerBase
//{
//    private readonly string _conn;
//    public EmailController(IConfiguration config)
//    {
//        _conn = config.GetConnectionString("DefaultConnection");
//    }

//    [HttpGet("send-equipment-log")]
//    public IActionResult SendEquipmentLogEmail()
//    {

//        DataTable dt = new DataTable();

//        using (SqlConnection con = new SqlConnection(_conn))
//        using (SqlCommand cmd = new SqlCommand("GetEquipmentLogs", con))
//        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
//        {
//            cmd.CommandType = CommandType.StoredProcedure;
//            da.Fill(dt);
//        }


//        string html = "<h2>Equipment Log Report</h2>";
//        html += "<table border='1' cellpadding='8' cellspacing='0'><tr>";

//        foreach (DataColumn col in dt.Columns)
//        {
//            html += $"<th>{col.ColumnName}</th>";
//        }
//        html += "</tr>";

//        foreach (DataRow row in dt.Rows)
//        {
//            html += "<tr>";
//            foreach (var cell in row.ItemArray)
//            {
//                html += $"<td>{cell}</td>";
//            }
//            html += "</tr>";
//        }
//        html += "</table>";


//        var email = new MimeMessage();
//        email.From.Add(MailboxAddress.Parse("molly.sipes64@ethereal.email"));
//        email.To.Add(MailboxAddress.Parse("molly.sipes64@ethereal.email"));
//        email.Subject = "Equipment Status Report";
//        email.Body = new TextPart(TextFormat.Html) { Text = html };


//        using var smtp = new SmtpClient();
//        smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
//        smtp.Authenticate("molly.sipes64@ethereal.email", "3ebhTA1zDYSPBUnMXR");
//        smtp.Send(email);
//        smtp.Disconnect(true);

//        return Ok("Equipment Log Email Sent Successfully");
//    }
//}



using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MimeKit;
using MimeKit.Text;

public class EquipmentLogEmailService : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly string _conn;

    public EquipmentLogEmailService(IConfiguration config)
    {
        _config = config;
        _conn = config.GetConnectionString("DefaultConnection");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;

            // Check 9:00 or 17:00 (5 PM)
            if ((now.Hour == 9 || now.Hour == 17) && now.Minute == 0)
            {
                await SendEquipmentLogEmail();
                await Task.Delay(TimeSpan.FromMinutes(1));
            }

            await Task.Delay(1000 * 30); // Check every 30 seconds instead of 1 minute to avoid missing exact time
        }
    }

    private async Task SendEquipmentLogEmail()
    {
        DataTable dt = new DataTable();

        using (SqlConnection con = new SqlConnection(_conn))
        using (SqlCommand cmd = new SqlCommand("GetEquipmentLogs", con))
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            da.Fill(dt);
        }

        string html = "<h2>Equipment Log Report</h2><table border='1'>";
        html += "<tr>";
        foreach (DataColumn col in dt.Columns)
            html += $"<th>{col.ColumnName}</th>";
        html += "</tr>";

        foreach (DataRow row in dt.Rows)
        {
            html += "<tr>";
            foreach (var cell in row.ItemArray)
                html += $"<td>{cell}</td>";
            html += "</tr>";
        }
        html += "</table>";

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("molly.sipes64@ethereal.email"));
        email.To.Add(MailboxAddress.Parse("client@gmail.com"));
        email.Subject = "Scheduled Equipment Log";
        email.Body = new TextPart(TextFormat.Html) { Text = html };

        using var smtp = new SmtpClient();
        smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
        smtp.Authenticate("molly.sipes64@ethereal.email", "3ebhTA1zDYSPBUnMXR");
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
}
