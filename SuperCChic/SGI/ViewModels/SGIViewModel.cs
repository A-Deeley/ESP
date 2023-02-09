using Backend;
using Backend.Extensions;
using Backend.Models;
using Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SGI.ViewModels;

public class SGIViewModel : BaseViewModel, IPageViewModel
{
    private ICommand? _addProduct;
    private ICommand? _deleteProduct;
    private ICommand? _editProduct;
    private A22Sda1532463Context _dbContext;
    private Product? _selectedProduct;
    private int _currentPage;
    private int _totalPages;
    private int _pageSize = 10;
    private readonly Dictionary<int, List<Product>> _cachedProducts;

    public string PageId { get; set; }
    public string Title { get; set; }
    public event EventHandler<SGIEventArgs>? ViewChanged;

    public ObservableCollection<Product> CurrentPageProducts { get; private set; }

    private A22Sda1532463Context DbContext
    {
        get
        {
            return _dbContext ??= new A22Sda1532463Context();
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
        _cachedProducts = new();
        CurrentPageProducts = new(DbContext.Products.ToList());

    }

    #region Methods
    #region RelayCommands
    #region Executes
    void ExecuteAddProduct(object parameter) => ViewChanged.Raise(this, "add");
    void ExecuteDeleteProduct(object parameter) => ViewChanged.Raise(this, "delete", SelectedProduct);
    void ExecuteEditProduct(object parameter) => ViewChanged.Raise(this, "edit", SelectedProduct);
    #endregion
    #region CanExecutes
    bool CanExecuteDeleteProduct(object parameter) => SelectedProduct is not null;
    bool CanExecuteEditProduct(object parameter) => SelectedProduct is not null;
    #endregion
    #endregion
    #endregion
}
