using Caliburn.Micro;
using Projekt1.Enums;
using Projekt1.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Projekt1.ViewModels
{
    public class ProjectOneViewModel : Screen, IViewAware
    {
        private int _r;
        private int _g;
        private int _b;
        private int _c;
        private int _m;
        private int _y;
        private int _k;
        private Brush _rGB;
        private Brush _cMYK;
        private bool _shouldConvert;
        private int _xOneCoordinate;
        private int _yOneCoordinate;
        private int _xTwoCoordinate;
        private int _yTwoCoordinate;
        private double _xClickedCoordinate;
        private double _yClickedCoordinate;
        private double _xOneClickedCoordinate;
        private double _yOneClickedCoordinate;
        private double _xTwoClickedCoordinate;
        private double _yTwoClickedCoordinate;
        private double _heightClicked;
        private double _widthClicked;
        private bool _isDrawing;
        private bool _isMoving;
        private bool _isSetCoordinatesBlocked;
        private PointsToResizeEnum _mouseOverPointsToResize;
        private UIElement _selectedPrimitive;

        public ObservableCollection<string> PrimitivesList { get; } = new ObservableCollection<string> { "Linia", "Prostokąt", "Okrąg" };
        public string SelectedPrimitiveFromList { get; set; }
        public Canvas ContentGrid { get; set; }
        public double XOneCoordinate
        {
            get { return _xOneCoordinate; }
            set
            {
                _xOneCoordinate = (int)value;
                this.SynchronizeCoordinates();
                this.NotifyOfPropertyChange();
            }
        }
        public double YOneCoordinate
        {
            get { return _yOneCoordinate; }
            set
            {
                _yOneCoordinate = (int)value;
                this.SynchronizeCoordinates();
                this.NotifyOfPropertyChange();
            }
        }
        public double XTwoCoordinate
        {
            get { return _xTwoCoordinate; }
            set
            {
                _xTwoCoordinate = (int)value;
                this.SynchronizeCoordinates();
                this.NotifyOfPropertyChange();
            }
        }
        public double YTwoCoordinate
        {
            get { return _yTwoCoordinate; }
            set
            {
                _yTwoCoordinate = (int)value;
                this.SynchronizeCoordinates();
                this.NotifyOfPropertyChange();
            }
        }

        public int R
        {
            get { return _r; }
            set
            {
                _r = ValidateRGBValue(value);
                this.UpdateRGBColor();
                this.NotifyOfPropertyChange();
            }
        }

        public int G
        {
            get { return _g; }
            set
            {
                _g = ValidateRGBValue(value);
                this.UpdateRGBColor();
                this.NotifyOfPropertyChange();
            }
        }

        public int B
        {
            get { return _b; }
            set
            {
                _b = ValidateRGBValue(value);
                this.UpdateRGBColor();
                this.NotifyOfPropertyChange();
            }
        }

        public int C
        {
            get { return _c; }
            set
            {
                _c = ValidateCMYKValue(value);
                this.UpdateCMYKColor();
                this.NotifyOfPropertyChange();
            }
        }

        public int M
        {
            get { return _m; }
            set
            {
                _m = ValidateCMYKValue(value);
                this.UpdateCMYKColor();
                this.NotifyOfPropertyChange();
            }
        }

        public int Y
        {
            get { return _y; }
            set
            {
                _y = ValidateCMYKValue(value);
                this.UpdateCMYKColor();
                this.NotifyOfPropertyChange();
            }
        }

        public int K
        {
            get { return _k; }
            set
            {
                _k = ValidateCMYKValue(value);
                this.UpdateCMYKColor();
                this.NotifyOfPropertyChange();
            }
        }

        public Brush RGBColor
        {
            get { return _rGB; }
            set
            {
                _rGB = value;
                if (_shouldConvert)
                    this.ConvertFromRGBToCMYK();
                this.NotifyOfPropertyChange();
            }
        }

        public Brush CMYKColor
        {
            get { return _cMYK; }
            set
            {
                _cMYK = value;
                if (_shouldConvert)
                    this.ConvertFromCMYKToRGB();
                this.NotifyOfPropertyChange();
            }
        }

        public ProjectOneViewModel()
        {
            this.SelectedPrimitiveFromList = this.PrimitivesList.First();
            this.NotifyOfPropertyChange(() => this.SelectedPrimitiveFromList);
        }

        protected override void OnActivate()
        {
            _shouldConvert = true;

            this.R = this.G = this.B = byte.MinValue;
            this.UpdateRGBColor();

            base.OnActivate();
        }

        protected override void OnViewLoaded(object view)
        {
            var myView = view as ProjectOneView;
            this.ContentGrid = (Canvas)((Grid)myView.Content).FindName("ContentGrid");

            base.OnViewLoaded(view);
        }

        public static void ValidateText(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private static byte ValidateRGBValue(int value)
        {
            return (byte)(value < 0 ? 0 : (value > 255 ? 255 : value));
        }

        private static int ValidateCMYKValue(int value)
        {
            return value < 0 ? 0 : (value > 100 ? 100 : value);
        }

        private void UpdateRGBColor()
        {
            this.RGBColor = new SolidColorBrush(Color.FromRgb((byte)this.R, (byte)this.G, (byte)this.B));
        }

        private void UpdateCMYKColor()
        {
            var cyan = this.C / 100f;
            var magenta = this.M / 100f;
            var yellow = this.Y / 100f;
            var blackKey = this.K / 100f;

            var red = 255 * (1 - Math.Min(1, cyan * (1 - blackKey) + blackKey));
            var green = 255 * (1 - Math.Min(1, magenta * (1 - blackKey) + blackKey));
            var blue = 255 * (1 - Math.Min(1, yellow * (1 - blackKey) + blackKey));

            this.CMYKColor = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));
        }

        private void ConvertFromCMYKToRGB()
        {
            _shouldConvert = false;

            var cyan = this.C / 100f;
            var magenta = this.M / 100f;
            var yellow = this.Y / 100f;
            var blackKey = this.K / 100f;

            this.R = (int)(255 * (1 - Math.Min(1, cyan * (1 - blackKey) + blackKey)));
            this.G = (int)(255 * (1 - Math.Min(1, magenta * (1 - blackKey) + blackKey)));
            this.B = (int)(255 * (1 - Math.Min(1, yellow * (1 - blackKey) + blackKey)));

            _shouldConvert = true;
        }

        private void ConvertFromRGBToCMYK()
        {
            _shouldConvert = false;

            var red = this.R / 255f;
            var green = this.G / 255f;
            var blue = this.B / 255f;

            var blackKey = Math.Min(1 - red, Math.Min(1 - green, 1 - blue));

            this.C = (int)((1 - red - blackKey) / (1 - blackKey) * 100);
            this.M = (int)((1 - green - blackKey) / (1 - blackKey) * 100);
            this.Y = (int)((1 - blue - blackKey) / (1 - blackKey) * 100);
            this.K = (int)(blackKey * 100);

            _shouldConvert = true;
        }

        public void AddPrimitive()
        {
            this.DrawPrimitive();
        }

        public void DeletePrimitive()
        {
            this.ContentGrid.Children.Remove(_selectedPrimitive);
            this.XOneCoordinate = 0;
            this.YOneCoordinate = 0;
            this.XTwoCoordinate = 0;
            this.YTwoCoordinate = 0;
        }

        public void SetRGBColor()
        {
            _selectedPrimitive.GetType().GetProperty("Stroke").SetValue(_selectedPrimitive, this.RGBColor);
        }

        public void SetCMYKColor()
        {
            _selectedPrimitive.GetType().GetProperty("Stroke").SetValue(_selectedPrimitive, this.CMYKColor);
        }

        private void DrawPrimitive()
        {
            if (this.SelectedPrimitiveFromList == "Linia")
                this.DrawLine();

            if (this.SelectedPrimitiveFromList == "Prostokąt")
                this.DrawRectangle();

            if (this.SelectedPrimitiveFromList == "Okrąg")
                this.DrawCircle();
        }

        public void DrawLine()
        {
            _selectedPrimitive = new Line();
            ((Line)_selectedPrimitive).Stroke = Brushes.Black;
            ((Line)_selectedPrimitive).Cursor = Cursors.Hand;
            Message.SetAttach(_selectedPrimitive, "[MouseMove] = [MouseMove($eventArgs)]");

            this.ResizeAndMoveLine();

            this.ContentGrid.Children.Add(_selectedPrimitive);
        }

        public void DrawRectangle()
        {
            _selectedPrimitive = new Rectangle();

            ((Rectangle)_selectedPrimitive).Stroke = Brushes.Black;
            ((Rectangle)_selectedPrimitive).Cursor = Cursors.Hand;
            ((Rectangle)_selectedPrimitive).Fill = Brushes.Transparent;
            Message.SetAttach((Rectangle)_selectedPrimitive, "[MouseMove] = [MouseMove($eventArgs)]");

            this.ResizeAndMoveRectangle();

            this.ContentGrid.Children.Add((Rectangle)_selectedPrimitive);
        }

        public void DrawCircle()
        {
            _selectedPrimitive = new Ellipse();

            ((Ellipse)_selectedPrimitive).Stroke = Brushes.Black;
            ((Ellipse)_selectedPrimitive).Cursor = Cursors.Hand;
            ((Ellipse)_selectedPrimitive).Fill = Brushes.Transparent;
            Message.SetAttach(((Ellipse)_selectedPrimitive), "[MouseMove] = [MouseMove($eventArgs)]");

            this.ResizeAndMoveCircle();

            this.ContentGrid.Children.Add(((Ellipse)_selectedPrimitive));
        }

        private void SelectPrimitive()
        {
            _isSetCoordinatesBlocked = true;

            if (_selectedPrimitive is Line)
            {
                this.XOneCoordinate = ((Line)_selectedPrimitive).X1;
                this.YOneCoordinate = ((Line)_selectedPrimitive).Y1;
                this.XTwoCoordinate = ((Line)_selectedPrimitive).X2;
                this.YTwoCoordinate = ((Line)_selectedPrimitive).Y2;
            }

            if (_selectedPrimitive is Rectangle)
            {
                this.CheckWhichCoordinatesAreLess();
            }

            if (_selectedPrimitive is Ellipse)
            {
                this.CheckWhichCoordinatesAreLess();
            }

            _isSetCoordinatesBlocked = false;
        }

        private void CheckWhichCoordinatesAreLess()
        {
            if (this.XOneCoordinate < this.XTwoCoordinate)
            {
                this.XOneCoordinate = Canvas.GetLeft(_selectedPrimitive);
                this.XTwoCoordinate = this.XOneCoordinate + (double)_selectedPrimitive.GetType().GetProperty("ActualWidth").GetValue(_selectedPrimitive);
            }
            else
            {
                this.XTwoCoordinate = Canvas.GetLeft(_selectedPrimitive);
                this.XOneCoordinate = this.XTwoCoordinate + (double)_selectedPrimitive.GetType().GetProperty("ActualWidth").GetValue(_selectedPrimitive);
            }

            if (this.YOneCoordinate < this.YTwoCoordinate)
            {
                this.YOneCoordinate = Canvas.GetTop(_selectedPrimitive);
                this.YTwoCoordinate = this.YOneCoordinate + (double)_selectedPrimitive.GetType().GetProperty("ActualHeight").GetValue(_selectedPrimitive);
            }
            else
            {
                this.YTwoCoordinate = Canvas.GetTop(_selectedPrimitive);
                this.YOneCoordinate = this.YTwoCoordinate + (double)_selectedPrimitive.GetType().GetProperty("ActualHeight").GetValue(_selectedPrimitive);
            }
        }

        public void MouseDown(MouseEventArgs e)
        {
            _selectedPrimitive = e.OriginalSource as UIElement;
            this.SelectPrimitive();
            var currentX = e.GetPosition(this.ContentGrid).X;
            var currentY = e.GetPosition(this.ContentGrid).Y;

            if (e.OriginalSource is Canvas && e.LeftButton == MouseButtonState.Pressed)
            {
                this.XOneCoordinate = this.XTwoCoordinate = currentX;
                this.YOneCoordinate = this.YTwoCoordinate = currentY;
                _isDrawing = true;
                this.DrawPrimitive();
            }

            if (e.OriginalSource is Line && e.LeftButton == MouseButtonState.Pressed)
            {
                this.SetClickedCoordinates(currentX, currentY);
            }

            if (e.OriginalSource is Rectangle && e.LeftButton == MouseButtonState.Pressed)
            {
                this.SetClickedCoordinates(currentX, currentY);
            }

            if (e.OriginalSource is Ellipse && e.LeftButton == MouseButtonState.Pressed)
            {
                this.SetClickedCoordinates(currentX, currentY);
            }
        }

        public void MouseMove(MouseEventArgs e)
        {
            if (!_isDrawing && !_isMoving && e.LeftButton != MouseButtonState.Pressed && e.OriginalSource is Line)
                this.IsMouseOverEndOfLine(e.GetPosition(this.ContentGrid).X, (Line)e.OriginalSource);

            if (!_isDrawing && !_isMoving && e.LeftButton != MouseButtonState.Pressed && e.OriginalSource is Rectangle)
                this.IsMouseOverBorderOfRectangle(e.GetPosition(this.ContentGrid).X, e.GetPosition(this.ContentGrid).Y, (Rectangle)e.OriginalSource);

            if (!_isDrawing && !_isMoving && e.LeftButton != MouseButtonState.Pressed && e.OriginalSource is Ellipse)
                this.IsMouseOverBorderOfCircle(e.GetPosition(this.ContentGrid).X, e.GetPosition(this.ContentGrid).Y, (Ellipse)e.OriginalSource);

            var currentX = e.GetPosition(this.ContentGrid).X;
            var currentY = e.GetPosition(this.ContentGrid).Y;

            if (_isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                this.XTwoCoordinate = currentX;
                this.YTwoCoordinate = currentY;

                if (this.SelectedPrimitiveFromList == "Linia")
                    this.ResizeAndMoveLine();

                if (this.SelectedPrimitiveFromList == "Prostokąt")
                    this.ResizeAndMoveRectangle();

                if (this.SelectedPrimitiveFromList == "Okrąg")
                    this.ResizeAndMoveCircle();
            }

            if (_isMoving && _selectedPrimitive is Rectangle && e.LeftButton == MouseButtonState.Pressed)
            {
                this.SetCoordinatesByMousePosition(currentX, currentY);
                this.ResizeAndMoveRectangle();
            }

            if (_isMoving && _selectedPrimitive is Ellipse && e.LeftButton == MouseButtonState.Pressed)
            {
                this.SetCoordinatesByMousePosition(currentX, currentY);
                this.ResizeAndMoveCircle();
            }

            if (_isMoving && _selectedPrimitive is Line && e.LeftButton == MouseButtonState.Pressed)
            {
                if (_mouseOverPointsToResize == PointsToResizeEnum.None)
                {
                    this.XOneCoordinate = currentX - (_xClickedCoordinate - _xOneClickedCoordinate);
                    this.YOneCoordinate = currentY - (_yClickedCoordinate - _yOneClickedCoordinate);
                    this.XTwoCoordinate = currentX - (_xClickedCoordinate - _xTwoClickedCoordinate);
                    this.YTwoCoordinate = currentY - (_yClickedCoordinate - _yTwoClickedCoordinate);
                }

                if (_mouseOverPointsToResize == PointsToResizeEnum.Left)
                {
                    this.XOneCoordinate = currentX - (_xClickedCoordinate - _xOneClickedCoordinate);
                    this.YOneCoordinate = currentY - (_yClickedCoordinate - _yOneClickedCoordinate);
                }

                if (_mouseOverPointsToResize == PointsToResizeEnum.Right)
                {
                    this.XTwoCoordinate = currentX - (_xClickedCoordinate - _xTwoClickedCoordinate);
                    this.YTwoCoordinate = currentY - (_yClickedCoordinate - _yTwoClickedCoordinate);
                }

                this.ResizeAndMoveLine();
            }
        }

        private double CalculateShiftForXLeft(double xLeft, double xRight, double width, double currentX)
        {
            if (xLeft < xRight)
            {
                var widthCoefficient = (_widthClicked - width) / _widthClicked;
                var widthShift = Math.Abs((currentX - xLeft) * widthCoefficient);
                xLeft += widthShift;
            }
            else
            {
                var widthShift = Math.Abs(currentX - xLeft);
                xLeft += widthShift;
            }

            return xLeft;
        }

        private double CalculateShiftForXRight(double xLeft, double xRight, double width, double currentX)
        {
            if (xLeft < xRight)
            {
                var widthCoefficient = (_widthClicked - width) / _widthClicked;
                var widthShift = Math.Abs((currentX - xRight) * widthCoefficient);
                xRight -= widthShift;
            }
            else
            {
                var widthShift = Math.Abs(currentX - xRight);
                xRight -= widthShift;
            }

            return xRight;
        }

        private double CalculateShiftForYTop(double yTop, double yBottom, double height, double currentY)
        {
            if (yTop < yBottom)
            {
                var heightCoefficient = (_heightClicked - height) / _heightClicked;
                var heightShift = Math.Abs((currentY - yTop) * heightCoefficient);
                yTop += heightShift;
            }
            else
            {
                var heightShift = Math.Abs(currentY - yTop);
                yTop += heightShift;
            }

            return yTop;
        }

        private double CalculateShiftForYBottom(double yTop, double yBottom, double height, double currentY)
        {
            if (yTop < yBottom)
            {
                var heightCoefficient = (_heightClicked - height) / _heightClicked;
                var heightShift = Math.Abs((currentY - yBottom) * heightCoefficient);
                yBottom -= heightShift;
            }
            else
            {
                var heightShift = Math.Abs(currentY - yBottom);
                yBottom -= heightShift;
            }

            return yBottom;
        }

        private void SetCoordinatesByMousePosition(double x, double y)
        {
            var width = Math.Abs(this.XTwoCoordinate - this.XOneCoordinate);
            var height = Math.Abs(this.YTwoCoordinate - this.YOneCoordinate);

            if (_mouseOverPointsToResize == PointsToResizeEnum.None)
            {
                this.XOneCoordinate = x - (_xClickedCoordinate - _xOneClickedCoordinate);
                this.YOneCoordinate = y - (_yClickedCoordinate - _yOneClickedCoordinate);
                this.XTwoCoordinate = x - (_xClickedCoordinate - _xTwoClickedCoordinate);
                this.YTwoCoordinate = y - (_yClickedCoordinate - _yTwoClickedCoordinate);
            }

            if (_mouseOverPointsToResize == PointsToResizeEnum.LeftUpper)
            {
                this.CalculateXLeft(x, width);
                this.CalculateYTop(y, height);
            }

            if (_mouseOverPointsToResize == PointsToResizeEnum.LeftDown)
            {
                this.CalculateXLeft(x, width);
                this.CalculateYBottom(y, height);
            }

            if (_mouseOverPointsToResize == PointsToResizeEnum.Left)
                this.CalculateXLeft(x, width);

            if (_mouseOverPointsToResize == PointsToResizeEnum.RightUpper)
            {
                this.CalculateXRight(x, width);
                this.CalculateYTop(y, height);
            }

            if (_mouseOverPointsToResize == PointsToResizeEnum.RightDown)
            {
                this.CalculateXRight(x, width);
                this.CalculateYBottom(y, height);
            }

            if (_mouseOverPointsToResize == PointsToResizeEnum.Right)
                this.CalculateXRight(x, width);

            if (_mouseOverPointsToResize == PointsToResizeEnum.Top)
                this.CalculateYTop(y, height);

            if (_mouseOverPointsToResize == PointsToResizeEnum.Bottom)
                this.CalculateYBottom(y, height);
        }

        private void CalculateXLeft(double x, double width)
        {
            if (_xOneClickedCoordinate < _xTwoClickedCoordinate)
            {
                this.XOneCoordinate = x - (_xClickedCoordinate - _xOneClickedCoordinate);
                this.XOneCoordinate = this.CalculateShiftForXLeft(this.XOneCoordinate, this.XTwoCoordinate, width, x);
            }
            else
            {
                this.XTwoCoordinate = x - (_xClickedCoordinate - _xTwoClickedCoordinate);
                this.XTwoCoordinate = this.CalculateShiftForXLeft(this.XTwoCoordinate, this.XOneCoordinate, width, x);
            }
        }

        private void CalculateXRight(double x, double width)
        {
            if (_xOneClickedCoordinate > _xTwoClickedCoordinate)
            {
                this.XOneCoordinate = x - (_xClickedCoordinate - _xOneClickedCoordinate);
                this.XOneCoordinate = this.CalculateShiftForXRight(this.XTwoCoordinate, this.XOneCoordinate, width, x);
            }
            else
            {
                this.XTwoCoordinate = x - (_xClickedCoordinate - _xTwoClickedCoordinate);
                this.XTwoCoordinate = this.CalculateShiftForXRight(this.XOneCoordinate, this.XTwoCoordinate, width, x);
            }
        }

        private void CalculateYTop(double y, double height)
        {
            if (_yOneClickedCoordinate < _yTwoClickedCoordinate)
            {
                this.YOneCoordinate = y - (_yClickedCoordinate - _yOneClickedCoordinate);
                this.YOneCoordinate = this.CalculateShiftForYTop(this.YOneCoordinate, this.YTwoCoordinate, height, y);
            }
            else
            {
                this.YTwoCoordinate = y - (_yClickedCoordinate - _yTwoClickedCoordinate);
                this.YTwoCoordinate = this.CalculateShiftForYTop(this.YTwoCoordinate, this.YOneCoordinate, height, y);
            }
        }

        private void CalculateYBottom(double y, double height)
        {
            if (_yOneClickedCoordinate > _yTwoClickedCoordinate)
            {
                this.YOneCoordinate = y - (_yClickedCoordinate - _yOneClickedCoordinate);
                this.YOneCoordinate = this.CalculateShiftForYBottom(this.YTwoCoordinate, this.YOneCoordinate, height, y);
            }
            else
            {
                this.YTwoCoordinate = y - (_yClickedCoordinate - _yTwoClickedCoordinate);
                this.YTwoCoordinate = this.CalculateShiftForYBottom(this.YOneCoordinate, this.YTwoCoordinate, height, y);
            }
        }

        private void ResizeAndMoveLine()
        {
            _isSetCoordinatesBlocked = true;

            this.SetCoordinates();

            ((Line)_selectedPrimitive).X1 = this.XOneCoordinate;
            ((Line)_selectedPrimitive).Y1 = this.YOneCoordinate;
            ((Line)_selectedPrimitive).X2 = this.XTwoCoordinate;
            ((Line)_selectedPrimitive).Y2 = this.YTwoCoordinate;

            _isSetCoordinatesBlocked = false;
        }

        private void ResizeAndMoveRectangle()
        {
            _isSetCoordinatesBlocked = true;

            this.SetCoordinates();

            ((Rectangle)_selectedPrimitive).Width = Math.Abs(this.XTwoCoordinate - this.XOneCoordinate);
            ((Rectangle)_selectedPrimitive).Height = Math.Abs(this.YTwoCoordinate - this.YOneCoordinate);

            _isSetCoordinatesBlocked = false;
        }

        private void ResizeAndMoveCircle()
        {
            _isSetCoordinatesBlocked = true;

            this.SetCoordinates();

            ((Ellipse)_selectedPrimitive).Width = Math.Abs(this.XTwoCoordinate - this.XOneCoordinate);
            ((Ellipse)_selectedPrimitive).Height = Math.Abs(this.YTwoCoordinate - this.YOneCoordinate);

            _isSetCoordinatesBlocked = false;
        }

        public void ClearFlags()
        {
            _isDrawing = false;
            _isMoving = false;
            if (this.XOneCoordinate == this.XTwoCoordinate && this.YOneCoordinate == this.YTwoCoordinate)
                this.DeletePrimitive();
        }

        public void SynchronizeCoordinates()
        {
            if (_selectedPrimitive != null && !_isMoving && !_isSetCoordinatesBlocked)
            {
                switch (_selectedPrimitive.GetType().Name)
                {
                    case "Line":
                        this.ResizeAndMoveLine();
                        break;
                    case "Rectangle":
                        this.ResizeAndMoveRectangle();
                        break;
                    case "Ellipse":
                        this.ResizeAndMoveCircle();
                        break;
                    default:
                        break;
                }
            }
        }

        private void SetCoordinates()
        {
            var width = Math.Abs(this.XTwoCoordinate - this.XOneCoordinate);
            var height = Math.Abs(this.YTwoCoordinate - this.YOneCoordinate);
            double diff1;
            double diff2;

            if (this.XOneCoordinate < this.XTwoCoordinate)
            {
                if (_mouseOverPointsToResize != PointsToResizeEnum.None)
                {
                    diff1 = this.XOneCoordinate < 0 ? 0 - this.XOneCoordinate : 0;
                    diff2 = this.XTwoCoordinate > this.ContentGrid.ActualWidth ? this.XTwoCoordinate - this.ContentGrid.ActualWidth : 0;
                    width = width - diff1 - diff2;
                }

                this.XOneCoordinate = this.XOneCoordinate < 0 ? 0 :
                    (this.XOneCoordinate + width > this.ContentGrid.ActualWidth ? this.ContentGrid.ActualWidth - width : this.XOneCoordinate);
                this.XTwoCoordinate = this.XOneCoordinate + width;

                if (_selectedPrimitive is Rectangle || _selectedPrimitive is Ellipse)
                    Canvas.SetLeft(_selectedPrimitive, this.XOneCoordinate);
            }
            else
            {
                if (_mouseOverPointsToResize != PointsToResizeEnum.None)
                {
                    diff1 = this.XOneCoordinate > this.ContentGrid.ActualWidth ? this.XOneCoordinate - this.ContentGrid.ActualWidth : 0;
                    diff2 = this.XTwoCoordinate < 0 ? 0 - this.XTwoCoordinate : 0;
                    width = width - diff1 - diff2;
                }

                this.XTwoCoordinate = this.XTwoCoordinate < 0 ? 0 :
                    (this.XTwoCoordinate + width > this.ContentGrid.ActualWidth ? this.ContentGrid.ActualWidth - width : this.XTwoCoordinate);
                this.XOneCoordinate = this.XTwoCoordinate + width;

                if (_selectedPrimitive is Rectangle || _selectedPrimitive is Ellipse)
                    Canvas.SetLeft(_selectedPrimitive, this.XTwoCoordinate);
            }

            if (this.YOneCoordinate < this.YTwoCoordinate)
            {
                if (_mouseOverPointsToResize != PointsToResizeEnum.None)
                {
                    diff1 = this.YOneCoordinate < 0 ? 0 - this.YOneCoordinate : 0;
                    diff2 = this.YTwoCoordinate > this.ContentGrid.ActualHeight ? this.YTwoCoordinate - this.ContentGrid.ActualHeight : 0;
                    height = height - diff1 - diff2;
                }

                this.YOneCoordinate = this.YOneCoordinate < 0 ? 0 :
                    (this.YOneCoordinate + height > this.ContentGrid.ActualHeight ? this.ContentGrid.ActualHeight - height : this.YOneCoordinate);
                this.YTwoCoordinate = this.YOneCoordinate + height;

                if (_selectedPrimitive is Rectangle || _selectedPrimitive is Ellipse)
                    Canvas.SetTop(_selectedPrimitive, this.YOneCoordinate);
            }
            else
            {
                if (_mouseOverPointsToResize != PointsToResizeEnum.None)
                {
                    diff1 = this.YTwoCoordinate < 0 ? 0 - this.YTwoCoordinate : 0;
                    diff2 = this.YOneCoordinate > this.ContentGrid.ActualHeight ? this.YOneCoordinate - this.ContentGrid.ActualHeight : 0;
                    height = height - diff1 - diff2;
                }

                this.YTwoCoordinate = this.YTwoCoordinate < 0 ? 0 :
                    (this.YTwoCoordinate + height > this.ContentGrid.ActualHeight ? this.ContentGrid.ActualHeight - height : this.YTwoCoordinate);
                this.YOneCoordinate = this.YTwoCoordinate + height;

                if (_selectedPrimitive is Rectangle || _selectedPrimitive is Ellipse)
                    Canvas.SetTop(_selectedPrimitive, this.YTwoCoordinate);
            }
        }

        private void SetClickedCoordinates(double x, double y)
        {
            _isMoving = true;
            _xClickedCoordinate = x;
            _yClickedCoordinate = y;
            _xOneClickedCoordinate = this.XOneCoordinate;
            _yOneClickedCoordinate = this.YOneCoordinate;
            _xTwoClickedCoordinate = this.XTwoCoordinate;
            _yTwoClickedCoordinate = this.YTwoCoordinate;
            _heightClicked = Math.Abs(_yOneClickedCoordinate - _yTwoClickedCoordinate);
            _widthClicked = Math.Abs(_xOneClickedCoordinate - _xTwoClickedCoordinate);
        }

        private void IsMouseOverEndOfLine(double x, Line line)
        {
            if (x <= line.X1 + 10 && x >= line.X1 - 10)
            {
                line.Cursor = Cursors.SizeWE;
                _mouseOverPointsToResize = PointsToResizeEnum.Left;
                return;
            }

            if (x >= line.X2 - 10 && x <= line.X2 + 10)
            {
                line.Cursor = Cursors.SizeWE;
                _mouseOverPointsToResize = PointsToResizeEnum.Right;
                return;
            }

            line.Cursor = Cursors.Hand;
            _mouseOverPointsToResize = PointsToResizeEnum.None;
            return;
        }

        private void IsMouseOverBorderOfRectangle(double x, double y, Rectangle rectangle)
        {
            if (x <= Canvas.GetLeft(rectangle) + 10
                && y <= Canvas.GetTop(rectangle) + 10)
            {
                rectangle.Cursor = Cursors.SizeNWSE;
                _mouseOverPointsToResize = PointsToResizeEnum.LeftUpper;
                return;
            }

            if (x <= Canvas.GetLeft(rectangle) + 10
                && y >= Canvas.GetTop(rectangle) + rectangle.ActualHeight - 10)
            {
                rectangle.Cursor = Cursors.SizeNESW;
                _mouseOverPointsToResize = PointsToResizeEnum.LeftDown;
                return;
            }

            if (x >= Canvas.GetLeft(rectangle) + rectangle.ActualWidth - 10
                && y <= Canvas.GetTop(rectangle) + 10)
            {
                rectangle.Cursor = Cursors.SizeNESW;
                _mouseOverPointsToResize = PointsToResizeEnum.RightUpper;
                return;
            }

            if (x >= Canvas.GetLeft(rectangle) + rectangle.ActualWidth - 10
                && y >= Canvas.GetTop(rectangle) + rectangle.ActualHeight - 10)
            {
                rectangle.Cursor = Cursors.SizeNWSE;
                _mouseOverPointsToResize = PointsToResizeEnum.RightDown;
                return;
            }

            if (x < Canvas.GetLeft(rectangle) + rectangle.ActualWidth - 10 && x > Canvas.GetLeft(rectangle) + 10
                && y <= Canvas.GetTop(rectangle) + 10)
            {
                rectangle.Cursor = Cursors.SizeNS;
                _mouseOverPointsToResize = PointsToResizeEnum.Top;
                return;
            }

            if (x < Canvas.GetLeft(rectangle) + rectangle.ActualWidth - 10 && x > Canvas.GetLeft(rectangle) + 10
                && y >= Canvas.GetTop(rectangle) + rectangle.ActualHeight - 10)
            {
                rectangle.Cursor = Cursors.SizeNS;
                _mouseOverPointsToResize = PointsToResizeEnum.Bottom;
                return;
            }

            if (x <= Canvas.GetLeft(rectangle) + 10
                && y < Canvas.GetTop(rectangle) + rectangle.ActualHeight - 10 && y > Canvas.GetTop(rectangle) + 10)
            {
                rectangle.Cursor = Cursors.SizeWE;
                _mouseOverPointsToResize = PointsToResizeEnum.Left;
                return;
            }

            if (x >= Canvas.GetLeft(rectangle) + rectangle.ActualWidth - 10
                && y < Canvas.GetTop(rectangle) + rectangle.ActualHeight - 10 && y > Canvas.GetTop(rectangle) + 10)
            {
                rectangle.Cursor = Cursors.SizeWE;
                _mouseOverPointsToResize = PointsToResizeEnum.Right;
                return;
            }

            rectangle.Cursor = Cursors.Hand;
            _mouseOverPointsToResize = PointsToResizeEnum.None;
            return;
        }

        private void IsMouseOverBorderOfCircle(double x, double y, Ellipse ellipse)
        {
            if (x <= Canvas.GetLeft(ellipse) + ellipse.ActualWidth / 2 - (x - Canvas.GetLeft(ellipse))
                && y <= Canvas.GetTop(ellipse) + ellipse.ActualHeight / 2 - (y - Canvas.GetTop(ellipse)))
            {
                ellipse.Cursor = Cursors.SizeNWSE;
                _mouseOverPointsToResize = PointsToResizeEnum.LeftUpper;
                return;
            }

            if (x <= Canvas.GetLeft(ellipse) + ellipse.ActualWidth / 2 - (x - Canvas.GetLeft(ellipse))
                && y >= Canvas.GetTop(ellipse) + ellipse.ActualHeight / 2 + (Canvas.GetTop(ellipse) + ellipse.ActualHeight - y))
            {
                ellipse.Cursor = Cursors.SizeNESW;
                _mouseOverPointsToResize = PointsToResizeEnum.LeftDown;
                return;
            }

            if (x >= Canvas.GetLeft(ellipse) + ellipse.ActualWidth / 2 + (Canvas.GetLeft(ellipse) + ellipse.ActualWidth - x)
                && y <= Canvas.GetTop(ellipse) + ellipse.ActualHeight / 2 - (y - Canvas.GetTop(ellipse)))
            {
                ellipse.Cursor = Cursors.SizeNESW;
                _mouseOverPointsToResize = PointsToResizeEnum.RightUpper;
                return;
            }

            if (x >= Canvas.GetLeft(ellipse) + ellipse.ActualWidth / 2 + (Canvas.GetLeft(ellipse) + ellipse.ActualWidth - x)
                && y >= Canvas.GetTop(ellipse) + ellipse.ActualHeight / 2 + (Canvas.GetTop(ellipse) + ellipse.ActualHeight - y))
            {
                ellipse.Cursor = Cursors.SizeNWSE;
                _mouseOverPointsToResize = PointsToResizeEnum.RightDown;
                return;
            }

            if (x < Canvas.GetLeft(ellipse) + ellipse.ActualWidth - 10 && x > Canvas.GetLeft(ellipse) + 10
                && y <= Canvas.GetTop(ellipse) + 10)
            {
                ellipse.Cursor = Cursors.SizeNS;
                _mouseOverPointsToResize = PointsToResizeEnum.Top;
                return;
            }

            if (x < Canvas.GetLeft(ellipse) + ellipse.ActualWidth - 10 && x > Canvas.GetLeft(ellipse) + 10
                && y >= Canvas.GetTop(ellipse) + ellipse.ActualHeight - 10)
            {
                ellipse.Cursor = Cursors.SizeNS;
                _mouseOverPointsToResize = PointsToResizeEnum.Bottom;
                return;
            }

            if (x <= Canvas.GetLeft(ellipse) + 10
                && y < Canvas.GetTop(ellipse) + ellipse.ActualHeight - 10 && y > Canvas.GetTop(ellipse) + 10)
            {
                ellipse.Cursor = Cursors.SizeWE;
                _mouseOverPointsToResize = PointsToResizeEnum.Left;
                return;
            }

            if (x >= Canvas.GetLeft(ellipse) + ellipse.ActualWidth - 10
                && y < Canvas.GetTop(ellipse) + ellipse.ActualHeight - 10 && y > Canvas.GetTop(ellipse) + 10)
            {
                ellipse.Cursor = Cursors.SizeWE;
                _mouseOverPointsToResize = PointsToResizeEnum.Right;
                return;
            }

            ellipse.Cursor = Cursors.Hand;
            _mouseOverPointsToResize = PointsToResizeEnum.None;
            return;
        }
    }
}
