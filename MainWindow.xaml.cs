using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Point? startPoint;
        private Point? endPoint;
        private Color selectedColor;
        private double brushSize;
        private DrawingMode currentMode;

        public enum DrawingMode
        {
            Draw,
            Edit,
            Erase
        }
        public MainWindow()
        {
            InitializeComponent();
            selectedColor = Colors.Black;
            brushSize = 10;
            currentMode = DrawingMode.Draw;
            ColorPicker.ItemsSource = new List<Color>
            {
                Colors.Black, Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Magenta, Colors.Cyan
            };
            ColorPicker.SelectedIndex = 0;
            BrushSizeSlider.Minimum = 1;
            BrushSizeSlider.Maximum = 100;
            BrushSizeSlider.Value = brushSize;
            DrawRadioButton.IsChecked = true;
        }
        private void ColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedColor = (Color)ColorPicker.SelectedItem;
        }
        private void BrushSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            brushSize = BrushSizeSlider.Value;
        }
        private void ModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == DrawRadioButton)
            {
                currentMode = DrawingMode.Draw;
            }
            else if (sender == EditRadioButton)
            {
                currentMode = DrawingMode.Edit;
            }
            else if (sender == EraseRadioButton)
            {
                currentMode = DrawingMode.Erase;
            }
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(Canvas);
            endPoint = startPoint;

            if (currentMode == DrawingMode.Draw)
            {
                DrawLine(startPoint, endPoint, selectedColor, brushSize);
            }
            else if (currentMode == DrawingMode.Erase)
            {
                EraseLineAtPoint(startPoint);
            }
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && startPoint.HasValue)
            {
                endPoint = e.GetPosition(Canvas);

                if (currentMode == DrawingMode.Draw)
                {
                    DrawLine(startPoint, endPoint, selectedColor, brushSize);
                }
                else if (currentMode == DrawingMode.Edit)
                {
                    DrawLine(startPoint, endPoint, Colors.White, brushSize);
                }

                startPoint = endPoint;
            }
        }
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            startPoint = null;
        }
        private void DrawLine(Point? startPoint, Point? endPoint, Color color, double thickness)
        {
            if (startPoint.HasValue && endPoint.HasValue)
            {
                Line line = new Line
                {
                    Stroke = new SolidColorBrush(color),
                    StrokeThickness = thickness,
                    X1 = startPoint.Value.X,
                    Y1 = startPoint.Value.Y,
                    X2 = endPoint.Value.X,
                    Y2 = endPoint.Value.Y
                };
                Canvas.Children.Add(line);
            }
        }
        private void EraseLineAtPoint(Point? point)
        {
            if (point.HasValue)
            {
                var lineToRemove = FindLineAtPoint(point.Value);
                if (lineToRemove != null)
                {
                    Canvas.Children.Remove(lineToRemove);
                }
            }
        }
        private Line FindLineAtPoint(Point point)
        {
            foreach (UIElement element in Canvas.Children)
            {
                if (element is Line line)
                {
                    if (IsPointOnLine(point, line))
                    {
                        return line;
                    }
                }
            }
            return null;
        }

        private bool IsPointOnLine(Point point, Line line)
        {
            double distance = Math.Abs((line.Y2 - line.Y1) * point.X - (line.X2 - line.X1) * point.Y + line.X2 * line.Y1 - line.Y2 * line.X1) /
                              Math.Sqrt(Math.Pow(line.Y2 - line.Y1, 2) + Math.Pow(line.X2 - line.X1, 2));

            return distance < 5;
        }
    }
}