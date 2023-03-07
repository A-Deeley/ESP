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

    public float? Subtotal
    {
        get
        {
            if (QtyUnit > 0)
                return (PriceUnit - DiscountAmtUnit) * QtyUnit;
            else
                return (PriceUnit + DiscountAmtUnit) * QtyUnit;
        }
    }

    public string TextCaisse
    {
        get
        {
            int length = Product?.Name.Length ?? 0;
            string productName = Product?.Name ?? "";
            if (length > 15)
                productName = $"{productName[..15]}...";
            else
                productName = productName.PadRight(18);

            string text = $"{productName} | {QtyUnit,3} @ {PriceUnit:C2}";
            if (DiscountAmtUnit > 0)
                text = $"{productName} | {QtyUnit,3} @ {PriceUnit:C2} (-${Math.Round(DiscountAmtUnit ?? 0, 2, MidpointRounding.AwayFromZero):C2})";

            return text;
        }
    }
}
