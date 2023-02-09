using Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Extensions;

public static class ModelConverters
{
    #region Product
    public static Product GetCopy(this Product product)
    {
        var copy = new Product
        {
            Id = product.Id,
            Name = product.Name,
            Qty = product.Qty,
            Cup = product.Cup,
            Price = product.Price,
            UnitType = product.UnitType,
            ApplyTps = product.ApplyTps,
            ApplyTvq = product.ApplyTvq,
            DiscountType = product.DiscountType,
            DiscountAmt = product.DiscountAmt,
            Company = product.Company,
            Department = product.Department,
            CompanyId = product.CompanyId,
            DepartmentId = product.DepartmentId
        };

        return copy;
    }
    #endregion

}
