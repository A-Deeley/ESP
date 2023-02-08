using Backend;
using Backend.DTO;
using Backend.Models;
using Backend.ViewModels;
using SGI.Properties;
using System;

namespace SGI.ViewModels;

public class AddModifyViewModel : BaseViewModel, IPageViewModel
{
	public ProductDTO SelectedProduct { get; set; }
    public string PageId { get; set; }
    public string Title { get; set; }

    public event EventHandler<SGIEventArgs<string>>? ViewChanged;

    public AddModifyViewModel()
	{
		PageId = "add";
		Title = $"{Resources.Add_Title}";
		SelectedProduct = new ProductDTO();
	}

	public AddModifyViewModel(Product p)
	{
		PageId = "edit";
		Title = $"{Resources.Edit_Title}";
		SelectedProduct = new ProductDTO(p);
	}
}
