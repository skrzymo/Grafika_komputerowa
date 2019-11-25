using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1.ImageFilters
{
    public class EdgeDetectionFilter : Filter
    {
        private int[,] _verticalMask = new int[_maskHeight, _maskWidth] { { -1, 0, 1 },
                                                                          { -2, 0, 2 },
                                                                          { -1, 0, 1 }};

        private int[,] _horizontalMask = new int[_maskHeight, _maskWidth] { { -1, -2, -1 },
                                                                            { 0, 0, 0 },
                                                                            { 1, 2, 1 } };

        protected override Color ComputeColorForPixel(int x, int y)
        {
            int tmpRed = 0;
            int tmpGreen = 0;
            int tmpBlue = 0;
            int tmpVerticalRed = 0;
            int tmpVerticalGreen = 0;
            int tmpVerticalBlue = 0;
            int tmpHorizontalRed = 0;
            int tmpHorizontalGreen = 0;
            int tmpHorizontalBlue = 0;
            int counter = 0;

            if (x == 1) { 
            }

            var xStartingPoint = x - _maskHeight / 2;
            var yStartingPoint = y - _maskWidth / 2;

            for (int i = xStartingPoint, maskI = 0; i <= x + _maskHeight / 2; i++, maskI++)
            {
                for (int j = yStartingPoint, maskJ = 0; j <= y + _maskWidth / 2; j++, maskJ++)
                {
                    if (this.CheckIfPixelExists(i, j))
                    {
                        Color tmpColor = _sourceBitmap.GetPixel(j, i);
                        var rgb = tmpColor.R * 0.21 + tmpColor.G * 0.72 + tmpColor.B * 0.07;
                        tmpVerticalRed += (int)(rgb * _verticalMask[maskI, maskJ]);
                        tmpHorizontalRed += (int)(rgb * _horizontalMask[maskI, maskJ]);
                        tmpVerticalGreen += (int)(rgb * _verticalMask[maskI, maskJ]);
                        tmpHorizontalGreen += (int)(rgb * _horizontalMask[maskI, maskJ]);
                        tmpVerticalBlue += (int)(rgb * _verticalMask[maskI, maskJ]);
                        tmpHorizontalBlue += (int)(rgb * _horizontalMask[maskI, maskJ]);

                        counter++;
                    }
                }
            }

            tmpRed = CheckIfValueIsCorrect((int)Math.Sqrt((tmpVerticalRed * tmpVerticalRed) + (tmpHorizontalRed * tmpHorizontalRed)));
            tmpGreen = CheckIfValueIsCorrect((int)Math.Sqrt((tmpVerticalGreen * tmpVerticalGreen) + (tmpHorizontalGreen * tmpHorizontalGreen)));
            tmpBlue = CheckIfValueIsCorrect((int)Math.Sqrt((tmpVerticalBlue * tmpVerticalBlue) + (tmpHorizontalBlue * tmpHorizontalBlue)));

            var color = Color.Black;

            return Color.FromArgb(tmpRed, tmpGreen, tmpBlue);
        }

        private int CheckIfValueIsCorrect(int value)
        {
            if (value > 255)
                return 255;

            if (value < 0)
                return 0;

            return value;
        }
    }
}
