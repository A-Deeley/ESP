using Backend;
using Backend.Extensions;
using Backend.Models;
using Backend.ViewModels;
using SGI.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SGI.ViewModels;

public class SGIViewModel : BaseViewModel, IPageViewModel
{
    private ICommand? _addProduct;
    private ICommand? _deleteProduct;
    private ICommand? _editProduct;
    private ICommand? _reports;
    private Product? _selectedProduct;
    private int _currentPage;
    private int _totalPages;
    private int _pageSize = 10;
    private Dictionary<int, List<Product>> _cachedProducts;

    public string PageId { get; set; }
    public string Title { get; set; }
    public event EventHandler<SGIEventArgs>? ViewChanged;

    public ObservableCollection<Product> CurrentPageProducts { get; private set; }



    public ICommand Reports
    {
        get
        {
            return _reports ??= new RelayCommand(ExecuteReportsView, (p) => false);
        }
    }

    public ICommand AddProduct
    {
        get
        {
            return _addProduct ??= new RelayCommand(ExecuteAddProduct);
        }
    }

    public ICommand DeleteProduct
    {
        get
        {
            return _deleteProduct ??= new RelayCommand(ExecuteDeleteProduct, CanExecuteDeleteProduct);
        }
    }

    public ICommand EditProduct
    {
        get
        {
            return _editProduct ??= new RelayCommand(ExecuteEditProduct, CanExecuteEditProduct);
        }
    }

    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            _selectedProduct = value;
            OnPropertyChanged();
        }
    }

    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            OnPropertyChanged();
        }
    }

    public int TotalPages
    {
        get => _totalPages;
        set
        {
            _totalPages = value;
            OnPropertyChanged();
        }
    }

    public SGIViewModel()
    {
        using var dbContext = new A22Sda1532463Context();
        Title = Resources.List_Title;
        _cachedProducts = new();
        CurrentPageProducts = new(dbContext.Products.ToList());
    }

    public async void RefreshProducts()
    {
        using var dbContext = new A22Sda1532463Context();
        _cachedProducts = new();
        CurrentPageProducts = new(dbContext.Products.ToList());
        OnPropertyChanged(nameof(CurrentPageProducts));
    }

    #region Methods
    #region RelayCommands
    #region Executes
    void ExecuteAddProduct(object parameter) => ViewChanged.Raise(this, "add");
    //void ExecuteDeleteProduct(object parameter) => ViewChanged.Raise(this, "delete", SelectedProduct);
    async void ExecuteDeleteProduct(object parameter)
    {
        using var dbContext = new A22Sda1532463Context();
        var result = MessageBox.Show($"{Resources.SGI_Delete_Msg} ({SelectedProduct.Cup}) {SelectedProduct.Name}?", "", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            dbContext.Products.Remove(SelectedProduct);
            await dbContext.SaveChangesAsync();
            RefreshProducts();
        }
    }
    void ExecuteEditProduct(object parameter) => ViewChanged.Raise(this, "edit", SelectedProduct);
    void ExecuteReportsView(object obj) => ViewChanged.Raise(this, "reports");
    #endregion
    #region CanExecutes
    bool CanExecuteDeleteProduct(object parameter) => SelectedProduct is not null;
    bool CanExecuteEditProduct(object parameter) => SelectedProduct is not null;
    #endregion
    #endregion
    #endregion
}
