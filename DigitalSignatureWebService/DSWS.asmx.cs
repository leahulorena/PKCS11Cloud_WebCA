using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Asn1.Pkcs;

namespace DigitalSignatureWebService
{
    /// <summary>
    /// Summary description for DSWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DSWS : System.Web.Services.WebService
    {
        
        [WebMethod]
        public byte[] GetUserCertificate(string username)
        {
            X509Certificate2 certificate = new X509Certificate2();
            StringBuilder path = new StringBuilder();
            path.Append(@"E:\ATM\LICENTA!!!\licenta\WebCA_LeahuLorena\WebCA_LeahuLorena\Certificates\");
            path.Append(username);
            string[] files = Directory.GetFiles(path.ToString());
            byte[] cetificateRawData = { 0 };
            if (files != null)
            {
                foreach (var file in files)
                {
                    string extension = Path.GetExtension(path + @"\" + file);
                    if (extension == ".crt")
                    {
                        certificate = new X509Certificate2(file);
                        cetificateRawData = certificate.RawData;
                    }
                    return cetificateRawData;
                }

            }
            return cetificateRawData;
        }

      

        [WebMethod]
        public byte[] SignData(string username, string password, byte[] data)
        {
            byte[] signature;

            StringBuilder path = new StringBuilder();
            path.Append(@"E:\ATM\LICENTA!!!\licenta\WebCA_LeahuLorena\WebCA_LeahuLorena\Certificates\");
            path.Append(username);
            path.Append(@"\");
            path.Append(username);
            path.Append(".key.pem");


            //trebuie sa verific daca exista fisierul cu cheia
            //to be continued

            IPasswordFinder pFinder = new PasswordStore(password);    
        
            AsymmetricCipherKeyPair keyPair;
            using (var reader = File.OpenText(path.ToString()))
                keyPair = (AsymmetricCipherKeyPair)new PemReader(reader, pFinder).ReadObject();

            AsymmetricKeyParameter privateKey = keyPair.Private;

            //ISigner sig = SignerUtilities.GetSigner("SHA256withRSA");
            ISigner sig = SignerUtilities.GetSigner("RSA");

            sig.Init(true, privateKey);

            sig.BlockUpdate(data, 0, data.Length);

            signature = sig.GenerateSignature();

            return signature;
        }

        [WebMethod]
        public int getSignaturelLen(string username, string password, byte[] data)
        {
            byte[] signature = SignData(username, password, data);
            return signature.Count();
        }
       
    }

    class PasswordStore : IPasswordFinder
    {
        private char[] password;

        public PasswordStore(string passphrase)
        {
            password = passphrase.ToCharArray();
        }

        public char[] GetPassword()
        {
            return password;
        }
    }


}
