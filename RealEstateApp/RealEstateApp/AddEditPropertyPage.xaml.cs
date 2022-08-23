using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditPropertyPage : ContentPage
    {
        private IRepository Repository;

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

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Property.Address)
                || Property.Beds == null
                || Property.Price == null
                || Property.AgentId == null)
                return false;

            return true;
        }

        private async void CancelSave_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private async void Set_Position_Button_Clicked(object sender, System.EventArgs e)
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

        private async void Set_Address_Button_Clicked(object sender, EventArgs e)
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
}