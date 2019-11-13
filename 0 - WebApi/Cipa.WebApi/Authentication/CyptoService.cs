using System.Text;  
using System.Security.Cryptography;
using Cipa.Domain.Exceptions;

namespace Cipa.WebApi.Authentication {
    public class CryptoService
    {
        public static string ComputeSha256Hash(string rawData)
        {
            if (string.IsNullOrWhiteSpace(rawData)) throw new CustomException("Não é possível criptografar valor nulo!");
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())  
            {  
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));  
  
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();  
                for (int i = 0; i < bytes.Length; i++)  
                {  
                    builder.Append(bytes[i].ToString("x2"));  
                }  
                return builder.ToString();  
            }  
        }
    }
}