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
    public class Encryption2LSB : IEncryptor
    {
        public Bitmap WriteImage(Bitmap primaryImg, Bitmap secondaryImg)
        {
            Bitmap imageToReturn = new Bitmap(primaryImg);
            //Bitmap EightbppImage;      
            //EightbppImage = ImageTo8bpp(secondaryImg);
            byte [] tablica = EncryptionHelper.ImageToByte(secondaryImg);
            
            if (tablica.Length > primaryImg.Size.Height * primaryImg.Size.Width/2)
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
                    var bits = new BitArray(new byte[] { tablica[counter] });

                    var pixel = imageToReturn.GetPixel(i, j);
                    var pixel1 = imageToReturn.GetPixel(i, j + 1);

                    Color color = EncryptionHelper.GetColor(pixel, new bool[] { bits[7], bits[6], bits[5], bits[4] });
                    Color color1 = EncryptionHelper.GetColor(pixel1, new bool[] { bits[3], bits[2], bits[1], bits[0] });

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
        public Bitmap ReadImage(Bitmap image)
        {
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

                    bajt = EncryptionHelper.ConvertToByte(bits);
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

        
    }
}
