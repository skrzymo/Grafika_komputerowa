using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1.ImageFilters
{
    public class SmoothingFilter : Filter
    {
        protected override Color ComputeColorForPixel(int x, int y)
        {
            int tmpRed = 0;
            int tmpGreen = 0;
            int tmpBlue = 0;
            int counter = 0;

            var xStartingPoint = x - _maskHeight / 2;
            var yStartingPoint = y - _maskWidth / 2;

            for (int i = xStartingPoint; i <= x + _maskHeight / 2; i++)
            {
                for (int j = yStartingPoint; j <= y + _maskWidth / 2; j++)
                {
                    if (this.CheckIfPixelExists(i, j))
                    {
                        Color tmpColor = _sourceBitmap.GetPixel(j, i);
                        tmpRed += tmpColor.R;
                        tmpGreen += tmpColor.G;
                        tmpBlue += tmpColor.B;

                        counter++;
                    }
                }
            }

            return Color.FromArgb(tmpRed / counter, tmpGreen / counter, tmpBlue / counter);
        }
    }
}
