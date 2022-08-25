﻿using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PropertyDetailPage : ContentPage
    {
        public PropertyDetailPage(PropertyListItem propertyListItem)
        {
            InitializeComponent();

            Property = propertyListItem.Property;

            IRepository Repository = TinyIoCContainer.Current.Resolve<IRepository>();
            Agent = Repository.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) => {
                await Navigation.PushAsync(new ImageListPage(propertyListItem.Property));
            };
            MainImage.GestureRecognizers.Add(tapGestureRecognizer);

            BindingContext = this;
        }



        public Agent Agent { get; set; }

        public Property Property { get; set; }

        private async void EditProperty_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new AddEditPropertyPage(Property));
        }

        private async void Phone_Button_Clicked_1(object sender, System.EventArgs e)
        {
            string action = await DisplayActionSheet($"{Property.PropertyVendor.Phone}", "Cancel", "OK", "Call", "SMS");
            if (action == "Call")
            {
                try
                {
                    PhoneDialer.Open(Property.PropertyVendor.Phone);
                }
                catch (FeatureNotSupportedException)
                {
                    throw;
                }
            }
            else if (action == "SMS")
            {
                var message = new SmsMessage
                {
                    Recipients = new List<string> { "53604669" },
                    Body = $"Hej {Property.PropertyVendor.FirstName} angående {Property.Address}"
                };

                await Sms.ComposeAsync(message);
            }
        }

        private async void Email_Button_Clicked(object sender, System.EventArgs e)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var attachmentFilePath = Path.Combine(folder, "property.txt");
            File.WriteAllText(attachmentFilePath, $"{Property.Address}");

            var message = new EmailMessage
            {
                Subject = "Buy house!",
                Body = "This house is awesome",
                To = new List<string>{ Property.PropertyVendor.Email}
            };

            message.Attachments.Add(new EmailAttachment(attachmentFilePath));
            try
            {
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException)
            {
                throw;
            }
            
        }
    }
}