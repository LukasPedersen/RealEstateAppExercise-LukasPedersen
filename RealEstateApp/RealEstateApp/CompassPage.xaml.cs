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
    public partial class CompassPage : ContentPage
    {
        public Property _Property { get; set; }
        public CompassPage(Property property)
        {
            InitializeComponent();
            _Property = property;
            Compass.ReadingChanged += Compass_ReadingChanged;
        }

        protected override void OnAppearing()
        {
            ToggleCompass();
        }

        protected override void OnDisappearing()
        {
            ToggleCompass();
        }

        // Set speed delay for monitoring changes.
        SensorSpeed speed = SensorSpeed.UI;


        void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            var data = e.Reading;

            CompassImage.Rotation = ((data.HeadingMagneticNorth) * (-1) );
            HeadingLabel.Text = data.HeadingMagneticNorth.ToString();
            if (data.HeadingMagneticNorth < 100 && data.HeadingMagneticNorth > 80)
            {
                AspectSpan.Text = "Øst";
            }
            else if (data.HeadingMagneticNorth < 190 && data.HeadingMagneticNorth > 170)
            {
                AspectSpan.Text = "Syd";
            }
            else if (data.HeadingMagneticNorth < 280 && data.HeadingMagneticNorth > 260)
            {
                AspectSpan.Text = "Vest";
            }
            else if (data.HeadingMagneticNorth < 360 && data.HeadingMagneticNorth > 350)
            {
                AspectSpan.Text = "Nord";
            }
        }

        public void ToggleCompass()
        {
            try
            {
                if (Compass.IsMonitoring)
                    Compass.Stop();
                else
                    Compass.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Some other exception has occurred
            }
        }

        private async void Save_Button_Clicked(object sender, EventArgs e)
        {
            ToggleCompass();
            _Property.PropertyHeading = AspectSpan.Text;
            await Navigation.PopAsync();
        }
    }
}