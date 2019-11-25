using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1.ImageFilters
{
    public abstract class Filter
    {
        protected const byte _maskWidth = 3;
        protected const byte _maskHeight = 3;
        protected Bitmap _tmpBitmap;
        protected Bitmap _sourceBitmap;
        protected int _width;
        protected int _height;

        public Bitmap ExecuteFilter(Bitmap sourceImage)
        {
            _width = sourceImage.Width;
            _height = sourceImage.Height;

            _sourceBitmap = sourceImage;
            _tmpBitmap = _sourceBitmap;

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    _tmpBitmap.SetPixel(j, i, ComputeColorForPixel(i, j));
                }
            }

            return _sourceBitmap;
        }

        protected abstract Color ComputeColorForPixel(int x, int y);

        protected bool CheckIfPixelExists(int x, int y)
        {
            if (x < 0 || x >= _height)
                return false;

            if (y < 0 || y >= _width)
                return false;

            return true;
        }
    }
}
