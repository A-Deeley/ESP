using Backend;
using Backend.Models;
using Backend.TransactionBuilder;
using Backend.ViewModels;
using Caisse.Views;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
            if (value.Length < 12)
            {
                _cupInput = value;
                OnPropertyChanged();
            }
            else
            {
                _cupInput = value;
                OnPropertyChanged();

                ExecuteCUPAction(value);
            }
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
        TransactionRows = new();
        CurrentPageViewModel = this;
        CustomQuantityModeEnabled = false;
        RemoveModeEnabled = false;
    }

    private void ExecuteCUPAction(string cup)
    {
        if (RemoveModeEnabled is true)
            ExecuteCUPRemove(cup);
        else
            ExecuteCUPAdd(cup);
    }

    private void ExecuteCUPRemove(string cup)
    {

    }

    private void ExecuteCUPAdd(string cup)
    {
        TransactionRow newRow = null;
        if (CustomQuantityModeEnabled is null or false)
            newRow = _tBuilder.AddProduct(cup, 1);
        else
            newRow = _tBuilder.AddProduct(cup, CustomQuantity);


        // Force the list display to update.
        TransactionRows.Add(newRow);

        _cupInput = string.Empty;
        OnPropertyChanged(nameof(CUPInput));
        OnPropertyChanged(nameof(Subtotal));
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(TotalTps));
        OnPropertyChanged(nameof(TotalTvq));
    }
}
