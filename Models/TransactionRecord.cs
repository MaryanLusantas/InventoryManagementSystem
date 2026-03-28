// This file defines the TransactionRecord class
// Every time something changes in the inventory, we save a record here
// This works like a history log so we can track all changes

namespace InventoryManagementSystem.Models
{
    // the type of action that was done
    public enum TransactionType
    {
        Restock,        // added stock to a product
        Deduction,      // removed stock from a product
        ProductAdded,   // a new product was added
        ProductUpdated, // a product's details were changed
        ProductDeleted  // a product was deleted
    }

    public class TransactionRecord
    {
        // auto-increment ID for each transaction
        private static int _nextId = 1;

        // all properties are read-only after creation
        // we don't want anyone changing the history
        public int             Id              { get; private set; }
        public int             ProductId       { get; private set; }
        public string          ProductName     { get; private set; } // saved so history stays even if product is deleted
        public TransactionType Type            { get; private set; }
        public int             QuantityChanged { get; private set; }
        public int             QuantityBefore  { get; private set; }
        public int             QuantityAfter   { get; private set; }
        public string          PerformedBy     { get; private set; }
        public DateTime        Timestamp       { get; private set; }
        public string          Remarks         { get; private set; }

        // constructor - all values are set here and cannot be changed after
        public TransactionRecord(int productId, string productName,
                                 TransactionType type, int quantityChanged,
                                 int quantityBefore, int quantityAfter,
                                 string performedBy, string remarks = "")
        {
            Id              = _nextId++;
            ProductId       = productId;
            ProductName     = productName;
            Type            = type;
            QuantityChanged = quantityChanged;
            QuantityBefore  = quantityBefore;
            QuantityAfter   = quantityAfter;
            PerformedBy     = performedBy;
            Timestamp       = DateTime.Now; // capture the exact time automatically
            Remarks         = remarks;
        }

        // returns a one-line summary of this transaction
        public override string ToString()
        {
            string change = Type == TransactionType.Restock ||
                            Type == TransactionType.ProductAdded
                ? $"+{QuantityChanged}"
                : (QuantityChanged > 0 ? $"-{QuantityChanged}" : "--");

            return $"[{Id}] {Timestamp:yyyy-MM-dd HH:mm:ss} | " +
                   $"{Type,-15} | {ProductName,-26} | " +
                   $"Change: {change,5} | " +
                   $"Stock: {QuantityBefore} -> {QuantityAfter} | " +
                   $"By: {PerformedBy}";
        }
    }
}
