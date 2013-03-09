﻿using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace PicToAscii
{
    internal class Converter
    {
        private Bitmap bmp;

        public Bitmap Bmp
        {
            get { return bmp; }
            set { bmp = value; }
        }

        private Color conCol;
        private Color tmpCol;
        private int precision;

        public int Precision
        {
            get { return precision; }
            set
            {
                if (value > 0)
                    precision = value;
            }
        }

        private bool colour;

        public bool Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        private Point outSize;

        public Point OutSize
        {
            get { return outSize; }
            set { outSize = value; }
        }

        //MN#$@QHKER&Bk%DW68F9gh50ZXUOAdPGpS2IJbLmfVq4TCYny3u[l*]xw7avs{}ej|r1+z?/o()=tci\\<>!_\"-~^;,':`.
        private static string characters = "MN#$@QEHKRB&D%Wk896FA0U5XZghOdPSGpL4bIfTJVm2q3nYCy[]" +
            "lu*awx7sev{|}r?zj+1)/(=tco><\\i!_\"~-^,;':.`  ";

        public Converter(Bitmap bmp)
        {
            Bmp = bmp;
            Precision = 1;
            Colour = true;
            OutSize = new Point(Bmp.Width,Bmp.Height);
        }

        public void writeTxt(string txt)
        {
            int stepW = Precision;
            int stepH = 2 * stepW;
            StreamWriter sw = new StreamWriter(txt);
            for (int i = 1; i + stepH < Bmp.Height; i += stepH)
            {
                for (int j = 1; j + stepW < Bmp.Width; j += stepW)
                {
                    conCol = getColor(Bmp,j, i, stepW, stepH);
                    int index = (int)((characters.Length - 1) * conCol.GetBrightness());
                    sw.Write(characters.ElementAt(index));
                }
                sw.WriteLine();
            }
            sw.Close();
        }

        public void writeHtml(string html)
        {
            StreamWriter sw = new StreamWriter(html);
            writeHtmlHead(sw);
            writeHtmlBody(sw);
            writeHtmlFoot(sw);
            sw.Close();
        }

        private void writeHtmlHead(StreamWriter sw)
        {
            if (sw != null)
            {
                sw.WriteLine("<html><head><style type=\"text/css\">pre { font-size:2px; line-height:1; }</style></head><body>");
            }
        }

        private void writeHtmlBody(StreamWriter sw)
        {
            if (sw != null)
            {
                int stepW = Precision;
                int stepH = 2 * stepW;
                sw.WriteLine("<pre>");
                for (int i = 1; i + stepH < Bmp.Height; i += stepH)
                {
                    for (int j = 1; j + stepW < Bmp.Width; j += stepW)
                    {
                        conCol = getColor(Bmp,j, i, stepW, stepH);
                        int index = (int)((characters.Length - 1) * conCol.GetBrightness());
                        if (i * j == 1)
                        {
                            sw.Write("<c style=\"color:" + ColorTranslator.ToHtml(conCol) + "\">"
                                                       + characters.ElementAt(index));
                        }
                        else
                        {
                            if (conCol.Equals(tmpCol))
                            {
                                sw.Write(characters.ElementAt(index));
                            }
                            else
                            {
                                sw.Write("</c><c style=\"color:" + ColorTranslator.ToHtml(conCol) + "\">"
                                    + characters.ElementAt(index));
                            }
                            tmpCol = conCol;
                        }
                    }
                    sw.WriteLine();
                }
                sw.WriteLine("</c></pre>");
            }
        }

        private void writeHtmlFoot(StreamWriter sw)
        {
            if (sw != null)
            {
                sw.WriteLine("</body></html>");
            }
        }

        public void writePic(string pic)
        {
            // StreamWriter sw = new StreamWriter(pic);
            float ProWidth=OutSize.X/Bmp.Width;
            float ProHeight = OutSize.Y / Bmp.Height;
            int stepW = Precision;
            int stepH = stepW;           

            Bitmap tmpBmp = new Bitmap(OutSize.X, OutSize.Y);
            Graphics og = Graphics.FromImage(tmpBmp);
            og.Clear(Color.White);
            og.ScaleTransform(1/ProWidth,1/ProHeight);
            og.DrawImage(Bmp,0,0);
            og.Dispose();
            Bitmap newBmp = new Bitmap(OutSize.X, OutSize.Y);
            Graphics g = Graphics.FromImage(newBmp);
            g.Clear(Color.White);
            Font font = new Font(FontFamily.GenericSansSerif, stepW);
            for (int i = 1; i + stepH <= OutSize.X; i += stepH)
            {
                for (int j = 1; j + stepW <= OutSize.Y; j += stepW)
                {
                    conCol = getColor(tmpBmp,j, i, stepW, stepH);
                    int index = (int)((characters.Length - 1) * conCol.GetBrightness());
                    g.DrawString(characters.ElementAt(index) + "", font, new SolidBrush(conCol), j , i );
                }
            }
            newBmp.Save(pic, ImageFormat.Bmp);
        }

        private Color getColor(Bitmap bitmap,int x, int y, int w, int h)
        {
            int r, g, b;
            Color tmpC;
            r = g = b = 0;
            for (int i = y; i < y + h; i++)
            {
                for (int j = x; j < x + w; j++)
                {
                    tmpC = bitmap.GetPixel(j, i);
                    r += tmpC.R;
                    g += tmpC.G;
                    b += tmpC.B;
                }
            }
            r /= (w * h);
            g /= (w * h);
            b /= (w * h);
            return Color.FromArgb(r, g, b);
        }
    }
}