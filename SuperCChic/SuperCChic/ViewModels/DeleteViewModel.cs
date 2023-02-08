using Backend;
using Backend.ViewModels;
using System;

namespace SGI.ViewModels;

public class DeleteViewModel : BaseViewModel, IPageViewModel
{
	public DeleteViewModel()
	{

	}

    public string PageId { get; set; }
    public string Title { get; set; }

    public event EventHandler<EventArgs<string>>? ViewChanged;
}
