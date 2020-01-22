using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Projekt1.Models
{
    public class RectangleExt : Screen
    {
        public Rectangle Rectangle { get; set; }
        public Point Point { get; set; }

        public RectangleExt()
        {
            Point = new Point();
            Rectangle = new Rectangle
            {
                Stroke = Brushes.Black,
                Cursor = Cursors.Hand,
                Fill = Brushes.Transparent,
                Width = 10,
                Height = 10
            };
            Message.SetAttach(Rectangle, "[MouseDown] = [RectangleMouseDown($eventArgs)]; [MouseUp] = [RectangleMouseUp()]");
        }
    }
}
