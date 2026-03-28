// This is the main file of the program
// It handles the login screen, main menu, and all the sub-menus
// It uses InventoryService to do the actual work and UIHelper to display things

using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;

class Program
{
    // one shared service object used by all methods in this file
    private static InventoryService _service = new InventoryService();

    // starting point of the app
    static void Main(string[] args)
    {
        Console.Title = "Glow Beauty Shop  •  Makeup & Skincare Inventory System";
        Console.OutputEncoding = System.Text.Encoding.UTF8; // needed for box-drawing characters

        SplashScreen();

        // show login - if user fails 3 times, exit the program
        if (!LoginScreen())
        {
            UIHelper.ClearScreen();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n  Access denied. Goodbye.\n");
            Console.ResetColor();
            return;
        }

        MainMenu();
    }

    // shows a startup screen with a short loading animation
    static void SplashScreen()
    {
        UIHelper.ClearScreen();
        Console.WriteLine();
        UIHelper.PrintBanner();
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  Initializing Glow Beauty Shop system...");
        System.Threading.Thread.Sleep(400);
        Console.WriteLine("  Loading makeup & skincare inventory...");
        System.Threading.Thread.Sleep(400);
        Console.WriteLine("  System ready. Let's glow!\n");
        System.Threading.Thread.Sleep(300);
        Console.ResetColor();
    }

    // shows the login form and checks the credentials
    // allows up to 3 attempts before locking out
    static bool LoginScreen()
    {
        int attempts = 0;

        while (attempts < 3)
        {
            UIHelper.ClearScreen();
            UIHelper.PrintBanner();
            Console.WriteLine();
            UIHelper.PrintLoginBox();

            string username = UIHelper.PromptInput("  Username");
            string password = UIHelper.PromptPassword("  Password");

            UIHelper.PrintLoginBoxBottom();

            try
            {
                if (_service.Login(username, password))
                {
                    UIHelper.PrintSuccess(
                        $"Welcome back, {_service.CurrentUser!.FullName}!" +
                        $"  Role: {_service.CurrentUser.Role}");
                    UIHelper.PressAnyKey();
                    return true;
                }

                UIHelper.PrintError("Invalid username or password.");
                attempts++;

                if (attempts < 3)
                    UIHelper.PrintWarning($"{3 - attempts} login attempt(s) remaining.");

                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.PrintError($"Login error: {ex.Message}");
                UIHelper.PressAnyKey();
                attempts++;
            }
        }

        UIHelper.PrintError("Too many failed attempts. System locked.");
        UIHelper.PressAnyKey();
        return false;
    }

    // main menu - shows the dashboard and routes to sub-menus
    static void MainMenu()
    {
        while (true)
        {
            UIHelper.ClearScreen();
            UIHelper.PrintBanner();
            Console.WriteLine();

            // show who is logged in and current date/time
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  User: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(_service.CurrentUser!.FullName);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  |  Role: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(_service.CurrentUser.Role.ToString());
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"  |  {DateTime.Now:dddd, MMMM dd, yyyy  HH:mm}");
            Console.ResetColor();
            Console.WriteLine();
            UIHelper.PrintThinDivider();

            // live dashboard cards - updates every time the menu loads
            var products = _service.GetAllProducts();
            var lowStock = _service.GetLowStockProducts();
            UIHelper.PrintStatCards(
                ("Total Products",   products.Count.ToString(),
                                     ConsoleColor.Magenta),
                ("Low Stock Alerts", lowStock.Count.ToString(),
                                     lowStock.Count > 0 ? ConsoleColor.Red : ConsoleColor.Green),
                ("Inventory Value",  $"PHP {_service.GetTotalInventoryValue():N2}",
                                     ConsoleColor.Yellow),
                ("Transactions",     _service.GetTransactionHistory().Count.ToString(),
                                     ConsoleColor.DarkMagenta)
            );

            UIHelper.PrintThinDivider();
            UIHelper.PrintMenuGroup("Inventory");
            UIHelper.PrintMenuItem("1", "Category Management",  "Add & view product categories");
            UIHelper.PrintMenuItem("2", "Supplier Management",  "Add & view beauty suppliers");
            UIHelper.PrintMenuItem("3", "Product Management",   "Add, search, update, delete products");
            UIHelper.PrintMenuGroup("Operations");
            UIHelper.PrintMenuItem("4", "Stock Operations",     "Restock or deduct inventory stock");
            UIHelper.PrintMenuItem("5", "Reports & Analytics",  "Low stock, value, inventory summary");
            UIHelper.PrintMenuItem("6", "Transaction History",  "Full audit trail of all changes");
            UIHelper.PrintMenuGroup("Administration");
            UIHelper.PrintMenuItem("7", "User Management",      "Manage system users  [Admin only]");
            UIHelper.PrintMenuItem("0", "Logout",               "End current session");
            UIHelper.PrintThinDivider();
            UIHelper.PrintChoicePrompt();

            switch (Console.ReadLine()?.Trim())
            {
                case "1": CategoryMenu();           break;
                case "2": SupplierMenu();           break;
                case "3": ProductMenu();            break;
                case "4": StockMenu();              break;
                case "5": ReportsMenu();            break;
                case "6": ViewTransactionHistory(); break;
                case "7": UserMenu();               break;
                case "0":
                    if (UIHelper.PromptConfirm("Logout and end current session?"))
                    {
                        _service.Logout();
                        UIHelper.PrintSuccess("Session ended. Goodbye!");
                        UIHelper.PressAnyKey();
                        if (!LoginScreen()) return;
                    }
                    break;
                default:
                    UIHelper.PrintError("Invalid choice. Please enter a number from the menu.");
                    UIHelper.PressAnyKey();
                    break;
            }
        }
    }

    // =========================================================
    // CATEGORY MANAGEMENT
    // =========================================================

    static void CategoryMenu()
    {
        while (true)
        {
            UIHelper.ClearScreen();
            UIHelper.PrintHeader("Category Management", ConsoleColor.DarkMagenta);
            UIHelper.PrintMenuGroup("Options");
            UIHelper.PrintMenuItem("1", "View All Categories");
            UIHelper.PrintMenuItem("2", "Add New Category");
            UIHelper.PrintMenuItem("0", "Back to Main Menu");
            UIHelper.PrintThinDivider();
            UIHelper.PrintChoicePrompt();

            switch (Console.ReadLine()?.Trim())
            {
                case "1": ViewAllCategories(); break;
                case "2": AddCategory();       break;
                case "0": return;
                default:
                    UIHelper.PrintError("Invalid choice.");
                    UIHelper.PressAnyKey();
                    break;
            }
        }
    }

    // shows all categories in a table
    static void ViewAllCategories()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("All Beauty Categories", ConsoleColor.DarkMagenta);

        var cats = _service.GetAllCategories();

        if (cats.Count == 0)
        {
            UIHelper.PrintWarning("No categories found. Please add one first.");
            UIHelper.PressAnyKey();
            return;
        }

        UIHelper.PrintTableHeader(("ID", 4), ("Category Name", 18), ("Description", 52));
        foreach (var c in cats)
            UIHelper.PrintTableRow(ConsoleColor.White,
                (c.Id.ToString(), 4), (c.Name, 18), (c.Description, 52));
        UIHelper.PrintTableFooter(4, 18, 52);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"\n  {cats.Count} category/categories found.");
        Console.ResetColor();
        UIHelper.PressAnyKey();
    }

    // collects input and saves a new category
    static void AddCategory()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("Add New Category", ConsoleColor.DarkMagenta);

        try
        {
            string name = UIHelper.PromptInput("Category Name");

            if (string.IsNullOrWhiteSpace(name))
            {
                UIHelper.PrintError("Category name cannot be blank.");
                UIHelper.PressAnyKey();
                return;
            }

            string desc = UIHelper.PromptInput("Description");
            _service.AddCategory(name, desc);
            UIHelper.PrintSuccess($"Category '{name}' added successfully!");
        }
        catch (Exception ex)
        {
            UIHelper.PrintError(ex.Message);
        }

        UIHelper.PressAnyKey();
    }

    // =========================================================
    // SUPPLIER MANAGEMENT
    // =========================================================

    static void SupplierMenu()
    {
        while (true)
        {
            UIHelper.ClearScreen();
            UIHelper.PrintHeader("Supplier Management", ConsoleColor.DarkMagenta);
            UIHelper.PrintMenuGroup("Options");
            UIHelper.PrintMenuItem("1", "View All Suppliers");
            UIHelper.PrintMenuItem("2", "Add New Supplier");
            UIHelper.PrintMenuItem("0", "Back to Main Menu");
            UIHelper.PrintThinDivider();
            UIHelper.PrintChoicePrompt();

            switch (Console.ReadLine()?.Trim())
            {
                case "1": ViewAllSuppliers(); break;
                case "2": AddSupplier();      break;
                case "0": return;
                default:
                    UIHelper.PrintError("Invalid choice.");
                    UIHelper.PressAnyKey();
                    break;
            }
        }
    }

    // shows all suppliers in a table
    static void ViewAllSuppliers()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("All Suppliers", ConsoleColor.DarkMagenta);

        var suppliers = _service.GetAllSuppliers();

        if (suppliers.Count == 0)
        {
            UIHelper.PrintWarning("No suppliers found.");
            UIHelper.PressAnyKey();
            return;
        }

        UIHelper.PrintTableHeader(
            ("ID", 4), ("Supplier Name", 26), ("Contact Person", 16),
            ("Email Address", 28), ("Phone", 13));

        foreach (var s in suppliers)
            UIHelper.PrintTableRow(ConsoleColor.White,
                (s.Id.ToString(), 4), (s.Name, 26),
                (s.ContactPerson, 16), (s.Email, 28), (s.Phone, 13));

        UIHelper.PrintTableFooter(4, 26, 16, 28, 13);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"\n  {suppliers.Count} supplier(s) found.");
        Console.ResetColor();
        UIHelper.PressAnyKey();
    }

    // collects input and saves a new supplier
    static void AddSupplier()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("Add New Supplier", ConsoleColor.DarkMagenta);

        try
        {
            string name    = UIHelper.PromptInput("Supplier Name");
            string contact = UIHelper.PromptInput("Contact Person");
            string email   = UIHelper.PromptInput("Email Address");
            string phone   = UIHelper.PromptInput("Phone Number");
            string address = UIHelper.PromptInput("Complete Address");
            _service.AddSupplier(name, contact, email, phone, address);
            UIHelper.PrintSuccess($"Supplier '{name}' added successfully!");
        }
        catch (Exception ex)
        {
            UIHelper.PrintError(ex.Message);
        }

        UIHelper.PressAnyKey();
    }

    // =========================================================
    // PRODUCT MANAGEMENT
    // =========================================================

    static void ProductMenu()
    {
        while (true)
        {
            UIHelper.ClearScreen();
            UIHelper.PrintHeader("Product Management", ConsoleColor.Magenta);
            UIHelper.PrintMenuGroup("Options");
            UIHelper.PrintMenuItem("1", "View All Products");
            UIHelper.PrintMenuItem("2", "Add New Product");
            UIHelper.PrintMenuItem("3", "Search Products");
            UIHelper.PrintMenuItem("4", "Update Product");
            UIHelper.PrintMenuItem("5", "Delete Product");
            UIHelper.PrintMenuItem("0", "Back to Main Menu");
            UIHelper.PrintThinDivider();
            UIHelper.PrintChoicePrompt();

            switch (Console.ReadLine()?.Trim())
            {
                case "1": ViewAllProducts(); break;
                case "2": AddProduct();      break;
                case "3": SearchProducts();  break;
                case "4": UpdateProduct();   break;
                case "5": DeleteProduct();   break;
                case "0": return;
                default:
                    UIHelper.PrintError("Invalid choice.");
                    UIHelper.PressAnyKey();
                    break;
            }
        }
    }

    // shows all products - low stock rows are shown in red
    static void ViewAllProducts()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("All Makeup & Skincare Products", ConsoleColor.Magenta);

        var products = _service.GetAllProducts();

        if (products.Count == 0)
        {
            UIHelper.PrintWarning("No products in inventory yet.");
            UIHelper.PressAnyKey();
            return;
        }

        UIHelper.PrintTableHeader(
            ("ID", 3), ("SKU Code", 13), ("Product Name", 26),
            ("Category", 16), ("Price (PHP)", 11), ("Stock", 5), ("Status", 9));

        foreach (var p in products)
        {
            bool isLow = p.IsLowStock;
            ConsoleColor rowColor = isLow ? ConsoleColor.Red : ConsoleColor.White;
            string status = isLow ? "LOW STOCK" : "   OK    ";

            UIHelper.PrintTableRow(rowColor,
                (p.Id.ToString(), 3), (p.SKU, 13), (p.Name, 26),
                (_service.GetCategoryName(p.CategoryId), 16),
                ($"{p.Price:N2}", 11), (p.StockQuantity.ToString(), 5),
                (status, 9));
        }

        UIHelper.PrintTableFooter(3, 13, 26, 16, 11, 5, 9);

        // summary line
        int totalUnits = products.Sum(p => p.StockQuantity);
        int lowCount   = products.Count(p => p.IsLowStock);
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  Total Products: {products.Count}  |" +
                          $"  Total Units: {totalUnits:N0}  |" +
                          $"  Low Stock Items: {lowCount}");
        Console.ResetColor();
        UIHelper.PressAnyKey();
    }

    // shows available categories and suppliers, then collects product info
    static void AddProduct()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("Add New Product", ConsoleColor.Magenta);

        var categories = _service.GetAllCategories();
        var suppliers  = _service.GetAllSuppliers();

        // can't add a product without categories and suppliers
        if (categories.Count == 0)
        {
            UIHelper.PrintError("No categories found. Please add a category first.");
            UIHelper.PressAnyKey();
            return;
        }
        if (suppliers.Count == 0)
        {
            UIHelper.PrintError("No suppliers found. Please add a supplier first.");
            UIHelper.PressAnyKey();
            return;
        }

        // show available categories so user knows which ID to pick
        UIHelper.PrintSection("Available Categories");
        UIHelper.PrintTableHeader(("ID", 4), ("Name", 18), ("Description", 52));
        foreach (var c in categories)
            UIHelper.PrintTableRow(ConsoleColor.White,
                (c.Id.ToString(), 4), (c.Name, 18), (c.Description, 52));
        UIHelper.PrintTableFooter(4, 18, 52);

        // show available suppliers
        UIHelper.PrintSection("Available Suppliers");
        UIHelper.PrintTableHeader(("ID", 4), ("Supplier Name", 26), ("Contact", 16), ("Phone", 13));
        foreach (var s in suppliers)
            UIHelper.PrintTableRow(ConsoleColor.White,
                (s.Id.ToString(), 4), (s.Name, 26), (s.ContactPerson, 16), (s.Phone, 13));
        UIHelper.PrintTableFooter(4, 26, 16, 13);

        UIHelper.PrintSection("Enter New Product Details");

        try
        {
            string  name      = UIHelper.PromptInput("Product Name");
            string  desc      = UIHelper.PromptInput("Description");
            int     catId     = UIHelper.PromptInt("Category ID", 1);
            int     supId     = UIHelper.PromptInt("Supplier ID", 1);
            string  sku       = UIHelper.PromptInput("SKU Code (e.g. GBY-FACE-007)");
            decimal price     = UIHelper.PromptDecimal("Price in PHP (e.g. 499.00)", 0);
            int     qty       = UIHelper.PromptInt("Initial Stock Quantity", 0);
            int     threshold = UIHelper.PromptInt("Low Stock Alert Threshold", 0);

            if (!UIHelper.PromptConfirm("Save this new product?"))
            {
                UIHelper.PrintInfo("Cancelled. Product was not saved.");
                UIHelper.PressAnyKey();
                return;
            }

            _service.AddProduct(name, desc, catId, supId, sku, price, qty, threshold);
            UIHelper.PrintSuccess($"Product '{name}' added to inventory!");
        }
        catch (Exception ex)
        {
            UIHelper.PrintError(ex.Message);
        }

        UIHelper.PressAnyKey();
    }

    // searches products by name, SKU, or description
    static void SearchProducts()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("Search Products", ConsoleColor.Magenta);

        string keyword = UIHelper.PromptInput("Enter keyword  (name, SKU, or description)");

        if (string.IsNullOrWhiteSpace(keyword))
        {
            UIHelper.PrintError("Keyword cannot be empty.");
            UIHelper.PressAnyKey();
            return;
        }

        var results = _service.SearchProducts(keyword);
        UIHelper.PrintSection($"Results for: \"{keyword}\"");

        if (results.Count == 0)
        {
            UIHelper.PrintWarning($"No products matched '{keyword}'.");
            UIHelper.PressAnyKey();
            return;
        }

        UIHelper.PrintTableHeader(
            ("ID", 3), ("SKU Code", 13), ("Product Name", 26),
            ("Category", 16), ("Price (PHP)", 11), ("Stock", 5));

        foreach (var p in results)
        {
            UIHelper.PrintTableRow(ConsoleColor.White,
                (p.Id.ToString(), 3), (p.SKU, 13), (p.Name, 26),
                (_service.GetCategoryName(p.CategoryId), 16),
                ($"{p.Price:N2}", 11), (p.StockQuantity.ToString(), 5));

            // show description and supplier outside the table
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"       Description : {p.Description}");
            Console.WriteLine($"       Supplier    : {_service.GetSupplierName(p.SupplierId)}");
            Console.ResetColor();
        }

        UIHelper.PrintTableFooter(3, 13, 26, 16, 11, 5);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"\n  {results.Count} product(s) found.");
        Console.ResetColor();
        UIHelper.PressAnyKey();
    }

    // shows current product info and lets the user change any field
    // pressing Enter without typing keeps the old value
    static void UpdateProduct()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("Update Product", ConsoleColor.Magenta);

        int productId = UIHelper.PromptInt("Enter the Product ID to update", 1);
        var product   = _service.FindProductById(productId);

        if (product == null)
        {
            UIHelper.PrintError($"Product ID {productId} not found.");
            UIHelper.PressAnyKey();
            return;
        }

        // show current values so user knows what to change
        UIHelper.PrintSection("Current Product Information");
        UIHelper.PrintTableHeader(("Field", 24), ("Current Value", 50));
        UIHelper.PrintTableRow(ConsoleColor.White,    ("Product Name",        24), (product.Name,                                                              50));
        UIHelper.PrintTableRow(ConsoleColor.White,    ("Description",         24), (product.Description,                                                       50));
        UIHelper.PrintTableRow(ConsoleColor.White,    ("SKU Code",            24), (product.SKU,                                                               50));
        UIHelper.PrintTableRow(ConsoleColor.Yellow,   ("Price (PHP)",         24), ($"{product.Price:N2}",                                                     50));
        UIHelper.PrintTableRow(ConsoleColor.Magenta,  ("Category ID & Name",  24), ($"{product.CategoryId}  -  {_service.GetCategoryName(product.CategoryId)}", 50));
        UIHelper.PrintTableRow(ConsoleColor.Magenta,  ("Supplier ID & Name",  24), ($"{product.SupplierId}  -  {_service.GetSupplierName(product.SupplierId)}", 50));
        UIHelper.PrintTableRow(ConsoleColor.White,    ("Low Stock Threshold", 24), (product.LowStockThreshold.ToString(),                                      50));
        UIHelper.PrintTableRow(ConsoleColor.DarkGray, ("Current Stock",       24), ($"{product.StockQuantity} units  (use Stock Operations to change)",        50));
        UIHelper.PrintTableFooter(24, 50);

        UIHelper.PrintSection("Enter New Values  —  press Enter to keep current value");

        try
        {
            string  name      = FallbackPrompt("Product Name",  product.Name);
            string  desc      = FallbackPrompt("Description",   product.Description);
            string  sku       = FallbackPrompt("SKU Code",      product.SKU);
            decimal price     = FallbackDecimalPrompt($"Price (current: {product.Price:N2})",                     product.Price);
            int     catId     = FallbackIntPrompt($"Category ID (current: {product.CategoryId})",                product.CategoryId);
            int     supId     = FallbackIntPrompt($"Supplier ID (current: {product.SupplierId})",                product.SupplierId);
            int     threshold = FallbackIntPrompt($"Low Stock Threshold (current: {product.LowStockThreshold})", product.LowStockThreshold);

            if (!UIHelper.PromptConfirm("Save all changes?"))
            {
                UIHelper.PrintInfo("Update cancelled. No changes were saved.");
                UIHelper.PressAnyKey();
                return;
            }

            _service.UpdateProduct(productId, name, desc, catId, supId, sku, price, threshold);
            UIHelper.PrintSuccess($"Product '{name}' updated successfully!");
        }
        catch (Exception ex)
        {
            UIHelper.PrintError(ex.Message);
        }

        UIHelper.PressAnyKey();
    }

    // shows product details and asks for confirmation before deleting
    static void DeleteProduct()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("Delete Product", ConsoleColor.Red);

        int productId = UIHelper.PromptInt("Enter the Product ID to delete", 1);
        var product   = _service.FindProductById(productId);

        if (product == null)
        {
            UIHelper.PrintError($"Product ID {productId} not found.");
            UIHelper.PressAnyKey();
            return;
        }

        // show what will be deleted before confirming
        UIHelper.PrintSection("Product Selected for Deletion");
        UIHelper.PrintTableHeader(("Field", 20), ("Value", 50));
        UIHelper.PrintTableRow(ConsoleColor.White,    ("Product Name",  20), (product.Name,                                  50));
        UIHelper.PrintTableRow(ConsoleColor.White,    ("SKU Code",      20), (product.SKU,                                   50));
        UIHelper.PrintTableRow(ConsoleColor.White,    ("Description",   20), (product.Description,                           50));
        UIHelper.PrintTableRow(ConsoleColor.Yellow,   ("Price (PHP)",   20), ($"{product.Price:N2}",                         50));
        UIHelper.PrintTableRow(ConsoleColor.Red,      ("Current Stock", 20), ($"{product.StockQuantity} units",              50));
        UIHelper.PrintTableRow(ConsoleColor.Magenta,  ("Category",      20), (_service.GetCategoryName(product.CategoryId),  50));
        UIHelper.PrintTableRow(ConsoleColor.Magenta,  ("Supplier",      20), (_service.GetSupplierName(product.SupplierId),  50));
        UIHelper.PrintTableFooter(20, 50);

        UIHelper.PrintWarning("This action is PERMANENT and cannot be undone!");

        if (!UIHelper.PromptConfirm($"Confirm deletion of '{product.Name}'?"))
        {
            UIHelper.PrintInfo("Deletion cancelled.");
            UIHelper.PressAnyKey();
            return;
        }

        try
        {
            string name = product.Name;
            _service.DeleteProduct(productId);
            UIHelper.PrintSuccess($"Product '{name}' has been permanently deleted.");
        }
        catch (Exception ex)
        {
            UIHelper.PrintError(ex.Message);
        }

        UIHelper.PressAnyKey();
    }

    // =========================================================
    // STOCK OPERATIONS
    // =========================================================

    static void StockMenu()
    {
        while (true)
        {
            UIHelper.ClearScreen();
            UIHelper.PrintHeader("Stock Operations", ConsoleColor.Magenta);
            UIHelper.PrintMenuGroup("Options");
            UIHelper.PrintMenuItem("1", "Restock Product",  "Add units to a product's stock");
            UIHelper.PrintMenuItem("2", "Deduct Stock",     "Remove units from stock");
            UIHelper.PrintMenuItem("0", "Back to Main Menu");
            UIHelper.PrintThinDivider();
            UIHelper.PrintChoicePrompt();

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RestockProduct(); break;
                case "2": DeductStock();    break;
                case "0": return;
                default:
                    UIHelper.PrintError("Invalid choice.");
                    UIHelper.PressAnyKey();
                    break;
            }
        }
    }

    // adds units to a product's stock
    static void RestockProduct()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("Restock Product", ConsoleColor.Green);

        int productId = UIHelper.PromptInt("Enter Product ID to restock", 1);
        var product   = _service.FindProductById(productId);

        if (product == null)
        {
            UIHelper.PrintError($"Product ID {productId} not found.");
            UIHelper.PressAnyKey();
            return;
        }

        UIHelper.PrintSection("Product Information");
        UIHelper.PrintTableHeader(("Field", 22), ("Value", 45));
        UIHelper.PrintTableRow(ConsoleColor.White, ("Product Name",   22), (product.Name, 45));
        UIHelper.PrintTableRow(ConsoleColor.White, ("SKU Code",       22), (product.SKU,  45));
        UIHelper.PrintTableRow(ConsoleColor.White, ("Category",       22), (_service.GetCategoryName(product.CategoryId), 45));
        UIHelper.PrintTableRow(
            product.IsLowStock ? ConsoleColor.Red : ConsoleColor.Green,
            ("Current Stock", 22),
            ($"{product.StockQuantity} units" + (product.IsLowStock ? "  <<  LOW STOCK" : ""), 45));
        UIHelper.PrintTableRow(ConsoleColor.DarkGray, ("Alert Threshold", 22), ($"{product.LowStockThreshold} units", 45));
        UIHelper.PrintTableFooter(22, 45);

        try
        {
            int qty = UIHelper.PromptInt("Quantity to Add", 1);
            string remarks = UIHelper.PromptInput("Remarks or Reference Number (optional)");

            if (!UIHelper.PromptConfirm($"Confirm adding {qty} units to '{product.Name}'?"))
            {
                UIHelper.PrintInfo("Restock cancelled.");
                UIHelper.PressAnyKey();
                return;
            }

            int before = product.StockQuantity;
            _service.RestockProduct(productId, qty, remarks);
            var updated = _service.FindProductById(productId)!;
            UIHelper.PrintSuccess(
                $"Restocked!  '{product.Name}'  |  Before: {before}  ->  After: {updated.StockQuantity}");
        }
        catch (Exception ex)
        {
            UIHelper.PrintError(ex.Message);
        }

        UIHelper.PressAnyKey();
    }

    // removes units from a product's stock (sales, damage, etc.)
    static void DeductStock()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("Deduct Stock", ConsoleColor.Yellow);

        int productId = UIHelper.PromptInt("Enter Product ID to deduct from", 1);
        var product   = _service.FindProductById(productId);

        if (product == null)
        {
            UIHelper.PrintError($"Product ID {productId} not found.");
            UIHelper.PressAnyKey();
            return;
        }

        UIHelper.PrintSection("Product Information");
        UIHelper.PrintTableHeader(("Field", 22), ("Value", 45));
        UIHelper.PrintTableRow(ConsoleColor.White, ("Product Name",   22), (product.Name, 45));
        UIHelper.PrintTableRow(ConsoleColor.White, ("SKU Code",       22), (product.SKU,  45));
        UIHelper.PrintTableRow(ConsoleColor.White, ("Category",       22), (_service.GetCategoryName(product.CategoryId), 45));
        UIHelper.PrintTableRow(ConsoleColor.Green, ("Available Stock",22), ($"{product.StockQuantity} units", 45));
        UIHelper.PrintTableFooter(22, 45);

        if (product.StockQuantity == 0)
        {
            UIHelper.PrintError("This product has 0 stock. Nothing to deduct.");
            UIHelper.PressAnyKey();
            return;
        }

        try
        {
            int qty = UIHelper.PromptInt($"Quantity to Deduct (1 to {product.StockQuantity})", 1, product.StockQuantity);
            string remarks = UIHelper.PromptInput("Remarks or Sales Order Number (optional)");

            if (!UIHelper.PromptConfirm($"Confirm deducting {qty} units from '{product.Name}'?"))
            {
                UIHelper.PrintInfo("Deduction cancelled.");
                UIHelper.PressAnyKey();
                return;
            }

            int before = product.StockQuantity;
            _service.DeductStock(productId, qty, remarks);
            var updated = _service.FindProductById(productId)!;
            UIHelper.PrintSuccess(
                $"Deducted!  '{product.Name}'  |  Before: {before}  ->  After: {updated.StockQuantity}");

            // warn if stock is now low
            if (updated.IsLowStock)
                UIHelper.PrintWarning(
                    $"'{updated.Name}' is now below the alert threshold! " +
                    $"Stock: {updated.StockQuantity} / Threshold: {updated.LowStockThreshold}");
        }
        catch (Exception ex)
        {
            UIHelper.PrintError(ex.Message);
        }

        UIHelper.PressAnyKey();
    }

    // =========================================================
    // REPORTS & ANALYTICS
    // =========================================================

    static void ReportsMenu()
    {
        while (true)
        {
            UIHelper.ClearScreen();
            UIHelper.PrintHeader("Reports & Analytics", ConsoleColor.Yellow);
            UIHelper.PrintMenuGroup("Available Reports");
            UIHelper.PrintMenuItem("1", "Low Stock Alert",       "Products at or below minimum threshold");
            UIHelper.PrintMenuItem("2", "Total Inventory Value", "Value per product and grand total");
            UIHelper.PrintMenuItem("3", "Inventory Summary",     "Full overview with category breakdown");
            UIHelper.PrintMenuItem("0", "Back to Main Menu");
            UIHelper.PrintThinDivider();
            UIHelper.PrintChoicePrompt();

            switch (Console.ReadLine()?.Trim())
            {
                case "1": ShowLowStockItems();       break;
                case "2": ShowTotalInventoryValue(); break;
                case "3": ShowInventorySummary();    break;
                case "0": return;
                default:
                    UIHelper.PrintError("Invalid choice.");
                    UIHelper.PressAnyKey();
                    break;
            }
        }
    }

    // shows products that are at or below their minimum stock level
    static void ShowLowStockItems()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("Low Stock Alert", ConsoleColor.Red);

        var low = _service.GetLowStockProducts();

        if (low.Count == 0)
        {
            UIHelper.PrintSuccess("All products have sufficient stock. No alerts.");
            UIHelper.PressAnyKey();
            return;
        }

        UIHelper.PrintWarning($"{low.Count} product(s) are at or below their minimum stock threshold!");

        UIHelper.PrintTableHeader(
            ("ID", 3), ("SKU Code", 13), ("Product Name", 26),
            ("Category", 16), ("Stock", 5), ("Threshold", 9), ("Deficit", 7));

        foreach (var p in low)
        {
            int deficit = p.StockQuantity - p.LowStockThreshold; // will be 0 or negative
            UIHelper.PrintTableRow(ConsoleColor.Red,
                (p.Id.ToString(), 3), (p.SKU, 13), (p.Name, 26),
                (_service.GetCategoryName(p.CategoryId), 16),
                (p.StockQuantity.ToString(), 5),
                (p.LowStockThreshold.ToString(), 9),
                (deficit.ToString(), 7));
        }

        UIHelper.PrintTableFooter(3, 13, 26, 16, 5, 9, 7);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n  Go to Stock Operations > Restock Product to fix these.");
        Console.ResetColor();
        UIHelper.PressAnyKey();
    }

    // shows the value of each product and the grand total
    static void ShowTotalInventoryValue()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("Total Inventory Value", ConsoleColor.Yellow);

        var products = _service.GetAllProducts();

        UIHelper.PrintTableHeader(
            ("ID", 3), ("Product Name", 26),
            ("Unit Price", 11), ("Stock", 5), ("Total Value (PHP)", 18));

        decimal grandTotal = 0;

        foreach (var p in products)
        {
            decimal tv  = p.TotalValue; // price x stock quantity
            grandTotal += tv;
            UIHelper.PrintTableRow(ConsoleColor.White,
                (p.Id.ToString(), 3), (p.Name, 26),
                ($"{p.Price:N2}", 11), (p.StockQuantity.ToString(), 5),
                ($"{tv:N2}", 18));
        }

        UIHelper.PrintTableFooter(3, 26, 11, 5, 18);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  GRAND TOTAL INVENTORY VALUE:  PHP {grandTotal:N2}");
        Console.ResetColor();
        UIHelper.PressAnyKey();
    }

    // shows a full summary with stats and a breakdown per category
    static void ShowInventorySummary()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("Inventory Summary — Glow Beauty Shop", ConsoleColor.Yellow);

        var products     = _service.GetAllProducts();
        var categories   = _service.GetAllCategories();
        var suppliers    = _service.GetAllSuppliers();
        var transactions = _service.GetTransactionHistory();
        var lowStock     = _service.GetLowStockProducts();

        // stat cards row 1
        UIHelper.PrintStatCards(
            ("Total Products",   products.Count.ToString(),     ConsoleColor.Magenta),
            ("Total Categories", categories.Count.ToString(),   ConsoleColor.Magenta),
            ("Total Suppliers",  suppliers.Count.ToString(),    ConsoleColor.Magenta),
            ("Transactions",     transactions.Count.ToString(), ConsoleColor.DarkMagenta)
        );

        // stat cards row 2
        UIHelper.PrintStatCards(
            ("Units In Stock",   products.Sum(p => p.StockQuantity).ToString("N0"),  ConsoleColor.White),
            ("Low Stock Items",  lowStock.Count.ToString(),
                                 lowStock.Count > 0 ? ConsoleColor.Red : ConsoleColor.Green),
            ("Inventory Value",  $"PHP {_service.GetTotalInventoryValue():N2}",      ConsoleColor.Yellow),
            ("",                 "", ConsoleColor.White)
        );

        // breakdown per category
        UIHelper.PrintSection("Stock Breakdown by Category");
        UIHelper.PrintTableHeader(
            ("Category", 18), ("Products", 9), ("Total Units", 11), ("Total Value (PHP)", 18));

        foreach (var cat in categories)
        {
            var cp  = products.FindAll(p => p.CategoryId == cat.Id);
            int cu  = cp.Sum(p => p.StockQuantity);
            decimal cv = cp.Sum(p => p.TotalValue);
            UIHelper.PrintTableRow(ConsoleColor.White,
                (cat.Name, 18), (cp.Count.ToString(), 9),
                (cu.ToString("N0"), 11), ($"{cv:N2}", 18));
        }

        UIHelper.PrintTableFooter(18, 9, 11, 18);
        UIHelper.PressAnyKey();
    }

    // =========================================================
    // TRANSACTION HISTORY
    // =========================================================

    // shows all recorded transactions with color coding by type
    static void ViewTransactionHistory()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("Transaction History", ConsoleColor.DarkMagenta);

        var transactions = _service.GetTransactionHistory();

        if (transactions.Count == 0)
        {
            UIHelper.PrintWarning("No transactions recorded yet.");
            UIHelper.PressAnyKey();
            return;
        }

        UIHelper.PrintTableHeader(
            ("ID", 4), ("Timestamp", 19), ("Type", 15),
            ("Product", 26), ("Change", 7), ("Before", 6),
            ("After", 6), ("By", 10));

        foreach (var t in transactions)
        {
            // color each row based on the type of transaction
            ConsoleColor color = t.Type switch
            {
                TransactionType.Restock        => ConsoleColor.Green,   // added stock
                TransactionType.Deduction      => ConsoleColor.Red,     // removed stock
                TransactionType.ProductAdded   => ConsoleColor.Magenta, // new product
                TransactionType.ProductDeleted => ConsoleColor.DarkRed, // deleted
                _                              => ConsoleColor.Gray     // updated
            };

            string change = t.Type == TransactionType.Restock ||
                            t.Type == TransactionType.ProductAdded
                ? $"+{t.QuantityChanged}"
                : (t.QuantityChanged > 0 ? $"-{t.QuantityChanged}" : "--");

            UIHelper.PrintTableRow(color,
                (t.Id.ToString(), 4),
                (t.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"), 19),
                (t.Type.ToString(), 15), (t.ProductName, 26),
                (change, 7), (t.QuantityBefore.ToString(), 6),
                (t.QuantityAfter.ToString(), 6), (t.PerformedBy, 10));
        }

        UIHelper.PrintTableFooter(4, 19, 15, 26, 7, 6, 6, 10);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"\n  {transactions.Count} transaction(s) on record.");
        Console.ResetColor();
        UIHelper.PressAnyKey();
    }

    // =========================================================
    // USER MANAGEMENT
    // =========================================================

    // only admins can access this section
    static void UserMenu()
    {
        if (_service.CurrentUser?.Role != UserRole.Admin)
        {
            UIHelper.PrintError("Access denied. Admin role is required.");
            UIHelper.PressAnyKey();
            return;
        }

        while (true)
        {
            UIHelper.ClearScreen();
            UIHelper.PrintHeader("User Management", ConsoleColor.DarkMagenta);
            UIHelper.PrintMenuGroup("Options");
            UIHelper.PrintMenuItem("1", "View All Users");
            UIHelper.PrintMenuItem("2", "Add New User");
            UIHelper.PrintMenuItem("0", "Back to Main Menu");
            UIHelper.PrintThinDivider();
            UIHelper.PrintChoicePrompt();

            switch (Console.ReadLine()?.Trim())
            {
                case "1": ViewAllUsers(); break;
                case "2": AddUser();      break;
                case "0": return;
                default:
                    UIHelper.PrintError("Invalid choice.");
                    UIHelper.PressAnyKey();
                    break;
            }
        }
    }

    // shows all system users in a table
    static void ViewAllUsers()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("All System Users — Glow Beauty Shop", ConsoleColor.DarkMagenta);

        var users = _service.GetAllUsers();

        UIHelper.PrintTableHeader(
            ("ID", 3), ("Username", 12), ("Full Name", 24),
            ("Email Address", 30), ("Role", 9), ("Status", 8), ("Created", 11));

        foreach (var u in users)
        {
            // inactive accounts show in gray
            ConsoleColor color = u.IsActive ? ConsoleColor.White : ConsoleColor.DarkGray;
            UIHelper.PrintTableRow(color,
                (u.Id.ToString(), 3), (u.Username, 12), (u.FullName, 24),
                (u.Email, 30), (u.Role.ToString(), 9),
                (u.IsActive ? "Active" : "Inactive", 8),
                (u.CreatedAt.ToString("yyyy-MM-dd"), 11));
        }

        UIHelper.PrintTableFooter(3, 12, 24, 30, 9, 8, 11);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"\n  {users.Count} user(s) registered.");
        Console.ResetColor();
        UIHelper.PressAnyKey();
    }

    // creates a new user account
    static void AddUser()
    {
        UIHelper.ClearScreen();
        UIHelper.PrintHeader("Add New User", ConsoleColor.DarkMagenta);

        try
        {
            string username = UIHelper.PromptInput("Username");
            string fullName = UIHelper.PromptInput("Full Name");
            string email    = UIHelper.PromptInput("Email Address");
            string password = UIHelper.PromptPassword("Password");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  Available Roles:");
            Console.WriteLine("    [0] Admin    — Full system access");
            Console.WriteLine("    [1] Manager  — Manage products, stock, reports");
            Console.WriteLine("    [2] Staff    — View and stock operations only");
            Console.ResetColor();

            int roleNum = UIHelper.PromptInt("Select Role Number", 0, 2);
            UserRole role = (UserRole)roleNum; // convert int to enum

            if (!UIHelper.PromptConfirm($"Create user '{username}' with role '{role}'?"))
            {
                UIHelper.PrintInfo("Cancelled. No user was created.");
                UIHelper.PressAnyKey();
                return;
            }

            _service.AddUser(username, fullName, email, password, role);
            UIHelper.PrintSuccess($"User '{username}' ({fullName}) created with role: {role}");
        }
        catch (Exception ex)
        {
            UIHelper.PrintError(ex.Message);
        }

        UIHelper.PressAnyKey();
    }

    // =========================================================
    // HELPER METHODS FOR UPDATE PRODUCT
    // =========================================================

    // shows a prompt with the current value - returns current if user presses Enter
    private static string FallbackPrompt(string label, string current)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.Write($"  ▸ {label} (current: {current}): ");
        Console.ForegroundColor = ConsoleColor.White;
        string input = Console.ReadLine()?.Trim() ?? "";
        Console.ResetColor();
        return string.IsNullOrWhiteSpace(input) ? current : input;
    }

    // same as above but for decimal values like price
    private static decimal FallbackDecimalPrompt(string label, decimal current)
    {
        while (true)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write($"  ▸ {label}: ");
            Console.ForegroundColor = ConsoleColor.White;
            string input = Console.ReadLine()?.Trim() ?? "";
            Console.ResetColor();
            if (string.IsNullOrWhiteSpace(input)) return current;
            if (decimal.TryParse(input, out decimal val) && val >= 0) return val;
            UIHelper.PrintError("Please enter a valid positive number.");
        }
    }

    // same as above but for integer values like IDs and thresholds
    private static int FallbackIntPrompt(string label, int current)
    {
        while (true)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write($"  ▸ {label}: ");
            Console.ForegroundColor = ConsoleColor.White;
            string input = Console.ReadLine()?.Trim() ?? "";
            Console.ResetColor();
            if (string.IsNullOrWhiteSpace(input)) return current;
            if (int.TryParse(input, out int val) && val >= 1) return val;
            UIHelper.PrintError("Please enter a valid positive whole number.");
        }
    }
}
