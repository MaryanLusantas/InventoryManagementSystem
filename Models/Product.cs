// This file defines the Product class
// It stores all product info

namespace InventoryManagementSystem.Models
{
    public class Product
    {
        // private fields - these store the actual values
        // we use private fields so we can add validation before saving
        private decimal _price;
        private int     _stockQuantity;
        private int     _lowStockThreshold;

        // auto-increment ID for each product
        private static int _nextId = 1;

        // basic properties - no validation needed here
        public int    Id          { get; private set; }
        public string Name        { get; private set; }
        public string Description { get; private set; }
        public int    CategoryId  { get; private set; }
        public int    SupplierId  { get; private set; }
        public string SKU         { get; private set; }

        // price property - throws error if negative
        public decimal Price
        {
            get => _price;
            private set
            {
                if (value < 0)
                    throw new ArgumentException("Price cannot be negative.");
                _price = value;
            }
        }

        // stock quantity - throws error if it goes below zero
        public int StockQuantity
        {
            get => _stockQuantity;
            private set
            {
                if (value < 0)
                    throw new ArgumentException("Stock quantity cannot be negative.");
                _stockQuantity = value;
            }
        }

        // the minimum stock level before we get a low stock alert
        public int LowStockThreshold
        {
            get => _lowStockThreshold;
            private set
            {
                if (value < 0)
                    throw new ArgumentException("Low stock threshold cannot be negative.");
                _lowStockThreshold = value;
            }
        }

        // returns true if stock is at or below the threshold
        public bool IsLowStock => StockQuantity <= LowStockThreshold;

        // constructor - creates a new product with all required info
        public Product(string name, string description, int categoryId,
                       int supplierId, string sku, decimal price,
                       int stockQuantity, int lowStockThreshold)
        {
            Id                = _nextId++;
            Name              = name;
            Description       = description;
            CategoryId        = categoryId;
            SupplierId        = supplierId;
            SKU               = sku;
            Price             = price;
            StockQuantity     = stockQuantity;
            LowStockThreshold = lowStockThreshold;
        }

        // updates product details - stock is not updated here
        public void Update(string name, string description, int categoryId,
                           int supplierId, string sku, decimal price,
                           int lowStockThreshold)
        {
            Name              = name;
            Description       = description;
            CategoryId        = categoryId;
            SupplierId        = supplierId;
            SKU               = sku;
            Price             = price;
            LowStockThreshold = lowStockThreshold;
        }

        // adds units to stock - used when restocking
        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");
            StockQuantity += quantity;
        }

        // removes units from stock - used for sales or damage
        public void DeductStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");
            if (quantity > StockQuantity)
                throw new InvalidOperationException(
                    $"Not enough stock. Available: {StockQuantity}");
            StockQuantity -= quantity;
        }

        // computes total value of this product in stock (price x quantity)
        public decimal TotalValue => Price * StockQuantity;

        // returns a short summary of this product
        public override string ToString()
        {
            return $"[{Id}] {Name} (SKU: {SKU}) | Price: {Price:C} | " +
                   $"Stock: {StockQuantity}" + (IsLowStock ? " [LOW STOCK]" : "");
        }
    }
}
