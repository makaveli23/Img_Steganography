using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img_Steganography.Functionality
{
    public interface IEncryptor
    {
        Bitmap WriteImage(Bitmap primaryImage, Bitmap secondaryImage);
        Bitmap ReadImage(Bitmap image);

    }
}
