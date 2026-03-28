// This file is the main service class of the system
// It handles all the data and business logic
// Program.cs calls the methods here to do things like add, search, update, delete

using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services
{
    public class InventoryService
    {
        // all data is stored in List<T> - no database used
        private List<Product>           _products;
        private List<Category>          _categories;
        private List<Supplier>          _suppliers;
        private List<User>              _users;
        private List<TransactionRecord> _transactions;

        // the user who is currently logged in (null if no one is logged in)
        public User? CurrentUser { get; private set; }

        // constructor - initializes all lists and loads sample data
        public InventoryService()
        {
            _products     = new List<Product>();
            _categories   = new List<Category>();
            _suppliers    = new List<Supplier>();
            _users        = new List<User>();
            _transactions = new List<TransactionRecord>();
            SeedData();
        }

        // loads starting data so the app has something to show right away
        private void SeedData()
        {
            // default user accounts
            _users.Add(new User("admin",    "System Administrator", "admin@glowbeauty.com",  "admin123",   UserRole.Admin));
            _users.Add(new User("manager1", "Maryan Lusantas",      "maryan@glowbeauty.com", "manager123", UserRole.Manager));
            _users.Add(new User("staff1",   "Sofia Dela Cruz",      "sofia@glowbeauty.com",  "staff123",   UserRole.Staff));
            _users.Add(new User("staff2",   "Andrea Santos",        "andrea@glowbeauty.com", "staff123",   UserRole.Staff));

            // product categories
            var face  = new Category("Face",            "Foundation, concealer, blush, bronzer, highlighter");
            var eyes  = new Category("Eyes",            "Eyeshadow, eyeliner, mascara, brow products");
            var lips  = new Category("Lips",            "Lipstick, lip gloss, lip liner, lip care");
            var skin  = new Category("Skincare",        "Moisturizers, serums, cleansers, toners, SPF");
            var tools = new Category("Tools & Brushes", "Makeup brushes, sponges, applicators");
            var nails = new Category("Nails",           "Nail polish, nail care, nail art accessories");
            _categories.AddRange(new[] { face, eyes, lips, skin, tools, nails });

            // suppliers
            var luxeBeauty  = new Supplier("LuxeBeauty Distrib. PH", "Camille Ong",
                                           "camille@luxebeauty.ph",   "09171112233", "Makati City, Metro Manila");
            var glowSource  = new Supplier("GlowSource Trading",      "Patricia Lim",
                                           "patricia@glowsource.com", "09282223344", "BGC, Taguig City");
            var beautyDepot = new Supplier("Beauty Depot Wholesale",  "Trisha Navarro",
                                           "trisha@beautydepot.com",  "09393334455", "Quezon City, Metro Manila");
            _suppliers.AddRange(new[] { luxeBeauty, glowSource, beautyDepot });

            // face products
            _products.Add(new Product("HD Matte Foundation",      "Full-coverage matte finish, 30 shades",   face.Id,  luxeBeauty.Id,  "GBY-FACE-001", 649.00m,  60, 15));
            _products.Add(new Product("Radiance Cushion SPF40",   "Dewy cushion foundation with SPF40",      face.Id,  glowSource.Id,  "GBY-FACE-002", 899.00m,  40, 10));
            _products.Add(new Product("Full Coverage Concealer",  "Creamy concealer, 20 shades",             face.Id,  luxeBeauty.Id,  "GBY-FACE-003", 349.00m,  75, 20));
            _products.Add(new Product("Baked Blush Palette",      "4-shade blush and bronzer duo palette",   face.Id,  beautyDepot.Id, "GBY-FACE-004", 549.00m,  30, 10));
            _products.Add(new Product("Loose Setting Powder",     "Translucent setting powder, oil-control", face.Id,  glowSource.Id,  "GBY-FACE-005", 429.00m,   7, 15));

            // eye products
            _products.Add(new Product("18-Pan Eyeshadow Palette", "Neutral to glam 18-shade palette",        eyes.Id,  luxeBeauty.Id,  "GBY-EYES-001", 799.00m,  45, 10));
            _products.Add(new Product("Waterproof Gel Eyeliner",  "Long-lasting gel liner, jet black",       eyes.Id,  glowSource.Id,  "GBY-EYES-002", 299.00m,  80, 20));
            _products.Add(new Product("Volume & Length Mascara",  "Buildable volume, smudge-proof formula",  eyes.Id,  beautyDepot.Id, "GBY-EYES-003", 379.00m,  55, 15));
            _products.Add(new Product("Brow Pencil & Spoolie",    "Micro-brow pencil, 6 shades",             eyes.Id,  luxeBeauty.Id,  "GBY-EYES-004", 249.00m,   9, 20));

            // lip products
            _products.Add(new Product("Velvet Matte Lipstick",    "Transfer-proof matte lipstick, 30 shades",lips.Id,  glowSource.Id,  "GBY-LIPS-001", 399.00m,  90, 25));
            _products.Add(new Product("Glossy Lip Oil",           "Nourishing tinted lip oil, 12 shades",    lips.Id,  beautyDepot.Id, "GBY-LIPS-002", 329.00m,  65, 15));
            _products.Add(new Product("Lip Liner Pencil",         "Long-wear lip liner, 24 shades",          lips.Id,  luxeBeauty.Id,  "GBY-LIPS-003", 199.00m, 110, 30));

            // skincare products
            _products.Add(new Product("Glow Serum Vit-C 30ml",   "Brightening Vitamin C serum, 30ml",       skin.Id,  glowSource.Id,  "GBY-SKIN-001", 799.00m,  25, 10));
            _products.Add(new Product("Hydra-Boost Moisturizer",  "72hr hydration moisturizer, 50ml",        skin.Id,  beautyDepot.Id, "GBY-SKIN-002", 549.00m,  38, 10));
            _products.Add(new Product("Gentle Foam Cleanser",     "pH-balanced daily foam cleanser, 150ml",  skin.Id,  luxeBeauty.Id,  "GBY-SKIN-003", 349.00m,  50, 15));
            _products.Add(new Product("SPF50 Sunscreen Fluid",    "Lightweight daily SPF50 sunscreen, 40ml", skin.Id,  glowSource.Id,  "GBY-SKIN-004", 469.00m,   6, 15));

            // tools and brushes
            _products.Add(new Product("Pro Brush Set (12pcs)",    "Complete 12-piece professional brush set", tools.Id, beautyDepot.Id, "GBY-TOOL-001", 999.00m,  20,  5));
            _products.Add(new Product("Beauty Blender Sponge",    "Seamless blending sponge, latex-free",    tools.Id, glowSource.Id,  "GBY-TOOL-002", 249.00m,  70, 20));

            // nail products
            _products.Add(new Product("Gel Nail Polish 15ml",     "Long-wear gel formula, 60 shades",        nails.Id, luxeBeauty.Id,  "GBY-NAIL-001", 199.00m, 120, 30));
            _products.Add(new Product("Nail Care Strengthener",   "Keratin nail strengthener and base coat", nails.Id, beautyDepot.Id, "GBY-NAIL-002", 149.00m,  45, 15));

            // log that the system started
            _transactions.Add(new TransactionRecord(
                0, "System", TransactionType.ProductAdded,
                0, 0, 0, "system", "Initial inventory data loaded."));
        }

        // --- LOGIN ---

        // checks if username and password are correct
        // returns true if login works, false if not
        public bool Login(string username, string password)
        {
            var user = _users.Find(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                && u.IsActive);

            if (user != null && user.ValidatePassword(password))
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        // logs out the current user
        public void Logout() => CurrentUser = null;

        // helper to get the current user's username for transaction logs
        private string CurrentUsername => CurrentUser?.Username ?? "system";

        // --- CATEGORY METHODS ---

        // adds a new category - name must not be blank or duplicate
        public void AddCategory(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty.");

            if (_categories.Exists(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Category '{name}' already exists.");

            _categories.Add(new Category(name, description));
        }

        // returns all categories
        public List<Category> GetAllCategories() => new List<Category>(_categories);

        // finds a category by its ID
        public Category? FindCategoryById(int id) => _categories.Find(c => c.Id == id);

        // returns just the name of a category by ID
        public string GetCategoryName(int id) => FindCategoryById(id)?.Name ?? "Unknown";

        // --- SUPPLIER METHODS ---

        // adds a new supplier
        public void AddSupplier(string name, string contactPerson,
                                string email, string phone, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Supplier name cannot be empty.");

            _suppliers.Add(new Supplier(name, contactPerson, email, phone, address));
        }

        // returns all suppliers
        public List<Supplier> GetAllSuppliers() => new List<Supplier>(_suppliers);

        // finds a supplier by its ID
        public Supplier? FindSupplierById(int id) => _suppliers.Find(s => s.Id == id);

        // returns just the name of a supplier by ID
        public string GetSupplierName(int id) => FindSupplierById(id)?.Name ?? "Unknown";

        // --- PRODUCT METHODS ---

        // adds a new product after checking all the inputs
        public void AddProduct(string name, string description, int categoryId,
                               int supplierId, string sku, decimal price,
                               int stockQuantity, int lowStockThreshold)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty.");
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU cannot be empty.");
            if (FindCategoryById(categoryId) == null)
                throw new InvalidOperationException("Category not found.");
            if (FindSupplierById(supplierId) == null)
                throw new InvalidOperationException("Supplier not found.");
            if (_products.Exists(p => p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"SKU '{sku}' is already used by another product.");

            var product = new Product(name, description, categoryId, supplierId,
                                      sku, price, stockQuantity, lowStockThreshold);
            _products.Add(product);

            // save a record of this action
            _transactions.Add(new TransactionRecord(
                product.Id, product.Name, TransactionType.ProductAdded,
                stockQuantity, 0, stockQuantity, CurrentUsername, "New product added."));
        }

        // returns all products
        public List<Product> GetAllProducts() => new List<Product>(_products);

        // finds a product by its ID
        public Product? FindProductById(int id) => _products.Find(p => p.Id == id);

        // searches products by name, SKU, or description
        public List<Product> SearchProducts(string keyword)
        {
            string kw = keyword.ToLower();
            return _products.FindAll(p =>
                p.Name.ToLower().Contains(kw)        ||
                p.SKU.ToLower().Contains(kw)         ||
                p.Description.ToLower().Contains(kw));
        }

        // updates a product's details
        public void UpdateProduct(int productId, string name, string description,
                                  int categoryId, int supplierId, string sku,
                                  decimal price, int lowStockThreshold)
        {
            var product = FindProductById(productId)
                ?? throw new InvalidOperationException($"Product ID {productId} not found.");

            if (FindCategoryById(categoryId) == null)
                throw new InvalidOperationException("Category not found.");
            if (FindSupplierById(supplierId) == null)
                throw new InvalidOperationException("Supplier not found.");

            // make sure the SKU is not already taken by a different product
            if (_products.Exists(p => p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase)
                && p.Id != productId))
                throw new InvalidOperationException($"SKU '{sku}' is already used by another product.");

            product.Update(name, description, categoryId, supplierId, sku, price, lowStockThreshold);

            // save a record of this update
            _transactions.Add(new TransactionRecord(
                product.Id, product.Name, TransactionType.ProductUpdated,
                0, product.StockQuantity, product.StockQuantity,
                CurrentUsername, "Product details updated."));
        }

        // removes a product from the inventory
        public void DeleteProduct(int productId)
        {
            var product = FindProductById(productId)
                ?? throw new InvalidOperationException($"Product ID {productId} not found.");

            // save the record before deleting
            _transactions.Add(new TransactionRecord(
                product.Id, product.Name, TransactionType.ProductDeleted,
                0, product.StockQuantity, 0, CurrentUsername, "Product deleted."));

            _products.Remove(product);
        }

        // adds units to a product's stock
        public void RestockProduct(int productId, int quantity, string remarks = "")
        {
            var product = FindProductById(productId)
                ?? throw new InvalidOperationException($"Product ID {productId} not found.");

            int before = product.StockQuantity;
            product.AddStock(quantity); // validation is inside the Product class

            _transactions.Add(new TransactionRecord(
                product.Id, product.Name, TransactionType.Restock,
                quantity, before, product.StockQuantity,
                CurrentUsername, string.IsNullOrWhiteSpace(remarks) ? "Restock" : remarks));
        }

        // removes units from a product's stock
        public void DeductStock(int productId, int quantity, string remarks = "")
        {
            var product = FindProductById(productId)
                ?? throw new InvalidOperationException($"Product ID {productId} not found.");

            int before = product.StockQuantity;
            product.DeductStock(quantity); // validation is inside the Product class

            _transactions.Add(new TransactionRecord(
                product.Id, product.Name, TransactionType.Deduction,
                quantity, before, product.StockQuantity,
                CurrentUsername, string.IsNullOrWhiteSpace(remarks) ? "Stock deduction" : remarks));
        }

        // --- REPORT METHODS ---

        // returns products that are low on stock
        public List<Product> GetLowStockProducts() => _products.FindAll(p => p.IsLowStock);

        // adds up the total value of all products in stock
        public decimal GetTotalInventoryValue() => _products.Sum(p => p.TotalValue);

        // returns all transaction records
        public List<TransactionRecord> GetTransactionHistory() =>
            new List<TransactionRecord>(_transactions);

        // --- USER METHODS ---

        // returns all users
        public List<User> GetAllUsers() => new List<User>(_users);

        // creates a new user account
        public void AddUser(string username, string fullName, string email,
                            string password, UserRole role)
        {
            if (_users.Exists(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Username '{username}' is already taken.");

            _users.Add(new User(username, fullName, email, password, role));
        }
    }
}
