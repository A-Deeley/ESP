using Backend.Models;

namespace Backend.TransactionBuilder;

public class TransactionBuilder : ITransactionProducts
{
    private Transaction _currentTransaction;
    private A22Sda1532463Context _context;

    public TransactionBuilder(A22Sda1532463Context dbProvider)
    {
        _currentTransaction = new();
        _context = dbProvider;
    }

    public static ITransactionProducts StartTransaction(A22Sda1532463Context context) => new TransactionBuilder(context);

    public TransactionRow AddProduct(string cup, float quantity)
    {
        Product? p = _currentTransaction.TransactionRows.FirstOrDefault(row => row.Product?.Cup == cup)?.Product;

        if (p is null)
            p = _context.Products.First(prod => prod.Cup == cup);

        float? discountAmtCalculated = 0f;

        if (p.DiscountType == 1)
            discountAmtCalculated = p.DiscountAmt * p.Price;
        if (p.DiscountType == 2)
            discountAmtCalculated = p.DiscountAmt;


        TransactionRow newRow = new()
        {
            Transaction = _currentTransaction,
            ProductId = p.Id,
            PriceUnit = p.Price,
            DiscountAmtUnit = discountAmtCalculated,
            TpsUnit = (p.ApplyTps == 1)
                ? p.Price * 0.05f
                : 0f,
            TvqUnit = (p.ApplyTvq == 1)
                ? p.Price * 0.09975f
                : 0f,
            QtyUnit = quantity
        };

        _currentTransaction.TransactionRows.Add(newRow);
        return newRow;
    }

    public int CompleteTransaction()
    {
        _currentTransaction.Date = DateTime.Now;
        using (var dbContext = new A22Sda1532463Context())
        {
            // Save the changes to the dbcontext
            foreach (TransactionRow row in _currentTransaction.TransactionRows)
            {
                var dbProduct = dbContext.Products.Find(row.ProductId);
                dbProduct.Qty -= row.QtyUnit;
                dbContext.SaveChanges();
            }

            dbContext.ChangeTracker.Clear();
            dbContext.Transactions.Add(_currentTransaction);
            dbContext.SaveChanges();

            dbContext.ChangeTracker.Clear();
            return dbContext.Transactions.OrderByDescending(selector => selector.Date).First().Id;
        }
    }

    public List<TransactionRow> GetTransactionRows() => new(_currentTransaction.TransactionRows);

    public TransactionRow RemoveProduct(string cup, float quantity) => AddProduct(cup, -quantity);

    public float GetSubtotalBeforeDiscounts()
    {
        float subTotal = 0;


        foreach (TransactionRow row in _currentTransaction.TransactionRows)
            subTotal += (float)(row.PriceUnit * row.QtyUnit);

        return subTotal;
    }

    public float GetTotalTps()
    {
        float taxableSubtotal = 0;
        using (var dbContext = new A22Sda1532463Context())
        {
            foreach (TransactionRow row in _currentTransaction.TransactionRows)
            {
                Product product = dbContext.Products.Find(row.ProductId);
                if (product?.ApplyTps == 1)
                    taxableSubtotal += (float)(row.PriceUnit * row.QtyUnit * 0.05);

            }

            return taxableSubtotal;
        }
    }

    public float GetTotalTvq()
    {
        float taxableSubtotal = 0;

        using (var dbContext = new A22Sda1532463Context())
        {
            foreach (TransactionRow row in _currentTransaction.TransactionRows)
            {
                Product product = dbContext.Products.Find(row.ProductId);
                if (product?.ApplyTvq == 1)
                    taxableSubtotal += (float)(row.PriceUnit * row.QtyUnit * 0.09975);
            }
        }
        return taxableSubtotal;
    }

    public float GetTotalDiscounts() => (float)_currentTransaction.TransactionRows.Sum(row => row.DiscountAmtUnit);
}
