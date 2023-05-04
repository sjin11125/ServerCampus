using System.Security.Cryptography;
using System.Text;

namespace Com2usServerCampus;

public class Security       //암호화
{
    private const String AllowableCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";
    public static string Encrypt(string password)
    {
        string result = "";
        byte[] passwordByte = Encoding.Default.GetBytes(password);      //바이트로 변환
        byte[] hash;

        using (SHA256 sha = SHA256.Create())
        {
            hash = sha.ComputeHash(passwordByte); //패스워드 바이트의 해쉬값
        }
        for (int i = 0; i < hash.Length; i++)
        {
            result += hash[i].ToString("x2");       //16진수로 변환
        }
        return result;
    }
    public static string CreateAuthToken()
    {
        var bytes = new Byte[25];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(bytes);
        }

        return new String(bytes.Select(x => AllowableCharacters[x % AllowableCharacters.Length]).ToArray());
    }

}

