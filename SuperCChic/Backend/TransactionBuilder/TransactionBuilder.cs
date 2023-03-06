using Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Backend.TransactionBuilder;

public class TransactionBuilder : ITransactionProducts
{
    private Transaction _currentTransaction;

    public TransactionBuilder()
    {
        _currentTransaction = new();
    }

    public static ITransactionProducts StartTransaction() => new TransactionBuilder();

    public TransactionRow AddProduct(string cup, float quantity)
    {
        using (var dbContext = new A22Sda1532463Context())
        {
            Product p = dbContext.Products.First(prod => prod.Cup == cup);

            float? discountAmtCalculated = 0f;

            if (p.DiscountType == 1)
                discountAmtCalculated = p.DiscountAmt;
            if (p.DiscountType == 2)
                discountAmtCalculated = p.DiscountAmt * p.Price;


            TransactionRow newRow = new()
            {
                Transaction = _currentTransaction,
                Product = p,
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
    }

    public int CompleteTransaction()
    {
        _currentTransaction.Date = DateTime.Now;
        using (var dbContext = new A22Sda1532463Context())
        {
            // Save the changes to the dbcontext
            foreach (TransactionRow row in _currentTransaction.TransactionRows)
            {
                var dbProduct = dbContext.Products.Find(row.Product.Id);
                dbProduct.Qty -= row.QtyUnit;
            }
            

            dbContext.SaveChanges();

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

        foreach (TransactionRow row in _currentTransaction.TransactionRows)
            if (row.Product.ApplyTps == 1)
                taxableSubtotal += (float)(row.PriceUnit * row.QtyUnit * 0.05);

        return taxableSubtotal;
    }

    public float GetTotalTvq()
    {
        float taxableSubtotal = 0;

        foreach (TransactionRow row in _currentTransaction.TransactionRows)
            if (row.Product.ApplyTvq == 1)
                taxableSubtotal += (float)(row.PriceUnit * row.QtyUnit * 0.09975);

        return taxableSubtotal;
    }

    public float GetTotalDiscounts() => (float)_currentTransaction.TransactionRows.Sum(row => row.DiscountAmtUnit);
}
