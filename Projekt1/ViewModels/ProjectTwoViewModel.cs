﻿using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Projekt1.ImageFilters;
using Projekt1.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Projekt1.ViewModels
{
    public class ProjectTwoViewModel : Screen
    {
        private BitmapImage _sourceImage;

        private string _imageFilePath;
        public string ImageFilePath
        {
            get { return _imageFilePath; }

            set
            {
                _imageFilePath = value;
                this.NotifyOfPropertyChange();
            }
        }

        private BitmapImage _displayedImage;
        public BitmapImage DisplayedImage
        {
            get { return _displayedImage; }

            set
            {
                _displayedImage = value;
                this.NotifyOfPropertyChange();
            }
        }

        public void ChooseImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp |" +
                         "All Files (*.*) | *.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            };

            var result = fileDialog.ShowDialog();

            if (result == true)
            {
                this.ImageFilePath = fileDialog.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(this.ImageFilePath);
                bitmap.EndInit();
                _sourceImage = bitmap;
                this.DisplayedImage = _sourceImage;
            }
        }

        public void ResetImage()
        {
            this.DisplayedImage = _sourceImage;
        }

        public async void SmoothingFilterButton()
        {
            var metroWindow = Application.Current.MainWindow as MetroWindow;
            var controller = await metroWindow.ShowProgressAsync("Proszę czekać...", "Trwa przetwarzanie obrazu");
            controller.SetCancelable(false);
            controller.SetIndeterminate();

            var smoothingFilter = new SmoothingFilter();
            var newBitmap = smoothingFilter.ExecuteFilter(this.ConvertFromBitmapImageToBitmap(this.DisplayedImage));
            this.DisplayedImage = this.ConvertFromBitmapToBitmapImage(newBitmap);

            await controller.CloseAsync();
        }

        public async void MedianFilterButton()
        {
            var metroWindow = Application.Current.MainWindow as MetroWindow;
            var controller = await metroWindow.ShowProgressAsync("Proszę czekać...", "Trwa przetwarzanie obrazu");
            controller.SetCancelable(false);
            controller.SetIndeterminate();

            var medianFilter = new MedianFilter();
            var newBitmap = medianFilter.ExecuteFilter(this.ConvertFromBitmapImageToBitmap(this.DisplayedImage));
            this.DisplayedImage = this.ConvertFromBitmapToBitmapImage(newBitmap);

            await controller.CloseAsync();
        }

        public async void EdgeDetectionFilterButton()
        {
            var metroWindow = Application.Current.MainWindow as MetroWindow;
            var controller = await metroWindow.ShowProgressAsync("Proszę czekać...", "Trwa przetwarzanie obrazu");
            controller.SetCancelable(false);
            controller.SetIndeterminate();

            var edgeDetectionFilter = new EdgeDetectionFilter();
            var newBitmap = edgeDetectionFilter.ExecuteFilter(this.ConvertFromBitmapImageToBitmap(this.DisplayedImage));
            this.DisplayedImage = this.ConvertFromBitmapToBitmapImage(newBitmap);

            await controller.CloseAsync();
        }

        private Bitmap ConvertFromBitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private BitmapImage ConvertFromBitmapToBitmapImage(Bitmap bitmap)
        {          
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Jpeg);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;                
                bitmapImage.EndInit();

                return bitmapImage;
            }            
        }
    }
}
