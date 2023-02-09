using Backend;
using Backend.Extensions;
using Backend.Models;
using Backend.ViewModels;
using SGI.Properties;
using System;
using System.Windows.Input;

namespace SGI.ViewModels;

public class AddModifyViewModel : BaseViewModel, IPageViewModel
{
    private ICommand? _previous;

    public ICommand Previous
    {
        get
        {
            return _previous ??= new RelayCommand(ExecuteBackToListView);
        }
    }
    public Product SelectedProduct { get; set; }
    public string PageId { get; set; }
    public string Title { get; set; }

    public event EventHandler<SGIEventArgs>? ViewChanged;

    public AddModifyViewModel()
    {
        PageId = "add";
        Title = $"{Resources.Add_Title}";
        SelectedProduct = new Product();
    }

    public AddModifyViewModel(Product p)
    {
        PageId = "edit";
        Title = $"{Resources.Edit_Title}";
        SelectedProduct = p.GetCopy();
    }

    void ExecuteBackToListView(object parameter)
    {
        ViewChanged.Raise(this, "list");
    }
}
