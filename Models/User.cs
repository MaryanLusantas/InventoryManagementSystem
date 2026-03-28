// This file defines the User class and UserRole enum
// Users can log in to the system and have different access levels

namespace InventoryManagementSystem.Models
{
    // roles that a user can have
    public enum UserRole
    {
        Admin,    // full access to everything
        Manager,  // can manage products, stock, and reports
        Staff     // can view products and do stock operations
    }

    public class User
    {
        // password is stored as a hash, not plain text, for security
        private string _passwordHash;

        // auto-increment ID for each user
        private static int _nextId = 1;

        // properties
        public int      Id        { get; private set; }
        public string   Username  { get; private set; }
        public string   FullName  { get; private set; }
        public string   Email     { get; private set; }
        public UserRole Role      { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool     IsActive  { get; private set; }

        // constructor - creates a new user and hashes the password right away
        public User(string username, string fullName, string email,
                    string password, UserRole role)
        {
            Id            = _nextId++;
            Username      = username;
            FullName      = fullName;
            Email         = email;
            _passwordHash = HashPassword(password); // never store plain text
            Role          = role;
            CreatedAt     = DateTime.Now;
            IsActive      = true; // new accounts are active by default
        }

        // simple hash function - converts password to a hashed string
        private string HashPassword(string password)
        {
            int hash = 0;
            foreach (char c in password)
                hash = (hash * 31) + c;
            return hash.ToString("X8");
        }

        // checks if the given password matches the stored hash
        public bool ValidatePassword(string password)
        {
            return _passwordHash == HashPassword(password);
        }

        // updates user info
        public void Update(string fullName, string email, UserRole role)
        {
            FullName = fullName;
            Email    = email;
            Role     = role;
        }

        // disables the account
        public void Deactivate() => IsActive = false;

        // re-enables the account
        public void Activate() => IsActive = true;

        // returns a readable string of this user
        public override string ToString()
        {
            return $"[{Id}] {Username} ({FullName}) | " +
                   $"Role: {Role} | Status: {(IsActive ? "Active" : "Inactive")}";
        }
    }
}
