using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ShopMVC;

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    public const string AUDIENCE = "ShopMVC"; // потребитель токена
    const string KEY = "keyforencoding123!1????123_dablindavaiuzhe123";   // ключ для шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(KEY));
}