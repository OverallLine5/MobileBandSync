using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using MobileBandSync.Common;
using MobileBandSync.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MobileBandSync
{
    //========================================================================================================================
    public sealed partial class SectionPage : Page
    //========================================================================================================================
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();


        //--------------------------------------------------------------------------------------------------------------------
        public SectionPage()
        //--------------------------------------------------------------------------------------------------------------------
        {
            this.InitializeComponent();

            Viewport = null;
            ViewInitialized = false;
            HideMap = false;

            MainGrid.RowDefinitions.Clear();

            this.navigationHelper = new NavigationHelper( this );
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            CancelTokenSource = new CancellationTokenSource();
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
        public GeoboundingBox Viewport { get; private set; }
        public bool MapInitialized { get; private set; }
        public CancellationTokenSource CancelTokenSource { get; private set; }
        public WorkoutItem CurrentWorkout { get; private set; }
        public bool ViewInitialized { get; private set; }
        public Line chartLine { get; private set; }
        public MapIcon PosNeedleIcon { get; private set; }
        public bool HideMap { get; set; }

        public static List<MapIcon> DistanceMarkers = new List<MapIcon>();

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

#if WINDOWS_UWP
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            currentView.BackRequested += System_BackRequested;
#endif

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
                HideMap = ( workout.LongitudeStart == 0 && workout.LatitudeStart == 0 );

                WorkoutMap.Visibility = HideMap ? Visibility.Collapsed : Visibility.Visible;
                ShareImage.Visibility = HideMap ? Visibility.Collapsed : Visibility.Visible;

                MainGrid.RowDefinitions.Clear();

                if( !HideMap )
                {
                    WorkoutMap.LoadingStatusChanged += this.Map_LoadingStatusChanged;
                    WorkoutMap.MapServiceToken = WorkoutDataSource.GetMapServiceToken();

                    MainGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 1, GridUnitType.Auto ) } );
                    MainGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 1, GridUnitType.Star ) } );
                    MainGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 180, GridUnitType.Pixel ) } );
                    MainGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 20, GridUnitType.Pixel ) } );
                }
                else
                {
                    MainGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 1, GridUnitType.Auto ) } );
                    MainGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 0, GridUnitType.Pixel ) } );
                    MainGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 1, GridUnitType.Star ) } );
                    MainGrid.RowDefinitions.Add( new RowDefinition() { Height = new GridLength( 30, GridUnitType.Pixel ) } );
                }

                try
                {
                    MapInitialized = false;
                    ViewInitialized = false;

                    StatusText.Text = "";
                    StatusGrid.Visibility = Visibility.Collapsed;

                    CurrentWorkout = workout;
                    this.DefaultViewModel["Workout"] = workout;

                    if( !HideMap )
                    {
                        var nwPos = new BasicGeoposition()
                        {
                            Latitude = (double) ( (double) ( workout.LatitudeStart + workout.LatDeltaRectNE ) / 10000000 ),
                            Longitude = (double) ( (double) ( workout.LongitudeStart + workout.LongDeltaRectSW ) / 10000000 )
                        };
                        var sePos = new BasicGeoposition()
                        {
                            Latitude = (double) ( (double) ( workout.LatitudeStart + workout.LatDeltaRectSW ) / 10000000 ),
                            Longitude = (double) ( (double) ( workout.LongitudeStart + workout.LongDeltaRectNE ) / 10000000 )
                        };
                        Viewport = new GeoboundingBox( nwPos, sePos, AltitudeReferenceSystem.Terrain );
                    }

                    if( CurrentWorkout != null )
                    {
                        CancelTokenSource.Dispose();
                        CancelTokenSource = new CancellationTokenSource();

                        if( CurrentWorkout.Items == null || CurrentWorkout.Items.Count == 0 )
                        {
                            CurrentWorkout.TracksLoaded += WorkoutTracks_Loaded;
                            PosNeedleIcon = null;

                            if( !HideMap )
                                WorkoutMap.MapElements.Clear();

                            CurrentWorkout.ReadTrackData( CancelTokenSource.Token );
                        }
                        else
                        {
                            LoadChartContents( CurrentWorkout );
                            await AddTracks( CurrentWorkout );
                        }
                    }
                }
                catch( Exception )
                {
                    if( HideMap )
                    {
                        PosNeedleIcon = null;
                        WorkoutMap.MapElements.Clear();
                    }
                }
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
                CurrentWorkout.SpeedChart.Clear();
            }

            if( CurrentWorkout.Modified )
            {
                await WorkoutDataSource.UpdateWorkoutAsync( CurrentWorkout.WorkoutId, CurrentWorkout.Title, CurrentWorkout.Notes );
                await CurrentWorkout.UpdateWorkout();
            }

#if WINDOWS_UWP
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
#endif
        }


        //--------------------------------------------------------------------------------------------------------------------
        public async void CreateDistancePoint( WorkoutItem item, TrackItem trackpoint, int iDistance )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( item == null )
                return;

            try
            {
                var displayInformation = DisplayInformation.GetForCurrentView();
                float scaleFactor = (float)((DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel * 2) / 6);

                int iSize = (int)((iDistance > 0 ? 50 : 30) * scaleFactor);
                string strDistance = iDistance.ToString();
                var device = CanvasDevice.GetSharedDevice();
                var distanceMarker = new CanvasRenderTarget( device, iSize, iSize, 96 );
                using( var ds = distanceMarker.CreateDrawingSession() )
                {
                    ds.FillRectangle(0, 0, iSize, iSize, iDistance > 0 ? Colors.DarkRed : Colors.Green);
                    ds.DrawRectangle(2 * scaleFactor, 2 * scaleFactor, iSize - 3 * scaleFactor, iSize - 3 * scaleFactor, Colors.White, 5 * scaleFactor);
                    if (iDistance > 0)
                        ds.DrawText(strDistance, (strDistance.Length > 1 ? 6 : 15) * scaleFactor, 1 * scaleFactor, Colors.White, new CanvasTextFormat() { FontSize = (int)(iSize / 1.5), FontWeight = FontWeights.Bold });
                }

                MapIcon mapIcon = new MapIcon();

                using ( InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream() )
                {
                    //create a bitmap encoder
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync( BitmapEncoder.PngEncoderId, stream );
                    encoder.SetPixelData(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Ignore,
                        (uint)distanceMarker.Size.Width,
                        (uint)distanceMarker.Size.Height,
                        displayInformation.LogicalDpi,
                        displayInformation.LogicalDpi,
                        distanceMarker.GetPixelBytes() );
                    await encoder.FlushAsync();

                    mapIcon.Image = RandomAccessStreamReference.CreateFromStream( stream );
                    mapIcon.ZIndex = iDistance + 2;
                    mapIcon.Title = "";
                    mapIcon.Visible = true;
                    mapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point( 0.5, 0.5 );
                    mapIcon.Location =
                        new Geopoint(
                            new BasicGeoposition()
                            {
                                Latitude = (double)( (double)( item.LatitudeStart + ( trackpoint == null ? 0 : trackpoint.LatDelta ) ) / 10000000 ),
                                Longitude = (double)( (double)( item.LongitudeStart + ( trackpoint == null ? 0 : trackpoint.LongDelta ) ) / 10000000 ),
                                Altitude = 0
                            } );

                    WorkoutMap.MapElements.Add(mapIcon);
                    DistanceMarkers.Add(mapIcon);
                }
            }
            catch ( Exception )
            {
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
                    if( !HideMap )
                    {
                        PosNeedleIcon = null;
                        WorkoutMap.MapElements.Clear();
                    }

                    CurrentWorkout.Items.Clear();
                    CurrentWorkout.ElevationChart.Clear();
                    CurrentWorkout.HeartRateChart.Clear();
                    CurrentWorkout.CadenceNormChart.Clear();
                    CurrentWorkout.SpeedChart.Clear();
                }

                if( chartLine != null )
                {
                    DiagramGrid.Children.Remove( chartLine );
                }

                CleanupChart();

                var workout = CurrentWorkout.GetPrevSibling();
                if( workout != null )
                {
                    CurrentWorkout = workout;
                    CancelTokenSource.Cancel();
                    ShowWorkout( workout );
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
                    if( !HideMap )
                    {
                        PosNeedleIcon = null;
                        WorkoutMap.MapElements.Clear();
                    }

                    CurrentWorkout.Items.Clear();
                    CurrentWorkout.ElevationChart.Clear();
                    CurrentWorkout.HeartRateChart.Clear();
                    CurrentWorkout.CadenceNormChart.Clear();
                    CurrentWorkout.SpeedChart.Clear();
                }

                if( chartLine != null )
                    DiagramGrid.Children.Remove( chartLine );

                CleanupChart();

                var workout = CurrentWorkout.GetNextSibling();
                if( workout != null )
                {
                    CurrentWorkout = workout;
                    CancelTokenSource.Cancel();
                    ShowWorkout( CurrentWorkout );
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
                bool bResult = false;

                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

                savePicker.FileTypeChoices.Add( "Garmin Training Center Database", new List<string>() { ".tcx" } );

                savePicker.SuggestedFileName = CurrentWorkout.FilenameTCX;
                var tcxFile = await savePicker.PickSaveFileAsync();

                if( tcxFile != null )
                    bResult = await CurrentWorkout.ExportWorkout( tcxFile );

                if( bResult )
                {
                    ViewInitialized = true;
                    MapInitialized = true;
                }
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
                                CancelTokenSource.Token.ThrowIfCancellationRequested();
                        }
                        while( !bResult && ( iRetry++ ) < 10 );
                        Viewport = null;

                        if( NetworkInterface.GetIsNetworkAvailable() )
                            WorkoutMap.Style = MapStyle.AerialWithRoads;
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
            if( HideMap )
                return;

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
                                Visible = true
                            };
                        WorkoutMap.MapElements.Add( mapPolygon );
                    }
                    catch( Exception )
                    {
                        PosNeedleIcon = null;
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

                try
                {
                    if( !HideMap )
                        DistanceMarkers.Clear();

                    LoadChartContents( CurrentWorkout );

                    if( !HideMap )
                    {
                        await Dispatcher.RunAsync( CoreDispatcherPriority.High, () =>
                        {
                            if( CurrentWorkout.Items.Count > 0 )
                            {
                                var distance = 0;
                                CreateDistancePoint( CurrentWorkout, CurrentWorkout.Items[0], distance );
                                foreach( var trackpoint in CurrentWorkout.Items )
                                {
                                    if( ( ( trackpoint.TotalMeters / 1000 ) * WorkoutDataSource.DistanceConversion ) >= distance + 1 )
                                    {
                                        distance++;
                                        CancelTokenSource.Token.ThrowIfCancellationRequested();
                                        CreateDistancePoint( CurrentWorkout, trackpoint, distance );
                                    }
                                }
                            }
                        } );
                    }

                    AddTracks( CurrentWorkout );
                }
                catch( Exception )
                {
                    if( !HideMap )
                    {
                        PosNeedleIcon = null;
                        WorkoutMap.MapElements.Clear();
                    }
                }
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
                        var hrLine = this.lineChart.Series[3] as LineSeries;
                        if( hrLine != null )
                            hrLine.ItemsSource = workout.HeartRateChart;
                        var speedLine = this.lineChart.Series[2] as LineSeries;
                        if( speedLine != null )
                            speedLine.ItemsSource = workout.SpeedChart;
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
                    for( int i = 0; i <= 3; i++ )
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


        //--------------------------------------------------------------------------------------------------------------------
        private async void Grid_Tapped( object sender, TappedRoutedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            var parentGrid = sender as Grid;
            if( parentGrid != null )
            {
                var tappedPos = e.GetPosition( parentGrid );
                if( tappedPos != null )
                {
                    bool bSelectionAdded = HideMap;
                    if( chartLine != null )
                        parentGrid.Children.Remove( chartLine );

                    if( PosNeedleIcon != null )
                        WorkoutMap.MapElements.Remove( PosNeedleIcon );

                    var heartLine = parentGrid.FindName( "heartLine" ) as LineSeries;
                    if( heartLine != null )
                    {
                        var workout = heartLine.DataContext as WorkoutItem;
                        var diagramData = heartLine.ItemsSource as ObservableCollection<DiagramData>;
                        if( workout != null && diagramData != null && diagramData.Count > 0 )
                        {
                            GeneralTransform gt = heartLine.TransformToVisual( parentGrid );
                            var rightBoundary = gt.TransformPoint( new Point( heartLine.ActualWidth, 0 ) );
                            var leftBoundary = gt.TransformPoint( new Point( 0, 0 ) );

                            if( tappedPos.X >= leftBoundary.X && tappedPos.X <= rightBoundary.X )
                            {
                                var dMultiply = ( heartLine.ActualWidth / diagramData.Count );
                                var iDiagramIndex = (int)( ( tappedPos.X - leftBoundary.X ) / dMultiply );
                                var iItemsIndex = diagramData[iDiagramIndex].Index;
                                var TrackItem = ( iItemsIndex < workout.Items.Count ? workout.Items[iItemsIndex] : null );

                                if( TrackItem != null )
                                {
                                    chartLine = new Line();
                                    chartLine.Stroke = new SolidColorBrush( Windows.UI.Colors.White );
                                    chartLine.X1 = chartLine.X2 = tappedPos.X;
                                    chartLine.Y1 = 0;
                                    chartLine.Y2 = parentGrid.ActualHeight;

                                    // add vertical marker line
                                    parentGrid.Children.Add( chartLine );

                                    // show status text
                                    var pace = ( 1 / TrackItem.SpeedMeterPerSecond ) / 60 * 1000;
                                    var leftover = pace % 1;
                                    var minutes = pace - leftover;
                                    var seconds = Math.Round( leftover * 60 );
                                    var speedKmH = ( TrackItem.SpeedMeterPerSecond * 3.6 ) * WorkoutDataSource.DistanceConversion;

                                    if( TrackItem.SpeedMeterPerSecond <= 0 )
                                        minutes = seconds = 0;

                                    var distKm = ( ( TrackItem.TotalMeters / (double) 1000 ) * WorkoutDataSource.DistanceConversion );
                                    StatusText.Text =
                                        distKm.ToString( "0.000" ) +
                                        ( WorkoutDataSource.DistanceConversion == 1 ? " km, " : " mi., " ) + 
                                        ( TrackItem.Elevation * ( WorkoutDataSource.DistanceConversion == 1 ? 1.0 : 3.2808399 ) ).ToString( "0.00" ) +
                                        ( WorkoutDataSource.DistanceConversion == 1 ? " m, " : " ft., " ) +
                                        ( WorkoutDataSource.DistanceConversion == 1 ? ( minutes.ToString() + ":" + seconds.ToString( "00" ) + "/km, " ) : "" ) +
                                        speedKmH.ToString( "0.00" ) +
                                        ( WorkoutDataSource.DistanceConversion == 1 ? " km/h, HR: " : " mph, HR: " ) + TrackItem.Heartrate.ToString() +
                                        ", GSR: " + TrackItem.GSR.ToString() +
                                        ", Temp: " + TrackItem.SkinTemp.ToString();
                                    StatusGrid.Visibility = Visibility.Visible;

                                    if( !HideMap )
                                    {
                                        Geopoint snPoint =
                                            new Geopoint(
                                                new BasicGeoposition()
                                                {
                                                    Latitude = (double) ( (double) ( workout.LatitudeStart + TrackItem.LatDelta ) / 10000000 ),
                                                    Longitude = (double) ( (double) ( workout.LongitudeStart + TrackItem.LongDelta ) / 10000000 ),
                                                    Altitude = 0
                                                } );

                                        PosNeedleIcon = new MapIcon
                                        {
                                            Location = snPoint,
                                            NormalizedAnchorPoint = new Point( 0.5, 1 ),
                                            ZIndex = 80,
                                            Image = RandomAccessStreamReference.CreateFromUri( new Uri( "ms-appx:///Assets/DetailPos.png" ) ),
                                            Title = "",
                                            Visible = true
                                        };

                                        WorkoutMap.MapElements.Add( PosNeedleIcon );

                                        // center in case the point is outside the visible area
                                        var bounds = GetBounds( WorkoutMap );
                                        if( !IsGeopointInGeoboundingBox( bounds, snPoint ) )
                                            WorkoutMap.Center = snPoint;
                                    }

                                    bSelectionAdded = true;
                                }
                            }

                            if( !bSelectionAdded )
                            {
                                StatusText.Text = "";
                                StatusGrid.Visibility = Visibility.Collapsed;

                                bool bResult = false;
                                int iRetry = 0;
                                var margin = new Thickness( 70, 70, 70, 70 );

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
                                var viewport = new GeoboundingBox( nwPos, sePos, AltitudeReferenceSystem.Terrain );

                                do
                                {
                                    bResult = await WorkoutMap.TrySetViewBoundsAsync( viewport, margin, MapAnimationKind.None );

                                    if( !bResult )
                                        CancelTokenSource.Token.ThrowIfCancellationRequested();
                                }
                                while( !bResult && ( iRetry++ ) < 10 );
                            }
                        }
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------------
        public GeoboundingBox GetBounds( MapControl map )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( map.Center.Position.Latitude == 0 ) { return default( GeoboundingBox ); }

            double degreePerPixel = ( 156543.04 * Math.Cos( map.Center.Position.Latitude * Math.PI / 180 ) ) / ( 111325 * Math.Pow( 2, map.ZoomLevel ) );

            double mHalfWidthInDegrees = map.ActualWidth * degreePerPixel / 0.9;
            double mHalfHeightInDegrees = map.ActualHeight * degreePerPixel / 1.7;

            double mNorth = map.Center.Position.Latitude + mHalfHeightInDegrees;
            double mWest = map.Center.Position.Longitude - mHalfWidthInDegrees;
            double mSouth = map.Center.Position.Latitude - mHalfHeightInDegrees;
            double mEast = map.Center.Position.Longitude + mHalfWidthInDegrees;

            GeoboundingBox mBounds = new GeoboundingBox(
                new BasicGeoposition()
                {
                    Latitude = mNorth,
                    Longitude = mWest
                },
                new BasicGeoposition()
                {
                    Latitude = mSouth,
                    Longitude = mEast
                } );

            return mBounds;
        }



        //--------------------------------------------------------------------------------------------------------------------
        public bool IsGeopointInGeoboundingBox( GeoboundingBox bounds, Geopoint point )
        //--------------------------------------------------------------------------------------------------------------------
        {
            bool bResult =
                ( point.Position.Latitude < bounds.NorthwestCorner.Latitude &&
                  point.Position.Longitude > bounds.NorthwestCorner.Longitude &&
                  point.Position.Latitude > bounds.SoutheastCorner.Latitude &&
                  point.Position.Longitude < bounds.SoutheastCorner.Longitude );

            return bResult;
        }

#if WINDOWS_UWP
        private void System_BackRequested( object sender, BackRequestedEventArgs e )
        {
            if( !e.Handled )
            {
                e.Handled = TryGoBack();
            }
        }
#endif

        //--------------------------------------------------------------------------------------------------------------------
        private bool TryGoBack()
        //--------------------------------------------------------------------------------------------------------------------
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if( rootFrame.CanGoBack )
            {
                rootFrame.GoBack();
                return true;
            }
            return false;
        }
    }
}
