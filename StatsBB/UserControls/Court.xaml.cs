using StatsBB.Model;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StatsBB.UserControls;

public partial class Court : UserControl
{
    public event EventHandler<CourtPointData> CourtClick;
    public Court()
    {
        InitializeComponent();
    }

    private void CourtCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Point clickPosition = e.GetPosition(CourtCanvas);
        double x = clickPosition.X;
        double y = clickPosition.Y;
        bool isLeftHalfOfCourt = false;
        bool isBehindThreePointLine = false;

        if (x <= 14)
        {
            isLeftHalfOfCourt = true;
            if (x < 4 && (y < 0.9 || y > 14.1))
            {
                isBehindThreePointLine = true;
            }
            else
            {
                double basketCenterX = 2.585;
                double basketCenterY = 7.5;
                double threePointRadius = 6.75;

                double distSq = (x - basketCenterX) * (x - basketCenterX) + (y - basketCenterY) * (y - basketCenterY);
                double threePtRadiusSq = threePointRadius * threePointRadius;

                if (distSq >= threePtRadiusSq && x >= 0)
                {
                    isBehindThreePointLine = true;
                }
            }
        }
        else
        {
            if (x > 24 && (y < 0.9 || y > 14.1))
            {
                isBehindThreePointLine = true;
            }
            else
            {
                double basketCenterX = 25.415;
                double basketCenterY = 7.5;
                double threePointRadius = 6.75;

                double distSq = (x - basketCenterX) * (x - basketCenterX) + (y - basketCenterY) * (y - basketCenterY);
                double threePtRadiusSq = threePointRadius * threePointRadius;

                if (distSq >= threePtRadiusSq && x <= 28)
                {
                    isBehindThreePointLine = true;
                }
            }
        }

        double markerSize = 1.0;
        Ellipse markerCircle = new Ellipse
        {
            Width = markerSize,
            Height = markerSize,
            Stroke = isLeftHalfOfCourt ? Brushes.Orange : Brushes.Green,
            StrokeThickness = 0.05,
            Fill = isLeftHalfOfCourt ? Brushes.Orange : Brushes.Green,
        };
        Canvas.SetLeft(markerCircle, clickPosition.X - markerSize / 2);
        Canvas.SetTop(markerCircle, clickPosition.Y - markerSize / 2);

        TextBlock markerText = new TextBlock
        {
            Text = isBehindThreePointLine ? "3" : "2",
            FontSize = 0.8,
            Foreground = Brushes.White,
            FontWeight = FontWeights.Bold,
            Width = markerSize,
            Height = markerSize,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        Canvas.SetLeft(markerText, clickPosition.X - markerSize / 2);
        Canvas.SetTop(markerText, clickPosition.Y - markerSize / 2 - 0.1);

        CourtCanvas.Children.Add(markerCircle);
        CourtCanvas.Children.Add(markerText);

        CourtPointData DataPoint = new(clickPosition, isBehindThreePointLine, isLeftHalfOfCourt);
        string position = "(" + clickPosition.X.ToString("F2") + " : " + clickPosition.Y.ToString("F2") + ")";
        string team = isLeftHalfOfCourt ? "Team A" : "Team B";
        string points = isBehindThreePointLine ? "3" : "2";
        Debug.WriteLine($"{position} {team} {points}PT");
        CourtClick?.Invoke(this, DataPoint);
    }
}
