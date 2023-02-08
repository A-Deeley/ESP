using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Product
{
    public int Id { get; set; }

    public int? CompanyId { get; set; }

    public int? DepartmentId { get; set; }

    public string Name { get; set; } = null!;

    public float? Qty { get; set; }

    public string? Cup { get; set; }

    public float? Price { get; set; }

    public string UnitType { get; set; } = null!;

    public ulong ApplyTps { get; set; }

    public ulong ApplyTvq { get; set; }

    public ulong DiscountType { get; set; }

    public float? DiscountAmt { get; set; }
}
