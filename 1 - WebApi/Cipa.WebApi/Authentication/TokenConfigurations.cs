using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace Cipa.WebApi.Authentication
{
    public class TokenConfigurations
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int Seconds { get; set; }
    }

    public class SigningConfigurations
    {
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningConfigurations(string plainKey)
        {
            Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(plainKey));

            SigningCredentials = new SigningCredentials(
                Key, SecurityAlgorithms.HmacSha256Signature);
        }
    }
}