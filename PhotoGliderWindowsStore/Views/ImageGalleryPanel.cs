using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using WinRTXamlToolkit.Controls.Extensions;

namespace PhotoGliderWindowsStore.Views
{
    public class ImageGalleryPanel : Panel
    {
        public ImageGalleryPanel()
        {
        }

        double _currArrWidth = 0;

        protected override Size MeasureOverride(Size availableSize)
        {
            // Measure each of the Children
            foreach (UIElement element in Children)
            {
                // Determine the size of the element
                element.Measure(availableSize);
            }

            var numPerCol = availableSize.Height / 150;

            // Return the total size required as an un-oriented quantity
            return new Size(_currArrWidth + 150, availableSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var point = new Point(0, 0);
            var currIdx = 0;
            var retSize = new Size(0, finalSize.Height);
            //var largestWidth = 0.0;
            var maxColumnWidth = 300;
            while (currIdx != Children.Count)
            {
                var currCol = new List<UIElement>();
                var currWidth = 500.0;
                var currHwRatioSum = 0.0;
                while (currWidth > maxColumnWidth && currIdx < Children.Count)
                {
                    var currEle = Children[currIdx];
                    currCol.Add(Children[currIdx]);
                    // Recalc currWidth.
                    currIdx++;
                    //if (c == null)
                    //{
                    //    continue;
                    //}
                    var img = currEle.GetFirstDescendantOfType<Image>();
                    //if (img == null)
                    //{
                    //    continue;
                    //}
                    var s = (BitmapImage)img.Source;
                    if (s != null)
                    {
                        var ratio = (double)s.PixelHeight / s.PixelWidth;
                        //if (ratio != double.NaN)
                        //{
                        currHwRatioSum += ratio;
                        //}
                        //else
                        //{
                        //hwRatioSum += 1;
                        //}
                    }

                    currWidth = finalSize.Height / currHwRatioSum;
                }
                
                if (!double.IsNaN(currWidth) && !double.IsInfinity(currWidth))
                {
                    foreach (var c in currCol)
                    {
                        var img = c.GetFirstDescendantOfType<Image>();
                        img.Width = currWidth;
                        var ratio = 1.0;
                        var s = (BitmapImage)img.Source;
                        if (s != null)
                        {
                            ratio = (double)s.PixelHeight / s.PixelWidth;
                        }
                        var dHeight = currWidth * ratio;

                        c.Arrange(new Rect(point, new Size(currWidth, dHeight)));

                        retSize.Width = point.X + currWidth;

                        point.Y = point.Y + dHeight;
                    }
                    point.Y = 0;
                    point.X = point.X + currWidth;
                }
            }
            _currArrWidth = retSize.Width;
            return retSize;
        }
    }
}
