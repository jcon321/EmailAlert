using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace EQLogReader_1._0
{
    class Program
    {
        static string filePath;
        static string fileName;

        static string phrase;
        static string email;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            //filePath = @"C:\EQMac\";
            //fileName = "eqlog_Surron_52.txt";
            //phrase = "ggggg";
            //email = "test@aol.com";
            filePath = @"C:\EQMac\";
            fileName = args[0];
            phrase = args[1];
            email = args[2];

            File.Delete(filePath + fileName);
            Watch();
            Console.Read();
        }
        public static void Watch()
        {
            var watch = new FileSystemWatcher();
            watch.Path = filePath;
            watch.Filter = fileName;
            watch.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite; //more options
            watch.Changed += new FileSystemEventHandler(OnChanged);
            watch.EnableRaisingEvents = true;
        }

        /// Functions:
        public static void OnChanged(object source, FileSystemEventArgs e)
        {
            if (e.FullPath == filePath + fileName)
            {
                //Anytime file changes I want to read it
                ReadFile(filePath + fileName);
            }
        }

        public static void ReadFile(string incfilePath)
        {
            using (StreamReader reader = new StreamReader(incfilePath))
            {
                string watchString = reader.ReadToEnd();
                if (watchString.Contains(phrase))
                {
                    Console.WriteLine("Phrase detected '" + phrase + "', sent email to " + email);
                    sendEmail();
                }
                //Console.WriteLine(watchString);
            }
            File.Delete(filePath + fileName);

        }

        public static void sendEmail()
        {
            var fromAddress = new MailAddress("eqlogreader@gmail.com", "EQLogReader");
            var toAddress = new MailAddress(email, email);
            const string fromPassword = "eqlogreader1";
            const string subject = "Detection";
            string body = "Your phrase '" + phrase + "' has been detected.";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
