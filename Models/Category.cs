// This file defines the Category class
// Categories are used to group products (e.g. Face, Lips, Eyes)

namespace InventoryManagementSystem.Models
{
    public class Category
    {
        // auto-increment ID for each category
        private static int _nextId = 1;

        // properties - using private set so they can't be changed from outside
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        // constructor - runs when we create a new Category object
        public Category(string name, string description)
        {
            Id          = _nextId++;
            Name        = name;
            Description = description;
        }

        // updates the category name and description
        public void Update(string name, string description)
        {
            Name        = name;
            Description = description;
        }

        // returns a readable string of this category
        public override string ToString()
        {
            return $"[{Id}] {Name} - {Description}";
        }
    }
}
