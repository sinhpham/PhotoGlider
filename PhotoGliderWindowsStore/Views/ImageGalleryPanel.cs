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
            return new Size(_currArrWidth, availableSize.Height);
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
                        }
                        var dHeight = currWidth * ratio;

                        c.Arrange(new Rect(point, new Size(currWidth, dHeight)));
                        point.Y = point.Y + dHeight;
                    }
                    point.Y = 0;
                    point.X = point.X + currWidth;
                }
            }
            var retSize = new Size(point.X, finalSize.Height);
            _currArrWidth = point.X;
            return retSize;
        }
    }

    internal struct OrientedSize
    {
        /// <summary>
        /// The orientation of the structure.
        /// </summary>
        private Orientation _orientation;

        /// <summary>
        /// Gets the orientation of the structure.
        /// </summary>
        public Orientation Orientation
        {
            get { return _orientation; }
        }

        /// <summary>
        /// The size dimension that grows directly with layout placement.
        /// </summary>
        private double _direct;

        /// <summary>
        /// Gets or sets the size dimension that grows directly with layout
        /// placement.
        /// </summary>
        public double Direct
        {
            get { return _direct; }
            set { _direct = value; }
        }

        /// <summary>
        /// The size dimension that grows indirectly with the maximum value of
        /// the layout row or column.
        /// </summary>
        private double _indirect;

        /// <summary>
        /// Gets or sets the size dimension that grows indirectly with the
        /// maximum value of the layout row or column.
        /// </summary>
        public double Indirect
        {
            get { return _indirect; }
            set { _indirect = value; }
        }

        /// <summary>
        /// Gets or sets the width of the size.
        /// </summary>
        public double Width
        {
            get
            {
                return (Orientation == Orientation.Horizontal) ?
                    Direct :
                    Indirect;
            }
            set
            {
                if (Orientation == Orientation.Horizontal)
                {
                    Direct = value;
                }
                else
                {
                    Indirect = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the size.
        /// </summary>
        public double Height
        {
            get
            {
                return (Orientation != Orientation.Horizontal) ?
                    Direct :
                    Indirect;
            }
            set
            {
                if (Orientation != Orientation.Horizontal)
                {
                    Direct = value;
                }
                else
                {
                    Indirect = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new OrientedSize structure.
        /// </summary>
        /// <param name="orientation">Orientation of the structure.</param>
        public OrientedSize(Orientation orientation) :
            this(orientation, 0.0, 0.0)
        {
        }

        /// <summary>
        /// Initializes a new OrientedSize structure.
        /// </summary>
        /// <param name="orientation">Orientation of the structure.</param>
        /// <param name="width">Un-oriented width of the structure.</param>
        /// <param name="height">Un-oriented height of the structure.</param>
        public OrientedSize(Orientation orientation, double width, double height)
        {
            _orientation = orientation;

            // All fields must be initialized before we access the this pointer
            _direct = 0.0;
            _indirect = 0.0;

            Width = width;
            Height = height;
        }
    }
}
