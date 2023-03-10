using Backend;
using Backend.Extensions;
using Backend.Models;
using Backend.ViewModels;
using SGI.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

    public bool? ApplyTvq
    {
        get => SelectedProduct.ApplyTvq > 0;
        set
        {
            SelectedProduct.ApplyTvq = (value is true)
                ? 1u
                : 0;
        }
    }

    public bool? ApplyTps
    {
        get => SelectedProduct.ApplyTps > 0;
        set
        {
            SelectedProduct.ApplyTps = (value is true)
                ? 1u
                : 0;
        }
    }

    public double? Price
    {
        get => Math.Round(_price, 2);
        set
        {
            _price = value ?? 0;
            OnPropertyChanged();
        }
    }

    public double? Qty
    {
        get => Math.Round(_qty, 2);
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

    private string _cup;
    public string Cup
    {
        get => _cup;
        set
        {
            if (value.Length < 12)
            {
                _cup = value;
                OnPropertyChanged();
            }
            else if (value.Length == 12)
            {
                _cup = value;
                OnPropertyChanged();
                //Window web = new WebDisplayer($"barcodelookup.com/{value}");
                //web.Show();
            }
        }
    }

    public ICommand Action { get; set; }
    public ICommand ActionAndQuit { get; set; }

    public Product SelectedProduct { get; set; }
    public string PageId { get; set; }
    public string Title { get; set; }

    public event EventHandler<SGIEventArgs>? ViewChanged;
    private A22Sda1532463Context dbContext;

    private void Init()
    {
        SelectedProduct = new Product();
        Departements = dbContext.Departments.ToList();
        Companies = dbContext.Companies.ToList();
    }

    private void Init(Product p)
    {
        Init();
        SelectedProduct = _originalReference.GetCopy();
        Price = SelectedProduct.Price;
        Qty = SelectedProduct.Qty;
        DiscountAmt = SelectedProduct.DiscountAmt;
        CompanyText = SelectedProduct.Company.Name;
        DeptText = SelectedProduct.Department.Name;
        Cup = SelectedProduct.Cup;
        SelectedDiscountIndex = (int)SelectedProduct.DiscountType;
    }

    public AddModifyViewModel()
    {
        dbContext = new A22Sda1532463Context();
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
        dbContext = new A22Sda1532463Context();
        _originalReference = dbContext.Products.Find(p.Id);
        Init(p);
        PageId = "edit";
        Title = $"{Resources.Edit_Title}";
        Action = new RelayCommand(ExecuteUpdate, CanExecuteUpdate);
        ActionAndQuit = new RelayCommand(ExecuteUpdateAndQuit, CanExecuteUpdate);
        ActionBtnText = Resources.Add_UpdateAndContinue_Btn;
        ActionQuitBtnText = Resources.Add_UpdateAndQuit_Btn;
    }

    void ExecuteBackToListView(object parameter) => ViewChanged.Raise(this, "list");

    public async Task CreateProduct(Product p)
    {
        
        p.Price = (float)Price;
        p.Qty = (float)Qty;
        p.DiscountType = (ulong)SelectedDiscountIndex;
        p.DiscountAmt = (SelectedDiscountIndex > 0)
            ? (float)DiscountAmt
            : 0f;

        AssignCompany(p);
        AssignDepartment(p);

        p.ApplyTps = SelectedProduct.ApplyTps;
        p.ApplyTvq = SelectedProduct.ApplyTvq;
        p.Cup = Cup;

        dbContext.Products.Add(p);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateProduct(Product p)
    {
        
        p.Price = (float)Price;
        p.Qty = (float)Qty;
        p.DiscountType = (ulong)SelectedDiscountIndex;
        p.DiscountAmt = (SelectedDiscountIndex > 0)
            ? (float)DiscountAmt
            : 0f;

        p.ApplyTps = SelectedProduct.ApplyTps;
        p.ApplyTvq = SelectedProduct.ApplyTvq;

        AssignCompany(p);
        AssignDepartment(p);


        await dbContext.SaveChangesAsync();
    }

    async void ExecuteCreate(object parameter)
    {
        await CreateProduct(SelectedProduct);
        CommandManager.InvalidateRequerySuggested();
        MessageBox.Show("Produit crée avec succès!", "Création", MessageBoxButton.OK);
        Init();
    }

    async void ExecuteCreateAndQuit(object parameter)
    {
        await CreateProduct(SelectedProduct);
        ViewChanged.Raise(this, "list");
    }

    async void ExecuteUpdate(object parameter)
    {
        
        Product dbContextProduct = await dbContext.Products.FindAsync(SelectedProduct.Id);

        await UpdateProduct(dbContextProduct);

        _originalReference = dbContextProduct;
        CommandManager.InvalidateRequerySuggested();
        MessageBox.Show("Produit mis à jour!", "Mise à jour", MessageBoxButton.OK);
        Init(SelectedProduct);
    }

    async void ExecuteUpdateAndQuit(object parameter)
    {
        
        Product dbContextProduct = await dbContext.Products.FindAsync(SelectedProduct.Id);

        await UpdateProduct(dbContextProduct);

        ViewChanged.Raise(this, "list");
    }

    public void AssignCompany(Product p)
    {
        Company? existingCompany = Companies.FirstOrDefault(company => company.Name == CompanyText);

        if (existingCompany is null)
            p.Company = new() { Name = CompanyText };
        else
            p.Company = existingCompany;
    }

    public void AssignDepartment(Product p)
    {
        Department? existingDepartment = Departements.FirstOrDefault(dept => dept.Name == DeptText);

        if (existingDepartment is null)
            p.Department = new() { Name = DeptText };
        else
            p.Department = existingDepartment;
    }

    bool CanExecuteUpdate(object parameter)
    {
        return true;
    }

    void ExecuteReset(object parameter)
    {
        if (_originalReference is null)
            Init();
        else
            Init(_originalReference);
    }

    bool CanExecuteReset(object parameter) => true;

    bool CanExecuteCreate(object parameter)
    {
        bool textBoxesFilled =
            !string.IsNullOrWhiteSpace(Cup) &&
            !string.IsNullOrWhiteSpace(SelectedProduct.Name) &&
            !string.IsNullOrWhiteSpace(SelectedProduct.UnitType);

        bool dropDownSelected =
            !string.IsNullOrWhiteSpace(CompanyText) &&
            !string.IsNullOrWhiteSpace(DeptText);

        bool priceQtyIsNumber =
            (Price is not null and >= 0) &&
            (Qty is not null and >= 0);

        bool companyValid = !string.IsNullOrWhiteSpace(CompanyText);
        bool departmentValid = !string.IsNullOrWhiteSpace(DeptText);


        return
            textBoxesFilled &&
            dropDownSelected &&
            priceQtyIsNumber &&
            companyValid &&
            departmentValid;

    }
}
