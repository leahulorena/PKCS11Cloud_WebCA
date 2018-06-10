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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IDigitalSignature
    {

        [OperationContract]
        byte[] Sha256Sign(byte[] hash, string certificateSubject);

        RSACryptoServiceProvider getPrivateKey(string certificateSubject);
    }
}
