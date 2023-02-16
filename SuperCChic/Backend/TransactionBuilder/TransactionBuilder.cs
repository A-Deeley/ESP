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
    private A22Sda1532463Context _dbContext;

    private A22Sda1532463Context DbContext
    {
        get
        {
            return _dbContext ??= new();
        }
    }

    public TransactionBuilder()
    {
        _currentTransaction = new();
    }

    public static ITransactionProducts StartTransaction() => new TransactionBuilder();

    public TransactionRow AddProduct(string cup, float quantity)
    {
        Product p = DbContext.Products.First(prod => prod.Cup == cup);

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

    public Transaction CompleteTransaction()
    {
        _currentTransaction.Date = DateTime.Now;

        return _currentTransaction;
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
