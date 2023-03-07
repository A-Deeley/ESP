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
using System.Windows.Input;

namespace Caisse.ViewModels;

public sealed class CaisseViewModel : BaseViewModel, IPageViewModel
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

    public float? CustomQuantity
    {
        get => _customQuantity;
        set
        {
            _customQuantity = value ?? 0;
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

    private string _modalText;

    public string ModalText
    {
        get => _modalText;
        set
        {
            _modalText = value;
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

    private IPrinter _printer;


    public CaisseViewModel(IPrinter printer)
    {
        _printer = printer;
        NewTransaction();
        this.PropertyChanged += OnCupInputChanged;
    }

    private void NewTransaction()
    {
        _tBuilder = TransactionBuilder.StartTransaction();
        CustomQuantity = null;
        TransactionRows = new();
        CurrentPageViewModel = this;
        CustomQuantityModeEnabled = false;
        RemoveModeEnabled = false;
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

    private async Task<float> CupExists(string cup)
    {
        using (var dbContext = new A22Sda1532463Context()) {
            var product = await dbContext.Products.FirstOrDefaultAsync(prod => prod.Cup == cup);
            float qtyInStock = product?.Qty ?? 0;
            float? qtyInCart = TransactionRows.Sum(row => row.Product.Cup == cup ? row.QtyUnit : 0);

            return (product is not null)
                ? qtyInStock - (float)qtyInCart
                : -1;
        }
    }

    private async Task ExecuteCUPAction(string cup)
    {
        var searchProductTask = Task.Run(() => CupExists(cup));

        CustomQuantity = 1;
        // Totally not MVVM but yolo.
        if (CustomQuantityModeEnabled is true)
        {
            DisplayQtyInputModal();
        }

        if (RemoveModeEnabled is true)
        {
            ExecuteCUPRemove(cup);
            return;
        }

        float qty = await searchProductTask;
        if (qty is -1 or 0)
        {
            DisplayErrorModal($"Aucun stock restant pour {cup}.");
            return;
        }

        if (CustomQuantityModeEnabled is true)
        {
            if (CustomQuantity > qty)
            {
                bool answer = DisplayYesNoModal($"Vous tentez d'ajouter {CustomQuantity}, mais il en reste seulement {qty} en stock. Désirez-vous ajouter la quantité maximale?");
                if (answer)
                    CustomQuantity = qty;
            }

        }
        
        ExecuteCUPAdd(cup);
    }

    void DisplayQtyInputModal()
    {
        Window modal = new ModalWindowQtyInput();
        modal.DataContext = this;
        modal.ShowDialog();
        modal = null;
    }

    private bool _yesNoAnswer;
    public bool YesNoAnswer
    {
        get => _yesNoAnswer;
        set
        {
            _yesNoAnswer = value;
            OnPropertyChanged();
        }
    }

    ICommand _yesBtn;
    ICommand _noBtn;
    ICommand _paymentBtn;

    public ICommand YesBtn
    {
        get => _yesBtn ??= new RelayCommand(ExecuteYesBtn);
    }
    public ICommand NoBtn
    {
        get => _noBtn ??= new RelayCommand(ExecuteNoBtn);
    }
    public ICommand PaymentBtn
    {
        get => _paymentBtn ??= new RelayCommand(ExecutePaymentBtn, CanExecutePaymentBtn);
    }

    ICommand _cancelPaymentBtn;

    public ICommand CancelPaymentBtn
    {
        get => _cancelPaymentBtn ??= new RelayCommand(ExecuteCancelPaymentBtn, (_) => TransactionRows.Count > 0);
    }

    void ExecuteYesBtn(object _) => YesNoAnswer = true;
    void ExecuteNoBtn(object _) => YesNoAnswer = false;

    void ExecuteCancelPaymentBtn(object _)
    {
        NewTransaction();
    }

    void ExecutePaymentBtn(object _)
    {
        int finishedTransactionId = _tBuilder.CompleteTransaction();

        _printer.Print(finishedTransactionId);
    }

    bool CanExecutePaymentBtn(object _) => TransactionRows.Count > 0 && TransactionRows.Sum(row => row.QtyUnit) > 0;

    bool DisplayYesNoModal(string yesNoQuestion)
    {
        Window modal = new ModalWindowYesNo();
        modal.DataContext = this;
        ModalText = yesNoQuestion;
        modal.ShowDialog();
        modal = null;

        return YesNoAnswer;
    }

    void DisplayErrorModal(string message)
    {
        Window modal = new ModalWindowNotEnough();
        modal.DataContext = this;
        // TODO: add ressource for string.
        // TODO: add ressource for strings in the modal windows.
        ModalText = message;
        modal.ShowDialog();
        modal = null;
    }

    private void ExecuteCUPRemove(string cup)
    {
        TransactionRow newRow = null;
        float totalQuantity = 0;

        if (!TransactionRows.Any(row => row.Product.Cup == cup))
        {
            // TODO: add ressource for string.
            DisplayErrorModal("Le produit n'existe pas dans la facture.");
            return;
        }
        else
            totalQuantity = TransactionRows.Sum(row => row.QtyUnit) ?? 0;

        if (totalQuantity == 0)
        {
            DisplayErrorModal("Le produit n'existe pas dans la facture.");
            return;
        }

        if (CustomQuantityModeEnabled is true)
        {
            if (totalQuantity < CustomQuantity)
                newRow = _tBuilder.RemoveProduct(cup, totalQuantity);
            else
                newRow = _tBuilder.RemoveProduct(cup, _customQuantity);
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
        if (CustomQuantity <= 0)
            return;

        TransactionRow newRow = null;
        if (CustomQuantityModeEnabled is null or false)
            newRow = _tBuilder.AddProduct(cup, 1);
        else
            newRow = _tBuilder.AddProduct(cup, _customQuantity);

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
