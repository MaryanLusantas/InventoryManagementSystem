// This file defines the Supplier class
// Suppliers are the companies or people we buy products from

namespace InventoryManagementSystem.Models
{
    public class Supplier
    {
        // auto-increment ID for each supplier
        private static int _nextId = 1;

        // properties - private set means only this class can change them
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string ContactPerson { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string Address { get; private set; }

        // constructor - sets all supplier info when object is created
        public Supplier(string name, string contactPerson, string email,
                        string phone, string address)
        {
            Id            = _nextId++;
            Name          = name;
            ContactPerson = contactPerson;
            Email         = email;
            Phone         = phone;
            Address       = address;
        }

        // updates the supplier's information
        public void Update(string name, string contactPerson, string email,
                           string phone, string address)
        {
            Name          = name;
            ContactPerson = contactPerson;
            Email         = email;
            Phone         = phone;
            Address       = address;
        }

        // returns a readable string of this supplier
        public override string ToString()
        {
            return $"[{Id}] {Name} | Contact: {ContactPerson} | Phone: {Phone}";
        }
    }
}
