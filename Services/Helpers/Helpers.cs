using System.Security.Cryptography;
using System.Text;

namespace NFTGenApi.Services;

public static class Helpers
{
    public static decimal IntToDecimal(decimal value) => value switch
    {
        < 0 => -1M,
        0 => 0M,
        > 1 => decimal.Divide(value, 100),
        _ => 0M
    };

    public static string ComputeSHA256HashAsString(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            // Convert the byte array to a hexadecimal string
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
