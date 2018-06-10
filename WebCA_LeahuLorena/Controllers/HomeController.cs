using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCA_LeahuLorena.Models;
using Microsoft.AspNet.Identity;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


namespace WebCA_LeahuLorena.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult generateCertificate()
        {
            /*
            var rows = from o in db.Users select o;
            foreach (var row in rows)
                db.Users.Remove(row);
            db.SaveChanges(); 
            */
            var userID = User.Identity.GetUserId();
            var existingModel = db.Models.FirstOrDefault(s => s.userID == userID);
            var user = db.Users.FirstOrDefault(s => s.Id == userID);

            if (existingModel == null || existingModel.domain == null)
                return RedirectToAction("Create", "Users");

            string password = existingModel.privatePassword;
            string email = user.Email;

            string secretPassword = "licenta";

             generatePrivateKey(password, email);
           // generatePrvKeyBouncyCastle(password, email);
            Thread.Sleep(3000);
              generateCSR(password, email, existingModel.countryShort, existingModel.country, existingModel.locality, existingModel.organization, existingModel.departament, existingModel.domain);
            Thread.Sleep(3000);
              getCertificate(secretPassword, email);
             Thread.Sleep(3000);


            return RedirectToAction("Index", "Home");
        }


        public void generatePrvKeyBouncyCastle(string password, string email)
        {
            int RSAKeySize = 1024;
            string keyDirectory = @"E:\ATM\LICENTA!!!\licenta\WebCA_LeahuLorena\WebCA_LeahuLorena\Certificates\" + email;
            using(TextWriter outputStream = System.IO.File.CreateText(keyDirectory+@"\"+email+@".key.pem"))
            {
                using(RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(RSAKeySize))
            {
                RSAParameters rsaKeyInfo = rsaProvider.ExportParameters(true);
                
                using (var stream = new MemoryStream())
                {
                    var writer = new BinaryWriter(stream);
                    writer.Write((byte)0x30); //SEQUENCE
                    using (var innerStream = new MemoryStream())
                    {
                        var innerWriter = new BinaryWriter(innerStream);
                        EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                        EncodeIntegerBigEndian(innerWriter, rsaKeyInfo.Modulus);
                        EncodeIntegerBigEndian(innerWriter, rsaKeyInfo.Exponent);
                        EncodeIntegerBigEndian(innerWriter, rsaKeyInfo.D);
                        EncodeIntegerBigEndian(innerWriter, rsaKeyInfo.P);
                        EncodeIntegerBigEndian(innerWriter, rsaKeyInfo.Q);
                        EncodeIntegerBigEndian(innerWriter, rsaKeyInfo.DP);
                        EncodeIntegerBigEndian(innerWriter, rsaKeyInfo.DQ);
                        EncodeIntegerBigEndian(innerWriter, rsaKeyInfo.InverseQ);
                        var length = (int)innerStream.Length;
                        EncodeLength(writer, length);
                        writer.Write(innerStream.GetBuffer(), 0, length);
                    }
                    var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                    outputStream.WriteLine("-----BEGIN RSA PRIVATE KEY-----");
                    // Output as Base64 with lines chopped at 64 characters
                    for (var i = 0; i < base64.Length; i += 64)
                    {
                        outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
                    }
                    outputStream.WriteLine("-----END RSA PRIVATE KEY-----");
                }
            }
            }
            
        }
        public void generatePrivateKey(string password, string email)
        {
            Process command = new Process();

            command.StartInfo.FileName = "cmd.exe";
            command.StartInfo.RedirectStandardInput = true;
            command.StartInfo.RedirectStandardOutput = true;
            command.StartInfo.CreateNoWindow = true;
            command.StartInfo.UseShellExecute = false;
            command.StartInfo.WorkingDirectory = @"C:\OpenSSL\bin";

            command.Start();

            string keyDirectory = @"E:\ATM\LICENTA!!!\licenta\WebCA_LeahuLorena\WebCA_LeahuLorena\Certificates\" + email + @"\" + email + ".key.pem";

            string directory = Directory.GetCurrentDirectory();
            string pswd = password;

            using (StreamWriter sw = command.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                    sw.WriteLine("openssl genrsa -aes256 -passout pass:" + pswd + " -out " + keyDirectory + " 2048");

            }

            command.Close();
        }

        public void generateCSR(string password, string email, string country, string countryShort, string locality, string organization, string unit, string commonName)
        {
            Process command = new Process();

            command.StartInfo.FileName = "cmd.exe";
            command.StartInfo.RedirectStandardInput = true;
            command.StartInfo.RedirectStandardOutput = true;
            command.StartInfo.CreateNoWindow = true;
            command.StartInfo.UseShellExecute = false;
            command.StartInfo.WorkingDirectory = @"C:\OpenSSL\bin"; //trebuie verificat

            string configLocation = @"E:\ATM\LICENTA!!!\licenta\root\ca\intermediate\openssl.cnf";
            string keyLocation = @"E:\ATM\LICENTA!!!\licenta\WebCA_LeahuLorena\WebCA_LeahuLorena\Certificates\" + email + @"\" + email + ".key.pem";
            string csrLocation = @"E:\ATM\LICENTA!!!\licenta\root\ca\intermediate\csr\" + email + ".csr.pem";
            string secretPassword = password;
            string csrCommand = "openssl req -config " + configLocation + " -key " + keyLocation + " -new -sha256 -out " + csrLocation + " -passin pass:" + secretPassword;

            command.Start();

            using (StreamWriter sw = command.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine(csrCommand);
                    sw.WriteLine(countryShort);
                    sw.WriteLine(country);
                    sw.WriteLine(locality);
                    sw.WriteLine(organization);
                    sw.WriteLine(unit);
                    sw.WriteLine(commonName);
                    sw.WriteLine(email);
                }
            }
            command.Close();
        }

        public void getCertificate(string password, string email)
        {
            Process command = new Process();

            command.StartInfo.FileName = "cmd.exe";
            command.StartInfo.RedirectStandardInput = true;
            command.StartInfo.RedirectStandardOutput = true;
            command.StartInfo.CreateNoWindow = true;
            command.StartInfo.UseShellExecute = false;
            command.StartInfo.WorkingDirectory = @"C:\OpenSSL\bin";

            string intermediatePassword = "licenta";
            string configLocation = @"E:\ATM\LICENTA!!!\licenta\root\ca\intermediate\openssl.cnf";
            string csrLocation = @"E:\ATM\LICENTA!!!\licenta\root\ca\intermediate\csr\" + email + ".csr.pem";
            string certificateLocation = @"E:\ATM\LICENTA!!!\licenta\WebCA_LeahuLorena\WebCA_LeahuLorena\Certificates\" + email + @"\" + email + ".cert.crt";
            string certificateCommand = "openssl ca -config " + configLocation + " -extensions usr_cert -days 365 -notext -md sha256 -in " + csrLocation + " -out " + certificateLocation + " -passin pass:" + intermediatePassword;

            command.Start();

            using (StreamWriter sw = command.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine(certificateCommand);
                    sw.WriteLine('y');
                    sw.WriteLine('y');
                }
            }

            command.Close();
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        private static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }
        private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    // Add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }
    }
}