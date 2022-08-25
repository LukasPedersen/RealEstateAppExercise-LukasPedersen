using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Collections.ObjectModel;
using RealEstateApp.Models;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeightCalculatorPage : ContentPage
    {
        private double currentPressure;
        private double currentAltitude;

        public double CurrentPressure
        {
            get { return currentPressure; }
            set 
            {
                currentPressure = value;
            }
        }
        public double CurrentAltitude
        {
            get { return currentAltitude; }
            set 
            { 
                currentAltitude = value; 
            }
        }
        public ObservableCollection<BarometerMeasurement> BarometerMeasurements { get; } = new ObservableCollection<BarometerMeasurement>();


        public HeightCalculatorPage()
        {
            InitializeComponent();

            BindingContext = this;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = this;

            BarometerMeasurementsList.ItemsSource = BarometerMeasurements;

            SensorSpeed sensorSpeed = SensorSpeed.UI;

            Barometer.ReadingChanged += Barometer_ReadingChanged;
            Barometer.Start(sensorSpeed);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Barometer.ReadingChanged -= Barometer_ReadingChanged;
            Barometer.Stop();
        }

        void Barometer_ReadingChanged(object sender, BarometerChangedEventArgs e)
        {
            CurrentPressure = e.Reading.PressureInHectopascals;
            CurrentAltitude = 44307.694 * (1 - Math.Pow(CurrentPressure / 1021.6, 0.190284));
        }

        private void Save_Button_Clicked(object sender, EventArgs e)
        {
            BarometerMeasurements.Add(new BarometerMeasurement
            {
                Altitude = CurrentAltitude,
                Pressure = CurrentPressure,
                Label = Measurement_Label.Text,
                HeightChange = (CurrentAltitude - BarometerMeasurements.FirstOrDefault()?.Altitude) ?? 0
            });
            Measurement_Label.Text = "";
            OnPropertyChanged();
        }
    }
    public class BarometerMeasurement
    {
        public double Pressure { get; set; }
        public double Altitude { get; set; }
        public string Label { get; set; }
        public double HeightChange { get; set; }

        public string Display => $"{Label}: {Altitude:N2}m Height difference: {HeightChange}m";
    }
}