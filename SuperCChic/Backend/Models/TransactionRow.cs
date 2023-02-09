using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class TransactionRow
{
    public int Id { get; set; }

    public int? TransactionId { get; set; }

    public int? ProductId { get; set; }

    public float? PriceUnit { get; set; }

    public float? DiscountAmtUnit { get; set; }

    public float? TpsUnit { get; set; }

    public float? TvqUnit { get; set; }

    public float? QtyUnit { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Transaction? Transaction { get; set; }
}
