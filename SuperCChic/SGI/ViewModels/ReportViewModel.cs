using Backend;
using Backend.Models;
using Backend.ViewModels;
using SGI.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace SGI.ViewModels;

public class ReportViewModel : BaseViewModel, IPageViewModel
{
    private readonly DateTime currentDate;
    private readonly int[] _months = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
    private readonly int[] _currentYearMonths;
    private int _selectedYearIndex;
    private int _selectedMonthIndex;

    public List<int> Years { get; set; }
    public List<int> Months { get; set; }

    public int SelectedMonthIndex
    {
        get => _selectedMonthIndex;
        set
        {
            _selectedMonthIndex = value;
            OnPropertyChanged();
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
                Months = new(_months);
                _selectedYearIndex = value;
                OnPropertyChanged();
                return;
            }

            // If it's the current year, set months to current month - 1.
            _selectedYearIndex = value;
            Months = new(_currentYearMonths);
            OnPropertyChanged();
        }
    }

    public string PageId { get; set; }
    public string Title { get; set; }

    public event EventHandler<SGIEventArgs> ViewChanged;

    public ReportViewModel()
    {
        using var dbContext = new A22Sda1532463Context();
        _currentYearMonths = new int[currentDate.Month];
        for (int month = 1; month < currentDate.Month; month++)
            _currentYearMonths[month] = month;

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
