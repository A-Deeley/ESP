using Backend;
using Backend.Models;
using Backend.ViewModels;
using SGI.Views;
using System.Collections.Generic;

namespace SGI.ViewModels;

public class MainViewModel : BaseViewModel
{
    #region BackingFields
    private IPageViewModel? _pageViewModel;
    private string _title;
    private readonly Dictionary<string, IPageViewModel>? _pageViewModels = new();
    #endregion

    #region Properties
    public IPageViewModel? CurrentPageViewModel
    {
        get => _pageViewModel;
        set
        {
            _pageViewModel = value;
            OnPropertyChanged();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }
    #endregion
    public MainViewModel()
    {
        // Prepare the product list viewmodel.
        AddViewModel("list", new SGIViewModel());
        CurrentPageViewModel = _pageViewModels["list"];
    }


    #region Methods
    void AddViewModel<T>(string index, T pageViewModel)
        where T: IPageViewModel
    {
        _pageViewModels[index] = pageViewModel;
        _pageViewModels[index].ViewChanged += OnViewChanged;
    }

    void OnViewChanged(object? sender, SGIEventArgs e)
    {
        switch (e.Value) 
        {
            case "add":
                CurrentPageViewModel = new AddModifyViewModel();
                CurrentPageViewModel.ViewChanged += OnViewChanged;
                Title = CurrentPageViewModel.Title;
                break;

            case "edit":
                if (e.Model is null)
                    goto default;

                CurrentPageViewModel = new AddModifyViewModel(e.Model);
                CurrentPageViewModel.ViewChanged += OnViewChanged;
                Title = CurrentPageViewModel.Title;
                break;

            case "delete":
                break;

            default:
                CurrentPageViewModel = _pageViewModels["list"];
                Title = CurrentPageViewModel.Title;
                break;

        }
    }
    #endregion
}
