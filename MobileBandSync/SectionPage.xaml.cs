using MobileBandSync.Common;
using MobileBandSync.Data;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MobileBandSync
{
    // Helper class for the diagram values
    public class DiagramData
    {
        public double Min
        {
            get;
            set;
        }
        public double Value
        {
            get;
            set;
        }
    }

    public sealed partial class SectionPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();


        //--------------------------------------------------------------------------------------------------------------------
        public SectionPage()
        //--------------------------------------------------------------------------------------------------------------------
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper( this );
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            WorkoutMap.LoadingStatusChanged += this.Map_LoadingStatusChanged;
            CancelTokenSource = new CancellationTokenSource();

            Viewport = null;
            ViewInitialized = false;
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

        public int currentWorkoutId { get; private set; }
        public MapIcon StartIcon { get; private set; }
        public GeoboundingBox Viewport { get; private set; }
        public bool MapInitialized { get; private set; }
        public CancellationTokenSource CancelTokenSource { get; private set; }
        public WorkoutItem CurrentWorkout { get; private set; }
        public bool ViewInitialized { get; private set; }

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
        private async void NavigationHelper_LoadState( object sender, LoadStateEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            currentWorkoutId = (int)e.NavigationParameter;

            var workout = await WorkoutDataSource.GetWorkoutAsync( currentWorkoutId );

            if( workout.Items.Count == 0 )
            {
                CancelTokenSource.Dispose();
                CancelTokenSource = new CancellationTokenSource();
            }

            workout.Modified = false;

            await ShowWorkout( workout );
        }

        //--------------------------------------------------------------------------------------------------------------------
        private async Task ShowWorkout( WorkoutItem workout )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( workout != null )
            {
                MapInitialized = false;
                ViewInitialized = false;

                CurrentWorkout = workout;
                this.DefaultViewModel["Workout"] = workout;

                var nwPos = new BasicGeoposition()
                {
                    Latitude = (double)( (double)( workout.LatitudeStart + workout.LatDeltaRectNE ) / 10000000 ),
                    Longitude = (double)( (double)( workout.LongitudeStart + workout.LongDeltaRectSW ) / 10000000 )
                };
                var sePos = new BasicGeoposition()
                {
                    Latitude = (double)( (double)( workout.LatitudeStart + workout.LatDeltaRectSW ) / 10000000 ),
                    Longitude = (double)( (double)( workout.LongitudeStart + workout.LongDeltaRectNE ) / 10000000 )
                };
                Viewport = new GeoboundingBox( nwPos, sePos, AltitudeReferenceSystem.Terrain );

                await Dispatcher.RunAsync( CoreDispatcherPriority.High, () =>
                {
                    WorkoutMap.MapElements.Clear();

                    StartIcon = new MapIcon();
                    StartIcon.Location = new Geopoint( new BasicGeoposition()
                    {
                        Latitude = (double)( (double)workout.LatitudeStart / 10000000 ),
                        Longitude = (double)( (double)workout.LongitudeStart / 10000000 )
                    } );
                    StartIcon.Title = "Start";

                    WorkoutMap.MapElements.Add( StartIcon );
                } );
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
        private async void NavigationHelper_SaveState( object sender, SaveStateEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            CleanupChart();

            if( CurrentWorkout != null && CurrentWorkout.Items != null && CurrentWorkout.Items.Count > 0 )
            {
                CurrentWorkout.Items.Clear();
                CurrentWorkout.ElevationChart.Clear();
                CurrentWorkout.HeartRateChart.Clear();
                CurrentWorkout.CadenceNormChart.Clear();
            }

            if( CurrentWorkout.Modified )
            {
                await WorkoutDataSource.UpdateWorkoutAsync( CurrentWorkout.WorkoutId, CurrentWorkout.Title, CurrentWorkout.Notes );
                await CurrentWorkout.UpdateWorkout();
            }
        }

        /// <summary>
        /// Shows the details of an item clicked on in the <see cref="ItemPage"/>
        /// </summary>
        /// <param name="sender">The GridView displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        //--------------------------------------------------------------------------------------------------------------------
        private void ItemView_ItemClick( object sender, ItemClickEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            var itemId = ( (TrackItem)e.ClickedItem ).UniqueId;
            if( !Frame.Navigate( typeof( ItemPage ), itemId ) )
            {
                var resourceLoader = ResourceLoader.GetForCurrentView( "Resources" );
                throw new Exception( resourceLoader.GetString( "NavigationFailedExceptionMessage" ) );
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
        protected override void OnNavigatedTo( NavigationEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            this.navigationHelper.OnNavigatedTo( e );
        }

        //--------------------------------------------------------------------------------------------------------------------
        protected override void OnNavigatedFrom( NavigationEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            this.navigationHelper.OnNavigatedFrom( e );
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------------
        private async void Left_Tapped( object sender, TappedRoutedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( CurrentWorkout != null )
            {
                if( CurrentWorkout.Modified )
                {
                    await WorkoutDataSource.UpdateWorkoutAsync( CurrentWorkout.WorkoutId, CurrentWorkout.Title, CurrentWorkout.Notes );
                    await CurrentWorkout.UpdateWorkout();
                    CurrentWorkout.Modified = false;
                }

                if( CurrentWorkout.Items != null && CurrentWorkout.Items.Count > 0 )
                {
                    CurrentWorkout.Items.Clear();
                    CurrentWorkout.ElevationChart.Clear();
                    CurrentWorkout.HeartRateChart.Clear();
                    CurrentWorkout.CadenceNormChart.Clear();
                }

                CleanupChart();
                CancelTokenSource.Cancel();

                var workout = CurrentWorkout.GetPrevSibling();
                if( workout != null )
                {
                    CurrentWorkout = workout;
                    await ShowWorkout( CurrentWorkout );
                }
            }
        }


        //--------------------------------------------------------------------------------------------------------------------
        private async void Right_Tapped( object sender, TappedRoutedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( CurrentWorkout != null )
            {
                if( CurrentWorkout.Modified )
                {
                    await WorkoutDataSource.UpdateWorkoutAsync( CurrentWorkout.WorkoutId, CurrentWorkout.Title, CurrentWorkout.Notes );
                    await CurrentWorkout.UpdateWorkout();
                    CurrentWorkout.Modified = false;
                }

                if( CurrentWorkout.Items != null && CurrentWorkout.Items.Count > 0 )
                {
                    CurrentWorkout.Items.Clear();
                    CurrentWorkout.ElevationChart.Clear();
                    CurrentWorkout.HeartRateChart.Clear();
                    CurrentWorkout.CadenceNormChart.Clear();
                }

                CleanupChart();
                CancelTokenSource.Cancel();

                var workout = CurrentWorkout.GetNextSibling();
                if( workout != null )
                {
                    CurrentWorkout = workout;
                    await ShowWorkout( CurrentWorkout );
                }
            }
        }


        //--------------------------------------------------------------------------------------------------------------------
        private async void Share_Tapped( object sender, TappedRoutedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( CurrentWorkout != null )
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

                savePicker.FileTypeChoices.Add( "Garmin Training Center Database", new List<string>() { ".tcx" } );

                savePicker.SuggestedFileName = CurrentWorkout.FilenameTCX;
                var tcxFile = await savePicker.PickSaveFileAsync();

                if( tcxFile != null )
                {
                    await CurrentWorkout.ExportWorkout( tcxFile );
                }
                ViewInitialized = true;
                MapInitialized = true;
            }
        }


        //--------------------------------------------------------------------------------------------------------------------
        private async void Map_LoadingStatusChanged( MapControl sender, object args )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( sender.LoadingStatus == MapLoadingStatus.Loaded )
            {
                try
                {
                    if( !MapInitialized && Viewport != null )
                    {
                        CancelTokenSource.Dispose();
                        CancelTokenSource = new CancellationTokenSource();

                        MapInitialized = true;

                        WorkoutMap.DesiredPitch = 0;

                        // we need to try twice or more depending on the system
                        bool bResult = false;
                        int iRetry = 0;
                        var margin = new Thickness( 70, 70, 70, 70 );
                        do
                        {
                            bResult = await WorkoutMap.TrySetViewBoundsAsync( Viewport, margin, MapAnimationKind.None );

                            if( !bResult )
                            {
                                //await Task.Delay( 100 );
                                CancelTokenSource.Token.ThrowIfCancellationRequested();
                            }
                        }
                        while( !bResult && ( iRetry++ ) < 10 );
                        Viewport = null;

                        if( NetworkInterface.GetIsNetworkAvailable() )
                            WorkoutMap.Style = MapStyle.AerialWithRoads;

                        if( CurrentWorkout != null )
                        {
                            if( CurrentWorkout.Items == null || CurrentWorkout.Items.Count == 0 )
                            {
                                CurrentWorkout.TracksLoaded += WorkoutTracks_Loaded;
                                await CurrentWorkout.ReadTrackData( CancelTokenSource.Token );
                            }
                            else
                            {
                                await AddTracks( CurrentWorkout );
                                await LoadChartContents( CurrentWorkout );
                            }
                        }
                    }
                }
                catch( Exception )
                {
                }
            }
        }

        
        //--------------------------------------------------------------------------------------------------------------------
        private async Task AddTracks( WorkoutItem workout )
        //--------------------------------------------------------------------------------------------------------------------
        {
            await Dispatcher.RunAsync( CoreDispatcherPriority.High, () =>
            {
                if( workout != null && workout.Items.Count > 0 && WorkoutMap.MapElements.Count <= 10 )
                {
                    try
                    {
                        var routePointList = new List<BasicGeoposition>();
                        var lastSec = -1;
                        var lastWaypoint = workout.Items[workout.Items.Count - 1];

                        foreach( var waypoint in workout.Items )
                        {
                            // show every 6 sec minimum to keep the number of waypoints low
                            if( lastSec < 0 || waypoint == lastWaypoint || ( waypoint.SecFromStart - lastSec ) >= 6 )
                            {
                                lastSec = waypoint.SecFromStart;
                                routePointList.Add(
                                    new BasicGeoposition
                                    {
                                        Latitude = (double)( (double)( workout.LatitudeStart + waypoint.LatDelta ) / 10000000 ),
                                        Longitude = (double)( (double)( workout.LongitudeStart + waypoint.LongDelta ) / 10000000 ),
                                    } );
                            }
                            CancelTokenSource.Token.ThrowIfCancellationRequested();
                        }

                        var trackPath = new Geopath( routePointList );
                        var mapPolygon =
                            new MapPolyline
                            {
                                Path = trackPath,
                                ZIndex = 1,
                                StrokeColor = Colors.Red,
                                StrokeThickness = 4,
                                StrokeDashed = false,
                            };
                        WorkoutMap.MapElements.Add( mapPolygon );
                    }
                    catch( Exception )
                    {
                        WorkoutMap.MapElements.Clear();
                    }
                }
            } );
        }

        //--------------------------------------------------------------------------------------------------------------------
        private async void WorkoutTracks_Loaded( object sender, TracksLoadedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( e.Workout == CurrentWorkout )
            {
                CurrentWorkout.TracksLoaded -= WorkoutTracks_Loaded;

                await AddTracks( CurrentWorkout );
                await LoadChartContents( CurrentWorkout );
            }
        }

        //--------------------------------------------------------------------------------------------------------------------
        private void RunIfSelected( UIElement element, Action action )
        //--------------------------------------------------------------------------------------------------------------------
        {
            action.Invoke();
        }


        //--------------------------------------------------------------------------------------------------------------------
        private async Task LoadChartContents( WorkoutItem workout )
        //--------------------------------------------------------------------------------------------------------------------
        {
            await Dispatcher.RunAsync( CoreDispatcherPriority.High, () =>
            {
                RunIfSelected(
                    lineChart,
                    () =>
                    {
                        var hrLine = this.lineChart.Series[2] as LineSeries;
                        if( hrLine != null )
                            hrLine.ItemsSource = workout.HeartRateChart;
                        var elevLine = this.lineChart.Series[1] as LineSeries;
                        if( elevLine != null )
                            elevLine.ItemsSource = workout.ElevationChart;
                        var cadenceLine = this.lineChart.Series[0] as LineSeries;
                        if( cadenceLine != null )
                            cadenceLine.ItemsSource = workout.CadenceNormChart;
                    } );
            } );
        }

        //--------------------------------------------------------------------------------------------------------------------
        private async void CleanupChart()
        //--------------------------------------------------------------------------------------------------------------------
        {
            await Dispatcher.RunAsync( CoreDispatcherPriority.High, () =>
            {
                if( lineChart != null && lineChart.Series != null && lineChart.Series.Count > 0 )
                {
                    for( int i = 0; i <= 2; i++ )
                    {
                        var chart = this.lineChart.Series[i] as LineSeries;
                        if( chart != null )
                        {
                            if( chart.Triggers != null )
                                chart.Triggers.Clear();
                            if( chart.Points != null )
                                chart.Points.Clear();
                            if( chart.Resources != null )
                                chart.Resources.Clear();

                            chart.ItemsSource = null;
                        }
                    }
                    if( lineChart.Transitions != null )
                        lineChart.Transitions.Clear();
                    if( lineChart.Triggers != null )
                        lineChart.Triggers.Clear();
                }
            } );
        }

        //--------------------------------------------------------------------------------------------------------------------
        private async void WorkoutMap_LayoutUpdated( object sender, object e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            try
            {
                if( !ViewInitialized && CurrentWorkout != null && Viewport != null )
                {
                    ViewInitialized = true;

                    // we need to try twice or more depending on the system
                    bool bResult = false;
                    int iRetry = 0;
                    var margin = new Thickness( 70, 70, 70, 70 );
                    do
                    {
                        bResult = await WorkoutMap.TrySetViewBoundsAsync( Viewport, margin, MapAnimationKind.None );

                        if( !bResult )
                            CancelTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    while( !bResult && ( iRetry++ ) < 10 );
                }
            }
            catch( Exception )
            {
            }
        }
    }
}
