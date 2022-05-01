using MobileBandSync.MSFTBandLib.UWP;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MobileBandSync.MSFTBandLib;
using System.Collections.Generic;
using MobileBandSync.Data;
using Windows.Storage;
using Windows.UI.Core;
using Windows.ApplicationModel.Resources;

namespace MobileBandSync.Common
{
    //========================================================================================================================
    public class SyncViewModel : INotifyPropertyChanged
    //========================================================================================================================
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //--------------------------------------------------------------------------------------------------------------------
        public SyncViewModel()
        //--------------------------------------------------------------------------------------------------------------------
        {
            ResourceLoader = ResourceLoader.GetForViewIndependentUse();

            Enabled = false;
            Connected = false;

            ConnectionText = ResourceLoader.GetString( "NotConnected" );
            DeviceText = "";
            StatusText = "";
            SyncProgress = 0.0;
        }

        public ResourceLoader ResourceLoader { get; private set; }

        //--------------------------------------------------------------------------------------------------------------------
        public bool Enabled
        //--------------------------------------------------------------------------------------------------------------------
        {
            get { return _bEnabled; }
            set
            {
                _bEnabled = value;
                this.OnPropertyChanged( "Enabled" );
            }
        }

        //--------------------------------------------------------------------------------------------------------------------
        public Double SyncProgress
        //--------------------------------------------------------------------------------------------------------------------
        {
            get { return _dProgress; }
            set
            {
                if( _dProgress != value )
                {
                    _dProgress = value;
                    this.OnPropertyChanged( "SyncProgress" );
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------------
        public string StatusText
        //--------------------------------------------------------------------------------------------------------------------
        {
            get { return _strStatusText; }
            set { _strStatusText = value; this.OnPropertyChanged( "StatusText" ); }
        }

        //--------------------------------------------------------------------------------------------------------------------
        public string ConnectionText
        //--------------------------------------------------------------------------------------------------------------------
        {
            get { return _strConnectionText; }
            set { _strConnectionText = value; this.OnPropertyChanged( "ConnectionText" ); }
        }

        //--------------------------------------------------------------------------------------------------------------------
        public string ConnectionLog
        //--------------------------------------------------------------------------------------------------------------------
        {
            get { return _strConnectionLog; }
            set { _strConnectionLog = value; this.OnPropertyChanged( "ConnectionLog" ); }
        }

        //--------------------------------------------------------------------------------------------------------------------
        public string DeviceText
        //--------------------------------------------------------------------------------------------------------------------
        {
            get { return _strDeviceText; }
            set { _strDeviceText = value; this.OnPropertyChanged( "DeviceText" ); }
        }

        //--------------------------------------------------------------------------------------------------------------------
        public bool CleanupSensorLog
        //--------------------------------------------------------------------------------------------------------------------
        {
            get { return _bCleanupSensorLog; }
            set { _bCleanupSensorLog = value; this.OnPropertyChanged( "CleanupSensorLog" ); }
        }

        //--------------------------------------------------------------------------------------------------------------------
        public bool StoreSensorLogLocally
        //--------------------------------------------------------------------------------------------------------------------
        {
            get { return _bStoreSensorLogLocally; }
            set { _bStoreSensorLogLocally = value; this.OnPropertyChanged( "StoreSensorLogLocally" ); }
        }

        //--------------------------------------------------------------------------------------------------------------------
        public BandClientUWP BandClient
        //--------------------------------------------------------------------------------------------------------------------
        {
            get { return _bandClient; }
        }

        //--------------------------------------------------------------------------------------------------------------------
        public void OnPropertyChanged( string propertyName = null )
        //--------------------------------------------------------------------------------------------------------------------
        {
            this.PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
        }


        //--------------------------------------------------------------------------------------------------------------------
        public async Task<bool> StartDeviceSearch()
        //--------------------------------------------------------------------------------------------------------------------
        {
            Connected = false;

            ConnectionText = ResourceLoader.GetString( "NotConnected" );
            DeviceText = "";
            StatusText = "";
            SyncProgress = 0.0;
            Enabled = false;

            if( BandClient != null )
            {
                try
                {
                    var BandList = await BandClient.GetPairedBands();

                    if( BandList.Count > 0 )
                        ConnectionText = ResourceLoader.GetString( "SearchingDevice" );

                    CurrentBand = null;

                    // make sure only one Band is connected now
                    foreach( var band in BandList )
                    {
                        try
                        {
                            await band.Connect( Progress );
                            CurrentBand = band;
                            Connected = true;
                            break;
                        }
                        catch( Exception )
                        {
                            CurrentBand = null;
                        }
                    }
                }
                catch( Exception )
                {
                }

                if( CurrentBand != null )
                {
                    ConnectionText = ResourceLoader.GetString( "Connecting" );

                    var strSerial = await CurrentBand.GetSerialNumber();
                    WorkoutDataSource.BandName = DeviceText = CurrentBand.GetName();

                    Connected = true;

                    // immediately synchronize device time
                    await CurrentBand.SetDeviceTime( DateTime.Now, Report );
                    ConnectionText = ResourceLoader.GetString( "Connected" ) + ": #" + strSerial;

                    Enabled = true;
                }
                else
                {
                    CurrentBand = new Band<BandSocketUWP>( "", "" );
                    ConnectionText = ResourceLoader.GetString( "NotConnected" );
                }
            }
            return Connected;
        }


        //---------------------------------------------------------------------------------------------------------------
        public void Report( String strReport )
        //---------------------------------------------------------------------------------------------------------------
        {
            if( strReport != null && strReport.Length > 0 )
                ConnectionLog += strReport;

            ConnectionLog += Environment.NewLine;
        }


        //---------------------------------------------------------------------------------------------------------------
        public void Status( String strStatus )
        //---------------------------------------------------------------------------------------------------------------
        {
            if( strStatus != null && strStatus.Length > 0 )
                StatusText = strStatus;
        }


        //---------------------------------------------------------------------------------------------------------------
        public async Task<IEnumerable<WorkoutItem>> StartDeviceSync()
        //---------------------------------------------------------------------------------------------------------------
        {
            if( Connected && CurrentBand != null )
            {
                Enabled = false;

                StatusText = ResourceLoader.GetString( "Downloading" );

                WorkoutDataSource.BandName = CurrentBand.GetName();
                var btResult = await CurrentBand.GetSensorLog( Report, Progress, CleanupSensorLog == true, StoreSensorLogLocally == true );
                if( btResult != null )
                {
                    Report( null );
                    StatusText = ResourceLoader.GetString( "Importing" );

                    try
                    {
                        var byteCount = btResult.Length;
                        var listWorkouts = await WorkoutDataSource.ImportFromSensorlog( btResult, Status, Progress );

                        if( CurrentBand != null && listWorkouts.Count > 0 )
                        {
                            StatusText = ResourceLoader.GetString( "Storing" ) + " " + listWorkouts.Count + " " + ( listWorkouts.Count == 1 ? ResourceLoader.GetString( "Workout" ) : ResourceLoader.GetString( "Workouts" ) );

                            var StepLength = WorkoutDataSource.DataSource.SensorLogEngine.StepLength;
                            var listResult = await WorkoutDataSource.StoreWorkouts( listWorkouts, Progress, StepLength );

                            var result = await WorkoutDataSource.GetWorkoutsAsync( true );

                            Report( null );
                            Progress( 0, 0 );
                            StatusText = "";
                            Enabled = true;

                            return result;
                        }
                    }
                    catch( Exception )
                    {
                    }
                }
            }

            Report( null );

            Progress( 0, 0 );
            StatusText = "";
            Enabled = true;

            return null;
        }


        //---------------------------------------------------------------------------------------------------------------
        public async Task<IEnumerable<WorkoutItem>> StartSyncFromLogs()
        //---------------------------------------------------------------------------------------------------------------
        {
            Enabled = false;

            WorkoutDataSource.BandName = "Virtual Test Band";
            List<WorkoutItem> listWorkouts = new List<WorkoutItem>();
            StorageFolder sensorLogFolder = await KnownFolders.DocumentsLibrary.CreateFolderAsync( "SensorLog", CreationCollisionOption.OpenIfExists );
            if( sensorLogFolder != null )
            {
                try
                {
                    StatusText = "Reading from sensor log";
                    listWorkouts = await WorkoutDataSource.ImportFromSensorlog( sensorLogFolder, Status, Progress );

                    if( WorkoutDataSource._debugOutput )
                        Report( "Read " + listWorkouts.Count + ( listWorkouts.Count == 1 ? " workout" : " workouts" ) + " from log" );
                }
                catch( Exception )
                {
                }
            }

            if( listWorkouts.Count > 0 )
            {
                Progress( 0, WorkoutDataSource.DataSource.SensorLogEngine.BufferSize );
                StatusText = "Storing " + listWorkouts.Count + ( listWorkouts.Count == 1 ? " workout" : " workouts" );

                var StepLength = WorkoutDataSource.DataSource.SensorLogEngine.StepLength;
                await WorkoutDataSource.StoreWorkouts( listWorkouts, Progress, StepLength );

                if( WorkoutDataSource._debugOutput )
                    Report( "Stored " + listWorkouts.Count + ( listWorkouts.Count == 1 ? " workout" : " workouts" ) );
            }

            Report( null );

            Progress( 0, 0 );
            StatusText = "";
            Enabled = true;

            return listWorkouts.Count > 0 ? await WorkoutDataSource.GetWorkoutsAsync( true ) : null;
        }


        //---------------------------------------------------------------------------------------------------------------
        public async void Progress( UInt64 uiCompleted, UInt64 uiTotal )
        //---------------------------------------------------------------------------------------------------------------
        {
            if( uiTotal > 0 && TotalProgress != uiTotal )
            {
                TotalProgress = uiTotal;
                CompletedProgress = 0;
                SyncProgress = 0;
            }

            if( TotalProgress != 0 )
            {
                if( uiCompleted == 0 && uiTotal == 0 )
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync( CoreDispatcherPriority.Normal,
                     () =>
                     {
                         SyncProgress = 0;
                         TotalProgress = 0;
                         CompletedProgress = 0;
                     } );
                }
                else
                {
                    CompletedProgress = Math.Min( TotalProgress, CompletedProgress + uiCompleted );
                    float uiVal = ( (float)( (float)( CompletedProgress * 100 ) / (float)TotalProgress ) );
                    if( uiVal >= _dProgress + 1 )
                    {
                        await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync( CoreDispatcherPriority.Normal,
                         () =>
                         {
                             SyncProgress = (ulong)uiVal;
                         } );
                    }
                }
            }
        }


        private bool _bEnabled = true;
        private bool _bCleanupSensorLog = true;
        private bool _bStoreSensorLogLocally = false;
        private Double _dProgress = 0.0;
        private string _strStatusText = "";
        private string _strConnectionText = "";
        private string _strDeviceText = "";
        private string _strConnectionLog = "";
        public bool Connected { get; set; }
        public BandInterface CurrentBand { get; set; }

        private BandClientUWP _bandClient = new BandClientUWP();
        public UInt64 TotalProgress { get; private set; }
        public UInt64 CompletedProgress { get; private set; }
    }
}
