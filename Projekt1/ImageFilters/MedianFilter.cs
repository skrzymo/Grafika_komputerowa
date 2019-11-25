using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt1.ImageFilters
{
    public class MedianFilter : Filter
    {
        protected override Color ComputeColorForPixel(int x, int y)
        {
            int tmpRed;
            int tmpGreen;
            int tmpBlue;

            List<int> redList = new List<int>();
            List<int> greenList = new List<int>();
            List<int> blueList = new List<int>();

            var xStartingPoint = x - _maskHeight / 2;
            var yStartingPoint = y - _maskWidth / 2;

            for (int i = xStartingPoint; i <= x + _maskHeight / 2; i++)
            {
                for (int j = yStartingPoint; j <= y + _maskWidth / 2; j++)
                {
                    if (this.CheckIfPixelExists(i, j))
                    {
                        Color tmpColor = _sourceBitmap.GetPixel(j, i);
                        redList.Add(tmpColor.R);
                        greenList.Add(tmpColor.G);
                        blueList.Add(tmpColor.B);
                    }
                }
            }

            redList.Sort();
            greenList.Sort();
            blueList.Sort();

            if (redList.Count % 2 == 0)
                tmpRed = (redList[redList.Count / 2] + redList[(redList.Count / 2) - 1]) / 2;
            else
                tmpRed = redList[redList.Count / 2];

            if (greenList.Count % 2 == 0)
                tmpGreen = (greenList[greenList.Count / 2] + greenList[(greenList.Count / 2) - 1]) / 2;
            else
                tmpGreen = greenList[greenList.Count / 2];

            if (blueList.Count % 2 == 0)
                tmpBlue = (blueList[blueList.Count / 2] + blueList[(blueList.Count / 2) - 1]) / 2;
            else
                tmpBlue = blueList[blueList.Count / 2];

            return Color.FromArgb(tmpRed, tmpGreen, tmpBlue);
        }
    }
}
