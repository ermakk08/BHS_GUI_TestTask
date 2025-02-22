using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Media;

namespace AvaloniaApplication3.Models;

public abstract class ShapeModel : INotifyPropertyChanged
{
    private IBrush _color = Brushes.Green;
    private string _text = "";
    private IBrush _textColor = Brushes.Red;
    
    public abstract string Title { get; }

    public IBrush Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }

    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    public IBrush TextColor
    {
        get => _textColor;
        set => SetProperty(ref _textColor, value);
    }

    public IBrush[] AvailableColors =>
    [
        Brushes.Red,
        Brushes.Green,
        Brushes.Blue,
        Brushes.Yellow,
        Brushes.Purple,
        Brushes.Orange,
        Brushes.Pink,
        Brushes.Brown,
        Brushes.Black,
        Brushes.White
    ];

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null!)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}