﻿using Sonaticket.Mobile.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace Sonaticket.Mobile.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}