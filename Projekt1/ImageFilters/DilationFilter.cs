using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1.ImageFilters
{
    public class DilationFilter : Filter
    {
        protected new const byte _maskWidth = 3;
        protected new const byte _maskHeight = 3;

        private int[,] _mask = new int[_maskHeight, _maskWidth] { { 0, 1, 0 },
                                                                  { 1, 1, 1 },
                                                                  { 0, 1, 0 }};

        protected override Color ComputeColorForPixel(int x, int y)
        {
            int tmpRed = 0;
            int tmpGreen = 0;
            int tmpBlue = 0;

            var xStartingPoint = x - _maskHeight / 2;
            var yStartingPoint = y - _maskWidth / 2;

            for (int i = xStartingPoint, maskI = 0; i <= x + _maskHeight / 2; i++, maskI++)
            {
                for (int j = yStartingPoint, maskJ = 0; j <= y + _maskWidth / 2; j++, maskJ++)
                {
                    if (this.CheckIfPixelExists(i, j))
                    {
                        Color tmpColor = _sourceBitmap.GetPixel(j, i);

                        if (tmpColor.R > tmpRed && _mask[maskI, maskJ] == 1)
                            tmpRed = tmpColor.R;

                        if (tmpColor.G > tmpGreen && _mask[maskI, maskJ] == 1)
                            tmpGreen = tmpColor.G;

                        if (tmpColor.B > tmpBlue && _mask[maskI, maskJ] == 1)
                            tmpBlue = tmpColor.B;
                    }
                }
            }

            return Color.FromArgb(tmpRed, tmpGreen, tmpBlue);
        }
    }
}
