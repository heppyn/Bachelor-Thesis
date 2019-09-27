using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ThermostatControler
{
    
    public class Users
    {
        private const int m_hashSize = 32;
        private const int m_saltSize = 16;
        private const int m_iterations = 80000;
        private const String m_defaultName = "Admin";
        private const String m_defaultPassHash = "YHitL/kUS+/krUbjeIFOASrt3sBBAmNYO83d22iCl0MnxqEpaf4JJuTH9PwVPjS1";

        private IData m_data;
        private User m_currentUser;

        public User CurrentUser { get => m_currentUser; set => m_currentUser = value; }

        public enum Response {
            OK,
            FAIL,
            DEFAULT
        }
        
        public Users()
        {
            m_data = new Data();
        }

        /// <summary>
        /// Checks if user is named Admin
        /// </summary>
        /// <returns>True if user is admin</returns>
        public bool IsUserAdmin()
        {
            return CurrentUser.Name.Equals(m_defaultName);
        }

        /// <summary>
        /// Checks  if user name exists, if it does returns false
        /// </summary>
        /// <param name="user">user to create</param>
        /// <returns>true if succesfuly created user</returns>
        public bool CreateUser(User user)
        {
            if (UserNameExists(user))
                return false;

            List<User> users;
            try
            {
                users = m_data.LoadUsers();
            }
            catch
            {
                users = new List<User>();
            }

            User u = new User(user.Name, CreatePass(user.Pass));
            users.Add(u);
            m_data.SaveUsers(users);

            return true;
        }
        /// <summary>
        /// Checks data for existing user and verifies password
        /// </summary>
        /// <param name="user"></param>
        /// <returns>default user if save file doesn't exist</returns>
        public Response UserExists(User user)
        {
            List<User> users;
            try
            {
                users = m_data.LoadUsers();
            }
            catch
            {
                if (user.Name == m_defaultName && VerifyPassword(user.Pass, m_defaultPassHash))
                    return Response.DEFAULT;
                return Response.FAIL;
            }
            foreach (User u in users)
            {
                if (u.HasSameName(user))
                {
                    if (VerifyPassword(user.Pass, u.Pass))
                        return Response.OK;
                }
            }
            return Response.FAIL;
        }

        private bool UserNameExists(User user)
        {
            List<User> users;
            try
            {
                users = m_data.LoadUsers();
            }
            catch
            {
                return false;
            }
            foreach (User u in users)
            {
                if (u.HasSameName(user))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Generates hash for password
        /// </summary>
        /// <param name="pass">users password</param>
        private String CreatePass(String pass)
        {
            // generate salt
            byte[] salt = new byte[m_saltSize];
            new RNGCryptoServiceProvider().GetBytes(salt);
            // generate hash
            //Rfc2898DeriveBytes rfc = new System.Security.Cryptography.Rfc2898DeriveBytes(pass, salt, m_iterations);
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(pass, salt, m_iterations, HashAlgorithmName.SHA256);
            byte[] hash = rfc.GetBytes(m_hashSize);
            // combine salt and hash
            byte[] hashed = new byte[m_hashSize + m_saltSize];
            Array.Copy(salt, 0, hashed, 0, m_saltSize);
            Array.Copy(hash, 0, hashed, m_saltSize, m_hashSize);

            return Convert.ToBase64String(hashed);
        }
        /// <summary>
        /// Verifies if the user enetered the correct password
        /// </summary>
        /// <param name="pass">the password</param>
        /// <param name="passHash">the hashed password</param>
        /// <returns></returns>
        private bool VerifyPassword(String pass, String passHash)
        {
            byte[] hashed = Convert.FromBase64String(passHash);
            // get salt
            byte[] salt = new byte[m_saltSize];
            Array.Copy(hashed, 0, salt, 0, m_saltSize);
            // generate hash
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(pass, salt, m_iterations, HashAlgorithmName.SHA256);
            byte[] hash = rfc.GetBytes(m_hashSize);
            // compare the two hashes
            for (int i = 0; i < m_hashSize; i++)
                if (hashed[i + m_saltSize] != hash[i])
                    return false;
            return true;
        }
    }
}
