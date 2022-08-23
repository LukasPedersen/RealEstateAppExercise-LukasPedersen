using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PropertyListPage : ContentPage
    {
        public bool sortFlip { get; set; }

        IRepository Repository;
        public ObservableCollection<PropertyListItem> PropertiesCollection { get; } = new ObservableCollection<PropertyListItem>();

        public PropertyListPage()
        {
            InitializeComponent();

            Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            LoadProperties();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadProperties();
        }

        void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            LoadProperties();
            list.IsRefreshing = false;
        }

        async void LoadProperties()
        {
            PropertiesCollection.Clear();
            var items = Repository.GetProperties();

            

            foreach (Property item in items)
            {
                PropertyListItem listItem = new PropertyListItem(item);
                Location propertyLocation = new Location((double)listItem.Property.Latitude, (double)listItem.Property.Longitude);
                listItem.Distance = Location.CalculateDistance(propertyLocation, await Geolocation.GetLastKnownLocationAsync(), DistanceUnits.Kilometers);

                PropertiesCollection.Add(listItem);
            }
        }

        private async void ItemsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new PropertyDetailPage(e.Item as PropertyListItem));
        }

        private async void AddProperty_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPropertyPage());
        }
    }
}