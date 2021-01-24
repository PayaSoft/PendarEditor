using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
namespace Paya.Automation.Editor
{
  public  class ImageUtility
  {
      #region Method
      public static byte[] InsertDateNumber(Stream imageStream, string waterMarkDate, string waterMarkNumber, string waterMarkAttachment, string fontName, int fontSize, bool adaptiveFontSize, int TableTop, int TableIndent, int TableRowHeightDateData, int TableRowHeightNumberData, int TableRowHeightAttachmentData)
      {
          if(imageStream==null)
              throw new ArgumentNullException("imageStream");
          using (var image = Image.FromStream(imageStream))
          {
              using (var newBitmap = new Bitmap(image))
              {
                  using (var g = Graphics.FromImage(newBitmap))
                  {



                      using (var stringFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft | StringFormatFlags.NoClip)
                      {
                          Alignment = StringAlignment.Center,
                          LineAlignment = StringAlignment.Center,
                          Trimming = StringTrimming.None
                      })
                      {


                          using (var font = new Font(fontName, adaptiveFontSize ? CalculateFontSize(fontSize, newBitmap.Size) : fontSize))
                          {

                              g.DrawString(waterMarkDate, font, Brushes.Black,
                                           new Point(TableIndent, (TableTop + TableRowHeightDateData)),
                                           stringFormat);
                              g.DrawString(waterMarkNumber, font, Brushes.Black,
                                           new Point(TableIndent, (TableTop + TableRowHeightNumberData)),
                                           stringFormat);
                              g.DrawString(waterMarkAttachment, font, Brushes.Black,
                                           new Point(TableIndent, (TableTop + TableRowHeightAttachmentData)),
                                           stringFormat);
                          }
                      }
                  }
                          
                  using (var ms = new MemoryStream())
                  {
                      newBitmap.Save(ms, image.RawFormat);
                      return ms.ToArray();
                  }
              }
          }

      }


      private static int CalculateFontSize(int fontSize, Size canvasSize)
      {
          double c = Math.Sqrt(canvasSize.Width * 1.0 * canvasSize.Width + canvasSize.Height * 1.0 * canvasSize.Height);
          double s = Math.Sqrt(1660.0 * 1660.0 + 2255.0 * 2255.0);

          double size = Math.Round(fontSize * c / s);
          if (size < 10)
              size = 10;
          if (size > fontSize * 4)
              size = fontSize * 4;
          return (int)size;
      }

      public static byte[] createWhiteBitmap(string waterMarkDate, string waterMarkNumber, string waterMarkAttachment, string fontName, int fontSize, bool adaptiveFontSize, int TableTop, int TableIndent, int TableRowHeightDateData, int TableRowHeightNumberData, int TableRowHeightAttachmentData)
      {
          
          using (var newBitmap = new Bitmap(100,100))
          {
             // newBitmap.SetPixel(100, 100, Brushes.Black);
              using (var g = Graphics.FromImage(newBitmap))
              {



                  using (var stringFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft | StringFormatFlags.NoClip)
                  {
                      
                      Alignment = StringAlignment.Center,
                      LineAlignment = StringAlignment.Center,
                      Trimming = StringTrimming.None
                  })
                  {


                      using (var font = new Font(fontName, adaptiveFontSize ? CalculateFontSize(fontSize, newBitmap.Size) : fontSize))
                      {

                          g.DrawString(waterMarkDate, font, Brushes.Black,
                                       new Point(TableIndent, (TableTop + TableRowHeightDateData)),
                                       stringFormat);
                          g.DrawString(waterMarkNumber, font, Brushes.Black,
                                       new Point(TableIndent, (TableTop + TableRowHeightNumberData)),
                                       stringFormat);
                          g.DrawString(waterMarkAttachment, font, Brushes.Black,
                                       new Point(TableIndent, (TableTop + TableRowHeightAttachmentData)),
                                       stringFormat);
                      }
                  }
              }

              using (var ms = new MemoryStream())
              {
                  newBitmap.Save(ms,ImageFormat.Jpeg);
                  return ms.ToArray();
              }
          }
      }
      #endregion
  }
}
