using Backend.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
    public class Printer : IPrinter
    {
        public Printer()
        {
            if (!Directory.Exists(@"./Factures/"))
            {
                Directory.CreateDirectory(@"./Factures");
            }
        }

        public void Print(int transactionId)
        {
            using var dbContext = new A22Sda1532463Context();
            Transaction transaction = dbContext.Transactions.Find(transactionId);
            var transactionsByDepartment = transaction.TransactionRows
                .GroupBy(selector => selector.Product.DepartmentId);

            BobTheBuilder bob = BobTheBuilder.CanWeBuildIt(CanWeBuildIt.YesWeCan);

            bob.Build("===========================");
            bob.Build($"ID TRANSACTION: {transaction.Id}");

            foreach (var deptGroup in transactionsByDepartment)
            {
                var dept = dbContext.Departments.Find(deptGroup.Key);
                bob.Build(dept.Name);

                foreach (TransactionRow groupRow in deptGroup)
                    bob.Build($"  {groupRow.TextCaisse}");
            }

            var totalTps = transaction.TransactionRows.Sum(row => row.TpsUnit * row.QtyUnit);
            var totalTvq = transaction.TransactionRows.Sum(row => row.TvqUnit * row.QtyUnit);
            var sousTotal = transaction.TransactionRows.Sum(row => row.PriceUnit * row.QtyUnit);

            string output = bob.Build($"SOUS-TOTAL: {sousTotal}".PadRight(27))
               .Build($"TPS: {totalTps}")
               .Build($"TVQ: {totalTvq}")
               .Build("-----------------------------")
               .Build($"{transaction.GetQtyArticles()} Article(s)")
               .Build($"Total: {sousTotal + totalTps + totalTvq:C2}")
               .Build("===========================")
               .AdmireYourWork();

            string facturePath = @$"./Factures/{transaction.Id}.txt";
            File.WriteAllText(facturePath, output);

            Process.Start("notepad.exe" ,facturePath);
        }
    }

    internal enum CanWeBuildIt
    {
        YesWeCan
    }

    internal class BobTheBuilder
    {
        public static BobTheBuilder CanWeBuildIt(CanWeBuildIt c) => new BobTheBuilder();

        private readonly StringBuilder _builder;

        public BobTheBuilder()
        {
            _builder = new();
        }

        public BobTheBuilder Build(string text)
        {
            _builder.AppendLine(text);

            return this;
        }

        public BobTheBuilder AddBrick(string text)
        {
            _builder.Append(text);

            return this;
        }

        public string AdmireYourWork() => _builder.ToString();
    }
}
