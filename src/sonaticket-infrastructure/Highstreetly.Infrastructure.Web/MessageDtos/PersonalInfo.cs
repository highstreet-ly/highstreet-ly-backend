using System;

namespace Highstreetly.Infrastructure.MessageDtos
{
    public class PersonalInfo : IEquatable<PersonalInfo>
    {
        public string First { get; set; }
        public string Last { get; set; }
        public string Email { get; set; }

        #region Equality

        public static bool operator ==(PersonalInfo obj1, PersonalInfo obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(PersonalInfo obj1, PersonalInfo obj2)
        {
            return !(obj1 == obj2);
        }

        public bool Equals(PersonalInfo other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Equals(this, obj as PersonalInfo);
        }

        public static bool Equals(PersonalInfo obj1, PersonalInfo obj2)
        {
            if (Object.Equals(obj1, null) && Object.Equals(obj2, null)) return true;
            if (ReferenceEquals(obj1, obj2)) return true;

            if (Object.Equals(null, obj1) ||
                Object.Equals(null, obj2) ||
                obj1.GetType() != obj2.GetType())
                return false;

            // Compare your object properties
            return string.Equals(obj1.Email, obj2.Email, StringComparison.InvariantCultureIgnoreCase) &&
                   obj1.First == obj2.First &&
                   obj1.Last == obj2.Last;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (Email != null)
                hash ^= Email.GetHashCode();
            if (First != null)
                hash ^= First.GetHashCode();
            if (Last != null)
                hash ^= Last.GetHashCode();

            return hash;
        }

        #endregion
    }
}