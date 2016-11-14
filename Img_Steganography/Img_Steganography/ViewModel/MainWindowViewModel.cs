using Img_Steganography.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Drawing;
using Img_Steganography.Functionality;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Img_Steganography.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Bitmap primaryImg;
        private Bitmap secondaryImg;
        private Bitmap resultImage;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand OpenPrimaryImg { get; set; }
        public ICommand OpenSecondaryImg { get; set; }

        public ICommand Write { get; set; }

        public ICommand SaveImg { get; set; }

        public ICommand ReadImg { get; set; }

        public MainWindowViewModel()
        {
            OpenPrimaryImg = new RelayCommand(openImg, (obj) => true);
            OpenSecondaryImg = new RelayCommand(openImg2, (obj) => true);
            Write = new RelayCommand(write, (obj) => PrimaryImgPath != "/Resource/image.png" && SecondaryImgPath != "/Resource/image.png");
            SaveImg = new RelayCommand(saveImg, (obj) => resultImage != null);
            ReadImg = new RelayCommand(readImg, (obj) => PrimaryImgPath != "/Resource/image.png" && SecondaryImgPath == "/Resource/image.png");
          

        }

        private void readImg(object obj)
        {
            resultImage = ImageWriter.ReadImage2LSB(primaryImg);
            ResultImageSource = loadBitmap(resultImage);

        }

        private void openImg(object obj = null)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp;*.tiff|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                PrimaryImgPath = op.FileName;
            }
            primaryImg = new Bitmap(PrimaryImgPath);
        }
        private void openImg2(object obj = null)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp;*.tiff|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                SecondaryImgPath = op.FileName;
            }
            secondaryImg = new Bitmap(SecondaryImgPath);
        }

        private void saveImg(object obj)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Images|*.png;*.bmp;*.tiff";

            Bitmap newImage = null;
            using (var image = new Bitmap(resultImage))
            {
                newImage = new Bitmap(image);
            }

            if (dialog.ShowDialog() == true)
            {
                newImage.Save(dialog.FileName, ImageFormat.Bmp);
            }

        }

        private string _primaryImgPath = "/Resource/image.png";
        private string _secondaryImgPath = "/Resource/image.png";
        private BitmapSource _resultImageSource;

        private void write(object obj)
        {
            resultImage = ImageWriter.WriteImage2LSB(primaryImg, secondaryImg);
            SecondaryImgPath = "/Resource/image.png";
            if(resultImage != null)
            {
                MessageBox.Show("Zapis powiodl sie.");
                ResultImageSource = loadBitmap(resultImage);
            }
            else
            {
                MessageBox.Show("Blad. ZDjecie ktore probujesz zapisac jest zbyt duze.");
            }

            
        }

       
        public string PrimaryImgPath
        {
            get
            {
                return _primaryImgPath;
            }
            set
            {
                _primaryImgPath = value;
                RaisePropertyChange("PrimaryImgPath");
            }
        }

        public string SecondaryImgPath
        {
            get
            {
                return _secondaryImgPath;
            }
            set
            {
                _secondaryImgPath = value;
                RaisePropertyChange("SecondaryImgPath");
            }
        }


        public BitmapSource ResultImageSource
        {
            get
            {
                return _resultImageSource;
            }
            set
            {
                _resultImageSource = value;
                RaisePropertyChange("ResultImageSource");
            }
        }


        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        public static BitmapSource loadBitmap(Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }


        private void RaisePropertyChange(string propName)
        {
            var x = PropertyChanged;
            if (x != null) x(this, new PropertyChangedEventArgs(propName));
        }
    }
}
