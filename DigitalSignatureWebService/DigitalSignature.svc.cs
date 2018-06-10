using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace DigitalSignatureWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.

    [Serializable]
    public class DigitalSignature : IDigitalSignature
    {
        public DigitalSignature() { }

        public byte[] Sha256Sign(byte[] hash, string certificateSubject)
        {
            bool isValid = false;
            RSACryptoServiceProvider myPrivateKey = getPrivateKey(certificateSubject);

            var enhCsp = new RSACryptoServiceProvider().CspKeyContainerInfo;
            var cspParams = new CspParameters(enhCsp.ProviderType, enhCsp.ProviderName, myPrivateKey.CspKeyContainerInfo.KeyContainerName);
            myPrivateKey = new RSACryptoServiceProvider(cspParams);

            byte[] signature = myPrivateKey.SignHash(hash, "SHA256");

            isValid = myPrivateKey.VerifyHash(hash, "SHA256", signature);

            if (isValid)
                return signature;
            else
                return null;
        }

        public RSACryptoServiceProvider getPrivateKey(string certificateSubject)
        {
            X509Store getCertificate = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            getCertificate.Open(OpenFlags.ReadOnly);

            var myPrivateKey = new RSACryptoServiceProvider();

            foreach(X509Certificate2 certificate in getCertificate.Certificates)
            {
                if (certificate.Subject.Equals(certificateSubject))
                {
                    myPrivateKey = (RSACryptoServiceProvider)certificate.PrivateKey;
                    break;
                }
            }

            if(myPrivateKey == null)
            {
                getCertificate.Close();
                throw new Exception("No valid certificate was found!");
            }

            else
            {
                getCertificate.Close();
                return myPrivateKey;
            }
        }
    }
}
