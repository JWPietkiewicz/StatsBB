using StatsBB.Model;
using System.Diagnostics;
using StatsBB.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StatsBB.UserControls
{
    public partial class Court : UserControl
    {
        public event EventHandler<CourtPointData>? CourtClick;

        // Store markers to enable removal if needed
        private readonly List<UIElement> _markerElements = new();
        private double MarkerSize = 0.4;
        public Court()
        {
            InitializeComponent();
        }

        // Called when user clicks on the court
        private void CourtCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPosition = e.GetPosition(CourtCanvas);

            bool isLeftHalf = IsLeftHalfOfCourt(clickPosition.X);
            bool isThreePointer = IsThreePointer(clickPosition.X, clickPosition.Y);

            var dataPoint = new CourtPointData(clickPosition, isThreePointer, isLeftHalf, e.ChangedButton);
            Debug.WriteLine($"[{GameClockService.TimeLeftString}] Clicked at ({clickPosition.X:F2}, {clickPosition.Y:F2})");

            CourtClick?.Invoke(this, dataPoint); // Notify VM
        }

        public void SetMarker(Point position, Brush color, bool isFilled)
        {
            RemoveMarkerAtPosition(position);

            if (color == Brushes.Transparent)
                return; // interpreted as "delete"

            var marker = new Ellipse
            {
                Width = MarkerSize,
                Height = MarkerSize,
                Stroke = color,
                StrokeThickness = 0.1,
                Fill = isFilled ? color : Brushes.Transparent
            };

            // Ensure markers appear above overlays such as the quick-shot panel
            Panel.SetZIndex(marker, 20);

            Canvas.SetLeft(marker, position.X - MarkerSize / 2);
            Canvas.SetTop(marker, position.Y - MarkerSize / 2);

            CourtCanvas.Children.Add(marker);
            _markerElements.Add(marker);
        }

        public void ClearAllMarkers()
        {
            foreach (var element in _markerElements)
            {
                CourtCanvas.Children.Remove(element);
            }
            _markerElements.Clear();
        }

        private void RemoveMarkerAtPosition(Point position)
        {
            double tolerance = 0.1;
            var elementsToRemove = _markerElements
                .Where(el =>
                {
                    double left = Canvas.GetLeft(el);
                    double top = Canvas.GetTop(el);
                    return Math.Abs(left + 0.5 - position.X) < tolerance &&
                           Math.Abs(top + 0.5 - position.Y) < tolerance;
                })
                .ToList();

            foreach (var el in elementsToRemove)
            {
                CourtCanvas.Children.Remove(el);
                _markerElements.Remove(el);
            }
        }

        private bool IsLeftHalfOfCourt(double x) => x <= 14;

        private bool IsThreePointer(double x, double y)
        {
            if (x <= 14)
            {
                if (x < 4 && (y < 0.9 || y > 14.1))
                    return true;

                double dx = x - 2.585;
                double dy = y - 7.5;
                return dx * dx + dy * dy >= 6.75 * 6.75 && x >= 0;
            }
            else
            {
                if (x > 24 && (y < 0.9 || y > 14.1))
                    return true;

                double dx = x - 25.415;
                double dy = y - 7.5;
                return dx * dx + dy * dy >= 6.75 * 6.75 && x <= 28;
            }
        }
        private UIElement? _tempMarker;

        public void ShowTemporaryMarker(Point point)
        {
            RemoveTemporaryMarker();

            var marker = new Ellipse
            {
                Width = MarkerSize,
                Height = MarkerSize,
                Stroke = Brushes.White,
                StrokeThickness = 0.1,
                Fill = Brushes.White
            };

            // Ensure temp marker is visible above overlays
            Panel.SetZIndex(marker, 20);

            Canvas.SetLeft(marker, point.X - MarkerSize / 2);
            Canvas.SetTop(marker, point.Y - MarkerSize / 2);

            CourtCanvas.Children.Add(marker);
            _tempMarker = marker;
        }

        public void RemoveTemporaryMarker()
        {
            if (_tempMarker != null)
            {
                CourtCanvas.Children.Remove(_tempMarker);
                _tempMarker = null;
            }
        }
    }
}