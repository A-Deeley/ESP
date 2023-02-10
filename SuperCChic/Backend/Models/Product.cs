using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Backend.Models;

public partial class Product : INotifyPropertyChanged
{
    public int Id { get; set; }

    public int? CompanyId { get; set; }

    public int? DepartmentId { get; set; }

    private string _name = null!;
    public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

    public float? Qty { get; set; }

    public string? Cup { get; set; }

    public float? Price { get; set; }

    private string _unitType = null!;
    public string UnitType { get => _unitType; set { _unitType = value; OnPropertyChanged(); } }

    public ulong ApplyTps { get; set; }

    public ulong ApplyTvq { get; set; }

    public ulong DiscountType { get; set; }

    public float? DiscountAmt { get; set; }

    public virtual Company? Company { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<TransactionRow> TransactionRows { get; } = new List<TransactionRow>();

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName]string property = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
}
