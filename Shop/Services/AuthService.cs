// Shop/Services/AuthService.cs
using System;
using System.Security.Cryptography;
using System.Text;
using Shop.Data;
using Shop.Models;

namespace Shop.Services
{
    public interface IAuthService
    {
        bool RegisterUser(string firstName, string lastName, string email, string password);
        User Authenticate(string email, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public bool RegisterUser(string firstName, string lastName, string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
                return false;

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = SimpleEncrypt(password)
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public User Authenticate(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || SimpleDecrypt(user.PasswordHash) != password)
                return null;

            return user;
        }

        private static string SimpleEncrypt(string input)
        {
            // Простое "шифрование" - в реальном приложении используйте хеширование (BCrypt, PBKDF2 и т.д.)
            byte[] data = Encoding.UTF8.GetBytes(input);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] + 1); // Простой сдвиг на 1
            }
            return Convert.ToBase64String(data);
        }

        private static string SimpleDecrypt(string input)
        {
            byte[] data = Convert.FromBase64String(input);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] - 1); // Обратный сдвиг
            }
            return Encoding.UTF8.GetString(data);
        }
    }
}