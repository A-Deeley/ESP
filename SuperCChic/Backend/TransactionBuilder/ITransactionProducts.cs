using Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.TransactionBuilder;

public interface ITransactionProducts
{
    public List<TransactionRow> GetTransactionRows();
    public TransactionRow AddProduct(string cup, float quantity);
    public TransactionRow RemoveProduct(string cup, float quantity);
    public int CompleteTransaction();
    public float GetSubtotalBeforeDiscounts();
    public float GetTotalDiscounts();
    public float GetTotalTps();
    public float GetTotalTvq();
}
