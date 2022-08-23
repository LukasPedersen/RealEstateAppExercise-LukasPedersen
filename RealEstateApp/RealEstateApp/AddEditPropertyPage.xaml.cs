﻿using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditPropertyPage : ContentPage
    {
        private IRepository Repository;
        CancellationTokenSource ttscts;

        #region PROPERTIES
        public ObservableCollection<Agent> Agents { get; }

        private Property _property;
        public Property Property
        {
            get => _property;
            set
            {
                _property = value;
                if (_property.AgentId != null)
                {
                    SelectedAgent = Agents.FirstOrDefault(x => x.Id == _property?.AgentId);
                }

            }
        }

        private Agent _selectedAgent;

        public Agent SelectedAgent
        {
            get => _selectedAgent;
            set
            {
                if (Property != null)
                {
                    _selectedAgent = value;
                    Property.AgentId = _selectedAgent?.Id;
                }
            }
        }

        public string StatusMessage { get; set; }

        public Color StatusColor { get; set; } = Color.White;
        #endregion

        public AddEditPropertyPage(Property property = null)
        {
            InitializeComponent();

            Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            Agents = new ObservableCollection<Agent>(Repository.GetAgents());

            if (property == null)
            {
                Title = "Add Property";
                Property = new Property();
            }
            else
            {
                Title = "Edit Property";
                Property = property;
            }

            BindingContext = this;
        }

        private async void SaveProperty_Clicked(object sender, System.EventArgs e)
        {
            if (CheckConnection())
            {
                if (IsValid() == false)
                {
                    StatusMessage = "Please fill in all required fields";
                    StatusColor = Color.Red;
                }
                else
                {
                    Repository.SaveProperty(Property);
                    await Navigation.PopToRootAsync();
                }
            }
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Property.Address)
                || Property.Beds == null
                || Property.Price == null
                || Property.AgentId == null)
                return false;

            return true;
        }

        protected override void OnAppearing()
        {
           CheckConnection();
        }

        protected override void OnDisappearing()
        {
            Vibration.Cancel();
            CancelSpeech();
        }

        public void CancelSpeech()
        {
            if (ttscts?.IsCancellationRequested ?? true)
                return;

            ttscts.Cancel();
        }

        private bool CheckConnection()
        {
            var current = Connectivity.NetworkAccess;
            switch (current)
            {
                case NetworkAccess.Unknown:
                    SignalIndicator.TextColor = Color.Yellow;
                    SignalIndicatorMesaage.Text = "Unknown signal";
                    WarnBadSiganl();
                    return false;
                case NetworkAccess.None:
                    SignalIndicator.TextColor = Color.Red;
                    SignalIndicatorMesaage.Text = "No signal";
                    WarnBadSiganl();
                    return false;
                case NetworkAccess.Local:
                    SignalIndicator.TextColor = Color.Red;
                    SignalIndicatorMesaage.Text = "No signal";
                    WarnBadSiganl();
                    return false;
                case NetworkAccess.ConstrainedInternet:
                    SignalIndicator.TextColor = Color.Yellow;
                    SignalIndicatorMesaage.Text = "Limited signal";
                    return true;
                case NetworkAccess.Internet:
                    SignalIndicator.TextColor = Color.Green;
                    SignalIndicatorMesaage.Text = "Good signal";
                    return true;
                default:
                    SignalIndicator.TextColor = Color.Yellow;
                    SignalIndicatorMesaage.Text = "Unknown";
                    return true;
            }
        }

        private async void WarnBadSiganl()
        {
            ttscts = new CancellationTokenSource();
            var ttsSettings = new SpeechOptions()
            {
                Volume = .75f,
                Pitch = 1.0f
            };

            Vibration.Vibrate();
            SignalIndicatorMesaage.Text = "Signal strength is not sufficient enough to upload";
            await TextToSpeech.SpeakAsync("Signal styrke er ikke stærkt nok, prøv igen senere",ttsSettings, ttscts.Token);
        }

        private async void CancelSave_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private async void Set_Position_Button_Clicked(object sender, System.EventArgs e)
        {
            if (CheckConnection())
            {
                Location location = await Geolocation.GetLocationAsync();
                Property.Latitude = location.Latitude;
                Property.Longitude = location.Longitude;

                var placemarks = await Geocoding.GetPlacemarksAsync((double)Property.Latitude, (double)Property.Longitude);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    var geocodeAddress =

                        $"{placemark.Thoroughfare}\n" +
                        $"{placemark.FeatureName}\n" +
                        $"{placemark.Locality}\n" +
                        $"{placemark.PostalCode}\n" +
                        $"{placemark.CountryName}\n";

                    Property.Address = geocodeAddress.ToString();
                }
            }
        }

        private async void Set_Address_Button_Clicked(object sender, EventArgs e)
        {
            if (CheckConnection())
            {
                var addressLocation = await Geocoding.GetLocationsAsync(Property.Address);
                var location = addressLocation?.FirstOrDefault();
                if (location != null)
                {
                    Property.Latitude = location.Latitude;
                    Property.Longitude = location.Longitude;
                }
            }
        }

        private async void Helicopter_Button_Clicked(object sender, EventArgs e)
        {
            await Flashlight.TurnOnAsync();
            await Flashlight.TurnOffAsync();
            await TextToSpeech.SpeakAsync("Bananananannanananananananananananananananananananananananananananananana");
            await Flashlight.TurnOnAsync();
            await Flashlight.TurnOffAsync();
            await Flashlight.TurnOnAsync();
            await Flashlight.TurnOffAsync();
            await Flashlight.TurnOnAsync();
            await Flashlight.TurnOffAsync();
        }
    }
}