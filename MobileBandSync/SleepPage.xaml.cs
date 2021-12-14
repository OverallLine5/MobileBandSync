using MobileBandSync.Common;
using MobileBandSync.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MobileBandSync
{

    //========================================================================================================================
    public sealed partial class SleepPage : Page
    //========================================================================================================================
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private TimeSpan awakeTimeSpan;
        private TimeSpan sleepTimeSpan;

        private Color colAwakeBar = new Color() { A = 255, R = 255, G = 139, B = 2 };
        private Color colLightBar = new Color() { A = 255, R = 0, G = 121, B = 214 };
        private Color colRestfulBar = new Color() { A = 255, R = 0, G = 61, B = 110 };
        private Color colHeaderBackground = new Color() { A = 255, R = 0, G = 90, B = 161 };
        private Color colHeaderSummaryDate = new Color() { A = 255, R = 144, G = 206, B = 255 };
        private Color colHeaderSummaryTime = new Color() { A = 255, R = 242, G = 255, B = 255 };
        private Color colHeaderSummaryText = new Color() { A = 255, R = 242, G = 255, B = 255 };
        private Color colDiagramHeader = new Color() { A = 255, R = 213, G = 213, B = 213 };
        private Color colDiagramXAxisText = new Color() { A = 255, R = 213, G = 213, B = 213 };
        private Color colDiagramYAxisText = new Color() { A = 255, R = 213, G = 213, B = 213 };
        private Color colDiagramFooterTitle = new Color() { A = 255, R = 235, G = 235, B = 235 };
        private Color colDiagramFooterSubtitle = new Color() { A = 255, R = 145, G = 145, B = 145 };
        private Color colDiagramFooterDuration = new Color() { A = 255, R = 35, G = 104, B = 169 };
        private Color colDiagramGrid = new Color() { A = 255, R = 239, G = 238, B = 236 };

        List<string> slCadence = new List<string>();
        private CultureInfo sleepPageCultureInfo = new CultureInfo( "en-US" );

        //--------------------------------------------------------------------------------------------------------------------
        public enum SleepType
        //--------------------------------------------------------------------------------------------------------------------
        {
            Unknown,
            Awake,
            LightSleep,
            RestfulSleep
        }


        //--------------------------------------------------------------------------------------------------------------------
        public SleepPage()
        //--------------------------------------------------------------------------------------------------------------------
        {
            this.InitializeComponent();

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
        public CancellationTokenSource CancelTokenSource { get; private set; }
        public WorkoutItem CurrentWorkout { get; private set; }
        public Size CanvasSize { get; private set; }

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
            currentWorkoutId = (int) e.NavigationParameter;

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

            ShowWorkout( workout );
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
            if( CurrentWorkout != null && CurrentWorkout.Items != null && CurrentWorkout.Items.Count > 0 )
            {
                CurrentWorkout.Items.Clear();
            }

#if WINDOWS_UWP
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
#endif
        }


        //--------------------------------------------------------------------------------------------------------------------
        private async Task ShowWorkout( WorkoutItem workout )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( workout != null )
            {
                try
                {
                    CurrentWorkout = workout;
                    this.DefaultViewModel["Workout"] = workout;

                    if( CurrentWorkout != null )
                    {
                        CancelTokenSource.Dispose();
                        CancelTokenSource = new CancellationTokenSource();

                        if( CurrentWorkout.Items == null || CurrentWorkout.Items.Count == 0 )
                        {
                            CurrentWorkout.TracksLoaded += WorkoutTracks_Loaded;
                            await CurrentWorkout.ReadSleepData( CancelTokenSource.Token );
                            await ShowChart( CanvasSize.Width, CanvasSize.Height, CurrentWorkout );
                        }
                    }
                }
                catch( Exception )
                {
                }
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
                if( CurrentWorkout.Items != null && CurrentWorkout.Items.Count > 0 )
                {
                    CurrentWorkout.Items.Clear();
                }

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
                if( CurrentWorkout.Items != null && CurrentWorkout.Items.Count > 0 )
                {
                    CurrentWorkout.Items.Clear();
                }

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
                }
            }
        }


#if WINDOWS_UWP
        //--------------------------------------------------------------------------------------------------------------------
        private void System_BackRequested( object sender, BackRequestedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
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

        //--------------------------------------------------------------------------------------------------------------------
        private bool DrawSleepDiagram()
        //--------------------------------------------------------------------------------------------------------------------
        {
            BarPanel.Children.Clear();
            BarPanel.ColumnDefinitions.Clear();

            if( CurrentWorkout != null && CurrentWorkout.Items.Count > 0 )
            {
                uint lastSleepType = CurrentWorkout.Items[0].SleepType;
                uint lastSegmentType = CurrentWorkout.Items[0].SegmentType;
                DateTime lastSegmentDate = CurrentWorkout.Start;
                DateTime currentDate = CurrentWorkout.Start;

                AddXAxis( CurrentWorkout.Start, CurrentWorkout.End );

                Hours.Text = CurrentWorkout.SleepDuration.Hours.ToString();
                Minutes.Text = CurrentWorkout.SleepDuration.Minutes.ToString( "00" );
                RestfulHours.Text = CurrentWorkout.TotalRestfulSleepDuration.Hours.ToString();
                RestfulMinutes.Text = CurrentWorkout.TotalRestfulSleepDuration.Minutes.ToString( "00" );
                LightHours.Text = CurrentWorkout.TotalRestlessSleepDuration.Hours.ToString();
                LightMinutes.Text = CurrentWorkout.TotalRestlessSleepDuration.Minutes.ToString( "00" );

                foreach( var item in CurrentWorkout.Items )
                {
                    currentDate = ( CurrentWorkout.Start + new TimeSpan( 0, 0, item.SecFromStart ) );
                    AddSleepItem( item, ref lastSleepType, ref lastSegmentType, ref lastSegmentDate );
                }

                // add remaining sleep segment
                AddSleepItem( null, ref lastSleepType, ref lastSegmentType, ref lastSegmentDate );
            }
            return true;
        }


        //--------------------------------------------------------------------------------------------------------------------
        private void AddSleepItem( TrackItem item, ref uint lastSleepType, ref uint lastSegmentType,
                                   ref DateTime lastSegmentDate )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( item != null )
            {
                var currentDate = ( CurrentWorkout.Start + new TimeSpan( 0, 0, item.SecFromStart ) );

                if( item.SleepType != lastSleepType || item.SegmentType != lastSegmentType ||
                    item == CurrentWorkout.Items[CurrentWorkout.Items.Count - 1] )
                {
                    var timeSpan = new TimeSpan( 0, (int) ( currentDate - lastSegmentDate ).TotalMinutes, 0 );

                    string binary = Convert.ToString( item.Cadence, 2 ).PadLeft( 32, '0' ); ;
                    slCadence.Add( binary );

                    if( lastSleepType == 0 && lastSegmentType == 0 )
                    {
                        AddAwakeBar( timeSpan );
                        awakeTimeSpan += timeSpan;
                    }
                    else
                    {
                        if( lastSleepType >= 20000 )
                            AddRestfulBar( timeSpan );
                        else
                            AddLightBar( timeSpan );

                        sleepTimeSpan += timeSpan;
                    }

                    lastSleepType = item.SleepType;
                    lastSegmentType = item.SegmentType;
                    lastSegmentDate = currentDate;
                }
            }
            else if( lastSegmentDate < CurrentWorkout.End )
            {
                var timeSpan = new TimeSpan( 0, (int) ( CurrentWorkout.End - lastSegmentDate ).TotalMinutes, 0 );
                AddAwakeBar( timeSpan );
                awakeTimeSpan += timeSpan;
            }
        }


        //--------------------------------------------------------------------------------------------------------------------
        private bool AddXAxis( DateTime dtStart, DateTime dtEnd )
        //--------------------------------------------------------------------------------------------------------------------
        {
            bool bResult = true;

            var startMinutes = 60 - dtStart.Minute;
            var remainingMinutes = dtEnd.Minute;
            var dtTemp = dtStart.AddMinutes( startMinutes );

            Date.Text = 
                dtStart.ToLocalTime().ToString( "ddd M/d", sleepPageCultureInfo ) + "   Avg HR: " + CurrentWorkout.AvgHR.ToString() +
                "   Max HR: " + CurrentWorkout.MaxHR.ToString() + "   Cal: " + CurrentWorkout.Calories.ToString();

            string strDateText = dtStart.ToLocalTime().ToString( "h:mmtt", sleepPageCultureInfo ).ToLower();
            strDateText = strDateText.Substring( 0, strDateText.Length - 1 );
            AsleepTime.Text = "Asleep " + strDateText;

            strDateText = dtEnd.ToLocalTime().ToString( "h:mmtt", sleepPageCultureInfo ).ToLower();
            strDateText = strDateText.Substring( 0, strDateText.Length - 1 );
            AwakeTime.Text = "Woke up " + strDateText;

            HourText.Children.Clear();
            LineHour.ColumnDefinitions.Clear();
            HourText.ColumnDefinitions.Clear();

            if( startMinutes > 0 )
            {
                LineHour.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( startMinutes, GridUnitType.Star ) } );
                HourText.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( startMinutes, GridUnitType.Star ) } );

                if( startMinutes > 15 )
                {
                    string strHourText;
                    if( dtStart.ToLocalTime().Hour == 23 || dtStart.ToLocalTime().Hour == 0 )
                    {
                        strHourText = dtStart.ToLocalTime().ToString( "htt", sleepPageCultureInfo ).ToLower();
                        strHourText = strHourText.Substring( 0, strHourText.Length - 1 );
                    }
                    else
                        strHourText = dtStart.ToLocalTime().ToString( "hh", sleepPageCultureInfo ).TrimStart( new char[] { '0' } );

                    TextBlock hourText =
                        new TextBlock()
                        {
                            Text = strHourText,
                            VerticalAlignment = VerticalAlignment.Bottom,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            FontSize = 16,
                            FontWeight = FontWeights.Normal,
                            Foreground = new SolidColorBrush( new Color() { A = 0xFF, R = 0x91, G = 0x91, B = 0x91 } ),
                            Margin = new Thickness( 0, 0, -4, 0 )
                        };

                    Grid.SetColumn( hourText, 0 );
                    HourText.Children.Add( hourText );
                }

                Border border = new Border();
                border.BorderThickness = new Thickness( 0, 0, 1, 0 );
                border.BorderBrush = new SolidColorBrush( new Color() { A = 0xFF, R = 0xED, G = 0xEC, B = 0xEA } );
                border.HorizontalAlignment = HorizontalAlignment.Stretch;
                border.VerticalAlignment = VerticalAlignment.Stretch;
                border.Margin = new Thickness( 0, 0, 0, 23 );

                Grid.SetColumn( border, LineHour.ColumnDefinitions.Count - 1 );
                Grid.SetRow( border, 0 );

                LineHour.Children.Add( border );
            }

            do
            {
                LineHour.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( 60, GridUnitType.Star ) } );
                HourText.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( 60, GridUnitType.Star ) } );

                string strHourText;
                if( dtTemp.ToLocalTime().Hour == 23 || dtTemp.ToLocalTime().Hour == 0 )
                {
                    strHourText = dtTemp.ToLocalTime().ToString( "htt", sleepPageCultureInfo ).ToLower();
                    strHourText = strHourText.Substring( 0, strHourText.Length - 1 );
                }
                else
                    strHourText = dtTemp.ToLocalTime().ToString( "hh", sleepPageCultureInfo ).TrimStart( new char[] { '0' } );

                TextBlock hourText =
                    new TextBlock()
                    {
                        Text = strHourText,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        FontSize = 16,
                        FontWeight = FontWeights.Normal,
                        Foreground = new SolidColorBrush( new Color() { A = 0xFF, R = 0x91, G = 0x91, B = 0x91 } ),
                        Margin = new Thickness( 0, 0, -4, 0 )
                    };

                Grid.SetColumn( hourText, HourText.ColumnDefinitions.Count - 1 );
                HourText.Children.Add( hourText );

                Border border = new Border();
                border.BorderThickness = new Thickness( 0, 0, 1, 0 );
                border.BorderBrush = new SolidColorBrush( new Color() { A = 0xFF, R = 0xED, G = 0xEC, B = 0xEA } );
                border.HorizontalAlignment = HorizontalAlignment.Stretch;
                border.VerticalAlignment = VerticalAlignment.Stretch;
                border.Margin = new Thickness( 0, 0, 0, 23 );

                Grid.SetColumn( border, LineHour.ColumnDefinitions.Count - 1 );
                Grid.SetRow( border, 0 );

                LineHour.Children.Add( border );

                dtTemp = dtTemp.AddHours( 1 );
            }
            while( dtTemp <= dtEnd );

            if( remainingMinutes > 0 )
            {
                LineHour.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( remainingMinutes, GridUnitType.Star ) } );
                HourText.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( remainingMinutes, GridUnitType.Star ) } );
            }

            return bResult;
        }


        //--------------------------------------------------------------------------------------------------------------------
        private bool AddBar( SleepType sleepType, 
                             TimeSpan tsLength )
        //--------------------------------------------------------------------------------------------------------------------
        {
            bool bResult = true;

            var bar = new Rectangle();
            bar.Margin = new Thickness( 0, 0, 0, 0 );
            bar.VerticalAlignment = VerticalAlignment.Stretch;
            bar.HorizontalAlignment = HorizontalAlignment.Stretch;

            switch( sleepType )
            {
                case SleepType.Awake:
                    bar.Fill = new SolidColorBrush( colAwakeBar );
                    bar.Stroke = new SolidColorBrush( colAwakeBar );
                    Grid.SetRow( bar, 0 );
                    break;

                case SleepType.LightSleep:
                    bar.Fill = new SolidColorBrush( colLightBar );
                    bar.Stroke = new SolidColorBrush( colLightBar );
                    Grid.SetRow( bar, 1 );
                    break;

                case SleepType.RestfulSleep:
                    bar.Fill = new SolidColorBrush( colRestfulBar );
                    bar.Stroke = new SolidColorBrush( colRestfulBar );
                    Grid.SetRow( bar, 1 );
                    Grid.SetRowSpan( bar, 2 );
                    break;

                default:
                    break;
            }

            BarPanel.ColumnDefinitions.Add( new ColumnDefinition() { Width = new GridLength( tsLength.TotalMinutes, GridUnitType.Star ) } );
            Grid.SetColumn( bar, BarPanel.ColumnDefinitions.Count - 1 );

            BarPanel.Children.Add( bar );

            return bResult;
        }


        //--------------------------------------------------------------------------------------------------------------------
        private bool AddAwakeBar( TimeSpan dtLength )
        //--------------------------------------------------------------------------------------------------------------------
        {
            return AddBar( SleepType.Awake, dtLength );
        }


        //--------------------------------------------------------------------------------------------------------------------
        private bool AddLightBar( TimeSpan dtLength )
        //--------------------------------------------------------------------------------------------------------------------
        {
            return AddBar( SleepType.LightSleep, dtLength );
        }


        //--------------------------------------------------------------------------------------------------------------------
        private bool AddRestfulBar( TimeSpan dtLength )
        //--------------------------------------------------------------------------------------------------------------------
        {
            return AddBar( SleepType.RestfulSleep, dtLength );
        }

        //--------------------------------------------------------------------------------------------------------------------
        private async void WorkoutTracks_Loaded( object sender, TracksLoadedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            e.Workout.TracksLoaded -= WorkoutTracks_Loaded;

            if( e.Workout == CurrentWorkout )
            {
                try
                {
                    await Dispatcher.RunAsync( CoreDispatcherPriority.High, () =>
                    {
                        if( CurrentWorkout.Items.Count > 0 )
                        {
                            DrawSleepDiagram();
                        }
                    } );
                }
                catch( Exception )
                {
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
        public async Task ShowChart( double width, double height, WorkoutItem workout )
        //--------------------------------------------------------------------------------------------------------------------
        {
            await Dispatcher.RunAsync( CoreDispatcherPriority.Normal, () =>
            {
                var heartRateMultiply = height / 70;
                var tempMultiply = height / 10;
                var xOffset = width / workout.Items.Count;
                double x = 0;

                SleepDiagrams.Children.Clear();

                PointCollection pointCollection = new PointCollection();
                PointCollection shapePointCollection = new PointCollection();
                //PointCollection tempPointCollection = new PointCollection();

                foreach( var item in workout.Items )
                {
                    pointCollection.Add( new Point( x, height - ( item.Heartrate * heartRateMultiply ) + 40 ) );
                    shapePointCollection.Add( new Point( x, height - ( item.Heartrate * heartRateMultiply ) + 40 ) );
                    //tempPointCollection.Add( new Point( x, height - ( item.SkinTemp * tempMultiply ) ) );

                    x += xOffset;
                }

                pointCollection.Add( new Point( x - xOffset, height ) );
                pointCollection.Add( new Point( 0, height ) );

                Polyline polyline = new Polyline();
                polyline.Points = pointCollection;
                polyline.Stroke = new SolidColorBrush( Windows.UI.Colors.Transparent );
                polyline.Fill = new SolidColorBrush( new Color() { A = 0x33, R = 0xFF, B = 0x00, G = 0x00 } );
                polyline.StrokeThickness = 1;

                Polyline shapePolyline = new Polyline();
                shapePolyline.Points = shapePointCollection;
                shapePolyline.Stroke = new SolidColorBrush( new Color() { A = 0xFF, R = 0xFF, B = 0x00, G = 0x00 } );
                shapePolyline.StrokeThickness = 1;

                //Polyline tempPolyline = new Polyline();
                //tempPolyline.Points = tempPointCollection;
                //tempPolyline.Stroke = new SolidColorBrush( new Color() { A = 0xCC, R = 0x00, B = 0x00, G = 0x00 } );
                //tempPolyline.StrokeThickness = 1;

                SleepDiagrams.Children.Add( polyline );
                SleepDiagrams.Children.Add( shapePolyline );
                //SleepDiagrams.Children.Add( tempPolyline );
            } );
        }


        //--------------------------------------------------------------------------------------------------------------------
        private async void SleepDiagrams_SizeChanged( object sender, SizeChangedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( e == null )
                return;

            CanvasSize = e.NewSize;

            if( e.PreviousSize != e.NewSize )
            {
                var canvas = sender as Canvas;
                var workout = ( canvas != null ? canvas.DataContext as WorkoutItem : null );

                if( workout != null )
                {
                    await ShowChart( e.NewSize.Width, e.NewSize.Height, workout );
                }
            }
        }
    }
}
