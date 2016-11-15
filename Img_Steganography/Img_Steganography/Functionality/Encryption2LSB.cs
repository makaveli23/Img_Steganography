using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img_Steganography.Functionality
{
    class Encryption2LSB : IEncryptor
    {
        public Bitmap WriteImage(Bitmap primaryImg, Bitmap secondaryImg)
        {
            Bitmap imageToReturn = new Bitmap(primaryImg);
            //Bitmap EightbppImage;      
            //EightbppImage = ImageTo8bpp(secondaryImg);
            byte[] tablica = EncryptionHelper.ImageToByte(secondaryImg);
           

            if (tablica.Length + 27> primaryImg.Size.Height * primaryImg.Size.Width / 2)
                return null;


            byte[] endWord = Encoding.UTF8.GetBytes("STOP_DECRYPTING_THE_PICTURE");

            int counter = 0, k = 0;
            BitArray bits = new BitArray(8);

            for (int i = 0; i < imageToReturn.Width; i++)
            {
                for (int j = 0; j < imageToReturn.Height; j += 2)
                {
                    if (counter >= tablica.Length && k <= 26)
                    {
                        bits = new BitArray(new byte[] { endWord[k] });
                        k++;
                    }
                    else if (k == 27)
                        break;

                    else if(counter < tablica.Length)
                        bits = new BitArray(new byte[] { tablica[counter] });

                    
                    var pixel = imageToReturn.GetPixel(i, j);
                    var pixel1 = imageToReturn.GetPixel(i, j + 1);

                    Color color = EncryptionHelper.GetColor(pixel, new bool[] { bits[7], bits[6], bits[5], bits[4] });
                    Color color1 = EncryptionHelper.GetColor(pixel1, new bool[] { bits[3], bits[2], bits[1], bits[0] });

                    imageToReturn.SetPixel(i, j, color);
                    imageToReturn.SetPixel(i, j + 1, color1);
                    counter++;

                }
                if (k==27)
                    break;
            }
            return imageToReturn;

        }


      
        public Bitmap ReadImage(Bitmap image)
        {
            List<byte> hidden = new List<byte>();
            byte[] byteArray;
            byte bajt = 0;
            byte[] end_word = Encoding.UTF8.GetBytes("STOP_DECRYPTING_THE_PICTURE");
            int k = 0;

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j += 2)
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
                    if (bajt == end_word[k])
                        k++;
                    else
                        k = 0;

                    hidden.Add(bajt);


                    if (k == 27)
                        break;
                }
                if (k == 27)
                    break;
                
            }

            hidden.RemoveRange(hidden.Count - 27, 27);

            byteArray = hidden.ToArray();

            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                Bitmap img = (Bitmap)Image.FromStream(ms);
                return img;
            }

        }
    }
}
