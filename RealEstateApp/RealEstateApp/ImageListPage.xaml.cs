using RealEstateApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageListPage : ContentPage
    {
        SensorSpeed speed = SensorSpeed.UI;
        public List<PropertyImage> PropertyImages { get; set; }

        public ImageListPage(Property propertyItem)
        {
            InitializeComponent();

            PropertyImages = new List<PropertyImage>();
            foreach (string imageUrl in propertyItem.ImageUrls)
            {
                PropertyImages.Add(new PropertyImage
                {
                    ImageUrl = imageUrl
                });
            }
            BindingContext = this;
            PropertyImagesListCarousel.ItemsSource = PropertyImages;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Accelerometer.ShakeDetected += Accelerometer_ShakeDetected;

            ToggleAccelerometer();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Accelerometer.ShakeDetected -= Accelerometer_ShakeDetected;
            ToggleAccelerometer();
        }
        void Accelerometer_ShakeDetected(object sender, EventArgs e)
        {
            if (PropertyImagesListCarousel.Position >= PropertyImages.Count - 1)
                PropertyImagesListCarousel.Position = 0;
            else
                PropertyImagesListCarousel.Position++;
        }
        public void ToggleAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                    Accelerometer.Stop();
                else
                    Accelerometer.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }
    }

    public class PropertyImage
    {
        public string ImageUrl { get; set; }
    }
}