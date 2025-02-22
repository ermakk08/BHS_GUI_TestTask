namespace AvaloniaApplication3.Models
{
    public class CircleModel : ShapeModel
    {
        public override string Title => "Окружность";

        private double _radius = 100;
        private double _horizontalCompression = 1;
        private double _verticalCompression = 1;

        public double Radius
        {
            get => _radius;
            set
            {
                if (SetProperty(ref _radius, value))
                {
                    OnPropertyChanged(nameof(Width));
                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        public double HorizontalCompression
        {
            get => _horizontalCompression;
            set
            {
                if (SetProperty(ref _horizontalCompression, value))
                {
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        public double VerticalCompression
        {
            get => _verticalCompression;
            set
            {
                if (SetProperty(ref _verticalCompression, value))
                {
                    OnPropertyChanged(nameof(Height));
                }
            }
        }
        
        public double Width => Radius * HorizontalCompression;
        public double Height => Radius * VerticalCompression;
    }
}