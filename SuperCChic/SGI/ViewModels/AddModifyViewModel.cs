using Backend;
using Backend.Extensions;
using Backend.Models;
using Backend.ViewModels;
using SGI.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Input;

namespace SGI.ViewModels;

public class AddModifyViewModel : BaseViewModel, IPageViewModel
{
    private ICommand? _previous;
    private ICommand? _reset;
    private Product _originalReference;
    private Company _selectedCompany;
    private Department _selectedDept;
    private int _selectedDiscountIndex;
    private int _selectedDepartmentIndex;
    private int _selectedCompanyIndex;
    private double _price;
    private double _qty;
    private double _discountAmt;
    private string _companyText;
    private string _deptText;
    private string _actionQuitBtnText;
    private string _actionBtnText;

    public string ActionQuitBtnText
    {
        get => _actionQuitBtnText;
        set
        {
            _actionQuitBtnText = value;
            OnPropertyChanged();
        }
    }

    public string ActionBtnText
    {
        get => _actionBtnText;
        set
        {
            _actionBtnText = value;
            OnPropertyChanged();
        }
    }

    public string CompanyText
    {
        get => _companyText;
        set
        {
            _companyText = value;
            OnPropertyChanged();
        }
    }

    public string DeptText
    {
        get => _deptText;
        set
        {
            _deptText = value;
            OnPropertyChanged();
        }
    }

    public bool ApplyTvq
    {
        get => SelectedProduct.ApplyTvq > 0;
        set
        {
            SelectedProduct.ApplyTvq = (value)
                ? 1u
                : 0u;
        }
    }

    public bool ApplyTps
    {
        get => SelectedProduct.ApplyTps > 0;
        set
        {
            SelectedProduct.ApplyTps = (value)
                ? 1u
                : 0u;
        }
    }

    public double? Price
    {
        get => _price;
        set
        {
            _price = value ?? 0;
            OnPropertyChanged();
        }
    }

    public double? Qty
    {
        get => _qty;
        set
        {
            _qty = value ?? 0;
            OnPropertyChanged();
        }
    }

    public double? DiscountAmt
    {
        get => _discountAmt;
        set
        {
            _discountAmt = value ?? 0;
            OnPropertyChanged();
        }
    }

    public Company SelectedCompany
    {
        get => _selectedCompany;
        set
        {
            _selectedCompany = value;
            OnPropertyChanged();
        }
    }

    public Department SelectedDept
    {
        get => _selectedDept;
        set
        {
            _selectedDept = value;
            OnPropertyChanged();
        }
    }

    public List<Department> Departements { get; private set; }
    public List<Company> Companies { get; private set; }

    public int SelectedDiscountIndex
    {
        get => _selectedDiscountIndex;
        set
        {
            _selectedDiscountIndex = value;
            OnPropertyChanged();
        }
    }

    public int SelectedDepartmentIndex
    {
        get => _selectedDepartmentIndex; set
        {
            _selectedDepartmentIndex = value;
            OnPropertyChanged();
        }
    }

    public int SelectedCompanyIndex
    {
        get => _selectedCompanyIndex; set
        {
            _selectedCompanyIndex = value;
            OnPropertyChanged();
        }
    }

    public ICommand Previous
    {
        get
        {
            return _previous ??= new RelayCommand(ExecuteBackToListView);
        }
    }

    public ICommand Reset
    {
        get
        {
            return _reset ??= new RelayCommand(ExecuteReset, CanExecuteReset);
        }
    }

    public ICommand Action { get; set; }
    public ICommand ActionAndQuit { get; set; }

    public Product SelectedProduct { get; set; }
    public string PageId { get; set; }
    public string Title { get; set; }

    public event EventHandler<SGIEventArgs>? ViewChanged;

    private void Init()
    {
        SelectedProduct = new Product();
        Departements = DbContext.Departments.ToList();
        Companies = DbContext.Companies.ToList();
    }

    private void Init(Product p)
    {
        Init();
        SelectedProduct = _originalReference.GetCopy();
        Price = SelectedProduct.Price;
        Qty = SelectedProduct.Qty;
        DiscountAmt = SelectedProduct.DiscountAmt;
        SelectedCompany = Companies.First(company => company.Name == SelectedProduct.Company.Name);
        SelectedDept = Departements.First(dept => dept.Name == SelectedProduct.Department.Name);
        CompanyText = SelectedCompany.Name;
        DeptText = SelectedDept.Name;
        SelectedDiscountIndex = (int)SelectedProduct.DiscountType;
    }

    public AddModifyViewModel()
    {
        _originalReference = new();
        Init();
        PageId = "add";
        Title = $"{Resources.Add_Title}";
        Action = new RelayCommand(ExecuteCreate, CanExecuteCreate);
        ActionAndQuit = new RelayCommand(ExecuteCreateAndQuit, CanExecuteCreate);
        ActionBtnText = Resources.Add_CreateAndContinue_Btn;
        ActionQuitBtnText = Resources.Add_CreateAndQuit_Btn;
    }

    public AddModifyViewModel(Product p)
    {
        _originalReference = p;
        Init(p);
        PageId = "edit";
        Title = $"{Resources.Edit_Title}";
        Action = new RelayCommand(ExecuteUpdate, CanExecuteUpdate);
        ActionAndQuit = new RelayCommand(ExecuteUpdateAndQuit, CanExecuteUpdate);
        ActionBtnText = Resources.Add_UpdateAndContinue_Btn;
        ActionQuitBtnText = Resources.Add_UpdateAndQuit_Btn;
    }

    void ExecuteBackToListView(object parameter)
    {
        ViewChanged.Raise(this, "list");
    }

    async void ExecuteCreate(object parameter)
    {
        SelectedProduct.Price = (float)Price;
        SelectedProduct.Qty = (float)Qty;
        SelectedProduct.DiscountAmt = (SelectedDiscountIndex > 0)
            ? (float)DiscountAmt
            : 0f;

        if (SelectedCompany == null)
            SelectedProduct.Company = new() { Name = CompanyText };
        else
            SelectedProduct.Company = SelectedCompany;

        if (SelectedDepartmentIndex == null)
            SelectedProduct.Department = new() { Name = DeptText };
        else
            SelectedProduct.Department = SelectedDept;

        DbContext.Products.Add(SelectedProduct);
        await DbContext.SaveChangesAsync();
        CommandManager.InvalidateRequerySuggested();
    }

    async void ExecuteCreateAndQuit(object parameter)
    {
        SelectedProduct.Price = (float)Price;
        SelectedProduct.Qty = (float)Qty;
        SelectedProduct.DiscountAmt = (SelectedDiscountIndex > 0)
            ? (float)DiscountAmt
            : 0f;

        if (SelectedCompany == null)
            SelectedProduct.Company = new() { Name = CompanyText };
        else
            SelectedProduct.Company = SelectedCompany;

        if (SelectedDepartmentIndex == null)
            SelectedProduct.Department = new() { Name = DeptText };
        else
            SelectedProduct.Department = SelectedDept;

        DbContext.Products.Add(SelectedProduct);
        await DbContext.SaveChangesAsync();
        ViewChanged.Raise(this, "list");
    }

    async void ExecuteUpdate(object parameter)
    {
        Product dbContextProduct = await DbContext.Products.FindAsync(SelectedProduct.Id);

        dbContextProduct.Cup = SelectedProduct.Cup;
        dbContextProduct.Price = (float)Price;
        dbContextProduct.Qty = (float)Qty;
        dbContextProduct.DiscountAmt = (SelectedDiscountIndex > 0)
            ? (float)DiscountAmt
            : 0f;

        dbContextProduct.ApplyTps = (ApplyTvq) ? (uint)1 : 0;
        dbContextProduct.ApplyTvq = (ApplyTps) ? (uint)1 : 0;

        dbContextProduct.UnitType = SelectedProduct.UnitType;

        dbContextProduct.DiscountType = SelectedProduct.DiscountType;

        if (SelectedCompany == null)
            dbContextProduct.Company = new() { Name = CompanyText };
        else
            dbContextProduct.Company = SelectedCompany;

        if (SelectedDepartmentIndex == null)
            dbContextProduct.Department = new() { Name = DeptText };
        else
            dbContextProduct.Department = SelectedDept;

        await DbContext.SaveChangesAsync();
        _originalReference = dbContextProduct;
        CommandManager.InvalidateRequerySuggested();
    }

    async void ExecuteUpdateAndQuit(object parameter)
    {
        Product dbContextProduct = await DbContext.Products.FindAsync(SelectedProduct.Id);

        dbContextProduct.Cup = SelectedProduct.Cup;
        dbContextProduct.Price = (float)Price;
        dbContextProduct.Qty = (float)Qty;
        dbContextProduct.DiscountAmt = (SelectedDiscountIndex > 0)
            ? (float)DiscountAmt
            : 0f;

        dbContextProduct.ApplyTps = (ApplyTvq) ? (uint)1 : 0;
        dbContextProduct.ApplyTvq = (ApplyTps) ? (uint)1 : 0;

        dbContextProduct.UnitType = SelectedProduct.UnitType;

        dbContextProduct.DiscountType = SelectedProduct.DiscountType;

        if (SelectedCompany == null)
            SelectedProduct.Company = new() { Name = CompanyText };
        else
            SelectedProduct.Company = SelectedCompany;

        if (SelectedDepartmentIndex == null)
            SelectedProduct.Department = new() { Name = DeptText };
        else
            SelectedProduct.Department = SelectedDept;

        await DbContext.SaveChangesAsync();
        ViewChanged.Raise(this, "list");
    }

    bool CanExecuteUpdate(object parameter)
    {
        return true;
    }

    void ExecuteReset(object parameter)
    {
        Init(_originalReference);
    }

    bool CanExecuteReset(object parameter) => false;

    bool CanExecuteCreate(object parameter)
    {
        bool textBoxesFilled =
            !string.IsNullOrWhiteSpace(SelectedProduct.Cup) &&
            !string.IsNullOrWhiteSpace(SelectedProduct.Name) &&
            !string.IsNullOrWhiteSpace(SelectedProduct.UnitType);

        bool dropDownSelected =
            !string.IsNullOrWhiteSpace(CompanyText) &&
            !string.IsNullOrWhiteSpace(DeptText);

        bool priceQtyIsNumber =
            (Price is not null and >= 0) &&
            (Qty is not null and >= 0);

        bool companyValid = SelectedCompanyIndex >= 0 || !Companies.Any(c => c.Name == CompanyText);
        bool departmentValid = SelectedDepartmentIndex >= 0 || !Departements.Any(c => c.Name == DeptText);


        return
            textBoxesFilled &&
            dropDownSelected &&
            priceQtyIsNumber &&
            companyValid &&
            departmentValid;

    }
}
