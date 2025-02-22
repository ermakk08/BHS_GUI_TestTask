namespace AvaloniaApplication3.Models
{
    public class RectangleModel : ShapeModel
    {
        private double _width = 100;
        private double _height = 100;
        private double _rotation = 0;
        
        public override string Title => "Прямоугольник";

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public double Rotation
        {
            get => _rotation;
            set => SetProperty(ref _rotation, value);
        }
    }
}