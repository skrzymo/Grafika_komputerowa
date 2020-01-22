using Caliburn.Micro;
using Projekt1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Rectangle = System.Windows.Shapes.Rectangle;
using Brushes = System.Windows.Media.Brushes;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using System.Windows.Media.Imaging;

namespace Projekt1.ViewModels
{
    public class ProjectThreeViewModel : Screen
    {
        private byte _insertedPoint = 0;
        private byte _bezierDegree;
        private Rectangle _selectedRectangle;
        private Canvas _bezierCanvas = new Canvas();

        public ObservableCollection<Point> PointList { get; set; } = new ObservableCollection<Point>();
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

        public ProjectThreeViewModel() { }

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
            for (int i = 0; i < BezierDegree; i++)
            {
                var point = new Point();
                PointList.Add(point);
            }
        }

        public void MouseDown(MouseEventArgs e)
        {
            if (_selectedRectangle != null)
                return;

            if (_insertedPoint + 1 <= PointList.Count)
            {
                var point = PointList[_insertedPoint];
                point.X = (int)e.GetPosition(this.ContentGrid).X;
                point.Y = (int)e.GetPosition(this.ContentGrid).Y;
                PointList[_insertedPoint] = point;

                var rectangle = new Rectangle
                {
                    Stroke = Brushes.Black,
                    Cursor = Cursors.Hand,
                    Fill = Brushes.Transparent,
                    Width = 10,
                    Height = 10
                };
                Message.SetAttach(rectangle, "[MouseDown] = [RectangleMouseDown($eventArgs)]; [MouseUp] = [RectangleMouseUp()]");

                Canvas.SetLeft(rectangle, point.X - 5);
                Canvas.SetTop(rectangle, point.Y - 5);
                this.ContentGrid.Children.Add(rectangle);

                _insertedPoint++;
            }

            if (_insertedPoint - 1 >= 1)
            {
                DrawBezier();
            }
        }

        public void RectangleMouseDown(MouseEventArgs e)
        {
            _selectedRectangle = e.OriginalSource as Rectangle;
        }

        public void RectangleMouseUp()
        {
            _selectedRectangle = null;
        }

        public void MouseMove(MouseEventArgs e)
        {
            if (_selectedRectangle != null)
            {
                Canvas.SetLeft(_selectedRectangle, e.GetPosition(this.ContentGrid).X - 5);
                Canvas.SetTop(_selectedRectangle, e.GetPosition(this.ContentGrid).Y - 5);
            }
        }

        private void DrawBezier()
        {
            for (int i = 0; i < _insertedPoint - 1; i++)
            {
                var line = new Line
                {
                    Stroke = Brushes.Black,
                    X1 = PointList[i].X,
                    Y1 = PointList[i].Y,
                    X2 = PointList[i + 1].X,
                    Y2 = PointList[i + 1].Y
                };

                this.ContentGrid.Children.Add(line);
            }

            DrawBezierCurve();
        }

        private void DrawBezierCurve()
        {
            this.ContentGrid.Children.Remove(_bezierCanvas);
            _bezierCanvas.Children.Clear();

            var pointList = new List<Point>();

            for (int i = 0; i < _insertedPoint; i++)
            {
                pointList.Add(PointList[i]);
            }

            DrawCasteljau(pointList);

            this.ContentGrid.Children.Add(_bezierCanvas);
        }

        private void DrawHigherBezierCurve(Point[] points, float t)
        {
            if (points.Length == 1)
            {
                var line = new Line
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    X1 = points[0].X,
                    Y1 = points[0].Y,
                    X2 = points[0].X + 1,
                    Y2 = points[0].Y + 1
                };

                _bezierCanvas.Children.Add(line);
            }
            else
            {
                var newPoints = new Point[points.Length - 1];

                for (int j = 0; j < newPoints.Length; j++)
                {
                    var x = (1 - t) * points[j].X + t * points[j + 1].X;
                    var y = (1 - t) * points[j].Y + t * points[j + 1].Y;
                    newPoints[j] = new Point((int)x, (int)y);
                }

                DrawHigherBezierCurve(newPoints, t);
            }
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
                    X2 = tmp.X + 1,
                    Y2 = tmp.Y + 1
                };

                //dla lepszego wyświetlania można dać bitmapę

                _bezierCanvas.Children.Add(line);
            }
        }


        private Point GetCasteljauPoint(int r, int i, double t)
        {
            if (r == 0) return PointList[i];

            Point p1 = GetCasteljauPoint(r - 1, i, t);
            Point p2 = GetCasteljauPoint(r - 1, i + 1, t);

            return new Point((int)((1 - t) * p1.X + t * p2.X), (int)((1 - t) * p1.Y + t * p2.Y));
        }

        private void DrawCubicBezierCurve()
        {
            for (float i = 0; i < 1; i += 0.01f)
            {
                var x1 = Math.Pow(1 - i, 3) * PointList[0].X + 3 * Math.Pow(1 - i, 2) * i * PointList[1].X
                        + 3 * (1 - i) * Math.Pow(i, 2) * PointList[2].X + Math.Pow(i, 3) * PointList[3].X;
                var y1 = Math.Pow(1 - i, 3) * PointList[0].Y + 3 * Math.Pow(1 - i, 2) * i * PointList[1].Y
                        + 3 * (1 - i) * Math.Pow(i, 2) * PointList[2].Y + Math.Pow(i, 3) * PointList[3].Y;
                var x2 = Math.Pow(1 - (i + 0.01f), 3) * PointList[0].X + 3 * Math.Pow(1 - (i + 0.01f), 2) * (i + 0.01f) * PointList[1].X
                        + 3 * (1 - (i + 0.01f)) * Math.Pow((i + 0.01f), 2) * PointList[2].X + Math.Pow((i + 0.01f), 3) * PointList[3].X;
                var y2 = Math.Pow(1 - (i + 0.01f), 3) * PointList[0].Y + 3 * Math.Pow(1 - (i + 0.01f), 2) * (i + 0.01f) * PointList[1].Y
                        + 3 * (1 - (i + 0.01f)) * Math.Pow((i + 0.01f), 2) * PointList[2].Y + Math.Pow((i + 0.01f), 3) * PointList[3].Y;

                var line = new Line
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2
                };

                _bezierCanvas.Children.Add(line);
            }
        }

        private void DrawQuadraticBezierCurve()
        {
            for (float i = 0; i < 1; i += 0.01f)
            {
                var x1 = (1 - i) * ((1 - i) * PointList[0].X + i * PointList[1].X) + i * ((1 - i) * PointList[1].X + i * PointList[2].X);
                var y1 = (1 - i) * ((1 - i) * PointList[0].Y + i * PointList[1].Y) + i * ((1 - i) * PointList[1].Y + i * PointList[2].Y);
                var x2 = (1 - (i + 0.01f)) * ((1 - (i + 0.01f)) * PointList[0].X + (i + 0.01f) * PointList[1].X) + (i + 0.01f) * ((1 - (i + 0.01f)) * PointList[1].X + (i + 0.01f) * PointList[2].X);
                var y2 = (1 - (i + 0.01f)) * ((1 - (i + 0.01f)) * PointList[0].Y + (i + 0.01f) * PointList[1].Y) + (i + 0.01f) * ((1 - (i + 0.01f)) * PointList[1].Y + (i + 0.01f) * PointList[2].Y);

                var line = new Line
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2
                };

                _bezierCanvas.Children.Add(line);
            }
        }

        private void DrawLinearBezierCurve()
        {
            for (float i = 0; i < 1; i += 0.01f)
            {
                var x1 = (1 - i) * PointList[0].X + i * PointList[1].X;
                var y1 = (1 - i) * PointList[0].Y + i * PointList[1].Y;
                var x2 = (1 - (i + 0.01f)) * PointList[0].X + (i + 0.01f) * PointList[1].X;
                var y2 = (1 - (i + 0.01f)) * PointList[0].Y + (i + 0.01f) * PointList[1].Y;

                var line = new Line
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2
                };

                _bezierCanvas.Children.Add(line);
            }
        }
    }
}
