using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

// XMPNGCodec: A simple, differential "Motion-PNG" codec.

namespace XMPNG
{
    public class XMPNGCodec
    {
        int current;
        FastBitmap last, next, diff;

        public XMPNGCodec()
        {
            this.current = 0;
        }

        public Bitmap Decode(Frame incoming)
        {
            if (current == 0 || incoming.Seq == 1)
            {
                current = incoming.Seq;
                last = new FastBitmap(incoming.Bitmap);
            }
            else
            {
                if (incoming.Seq == current + 1)
                {
                    current++;
                    FastBitmap inc = new FastBitmap(incoming.Bitmap);
                    FastBitmap fb = last ^ inc;
                    inc.Dispose();
                    last.Dispose();
                    last = fb;
                }
            }

            return last.Bitmap;
        }

        public void Reset()
        {
            this.current = 0;
        }

        public Frame Encode(Bitmap outgoing)
        { 
            if (current == 0)
            {
                current++;
                last = new FastBitmap(outgoing);
                return new Frame(outgoing, current);
            }
            else
            {
                next = new FastBitmap(outgoing);
                if (diff != null)
                    diff.Dispose();
                diff = next ^ last;
                last.Dispose();
                last = next;
                current++;
                return new Frame(diff.Bitmap, current);
            }
        }

        public int Current
        {
            get
            {
                return this.current;
            }
        }
    }
}