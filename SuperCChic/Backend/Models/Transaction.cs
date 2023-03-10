using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public virtual ICollection<TransactionRow> TransactionRows { get; } = new List<TransactionRow>();

    public float GetQtyArticles()
    {
        var articles = TransactionRows.GroupBy(row => row.Product.Id);

        return articles.Count();
    }

    public float GetSumOfSale() => TransactionRows.Sum(row => row.Subtotal + ((row.TpsUnit + row.TvqUnit) * row.QtyUnit)) ?? 0;
}
