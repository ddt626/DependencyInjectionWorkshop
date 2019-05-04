﻿using System.Text;

namespace DependencyInjectionWorkshop.Adapter
{
    public interface IHash
    {
        string GetPassword(string password);
    }

    public class Sha256Adapter : IHash
    {
        public string GetPassword(string password)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
            foreach (var theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            var hashedPassword = hash.ToString();
            return hashedPassword;
        }
    }
}