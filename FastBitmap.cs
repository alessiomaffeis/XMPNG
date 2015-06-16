using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace XMPNG
{
    public unsafe class FastBitmap : IDisposable
    {
        Bitmap bitmap;

        // three elements used for MakeGreyUnsafe
        int width;
        BitmapData bitmapData = null;
        Int32* pBase = null;

        private bool disposed = false;

        public FastBitmap(Bitmap bitmap)
        {
            this.bitmap = new Bitmap(bitmap);
        }

        public FastBitmap(string fileName)
        {
            this.bitmap = new Bitmap(fileName);
        }

        public FastBitmap(int width, int height)
        {
            this.bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.bitmap.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Bitmap Bitmap
        {
            get
            {
                return (bitmap);
            }
        }

        public BitmapData BitmapData
        {
            get
            {
                return (bitmapData);
            }
        }

        private Point PixelSize
        {
            get
            {
                GraphicsUnit unit = GraphicsUnit.Pixel;
                RectangleF bounds = bitmap.GetBounds(ref unit);

                return new Point((int)bounds.Width, (int)bounds.Height);
            }
        }

        public void LockBitmap()
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle((int)boundsF.X,
            (int)boundsF.Y,
            (int)boundsF.Width,
            (int)boundsF.Height);
            
            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length. 
            width = (int)boundsF.Width * sizeof(PixelData);
            if (width % 4 != 0)
            {
                width = 4 * (width / 4 + 1);
            }
            bitmapData =
           bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            pBase = (Int32*)bitmapData.Scan0.ToPointer();
        }

        // XOR operator overloading ^:

        public static FastBitmap operator ^(FastBitmap b1, FastBitmap b2)
        {
            b1.LockBitmap();
            b2.LockBitmap();
            if(b1.BitmapData.Width != b2.BitmapData.Width || b2.BitmapData.Height != b2.BitmapData.Height)
                throw new InvalidOperationException("Bitmaps have different size.");

            FastBitmap r = new FastBitmap(b1.BitmapData.Width, b1.BitmapData.Height);
            r.LockBitmap();
            int totBytes = b1.BitmapData.Width * b1.BitmapData.Height * 3;
            int pCount = (int) ( totBytes / sizeof(Int32));
            for(int i=0; i<pCount; i++)
            {
                *(r.pBase) = *(b2.pBase) ^ *(b1.pBase);
                r.pBase++;
                b1.pBase++;
                b2.pBase++;
            }

            b1.UnlockBitmap();
            b2.UnlockBitmap();
            r.UnlockBitmap();

            return r;
        }

        public PixelData GetPixel(int x, int y)
        {
            return *(PixelData*)(pBase + y * width + x * sizeof(PixelData));
        }

        public void SetPixel(int x, int y, PixelData colour)
        {
            PixelData* pixel = PixelAt(x, y);
            *pixel = colour;
        }

        public void UnlockBitmap()
        {
            bitmap.UnlockBits(bitmapData);
            bitmapData = null;
            pBase = null;
        }
        public PixelData* PixelAt(int x, int y)
        {
            return (PixelData*)(pBase + y * width + x * sizeof(PixelData));
        }
    }
    public struct PixelData
    {
        public byte blue;
        public byte green;
        public byte red;
    }
}
