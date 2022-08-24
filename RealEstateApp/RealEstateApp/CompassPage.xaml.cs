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
        public CompassPage()
        {
            InitializeComponent();

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
            CompassImage.Rotation = data.HeadingMagneticNorth;
            HeadingLabel.Text = data.HeadingMagneticNorth.ToString();
            if (data.HeadingMagneticNorth < 90 && data.HeadingMagneticNorth > 0)
            {
                AspectSpan.Text = "Øst";
            }
            else if (data.HeadingMagneticNorth < 180 && data.HeadingMagneticNorth > 180)
            {

            }
            // Process Heading Magnetic North
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
    }
}