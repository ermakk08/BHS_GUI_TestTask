using System.Collections.ObjectModel;
using System.Windows.Input;
using AvaloniaApplication3.Models;
using ReactiveUI;

namespace AvaloniaApplication3.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ShapeModel? _selectedShape;

    public MainWindowViewModel()
    {
        AddRectangleCommand = ReactiveCommand.Create(AddRectangle);
        AddCircleCommand = ReactiveCommand.Create(AddCircle);
    }

    public ObservableCollection<ShapeModel> Shapes { get; } = new();


    public ShapeModel? SelectedShape
    {
        get => _selectedShape;
        set => this.RaiseAndSetIfChanged(ref _selectedShape, value);
    }

    public ICommand AddRectangleCommand { get; }
    public ICommand AddCircleCommand { get; }

    private void AddRectangle()
    {
        Shapes.Add(new RectangleModel());
    }

    private void AddCircle()
    {
        Shapes.Add(new CircleModel());
    }
}