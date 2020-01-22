using Caliburn.Micro;
using Projekt1.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Point = Projekt1.Models.Point;
using Projekt1.Models;

namespace Projekt1.ViewModels
{
    public class ProjectThreeViewModel : Screen
    {
        private bool _isBlocked = false;
        private RectangleExt _lastInputedRectangle;
        private sbyte _insertedPoint = -1;
        private byte _bezierDegree;
        private RectangleExt _selectedRectangle;
        private Canvas _bezierCanvas = new Canvas();

        public ObservableCollection<RectangleExt> PointList { get; set; } = new ObservableCollection<RectangleExt>();
        public Canvas ContentGrid { get; set; }

        public byte BezierDegree
        {
            get { return _bezierDegree; }
            set
            {
                _bezierDegree = value;
                CreatePoints();
            }
        }

        public ProjectThreeViewModel()
        {
            IoC.Get<IEventAggregator>().Subscribe(this);
        }

        protected override void OnViewLoaded(object view)
        {
            var myView = view as ProjectThreeView;
            this.ContentGrid = (Canvas)((Grid)myView.Content).FindName("ContentGrid");

            base.OnViewLoaded(view);
        }

        public static void ValidateText(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void CreatePoints()
        {
            PointList.Clear();
            ContentGrid.Children.Clear();

            for (int i = 0; i < BezierDegree; i++)
            {
                var point = new RectangleExt();
                PointList.Add(point);
            }
        }

        public void PointXChanged(RectangleExt dataContext, TextChangedEventArgs e)
        {
            if ((e.OriginalSource as TextBox).Text != "" && !_isBlocked)
            {
                dataContext.Point.X = double.Parse((e.OriginalSource as TextBox).Text);

                if (_insertedPoint != PointList.Count - 1 && _lastInputedRectangle != dataContext && PointList.IndexOf(dataContext) > _insertedPoint)
                {
                    _insertedPoint++;
                }

                DrawBezier();
            }

            _lastInputedRectangle = dataContext;
        }

        public void PointYChanged(RectangleExt dataContext, TextChangedEventArgs e)
        {
            if ((e.OriginalSource as TextBox).Text != "" && !_isBlocked)
            {
                dataContext.Point.Y = double.Parse((e.OriginalSource as TextBox).Text);

                if (_insertedPoint != PointList.Count - 1 && _lastInputedRectangle != dataContext && PointList.IndexOf(dataContext) > _insertedPoint)
                {
                    _insertedPoint++;
                }

                DrawBezier();
            }

            _lastInputedRectangle = dataContext;
        }

        public void MouseDown(MouseEventArgs e)
        {
            if (_selectedRectangle != null)
                return;

            if (_insertedPoint + 2 <= PointList.Count)
            {
                _insertedPoint++;
                _isBlocked = true;

                PointList[_insertedPoint].Point.X = e.GetPosition(this.ContentGrid).X;
                PointList[_insertedPoint].Point.Y = e.GetPosition(this.ContentGrid).Y;

                _isBlocked = false;

            }

            DrawBezier();
        }

        public void RectangleMouseDown(MouseEventArgs e)
        {
            _selectedRectangle = PointList.FirstOrDefault(p => p.Rectangle == e.OriginalSource);
        }

        public void RectangleMouseUp()
        {
            _selectedRectangle = null;
        }

        public void MouseMove(MouseEventArgs e)
        {
            if (_selectedRectangle != null)
            {

                _selectedRectangle.Point.X = e.GetPosition(this.ContentGrid).X;
                _selectedRectangle.Point.Y = e.GetPosition(this.ContentGrid).Y;

                DrawBezier();
            }
        }

        private void DrawBezier()
        {

            this.ContentGrid.Children.Clear();

            for (int i = 0; i < _insertedPoint; i++)
            {
                var line = new Line
                {
                    Stroke = Brushes.Black,
                    X1 = PointList[i].Point.X,
                    Y1 = PointList[i].Point.Y,
                    X2 = PointList[i + 1].Point.X,
                    Y2 = PointList[i + 1].Point.Y
                };


                this.ContentGrid.Children.Add(line);
            }


            for (int i = 0; i <= _insertedPoint; i++)
            {
                Canvas.SetLeft(PointList[i].Rectangle, PointList[i].Point.X - 5);
                Canvas.SetTop(PointList[i].Rectangle, PointList[i].Point.Y - 5);

                this.ContentGrid.Children.Add(PointList[i].Rectangle);
            }


            DrawBezierCurve();
        }

        private void DrawBezierCurve()
        {
            this.ContentGrid.Children.Remove(_bezierCanvas);
            _bezierCanvas.Children.Clear();

            var pointList = new List<Point>();

            for (int i = 0; i <= _insertedPoint; i++)
            {
                pointList.Add(PointList[i].Point);
            }

            DrawCasteljau(pointList);

            this.ContentGrid.Children.Add(_bezierCanvas);
        }

        private void DrawCasteljau(List<Point> points)
        {

            Point tmp;
            for (double t = 0; t <= 1; t += 0.001)
            {
                tmp = GetCasteljauPoint(points.Count - 1, 0, t);

                var line = new Line
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    X1 = tmp.X,
                    Y1 = tmp.Y,
                    X2 = tmp.X + 0.1,
                    Y2 = tmp.Y + 0.1
                };

                _bezierCanvas.Children.Add(line);
            }
        }


        private Point GetCasteljauPoint(int r, int i, double t)
        {
            if (r <= 0) return PointList[i].Point;

            Point p1 = GetCasteljauPoint(r - 1, i, t);
            Point p2 = GetCasteljauPoint(r - 1, i + 1, t);

            return new Point(((1 - t) * p1.X + t * p2.X), ((1 - t) * p1.Y + t * p2.Y));
        }
    }
}


