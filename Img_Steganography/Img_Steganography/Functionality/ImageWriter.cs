using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Img_Steganography.Functionality
{
    public static class ImageWriter
    {
        public static Bitmap WriteImage2LSB(Bitmap primaryImg, Bitmap secondaryImg)
        {
            Bitmap imageToReturn = new Bitmap(primaryImg);
            //Bitmap EightbppImage;      
            //EightbppImage = ImageTo8bpp(secondaryImg);
            byte[] tablica = ImageToByte(secondaryImg);
            



            

            if (tablica.Length > primaryImg.Size.Height * primaryImg.Size.Width)
                return null;

            int counter = 0;
          

            for (int i = 0; i < imageToReturn.Width; i++)
            {
                for (int j = 0; j < imageToReturn.Height; j+=2)
                {
                    if (counter >= tablica.Length)
                    {
                        break;                       
                    }
                    var pixel = imageToReturn.GetPixel(i, j);
                    var pixel1 = imageToReturn.GetPixel(i, j + 1);
                    var pixelR = ByteArrayExtension.Set2LastBits(pixel.R, ByteArrayExtension.GetBit(tablica[counter], 7), ByteArrayExtension.GetBit(tablica[counter], 6));
                    var pixelG = ByteArrayExtension.SetLastBit(pixel.G, ByteArrayExtension.GetBit(tablica[counter], 5));
                    var pixelB = ByteArrayExtension.SetLastBit(pixel.B, ByteArrayExtension.GetBit(tablica[counter], 4));
                    var pixelR1 = ByteArrayExtension.Set2LastBits(pixel.R, ByteArrayExtension.GetBit(tablica[counter], 3), ByteArrayExtension.GetBit(tablica[counter], 2));
                    var pixelG1 = ByteArrayExtension.SetLastBit(pixel.G, ByteArrayExtension.GetBit(tablica[counter], 1));
                    var pixelB1 = ByteArrayExtension.SetLastBit(pixel.B, ByteArrayExtension.GetBit(tablica[counter], 0));
                    Color color = Color.FromArgb(pixel.A, pixelR, pixelG, pixelB);
                    Color color1 = Color.FromArgb(pixel1.A, pixelR1, pixelG1, pixelB1);
                    imageToReturn.SetPixel(i, j, color);
                    imageToReturn.SetPixel(i, j + 1, color1);
                    counter++;
                    
                }
                if (counter >= tablica.Length)
                    break;
            }


            

            return imageToReturn;

        }


        //public static Bitmap ReadImage2LSB(Bitmap image)
        //{
        //    Bitmap hiddenImage = new Bitmap(280, 210, PixelFormat.Format32bppArgb);
            
        //    List<byte> hidden = new List<byte>();
        //    byte[] byteArray;
        //    byte bajt=0;

        //    for (int i = 0; i < image.Width; i++)
        //    {
        //        for (int j = 0; j < image.Height; j++)
        //        {
        //            var pixel = image.GetPixel(i, j);
        //            BitArray bits = new BitArray(
        //                new bool[]
        //                          {
        //                              ByteArrayExtension.GetBit(pixel.A, 1),
        //                              ByteArrayExtension.GetBit(pixel.A, 0),
        //                              ByteArrayExtension.GetBit(pixel.R, 1),
        //                              ByteArrayExtension.GetBit(pixel.R, 0),
        //                              ByteArrayExtension.GetBit(pixel.G, 1),
        //                              ByteArrayExtension.GetBit(pixel.G, 0),
        //                              ByteArrayExtension.GetBit(pixel.B, 1),
        //                              ByteArrayExtension.GetBit(pixel.B, 0)
        //                });

        //            bajt = ConvertToByte(bits);                   
        //            hidden.Add(bajt);
                   
                    
        //        }
                

        //    }

        //    byteArray = hidden.ToArray();

        //    Bitmap bitmap = new Bitmap(736, 736, PixelFormat.Format32bppArgb);
        //    var bitmapData = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size), ImageLockMode.ReadWrite, bitmap.PixelFormat);
        //    Marshal.Copy(byteArray, 0, bitmapData.Scan0, byteArray.Length);
        //    bitmap.UnlockBits(bitmapData);
        //    bitmap.Save(@"C:\Users\micha\Pictures\Saved Pictures\test15.bmp", ImageFormat.Bmp);

        //    return bitmap;

        //}
        public static Bitmap ReadImage2LSB(Bitmap image)
        {
            Bitmap hiddenImage = new Bitmap(280, 210, PixelFormat.Format32bppArgb);

            List<byte> hidden = new List<byte>();
            byte[] byteArray;
            byte bajt = 0;

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j+=2)
                {
                    var pixel = image.GetPixel(i, j);
                    var pixel1 = image.GetPixel(i, j + 1);
                    BitArray bits = new BitArray(
                        new bool[]
                                  {
                                      ByteArrayExtension.GetBit(pixel.R, 1),
                                      ByteArrayExtension.GetBit(pixel.R, 0),
                                      ByteArrayExtension.GetBit(pixel.G, 0),
                                      ByteArrayExtension.GetBit(pixel.B, 0),
                                      ByteArrayExtension.GetBit(pixel1.R, 1),
                                      ByteArrayExtension.GetBit(pixel1.R, 0),
                                      ByteArrayExtension.GetBit(pixel1.G, 0),
                                      ByteArrayExtension.GetBit(pixel1.B, 0)

                        });

                    bajt = ConvertToByte(bits);
                    hidden.Add(bajt);


                }


            }

            byteArray = hidden.ToArray();

            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                Bitmap img = (Bitmap)Image.FromStream(ms);
                return img;
            }

        }

        static byte ConvertToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }
            var bitArray = new BitArray(bits.Cast<bool>().Reverse().ToArray());
            byte[] bytes = new byte[1];
            bitArray.CopyTo(bytes, 0);
            return bytes[0];
        }

        public static byte[] ImageToByte(Bitmap bmp)
        {
            using (var stream = new MemoryStream())
            {
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                return stream.ToArray();
            }
        }

        private static Bitmap ImageTo8bpp(Bitmap image)
        {

            var imageCodecInfo = GetEncoderInfo("image/tiff");
            var encoder = System.Drawing.Imaging.Encoder.ColorDepth;
            var encoderParameters = new EncoderParameters(1);
            var encoderParameter = new EncoderParameter(encoder, 8L);
            encoderParameters.Param[0] = encoderParameter;
            var memoryStream = new MemoryStream();

            image.Save(memoryStream, imageCodecInfo, encoderParameters);
            image.Save(@"C:\Users\micha\Pictures\Saved Pictures\test6bit.tiff", imageCodecInfo, encoderParameters);


            return (Bitmap)Image.FromStream(memoryStream);
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            return imageEncoders.FirstOrDefault(t => t.MimeType == mimeType);
        }
    }
}
