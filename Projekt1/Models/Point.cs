using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1.Models
{
    public class Point : Screen
    {
        private double _x;
        private double _y;

        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                NotifyOfPropertyChange();
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                NotifyOfPropertyChange();
            }
        }

        public Point()
        {

        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
