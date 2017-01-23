using System;
using CoConstruct.AmazingRace.PageModels;
using CoConstruct.AmazingRace.Services;
using FreshMvvm;
using Xamarin.Forms;

namespace CoConstruct.AmazingRace
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            SetupIOC();

            SetupNavigationContainers();
        }

        private void SetupIOC()
        {
            FreshIOC.Container.Register<IAddressService, AddressService>().AsSingleton();
            FreshIOC.Container.Register<IDirectionsService, DirectionsService>().AsSingleton();
        }

        private void SetupNavigationContainers()
        {
            var page = FreshPageModelResolver.ResolvePageModel<HomePageModel>();
            var container = new FreshNavigationContainer(page, Guid.NewGuid().ToString());
            MainPage = container;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
