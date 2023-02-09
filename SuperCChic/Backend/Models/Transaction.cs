using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public virtual ICollection<TransactionRow> TransactionRows { get; } = new List<TransactionRow>();
}
