using CryptoTests.Helper;
using NUnit.Framework;

namespace CryptoTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("C232F43D0332C12700D8FD932E575C232F43D0332C127", "F1F6DD2D251E74251E741D", "5DF13ffTEex4KfewsfsZ")]
        public void TestEncrypt(string tdescryptkey, string tdescryptiv, string password)
        {
            var cryptoExtensions = new TDESCryptoExtensions(new System.Security.Cryptography.RijndaelManaged(), tdescryptkey, tdescryptiv);
            var passwordEncrypt = cryptoExtensions.Encrypt(password);

            Assert.AreEqual(passwordEncrypt, "O10Cdasdasder8gRXZ+RxbtNvRES/ndg==");
            System.Diagnostics.Debug.Write(passwordEncrypt);
        }

        [TestCase("C232F43D0332C12700D8FD932E575C232F43D0332C127",
                  "F1F6DD2D251E74251E741D", 
                  "0Esx3j2GIgwkToN8Tewdewsdwesdg92zKqixwoZ5wGkFkEL8+WxFgo=",
                  "UmUvvO39LdasdasdddsTxvBIVKAyhx3Q==")]
        public void TestDecrypt(string tdescryptkey, string tdescryptiv, string username, string password)
        {
            var cryptoExtensions = new TDESCryptoExtensions(new System.Security.Cryptography.RijndaelManaged(), tdescryptkey, tdescryptiv);
            var usernameDecrypt = cryptoExtensions.Decrypt(username);
            var passwordDecrypt = cryptoExtensions.Decrypt(password);

            System.Diagnostics.Debug.Write(usernameDecrypt);
            System.Diagnostics.Debug.Write(passwordDecrypt);

            Assert.AreEqual(usernameDecrypt, "O10Cdasdasder8gRXZ+RxbtNvRES/ndg==");
            Assert.AreEqual(passwordDecrypt, "O222210CdssRES/ndg==");

        }

    }
}