using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img_Steganography.Functionality
{
    public static class EncryptionHelper
    {
        public static byte ConvertToByte(BitArray bits)
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

        public static Color GetColor(Color pixel, bool []tab)
        {
            var pixelR = ByteArrayExtension.Set2LastBits(pixel.R, tab[0], tab[1]);
            var pixelG = ByteArrayExtension.SetLastBit(pixel.G, tab[2]);
            var pixelB = ByteArrayExtension.SetLastBit(pixel.B, tab[3]);

            return Color.FromArgb(pixelR, pixelG, pixelB);
        }

        public static byte[] ImageToByte(Bitmap bmp)
        {
            using (var stream = new MemoryStream())
            {
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                return stream.ToArray();
            }
        }

        public static Bitmap ImageTo8bpp(Bitmap image)
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

        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            return imageEncoders.FirstOrDefault(t => t.MimeType == mimeType);
        }
    }
}
