
using System;
using System.Collections.Generic;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public static class CanvasLayout
    {
        public const double Padding = 14;
        public const double Spacing = 8;
        public const double HeaderHeight = 43;
        public const double CharacterWidth = 8;
        public const double HeaderChromeWidth = 100;

        public static double GetRequiredTextWidth(string? text, double extraWidth = HeaderChromeWidth)
        {
            return extraWidth + (text?.Length ?? 0) * CharacterWidth;
        }

        public static void ArrangeVertically(IEnumerable<CanvasItemBase> children)
        {
            double y = Padding;

            foreach (CanvasItemBase child in children)
            {
                child.X = Padding;
                child.Y = y;
                y += child.Height + Spacing;
            }
        }

        public static void ArrangeHorizontally(IEnumerable<CanvasItemBase> children)
        {
            double x = Padding;

            foreach (CanvasItemBase child in children)
            {
                child.X = x;
                child.Y = Padding;
                x += child.Width + Spacing;
            }
        }

        public static void ArrangeInTwoColumns(IEnumerable<CanvasItemBase> children)
        {
            double firstColumnWidth = 0;

            for (int i = 0; i < children.Count(); i += 2)
                firstColumnWidth = Math.Max(firstColumnWidth, children.ElementAt(i).Width);

            double y = Padding;

            for (int i = 0; i < children.Count(); i += 2)
            {
                CanvasItemBase leftChild = children.ElementAt(i);

                leftChild.X = Padding;
                leftChild.Y = y;

                double rowHeight = leftChild.Height;

                if (i + 1 < children.Count())
                {
                    CanvasItemBase rightChild = children.ElementAt(i + 1);

                    rightChild.X = Padding + firstColumnWidth + Spacing;
                    rightChild.Y = y;

                    rowHeight = Math.Max(rowHeight, rightChild.Height);
                }

                y += rowHeight + Spacing;
            }
        }

        public static void ArrangePrimaryColumnWithSideItem(
            IEnumerable<CanvasItemBase> primaryItems,
            CanvasItemBase? sideItem)
        {
            CanvasItemBase[] items = primaryItems.ToArray();

            ArrangeVertically(items);

            if (sideItem == null) return;

            double primaryWidth = items.Length > 0
                ? items.Max(item => item.Width)
                : 0;

            sideItem.X = Padding + (items.Length > 0 ? primaryWidth + Spacing : 0);
            sideItem.Y = Padding;
        }
    }
}
