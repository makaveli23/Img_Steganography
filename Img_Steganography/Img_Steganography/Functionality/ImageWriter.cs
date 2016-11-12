using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Img_Steganography.Functionality
{
    public static class ImageWriter
    {
        public static Bitmap WriteImage(Bitmap primaryImg, Bitmap secondaryImg)
        {
            Bitmap EightbppImage, imageToReturn = primaryImg;          
            EightbppImage = ImageTo8bpp(secondaryImg);
            byte[] tablica = ImageToByte(EightbppImage);

            var bits = new BitArray(tablica);

            if (tablica.Length > primaryImg.Size.Height * primaryImg.Size.Width)
                return null;

            int counter = 0;
          

            for (int i = 0; i < imageToReturn.Width; i++)
            {
                for (int j = 0; j < imageToReturn.Height; j++)
                {
                    if (counter >= bits.Count)
                        break;
                    var pixel = imageToReturn.GetPixel(i, j);
                    var pixelA = ByteArrayExtension.Set2LastBits(pixel.A, bits[counter], bits[counter + 1]);
                    var pixelR = ByteArrayExtension.Set2LastBits(pixel.R, bits[counter + 2], bits[counter + 3]);
                    var pixelG = ByteArrayExtension.Set2LastBits(pixel.G, bits[counter + 4], bits[counter + 5]);
                    var pixelB = ByteArrayExtension.Set2LastBits(pixel.G, bits[counter + 6], bits[counter + 7]);
                    Color color = Color.FromArgb(pixelA, pixelR, pixelG, pixelB);
                    imageToReturn.SetPixel(i, j, color);
                    counter += 8;
                    
                }
                if (counter >= bits.Count)
                    break;
            }



            return imageToReturn;

        }


        public static bool ReadImage(Bitmap image)
        {
            Bitmap hiddenImage;

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {

                }

            }

            return true;

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
