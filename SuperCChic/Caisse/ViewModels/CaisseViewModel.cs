using Backend;
using Backend.Models;
using Backend.TransactionBuilder;
using Backend.ViewModels;
using Caisse.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Caisse.ViewModels;

public class CaisseViewModel : BaseViewModel, IPageViewModel
{
    private ITransactionProducts _tBuilder;
    private IPageViewModel? _pageViewModel;
    private string _title;
    private bool? _removeModeEnabled;
    private bool? _customQuantityModeEnabled;
    private float _customQuantity;
    private string _cupInput;
    public float Subtotal
    {
        get => _tBuilder.GetSubtotalBeforeDiscounts() - _tBuilder.GetTotalDiscounts();
    }

    public float TotalTps
    {
        get => _tBuilder.GetTotalTps();
    }

    public float TotalTvq
    {
        get => _tBuilder.GetTotalTvq();
    }

    public float Total
    {
        get => Subtotal + TotalTps + TotalTvq;
    }

    public float CustomQuantity
    {
        get => _customQuantity;
        set
        {
            _customQuantity = value;
            OnPropertyChanged();
        }
    }

    public string CUPInput
    {
        get => _cupInput;
        set
        {
            _cupInput = value;
            OnPropertyChanged();
        }
    }

    public bool? RemoveModeEnabled
    {
        get => _removeModeEnabled;
        set
        {
            _removeModeEnabled = value;
            OnPropertyChanged();
        }
    }

    public bool? CustomQuantityModeEnabled
    {
        get => _customQuantityModeEnabled;
        set
        {
            _customQuantityModeEnabled = value;
            OnPropertyChanged();
        }
    }


    public ObservableCollection<TransactionRow> TransactionRows { get; set; }

    public event EventHandler<SGIEventArgs> ViewChanged;

    public IPageViewModel? CurrentPageViewModel
    {
        get => _pageViewModel;
        set
        {
            _pageViewModel = value;
            OnPropertyChanged();
            Title = CurrentPageViewModel.Title;
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

    public string PageId { get; set; }

    public CaisseViewModel()
    {
        _tBuilder = TransactionBuilder.StartTransaction();
        CustomQuantity = 1;
        TransactionRows = new();
        CurrentPageViewModel = this;
        CustomQuantityModeEnabled = false;
        RemoveModeEnabled = false;
        this.PropertyChanged += OnCupInputChanged;
    }

    private async void OnCupInputChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(CUPInput))
            return;

        if (CUPInput.Length < 12)
            return;

        await ExecuteCUPAction(CUPInput);
        CUPInput = string.Empty;
    }

    private Task<bool> CupExists(string cup) => DbContext.Products.AnyAsync(prod => prod.Cup == cup);

    private async Task ExecuteCUPAction(string cup)
    {
        var searchProductTask = Task.Run(() => CupExists(cup));

        CustomQuantity = 1;
        // Totally not MVVM but yolo.
        if (CustomQuantityModeEnabled is true)
        {
            Window modal = new ModalWindow();
            modal.DataContext = this;
            modal.ShowDialog();
            modal = null;
        }

        if (!await searchProductTask)
            return;

        if (RemoveModeEnabled is true)
            ExecuteCUPRemove(cup);
        else
            ExecuteCUPAdd(cup);
    }

    private void ExecuteCUPRemove(string cup)
    {
        TransactionRow newRow = null;
        float totalQuantity = 0;

        if (!TransactionRows.Any(row => row.Product.Cup == cup))
            return;
        else
            totalQuantity = TransactionRows.Sum(row => row.QtyUnit) ?? 0;

        if (totalQuantity == 0)
            return;

        if (CustomQuantityModeEnabled is true)
        {
            if (totalQuantity < CustomQuantity)
                newRow = _tBuilder.RemoveProduct(cup, totalQuantity);
            else
                newRow = _tBuilder.RemoveProduct(cup, CustomQuantity);
        }
        else
        {
            if (totalQuantity < 1)
                newRow = _tBuilder.RemoveProduct(cup, totalQuantity);
            else
                newRow = _tBuilder.RemoveProduct(cup, 1);
        }

        UpdateTotals(newRow);
    }

    private void ExecuteCUPAdd(string cup)
    {
        TransactionRow newRow = null;
        if (CustomQuantityModeEnabled is null or false)
            newRow = _tBuilder.AddProduct(cup, 1);
        else
            newRow = _tBuilder.AddProduct(cup, CustomQuantity);

        UpdateTotals(newRow);
    }

    private void UpdateTotals(TransactionRow newRow)
    {
        // Force the list display to update.
        TransactionRows.Add(newRow);

        OnPropertyChanged(nameof(Subtotal));
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(TotalTps));
        OnPropertyChanged(nameof(TotalTvq));
    }
}
