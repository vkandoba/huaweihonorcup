using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Borderer
{
    public static class ImageTool
    {
        public static Bitmap ToGrayStyle(this Bitmap image)
        {
            var bmp = new Bitmap(image.Width, image.Height);
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var p = image.GetPixel(x, y);
                    var rgb = (int)Math.Round(0.299 * p.R + 0.587 * p.G + .114 * p.B);
                    bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            }

            return bmp;
        }

        public static Bitmap AdjustContrast(this Bitmap image, float Value)
        {
            var bmp = new Bitmap(image.Width, image.Height);
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var p = image.GetPixel(x, y);
                    float Red = p.R / 255.0f;
                    float Green = p.G / 255.0f;
                    float Blue = p.B / 255.0f;
                    Red = (((Red - 0.5f) * Value) + 0.5f) * 255.0f;
                    Green = (((Green - 0.5f) * Value) + 0.5f) * 255.0f;
                    Blue = (((Blue - 0.5f) * Value) + 0.5f) * 255.0f;
                    bmp.SetPixel(x, y, Color.FromArgb(Clamp((int)Red), Clamp((int)Green), Clamp((int)Blue)));
                }
            }

            return bmp;
        }

        private static int Clamp(int n)
        {
            if (n < 0) return 0;
            if (n > 255) return 255;
            return n;
        }

    }
}