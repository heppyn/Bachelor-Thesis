using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermostatControler
{
    public class User : IEquatable<User>
    {
        public readonly String Name;
        public readonly String Pass;

        public User(string name, string pass)
        {
            Name = name;
            Pass = pass;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        public bool Equals(User other)
        {
            return other != null &&
                   Name == other.Name &&
                   Pass == other.Pass;
        }

        public override int GetHashCode()
        {
            var hashCode = 41748270;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Pass);
            return hashCode;
        }

        public bool HasSameName(User other)
        {
            return other != null && Name.Equals(other.Name);
        }

        public static bool operator ==(User user1, User user2)
        {
            return EqualityComparer<User>.Default.Equals(user1, user2);
        }

        public static bool operator !=(User user1, User user2)
        {
            return !(user1 == user2);
        }
    }
}
