using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Library_Desafio_Backend.Security
{
    public class RandomToken
    {
        public string CreateRandomKey(int size = 30)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytesHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString("yyyyMMddHHmmssfff")));
            string hashHexadecimal = BitConverter.ToString(bytesHash).Replace("-", "");
            System.Threading.Thread.Sleep(1);
            return hashHexadecimal.Substring(0, size);
        }
        public string GenerateTokenHash()
        {
            var dateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var bytes = Encoding.UTF8.GetBytes(dateTime);
            var hash = SHA256.Create().ComputeHash(bytes);

            var key = System.Convert.ToBase64String(hash);
            key = key.Replace("+", "").Replace("/", "").Replace("=", "");

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var randomKey = key;

            while (randomKey.Length < 100)
            {
                var buffer = new byte[4];
                RandomNumberGenerator.Create().GetBytes(buffer);
                var randomChar = chars[new Random(buffer[0]).Next(chars.Length)];
                randomKey += randomChar;
            }
            System.Threading.Thread.Sleep(1);
            return randomKey;
        }
        public bool ValidateKey(string chave)
        {
            if (chave.Length < 100)
            {
                return false;
            }
            var regex = new Regex("^[a-zA-Z0-9]+$");
            return regex.IsMatch(chave);
        }
    }
}
