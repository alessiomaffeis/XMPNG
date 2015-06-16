using System;
using System.IO;
using System.Drawing;

namespace XMPNG
{
    [Serializable] public class Frame : IDisposable
    {
        MemoryStream ms;
        int seq, mouseX=0, mouseY=0;
        private bool disposed = false;

        public Frame(Bitmap bm, int seq)
        {
            this.ms = new MemoryStream();
            bm.Save(this.ms, System.Drawing.Imaging.ImageFormat.Png);
            this.seq = seq; // sequence number
        }

        public void setMouseCoords(int x, int y)
        {
            this.mouseX = x;
            this.mouseY = y;
        }


        public Bitmap Bitmap
        {
            get
            {
                Bitmap bm = new Bitmap(this.ms);
                return bm;
            }
        }

        public int Seq
        {
            get
            {
                return this.seq;
            }

            set
            {
                this.seq = value;
            }
        }

        public int MouseX
        {
            get
            {
                return this.mouseX;
            }
        }

        public int MouseY
        {
            get
            {
                return this.mouseY;
            }
        }

        private void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    this.ms.Dispose();
                }		
            }
            disposed = true;         
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
