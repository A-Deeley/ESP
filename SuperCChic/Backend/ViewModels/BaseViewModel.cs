﻿using Backend.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Backend.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected A22Sda1532463Context DbContext { get; set; } = new();

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
