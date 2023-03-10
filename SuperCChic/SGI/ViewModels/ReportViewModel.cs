using Backend;
using Backend.Extensions;
using Backend.Models;
using Backend.ViewModels;
using SGI.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace SGI.ViewModels;

public class ReportViewModel : BaseViewModel, IPageViewModel
{
    private readonly DateTime currentDate;
    private readonly string[] _monthStrings = { "Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre", };
    private int _selectedYearIndex = -1;
    private int _selectedMonthIndex;
    private string _groupBoxTitle;

    public string GroupBoxTitle
    {
        get => _groupBoxTitle;
        set
        {
            _groupBoxTitle = value;
            OnPropertyChanged();
        }
    }

    private List<int> _years;
    private List<string> _months;

    public List<int> Years
    {
        get => _years;
        set
        {
            _years = value;
            OnPropertyChanged();
        }
    }

    public List<string> Months
    {
        get => _months;
        set
        {
            _months = value;
            OnPropertyChanged();
        }
    }

    public int SelectedMonthIndex
    {
        get => _selectedMonthIndex;
        set
        {
            _selectedMonthIndex = value;
            OnPropertyChanged();
            GroupBoxTitle = $"Informations pour le mois de {Months[value]} {Years[SelectedYearIndex]}";
            CalculateReport();
        }
    }

    public int SelectedYearIndex
    {
        get => _selectedYearIndex;
        set
        {
            // Do not change any values if the index doesn't change.
            if (value == _selectedYearIndex)
                return;

            // Set the months to 12 if it's a previous year.
            // Not last index (current year).
            if (value < Years.Count - 1)
            {
                Months = new(_monthStrings);
                _selectedYearIndex = value;
                SelectedMonthIndex = 0;
                OnPropertyChanged();
                return;
            }

            // If it's the current year, set months to current month - 1.
            _selectedYearIndex = value;
            Months = new(_monthStrings[..(currentDate.Month)]);
            OnPropertyChanged();
            SelectedMonthIndex = 0;
        }
    }

    public string PageId { get; set; }
    public string Title { get; set; }

    public event EventHandler<SGIEventArgs> ViewChanged;

    void ExecuteBackToListView(object parameter) => ViewChanged.Raise(this, "list");
    private ICommand? _previous;
    public ICommand Previous
    {
        get
        {
            return _previous ??= new RelayCommand(ExecuteBackToListView);
        }
    }

    public ReportViewModel()
    {
        using (var dbContext = new A22Sda1532463Context())
        {
            Years = new();
            currentDate = DateTime.Now;

            Title = Resources.Reports_Title;
            PageId = "reports";
            var transactDatesAscending = dbContext.Transactions.OrderBy(tr => tr.Date);

            int firstYear = transactDatesAscending.First().Date.Year;
            int lastYear = transactDatesAscending.Last().Date.Year;

            for (int year = firstYear; year <= lastYear; year++)
                Years.Add(year);

            SelectedYearIndex = Years.Count - 1;
        }
    }

    void CalculateReport()
    {

        float salesSum = 0f;
        int transactionCount = 0;
        float avgTransactionValue = 0f;
        Dictionary<int, float> avgSalePerDay = new();


        using (var dbContext = new A22Sda1532463Context())
        {
            DateTime currentMonth = new(Years[SelectedYearIndex], SelectedMonthIndex + 1, 1);
            var transactionsInMonth = dbContext.Transactions.Where(transact => transact.Date.Year == currentMonth.Year && transact.Date.Month == currentMonth.Month).ToList();
            var transactionsInMonthByDate = transactionsInMonth.GroupBy(transact => transact.Date.Day);

            foreach (var transaction in transactionsInMonth)
            {
                transactionCount++;
                salesSum += transaction.GetSumOfSale();
            }

            foreach (var day in transactionsInMonthByDate)
            {
                if (day.Count() > 0)
                    avgSalePerDay[day.Key] = day.Sum(d => d.GetSumOfSale()) / day.Count();
            }

            if (transactionCount > 0)
                avgTransactionValue = salesSum / transactionCount;
        }

        SalesSumMonth = salesSum;
        TransactionCountMonth = transactionCount;
        AvgTransactionValueMonth = avgTransactionValue;
        AvgSalePerDayInMonth = avgSalePerDay;
    }

    float _salesSumMonth;
    int _transactionCountMonth;
    float _avgTransactionValueMonth;
    Dictionary<int, float> _avgSalePerDayInMonth;

    public float SalesSumMonth
    {
        get => _salesSumMonth;
        set
        {
            _salesSumMonth = value;
            OnPropertyChanged();
        }
    }

    public int TransactionCountMonth
    {
        get => _transactionCountMonth;
        set
        {
            _transactionCountMonth = value;
            OnPropertyChanged();
        }
    }

    public float AvgTransactionValueMonth
    {
        get => _avgTransactionValueMonth;
        set
        {
            _avgTransactionValueMonth = value;
            OnPropertyChanged();
        }
    }

    public Dictionary<int, float> AvgSalePerDayInMonth
    {
        get => _avgSalePerDayInMonth;
        set
        {
            _avgSalePerDayInMonth = value;
            OnPropertyChanged();
        }
    }
}
