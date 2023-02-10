using Backend.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Backend.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private A22Sda1532463Context _dbContext;

    protected A22Sda1532463Context DbContext
    {
        get
        {
            return _dbContext ??= new A22Sda1532463Context();
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
