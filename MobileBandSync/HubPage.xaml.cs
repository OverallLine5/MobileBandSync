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
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;
using System.ComponentModel;
using System.Runtime.CompilerServices;


// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MobileBandSync
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    //========================================================================================================================
    public sealed partial class HubPage : Page
    //========================================================================================================================
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        public WorkoutData PageWorkoutData = new WorkoutData();

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

            MapServiceToken = "AobMbD2yKlST1QB_mh1mPfpnJGDtpm0lefHMTVPqU0NQR58-xEVO3KhAaOaqJL6y";
            WorkoutDataSource.SetMapServiceToken( MapServiceToken );
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
        public string MapServiceToken { get; private set; }
        public bool FilterAccepted { get; private set; }
        public bool MapPickerInitialized { get; private set; }
        public ToggleButton ToggleFilter { get; private set; }


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
            if( DefaultViewModel.ContainsKey( "WorkoutData" ) )
                DefaultViewModel.Remove( "WorkoutData" );

            PageWorkoutData.Workouts = await WorkoutDataSource.GetWorkoutsAsync( true );
            PageWorkoutData.WorkoutTitle = await WorkoutDataSource.GetWorkoutSummaryAsync(); ;

            DefaultViewModel["WorkoutData"] = PageWorkoutData;

            if( !DefaultViewModel.ContainsKey( "SyncView" ) )
            {
                SyncView.Enabled = WorkoutDataSource._offlineTest;
                SyncView.Connected = false;
                DefaultViewModel["SyncView"] = SyncView;

                SyncView.ConnectionText = "Disconnected";
                SyncView.DeviceText = "";
                SyncView.StatusText = "";
                SyncView.ConnectionLog = "";

                bool bConnected = WorkoutDataSource._offlineTest || await SyncView.StartDeviceSearch();

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
            try
            {
                if( e.ClickedItem != null )
                {
                    var workoutId = ( (WorkoutItem) e.ClickedItem ).WorkoutId;
                    var workout = e.ClickedItem as WorkoutItem;

                    if( workout != null )
                    {
                        Type pageType = typeof( SectionPage );
                        if( workout.WorkoutType == (byte) EventType.Sleeping )
                            pageType = typeof( SleepPage );

                        Frame.Navigate( pageType, workoutId );
                    }
                }
            }
            catch
            {
                // prevent loading an item
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
            if( !WorkoutDataSource._offlineTest )
                await SyncView.StartDeviceSync();
            else
                await SyncView.StartSyncFromLogs();

            PageWorkoutData.Workouts = WorkoutDataSource.GetWorkouts();
            PageWorkoutData.WorkoutTitle = await WorkoutDataSource.GetWorkoutSummaryAsync(); ;

            if( PageWorkoutData.Workouts != null )
            {
                if( DefaultViewModel.ContainsKey( "WorkoutData" ) )
                    DefaultViewModel.Remove( "WorkoutData" );

                this.DefaultViewModel["WorkoutData"] = PageWorkoutData;
            }
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

        //--------------------------------------------------------------------------------------------------------------------
        private async void BackupDatabase_Tapped( object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
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

        //--------------------------------------------------------------------------------------------------------------------
        private async void ToggleButton_Checked( object sender, RoutedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            Flyout MyFlyout = Resources["MyFlyout"] as Flyout;
            ToggleButton toggleButton = sender as ToggleButton;

            if( toggleButton != null )
            {
                ToggleFilter = toggleButton;

                if( ( DateTime.Now - startDatePicker.Date ) < new TimeSpan( 1, 0, 0 ) )
                    startDatePicker.Date = DateTime.Now - new TimeSpan( 15 * 365, 0, 0, 0, 0 );

                if( ( DateTime.Now - endDatePicker.Date ) < new TimeSpan( 1, 0, 0 ) )
                    endDatePicker.Date = DateTime.Now;

                MyFlyout.ShowAt( toggleButton );

                if( MapPickerInitialized == false )
                {
                    MapPickerInitialized = true;

                    try
                    {
                        Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 500 };
                        Geoposition pos = await geolocator.GetGeopositionAsync();

                        Dispatcher.RunAsync( CoreDispatcherPriority.High, () =>
                        {
                            var basicGeoPos =
                                new BasicGeoposition()
                                {
                                    Latitude = pos.Coordinate.Latitude,
                                    Longitude = pos.Coordinate.Longitude,
                                    Altitude = (double) pos.Coordinate.Altitude
                                };
                            MapPicker.ZoomLevel = 10;
                            MapPicker.Center = new Geopoint( basicGeoPos );
                        } );
                    }
                    catch
                    {
                        // don't select a center
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------------
        private async void ButtonOK_Tapped( object sender, TappedRoutedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            FilterAccepted = true;

            Flyout MyFlyout = Resources["MyFlyout"] as Flyout;
            MyFlyout.Hide();

            WorkoutDataSource.DataSource.CurrentFilter =
                new WorkoutFilterData()
                {
                    Start = startDatePicker.Date != null ? startDatePicker.Date.DateTime : DateTime.MinValue,
                    End = endDatePicker.Date != null ? endDatePicker.Date.DateTime : DateTime.MaxValue,
                    IsBikingWorkout = chkBike.IsChecked,
                    IsRunningWorkout = chkRun.IsChecked,
                    IsWalkingWorkout = chkWalk.IsChecked,
                    IsSleepingWorkout = chkSleep.IsChecked,
                };
            if( chkMap.IsChecked == true )
            {
                WorkoutDataSource.DataSource.CurrentFilter.SetBounds( MapPicker );
                WorkoutDataSource.DataSource.CurrentFilter.MapSelected = true;
            }
            else
                WorkoutDataSource.DataSource.CurrentFilter.SetBounds( null );

            PageWorkoutData.Workouts = await WorkoutDataSource.GetWorkoutsAsync( true, WorkoutDataSource.DataSource.CurrentFilter );
            PageWorkoutData.WorkoutTitle = await WorkoutDataSource.GetWorkoutSummaryAsync(); ;

            if( DefaultViewModel.ContainsKey( "WorkoutData" ) )
                DefaultViewModel.Remove( "WorkoutData" );

            DefaultViewModel["WorkoutData"] = PageWorkoutData;
        }

        //--------------------------------------------------------------------------------------------------------------------
        private void ButtonCancel_Tapped( object sender, TappedRoutedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            Flyout MyFlyout = Resources["MyFlyout"] as Flyout;
            MyFlyout.Hide();
        }

        //--------------------------------------------------------------------------------------------------------------------
        private async void ToggleFilter_Unchecked( object sender, RoutedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            WorkoutDataSource.DataSource.CurrentFilter = null;

            PageWorkoutData.Workouts = await WorkoutDataSource.GetWorkoutsAsync( true );
            PageWorkoutData.WorkoutTitle = await WorkoutDataSource.GetWorkoutSummaryAsync(); ;

            if( DefaultViewModel.ContainsKey( "WorkoutData" ) )
                DefaultViewModel.Remove( "WorkoutData" );

            DefaultViewModel["WorkoutData"] = PageWorkoutData;
        }

        //--------------------------------------------------------------------------------------------------------------------
        private async void Flyout_Closed( object sender, object e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( !FilterAccepted )
            {
                if( ToggleFilter != null )
                {
                    ToggleFilter.IsChecked = false;
                }
                WorkoutDataSource.DataSource.CurrentFilter = null;
                PageWorkoutData.Workouts = await WorkoutDataSource.GetWorkoutsAsync( true );
                PageWorkoutData.WorkoutTitle = await WorkoutDataSource.GetWorkoutSummaryAsync(); ;

                if( DefaultViewModel.ContainsKey( "WorkoutData" ) )
                    DefaultViewModel.Remove( "WorkoutData" );

                DefaultViewModel["WorkoutData"] = PageWorkoutData;
            }
            FilterAccepted = false;
        }

        //--------------------------------------------------------------------------------------------------------------------
        private async void PlusButton_Tapped( object sender, TappedRoutedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            filePicker.FileTypeFilter.Add( ".tcx" );

            var file = await filePicker.PickSingleFileAsync();
            if( file != null )
            {
                List<WorkoutItem> listWorkouts = new List<WorkoutItem>();
                var workout = await WorkoutItem.ReadWorkoutFromTcx( file.Path );

                if( workout != null )
                {
                    listWorkouts.Add( workout );

                    await WorkoutDataSource.StoreWorkouts( listWorkouts );
                    PageWorkoutData.Workouts = await WorkoutDataSource.GetWorkoutsAsync( true );
                    PageWorkoutData.WorkoutTitle = await WorkoutDataSource.GetWorkoutSummaryAsync(); ;

                    if( DefaultViewModel.ContainsKey( "WorkoutData" ) )
                        DefaultViewModel.Remove( "WorkoutData" );

                    DefaultViewModel["WorkoutData"] = PageWorkoutData;
                }
            }
        }
    }
}
