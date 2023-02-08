using Backend;
using Backend.Extensions;
using Backend.Models;
using Backend.ViewModels;
using System;
using System.Windows.Input;

namespace SGI.ViewModels;

public class SGIViewModel : BaseViewModel, IPageViewModel
{
    private ICommand? _testChangePage;

    public ICommand TestChangePage
    {
        get
        {
            return _testChangePage ??= new RelayCommand(exe =>
            {
                ViewChanged.Raise(this, "add");
            });
        }
    }
    
    public string PageId { get; set; }
    public string Title { get; set; }

    public event EventHandler<SGIEventArgs>? ViewChanged;

    public SGIViewModel()
	{

	}
}
