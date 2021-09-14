using MobileBandSync.Common;
using MobileBandSync.Data;
using System;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Bluetooth;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MobileBandSync.MSFTBandLib;
using System.Collections.Generic;


// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MobileBandSync
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    //------------------------------------------------------------------------------------------------------------------------
    public sealed partial class HubPage : Page
    //------------------------------------------------------------------------------------------------------------------------
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        //--------------------------------------------------------------------------------------------------------------------
        public HubPage()
        //--------------------------------------------------------------------------------------------------------------------
        {
            this.InitializeComponent();

            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            SyncView = new SyncViewModel();
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        //--------------------------------------------------------------------------------------------------------------------
        public NavigationHelper NavigationHelper
        //--------------------------------------------------------------------------------------------------------------------
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        //--------------------------------------------------------------------------------------------------------------------
        public ObservableDictionary DefaultViewModel
        //--------------------------------------------------------------------------------------------------------------------
        {
            get { return this.defaultViewModel; }
        }

        public SyncViewModel SyncView { get; set; }
        public DispatcherTimer DeviceTimer { get; private set; }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        //--------------------------------------------------------------------------------------------------------------------
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( !DefaultViewModel.ContainsKey( "Workouts" ) )
            {
                var sampleWorkouts = await WorkoutDataSource.GetWorkoutsAsync();
                DefaultViewModel["Workouts"] = sampleWorkouts;
            }
            if( !DefaultViewModel.ContainsKey( "SyncView" ) )
            {
                SyncView.Connected = false;
                DefaultViewModel["SyncView"] = SyncView;

                SyncView.ConnectionText = "Disconnected";
                SyncView.DeviceText = "";
                SyncView.StatusText = "";
                SyncView.ConnectionLog = "";

                bool bConnected = await SyncView.StartDeviceSearch();

                if( !bConnected )
                {
                    DeviceTimer = new DispatcherTimer();
                    DeviceTimer.Interval = new TimeSpan( 0, 0, 10 );
                    DeviceTimer.Tick += OnDeviceTimer;
                    DeviceTimer.Start();
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------------
        private async void OnDeviceTimer( object sender, object e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            DeviceTimer.Stop();

            bool bConnected = await SyncView.StartDeviceSearch();

            if( !bConnected )
            {
                // keep the timer running
                DeviceTimer.Start();
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        //--------------------------------------------------------------------------------------------------------------------
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        //--------------------------------------------------------------------------------------------------------------------
        {
            // TODO: Save the unique state of the page here.
        }

        /// <summary>
        /// Shows the details of a clicked workout in the <see cref="SectionPage"/>.
        /// </summary>
        //--------------------------------------------------------------------------------------------------------------------
        private void WorkoutItem_ItemClick(object sender, ItemClickEventArgs e)
        //--------------------------------------------------------------------------------------------------------------------
        {
            var workoutId = ((WorkoutItem)e.ClickedItem).WorkoutId;
            if (!Frame.Navigate(typeof(SectionPage), workoutId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        /// <summary>
        /// Shows the details of an item clicked on in the <see cref="ItemPage"/>
        /// </summary>
        //--------------------------------------------------------------------------------------------------------------------
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        //--------------------------------------------------------------------------------------------------------------------
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((TrackItem)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        //--------------------------------------------------------------------------------------------------------------------
        public async void btnSync_Click( object sender, RoutedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            await SyncView.StartDeviceSync();

            var sampleWorkouts = await WorkoutDataSource.GetWorkoutsAsync( true );
            this.DefaultViewModel["Workouts"] = sampleWorkouts;
        }


        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        //--------------------------------------------------------------------------------------------------------------------
        protected override void OnNavigatedTo(NavigationEventArgs e)
        //--------------------------------------------------------------------------------------------------------------------
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        //--------------------------------------------------------------------------------------------------------------------
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        //--------------------------------------------------------------------------------------------------------------------
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void BackupDatabase_Tapped( object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e )
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add( "*" );

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if( folder != null )
            {
                await WorkoutDataSource.BackupDatabase( folder );
            }
        }
    }
}
