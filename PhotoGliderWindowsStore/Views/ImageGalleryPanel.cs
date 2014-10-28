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

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
            {
                child.Measure(new Size(availableSize.Width, availableSize.Height));
            }

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var point = new Point(0, 0);
            var currIdx = 0;
            //var largestWidth = 0.0;
            var maxColumnWidth = 300;
            while (currIdx != Children.Count)
            {
                var currCol = new List<UIElement>();
                var currWidth = 500.0;
                while (currWidth > maxColumnWidth && currIdx < Children.Count)
                {
                    currCol.Add(Children[currIdx]);
                    currIdx++;
                    // Recalc currWidth.
                    var hwRatioSum = 0.0;
                    foreach (var c in currCol)
                    {
                        if (c == null)
                        {
                            continue;
                        }
                        var img = c.GetFirstDescendantOfType<Image>();
                        var s = (BitmapImage)img.Source;
                        if (s != null)
                        {
                            var ratio = (double)s.PixelHeight / s.PixelWidth;
                            if (ratio != double.NaN)
                            {
                                hwRatioSum += ratio;
                            }
                            else
                            {
                                hwRatioSum += 1;
                            }
                        }
                    }

                    currWidth = finalSize.Height / hwRatioSum;
                    if (currWidth < maxColumnWidth)
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
                                if (ratio == double.NaN)
                                {
                                    ratio = 1.0;
                                }
                            }
                            var dHeight = currWidth * ratio;

                            c.Arrange(new Rect(point, new Point(point.X + currWidth, point.Y + dHeight)));
                            point.Y = point.Y + dHeight;
                        }
                        point.Y = 0;
                        point.X = point.X + currWidth;
                    }
                }
                
            }

            return base.ArrangeOverride(finalSize);
        }
    }
}
