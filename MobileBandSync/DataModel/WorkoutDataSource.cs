using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Data.Sqlite;
using System.IO;
using MobileBandSync.Common;
using System.Globalization;
using MobileBandSync.OpenTcx;
using MobileBandSync.OpenTcx.Entities;
using Windows.Data.Xml.Dom;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using System.Threading;
using Windows.UI.Core;
using Windows.Storage.FileProperties;

namespace MobileBandSync.Data
{
    /// <summary>
    /// Workout item data model.
    /// </summary>
    public class WorkoutItem
    {
        public WorkoutItem( String uniqueId, String title, String subtitle, String imagePath, String description )
        {
            this.Title = title;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Items = new ObservableCollection<TrackItem>();

            HeartRateChart = new ObservableCollection<DiagramData>();
            ElevationChart = new ObservableCollection<DiagramData>();
            CadenceNormChart = new ObservableCollection<DiagramData>();
        }

        public WorkoutItem()
        {
            HeartRateChart = new ObservableCollection<DiagramData>();
            ElevationChart = new ObservableCollection<DiagramData>();
            CadenceNormChart = new ObservableCollection<DiagramData>();
        }

        public string UniqueId { get { return Guid.NewGuid().ToString( "B" ); } }
        public string Subtitle { get { return Notes; } }
        public string Description { get; set; }
        public string ImagePath { get; private set; }
        public ObservableCollection<TrackItem> Items { get; set; }

        public int WorkoutId { get; set; }
        public byte WorkoutType { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public byte AvgHR { get; set; }
        public byte MaxHR { get; set; }
        public int Calories { get; set; }
        public int AvgSpeed { get; set; }
        public int MaxSpeed { get; set; }
        public int DurationSec { get; set; }
        public Int64 DistanceMeters { get; set; }
        public Int64 LongitudeStart { get; set; }
        public Int64 LatitudeStart { get; set; }
        public int LongDeltaRectSW { get; set; }
        public int LatDeltaRectSW { get; set; }
        public int LongDeltaRectNE { get; set; }
        public int LatDeltaRectNE { get; set; }
        public string DbPath { get; set; }
        public string FilenameTCX { get; set; }
        public int Index { get; set; }
        public string WorkoutImageSource
        {
            get
            {
                switch( (EventType) WorkoutType )
                {
                    case EventType.Walking:
                        return "Resources/walking.png";
                    case EventType.Running:
                        return "Resources/running.png";
                    case EventType.Biking:
                        return "Resources/biking.png";
                    default:
                        return "Resources/walking.png";
                }
            }
        }


        //--------------------------------------------------------------------------------------------------------------------
        public Visibility DownVisibility
        //--------------------------------------------------------------------------------------------------------------------
        {
            get
            {
                return ( Index > 0 ? Visibility.Visible : Visibility.Collapsed );
            }
        }

        
        //--------------------------------------------------------------------------------------------------------------------
        public Visibility UpVisibility
        //--------------------------------------------------------------------------------------------------------------------
        {
            get
            {
                return ( Parent != null && Index < Parent.Count - 1 ? Visibility.Visible : Visibility.Collapsed );
            }
        }


        private EventRegistrationTokenTable<EventHandler<TracksLoadedEventArgs>>
            m_currentWorkout = null;


        //--------------------------------------------------------------------------------------------------------------------
        public event EventHandler<TracksLoadedEventArgs> TracksLoaded
        //--------------------------------------------------------------------------------------------------------------------
        {
            add
            {
                EventRegistrationTokenTable<EventHandler<TracksLoadedEventArgs>>
                    .GetOrCreateEventRegistrationTokenTable( ref m_currentWorkout )
                    .AddEventHandler( value );
            }
            remove
            {
                EventRegistrationTokenTable<EventHandler<TracksLoadedEventArgs>>
                    .GetOrCreateEventRegistrationTokenTable( ref m_currentWorkout )
                    .RemoveEventHandler( value );
            }
        }

        //--------------------------------------------------------------------------------------------------------------------
        internal void OnTracksLoaded( WorkoutItem workout )
        //--------------------------------------------------------------------------------------------------------------------
        {
            EventHandler<TracksLoadedEventArgs> temp =
                EventRegistrationTokenTable<EventHandler<TracksLoadedEventArgs>>
                .GetOrCreateEventRegistrationTokenTable( ref m_currentWorkout )
                .InvocationList;
            if( temp != null )
            {
                temp( this, new TracksLoadedEventArgs( workout ) );
            }
        }

        //--------------------------------------------------------------------------------------------------------------------
        public WorkoutItem GetNextSibling()
        //--------------------------------------------------------------------------------------------------------------------
        {
            WorkoutItem item = null;
            if( Parent != null )
            {
                var matches = Parent.Where( ( workout ) => workout.Index == ( Index + 1 ) );
                if( matches.Count() == 1 )
                    item = matches.First();
            }
            return item;
        }

        //--------------------------------------------------------------------------------------------------------------------
        public WorkoutItem GetPrevSibling()
        //--------------------------------------------------------------------------------------------------------------------
        {
            WorkoutItem item = null;
            if( Parent != null )
            {
                var matches = Parent.Where( ( workout ) => workout.Index == ( Index - 1 ) );
                if( matches.Count() == 1 )
                    item = matches.First();
            }
            return item;
        }

        public ObservableCollection<WorkoutItem> Parent { get; set; }
        public ObservableCollection<DiagramData> HeartRateChart { get; private set; }
        public ObservableCollection<DiagramData> ElevationChart { get; private set; }
        public ObservableCollection<DiagramData> CadenceNormChart { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }


        //--------------------------------------------------------------------------------------------------------------------
        public async Task<bool> StoreWorkout()
        //--------------------------------------------------------------------------------------------------------------------
        {
            bool bResult = false;
            string dbpath = Path.Combine( ApplicationData.Current.LocalFolder.Path, "workouts.db" );

            using( SqliteConnection db = new SqliteConnection( $"Filename={dbpath}" ) )
            {
                await db.OpenAsync();
                bResult = ( await StoreWorkout( db ) != -1 );
            }
            return bResult;
        }


        //--------------------------------------------------------------------------------------------------------------------
        public async Task<long> StoreWorkout( SqliteConnection dbParam, 
                                              Action<UInt64, UInt64> Progress = null,
                                              ulong ulStepLength = 0 )
        //--------------------------------------------------------------------------------------------------------------------
        {
            long lResult = -1;
            if( dbParam != null )
            {
                long lastId = 0;

                await Task.Run( () =>
                {
                    SqliteCommand insertCommand = new SqliteCommand();
                    insertCommand.Connection = dbParam;

                    insertCommand.CommandText =
                        "INSERT INTO Workouts VALUES (NULL, @WorkoutType, @Title, @Notes, @Start, @End, " +
                        "@AvgHR, @MaxHR, @Calories, @AvgSpeed, @MaxSpeed, @DurationSec, @LongitudeStart, @LatitudeStart, " +
                        "@DistanceMeters, @LongDeltaRectSW, @LatDeltaRectSW, @LongDeltaRectNE, @LatDeltaRectNE);";

                    insertCommand.Parameters.AddWithValue( "@WorkoutType", WorkoutType );
                    insertCommand.Parameters.AddWithValue( "@Title", Title );
                    insertCommand.Parameters.AddWithValue( "@Notes", Notes );
                    insertCommand.Parameters.AddWithValue( "@Start", Start );
                    insertCommand.Parameters.AddWithValue( "@End", End );
                    insertCommand.Parameters.AddWithValue( "@AvgHR", AvgHR );
                    insertCommand.Parameters.AddWithValue( "@MaxHR", MaxHR );
                    insertCommand.Parameters.AddWithValue( "@Calories", Calories );
                    insertCommand.Parameters.AddWithValue( "@AvgSpeed", AvgSpeed );
                    insertCommand.Parameters.AddWithValue( "@MaxSpeed", MaxSpeed );
                    insertCommand.Parameters.AddWithValue( "@DurationSec", DurationSec );
                    insertCommand.Parameters.AddWithValue( "@LongitudeStart", LongitudeStart );
                    insertCommand.Parameters.AddWithValue( "@LatitudeStart", LatitudeStart );
                    insertCommand.Parameters.AddWithValue( "@DistanceMeters", DistanceMeters );
                    insertCommand.Parameters.AddWithValue( "@LongDeltaRectSW", LongDeltaRectSW );
                    insertCommand.Parameters.AddWithValue( "@LatDeltaRectSW", LatDeltaRectSW );
                    insertCommand.Parameters.AddWithValue( "@LongDeltaRectNE", LongDeltaRectNE );
                    insertCommand.Parameters.AddWithValue( "@LatDeltaRectNE", LatDeltaRectNE );

                    insertCommand.ExecuteReader();

                    SqliteCommand getRowIdCommand = new SqliteCommand();
                    getRowIdCommand.Connection = dbParam;
                    getRowIdCommand.CommandText = @"select last_insert_rowid()";
                    lastId = (long) getRowIdCommand.ExecuteScalar();

                    // assign workout ID to be able to load the related tracks
                    lResult = WorkoutId = (int)lastId;

                } );

                SqliteCommand insertTrackCommand = new SqliteCommand();
                insertTrackCommand.Connection = dbParam;
                insertTrackCommand.CommandText =
                    "INSERT INTO Tracks VALUES (NULL, @WorkoutId, @SecFromStart, @LongDelta, @LatDelta, @Elevation, " +
                    "@Heartrate, @Barometer, @Cadence, @SkinTemp, @GSR, @UVExposure);";

                foreach( var track in Items )
                {
                    await Task.Run( () =>
                    {
                        insertTrackCommand.Parameters.AddWithValue( "@WorkoutId", lastId );
                        insertTrackCommand.Parameters.AddWithValue( "@SecFromStart", track.SecFromStart );
                        insertTrackCommand.Parameters.AddWithValue( "@LongDelta", track.LongDelta );
                        insertTrackCommand.Parameters.AddWithValue( "@LatDelta", track.LatDelta );
                        insertTrackCommand.Parameters.AddWithValue( "@Elevation", track.Elevation );
                        insertTrackCommand.Parameters.AddWithValue( "@Heartrate", track.Heartrate );
                        insertTrackCommand.Parameters.AddWithValue( "@Barometer", track.Barometer );
                        insertTrackCommand.Parameters.AddWithValue( "@Cadence", track.Cadence );
                        insertTrackCommand.Parameters.AddWithValue( "@SkinTemp", track.SkinTemp );
                        insertTrackCommand.Parameters.AddWithValue( "@GSR", track.GSR );
                        insertTrackCommand.Parameters.AddWithValue( "@UVExposure", track.UV );

                        var reader = insertTrackCommand.ExecuteReader();

                        insertTrackCommand.Parameters.Clear();
                        reader.Close();
                    } );

                    if( Progress != null )
                        Progress( ulStepLength, 0 );
                }
            }
            return lResult;
        }


        //--------------------------------------------------------------------------------------------------------------------
        public static async Task<ObservableCollection<WorkoutItem>> ReadWorkouts()
        //--------------------------------------------------------------------------------------------------------------------
        {
            var workouts = new ObservableCollection<WorkoutItem>();

            string dbpath = Path.Combine( ApplicationData.Current.LocalFolder.Path, "workouts.db" );

            using( SqliteConnection db =
                new SqliteConnection( $"Filename={dbpath}" ) )
            {
                await db.OpenAsync();

                SqliteCommand readCommand = new SqliteCommand();
                readCommand.Connection = db;

                readCommand.CommandText = "SELECT * FROM Workouts ORDER BY Start DESC";
                int iIndex = 0;

                using( var reader = await readCommand.ExecuteReaderAsync() )
                {
                    while( await reader.ReadAsync() )
                    {
                        var workout = new WorkoutItem()
                        {
                            WorkoutId = reader.GetInt32( 0 ),
                            WorkoutType = reader.GetByte( 1 ),
                            Title = reader.GetString( 2 ),
                            Notes = reader.GetString( 3 ),
                            Start = reader.GetDateTime( 4 ).ToUniversalTime(),
                            End = reader.GetDateTime( 5 ).ToUniversalTime(),
                            AvgHR = reader.GetByte( 6 ),
                            MaxHR = reader.GetByte( 7 ),
                            Calories = reader.GetInt32( 8 ),
                            AvgSpeed = reader.GetInt32( 9 ),
                            MaxSpeed = reader.GetInt32( 10 ),
                            DurationSec = reader.GetInt32( 11 ),
                            LongitudeStart = reader.GetInt64( 12 ),
                            LatitudeStart = reader.GetInt64( 13 ),
                            DistanceMeters = reader.GetInt64( 14 ),
                            LongDeltaRectSW = reader.GetInt32( 15 ),
                            LatDeltaRectSW = reader.GetInt32( 16 ),
                            LongDeltaRectNE = reader.GetInt32( 17 ),
                            LatDeltaRectNE = reader.GetInt32( 18 ),
                            Items = new ObservableCollection<TrackItem>(),
                            Parent = workouts,
                            Index = iIndex++
                        };

                        string strWorkoutType = workout.WorkoutType == (byte) EventType.Running ? "Running" : ( workout.WorkoutType == (byte)EventType.Biking ? "Biking" : "Walking" );

                        // summary
                        if( workout.AvgHR > 0 )
                        {
                            if( workout.AvgHR <= 120 )
                                strWorkoutType = "Walking";
                            else if( workout.AvgHR < 140 )
                                strWorkoutType = "WarmUp";
                            else if( workout.AvgHR < 145 )
                                strWorkoutType = "Light";
                            else if( workout.AvgHR < 151 )
                                strWorkoutType = "Moderate";
                            else if( workout.AvgHR < 158 )
                                strWorkoutType = "Hard";
                            else
                                strWorkoutType = "Maximum";
                        }

                        double averageMinPerKm = (double)( (double)workout.AvgSpeed / (double)1000 );

                        workout.FilenameTCX =
                            workout.Start.Year.ToString( "D4" ) + workout.Start.Month.ToString( "D2" ) + workout.Start.Day.ToString( "D2" ) + "_" +
                            workout.Start.Hour.ToString( "D2" ) + workout.Start.Minute.ToString( "D2" ) + "_" +
                            strWorkoutType + "_" + ( (double)workout.DistanceMeters / 1000 ).ToString( "F2", CultureInfo.InvariantCulture ) + "_" +
                            averageMinPerKm.ToString( "F2", CultureInfo.InvariantCulture ) + "_" + workout.AvgHR.ToString( "F0" ) + ".tcx";

                        workouts.Add( workout );
                    }
                }
            }
            return workouts;
        }


        //--------------------------------------------------------------------------------------------------------------------
        public static async Task<WorkoutItem> ReadWorkoutFromTcx( string strTcxPath )
        //--------------------------------------------------------------------------------------------------------------------
        {
            WorkoutItem workout = null;
            StorageFile fileTcx = null;

            try
            {
                fileTcx = await StorageFile.GetFileFromPathAsync( strTcxPath );
            }
            catch( Exception )
            {
            }

            if( fileTcx != null )
            {
                try
                {
                    var tcx = new Tcx();
                    TrainingCenterDatabase_t tcxDatabase = tcx.AnalyzeTcxFile( strTcxPath );

                    if( tcxDatabase != null && tcxDatabase.Activities != null && tcxDatabase.Activities.Activity[0] != null &&
                        tcxDatabase.Activities.Activity[0].Lap[0] != null )
                    {
                        var startTime = tcxDatabase.Activities.Activity[0].Id;

                        int iTotalTimeSeconds = 0;
                        long lTotalDistanceMeters = 0;
                        int iTotalCalories = 0;
                        int iMaxHR = 0;
                        int iAvgHR = 0;
                        int iCount = 1;

                        foreach( var currentLap in tcxDatabase.Activities.Activity[0].Lap )
                        {
                            iTotalTimeSeconds += (int)currentLap.TotalTimeSeconds;
                            lTotalDistanceMeters += (long)currentLap.DistanceMeters;
                            iTotalCalories += (int)currentLap.Calories;
                            if( currentLap.MaximumHeartRateBpm != null )
                                iMaxHR = Math.Max( (int)iMaxHR, (int)currentLap.MaximumHeartRateBpm.Value );
                            if( currentLap.AverageHeartRateBpm != null )
                            {
                                iAvgHR += currentLap.AverageHeartRateBpm.Value;
                                iCount++;
                            }
                        }

                        workout =
                            new WorkoutItem()
                            {
                                WorkoutType = (byte)( tcxDatabase.Activities.Activity[0].Sport == Sport_t.Running ? EventType.Running : ( tcxDatabase.Activities.Activity[0].Sport == Sport_t.Biking ? EventType.Biking : EventType.Hike ) ),
                                Notes = tcxDatabase.Activities.Activity[0].Notes,
                                Start = startTime.ToUniversalTime(),
                                End = startTime.AddSeconds( iTotalTimeSeconds ).ToUniversalTime(),
                                MaxHR = (byte)iMaxHR,
                                Calories = iTotalCalories,
                                DurationSec = (int)iTotalTimeSeconds,
                                DistanceMeters = (long)lTotalDistanceMeters,
                                Items = new ObservableCollection<TrackItem>()
                            };

                        if( iAvgHR > 0 && iCount > 0 )
                            workout.AvgHR = (byte)( iAvgHR / iCount );

                        double averageMeterPerSecond = (double)workout.DistanceMeters / (double)workout.DurationSec;
                        double averageMinPerKm = ( 1000 / averageMeterPerSecond ) / 60;
                        var secDecimal = ( averageMinPerKm % 1 );
                        var seconds = 0.6 * secDecimal;
                        averageMinPerKm -= secDecimal;
                        averageMinPerKm += seconds;

                        workout.AvgSpeed = (int)Math.Ceiling( (double)( averageMinPerKm * 1000 ) );
                        string strWorkoutType = tcxDatabase.Activities.Activity[0].Sport == Sport_t.Running ? "Running" : ( tcxDatabase.Activities.Activity[0].Sport == Sport_t.Biking ? "Biking" : "Walking" );

                        // summary
                        if( workout.AvgHR > 0 )
                        {
                            if( workout.AvgHR <= 120 )
                                strWorkoutType = "Walking";
                            else if( workout.AvgHR < 140 )
                                strWorkoutType = "WarmUp";
                            else if( workout.AvgHR < 145 )
                                strWorkoutType = "Light";
                            else if( workout.AvgHR < 151 )
                                strWorkoutType = "Moderate";
                            else if( workout.AvgHR < 158 )
                                strWorkoutType = "Hard";
                            else
                                strWorkoutType = "Maximum";
                        }

                        if( tcxDatabase.Activities.Activity[0].Lap[0].MaximumSpeedSpecified )
                            workout.MaxSpeed = (int)tcxDatabase.Activities.Activity[0].Lap[0].MaximumSpeed;

                        workout.FilenameTCX =
                            startTime.Year.ToString( "D4" ) + startTime.Month.ToString( "D2" ) + startTime.Day.ToString( "D2" ) + "_" +
                            startTime.Hour.ToString( "D2" ) + startTime.Minute.ToString( "D2" ) + "_" +
                            strWorkoutType + "_" + ( (double)( workout.DistanceMeters / 1000 ) ).ToString( "F2", CultureInfo.InvariantCulture ) + "_" +
                            averageMinPerKm.ToString( "F2", CultureInfo.InvariantCulture ) + "_" + workout.AvgHR.ToString( "F0" ) + ".tcx";

                        workout.Title =
                            startTime.ToString( new CultureInfo( "en-US" ) ) + " " + strWorkoutType + " " +
                            ( (double)( workout.DistanceMeters ) / 1000 ).ToString( "F2", CultureInfo.InvariantCulture ) + " km " +
                            averageMinPerKm.ToString( "F2", CultureInfo.InvariantCulture ) + " min/km " + workout.AvgHR.ToString( "F0" ) + " bpm";

                        if( workout.Notes == null || workout.Notes.Length == 0 )
                            workout.Notes = "TCX import " + DateTime.Now.ToString( new CultureInfo( "en-US" ) );

                        int minLatDelta = 0;
                        int minLongDelta = 0;
                        int maxLatDelta = 0;
                        int maxLongDelta = 0;

                        foreach( var currentLap in tcxDatabase.Activities.Activity[0].Lap )
                        {
                            foreach( var trackpoint in currentLap.Track )
                            {
                                if( trackpoint.Position != null )
                                {
                                    var trackItem = new TrackItem();
                                    workout.Items.Add( trackItem );

                                    if( workout.LongitudeStart == 0 )
                                    {
                                        workout.LongitudeStart = (long)( trackpoint.Position.LongitudeDegrees * 10000000 );
                                        workout.LatitudeStart = (long)( trackpoint.Position.LatitudeDegrees * 10000000 );
                                        trackItem.LongDelta = 0;
                                        trackItem.LatDelta = 0;
                                    }
                                    else
                                    {
                                        trackItem.LongDelta = (int)( (long)( trackpoint.Position.LongitudeDegrees * 10000000 ) - workout.LongitudeStart );
                                        trackItem.LatDelta = (int)( (long)( trackpoint.Position.LatitudeDegrees * 10000000 ) - workout.LatitudeStart );
                                    }

                                    minLatDelta = Math.Min( minLatDelta, trackItem.LatDelta );
                                    minLongDelta = Math.Min( minLongDelta, trackItem.LongDelta );
                                    maxLatDelta = Math.Max( maxLatDelta, trackItem.LatDelta );
                                    maxLongDelta = Math.Max( maxLongDelta, trackItem.LongDelta );

                                    if( trackpoint.AltitudeMetersSpecified )
                                        trackItem.Elevation = (int)trackpoint.AltitudeMeters;
                                    if( trackpoint.CadenceSpecified )
                                        trackItem.Cadence = (byte)trackpoint.Cadence;
                                    if( trackpoint.HeartRateBpm != null )
                                        trackItem.Heartrate = (byte)trackpoint.HeartRateBpm.Value;

                                    if( workout.Start == DateTime.MinValue )
                                        workout.Start = trackpoint.Time.ToUniversalTime();

                                    trackItem.SecFromStart = (int)( trackpoint.Time.ToUniversalTime() - workout.Start ).TotalSeconds;
                                }
                            }
                        }

                        workout.LatDeltaRectNE = maxLatDelta;
                        workout.LatDeltaRectSW = minLatDelta;
                        workout.LongDeltaRectNE = maxLongDelta;
                        workout.LongDeltaRectSW = minLongDelta;
                    }
                }
                catch( Exception )
                {
                }
            }

            return workout;
        }


        //--------------------------------------------------------------------------------------------------------------------
        public async Task ReadTrackData( CancellationToken token )
        //--------------------------------------------------------------------------------------------------------------------
        {
            string dbpath = Path.Combine( ApplicationData.Current.LocalFolder.Path, "workouts.db" );

            await Task.Run( () =>
            {
                using( SqliteConnection db =
                    new SqliteConnection( $"Filename={dbpath}" ) )
                {
                    try
                    {
                        db.Open();

                        SqliteCommand readCommand = new SqliteCommand();
                        readCommand.Connection = db;
                        List<WorkoutItem> preloadItems = new List<WorkoutItem>();

                        if( Items != null && Items.Count > 0 )
                        {
                            OnTracksLoaded( this );
                        }
                        else
                        {
                            token.ThrowIfCancellationRequested();

                            readCommand.CommandText = "SELECT * FROM Tracks WHERE WorkoutId = $id";
                            readCommand.Parameters.AddWithValue( "$id", WorkoutId );
                            Items = new ObservableCollection<TrackItem>();

                            using( var reader = readCommand.ExecuteReader() )
                            {
                                const long numValues = 150;
                                var totalSeconds = ( End - Start ).TotalSeconds;
                                double currentSeconds = -1;
                                var dataPointSeconds = ( totalSeconds - currentSeconds ) / numValues;
                                int endElev = -1;
                                int maxCadence = 0;

                                var cadence = new List<DiagramData>();

                                HeartRateChart.Clear();
                                ElevationChart.Clear();
                                CadenceNormChart.Clear();

                                while( reader.Read() )
                                {
                                    var item = new TrackItem()
                                    {
                                        TrackId = reader.GetInt32( 0 ),
                                        WorkoutId = reader.GetInt32( 1 ),
                                        SecFromStart = reader.GetInt32( 2 ),
                                        LongDelta = reader.GetInt32( 3 ),
                                        LatDelta = reader.GetInt32( 4 ),
                                        Elevation = reader.GetInt32( 5 ),
                                        Heartrate = reader.GetByte( 6 ),
                                        Barometer = reader.GetInt32( 7 ),
                                        Cadence = reader.GetByte( 8 ),
                                        SkinTemp = reader.GetByte( 9 ),
                                        GSR = reader.GetInt32( 10 ),
                                        UV = reader.GetInt32( 11 )
                                    };

                                    if( endElev < 0 )
                                        endElev = item.Elevation - ( MaxHR / 2 );
                                    if( currentSeconds < 0 )
                                        currentSeconds = (double)item.SecFromStart;

                                    Items.Add( item );

                                    if( (double)item.SecFromStart >= currentSeconds )
                                    {
                                        var min = (double)( (double)item.SecFromStart / (double)60 );
                                        HeartRateChart.Add(
                                            new DiagramData()
                                            {
                                                Min = min,
                                                Value = item.Heartrate
                                            } );
                                        ElevationChart.Add(
                                            new DiagramData()
                                            {
                                                Min = min,
                                                Value = Math.Max( 0, item.Elevation - endElev )
                                            } );
                                        cadence.Add(
                                            new DiagramData()
                                            {
                                                Min = min,
                                                Value = item.Cadence
                                            } );
                                        maxCadence = Math.Max( maxCadence, item.Cadence );
                                        currentSeconds += dataPointSeconds;
                                    }
                                }

                                if( maxCadence > 0 )
                                {
                                    var multiply = (double)( maxCadence > 0 ? ( (double)MaxHR / (double)( 2 * maxCadence ) ) : 1 );
                                    foreach( var currCadence in cadence )
                                    {
                                        token.ThrowIfCancellationRequested();
                                        CadenceNormChart.Add( new DiagramData()
                                        {
                                            Min = currCadence.Min,
                                            Value = currCadence.Value * multiply
                                        } );
                                    }
                                }
                                OnTracksLoaded( this );
                            }
                            readCommand.Parameters.Clear();
                        }
                    }
                    catch( Exception )
                    {
                        OnTracksLoaded( this );
                    }
                }
            } );
        }


        //------------------------------------------------------------------------------------------------------
        public async Task<string> GenerateTcxBuffer()
        //------------------------------------------------------------------------------------------------------
        {
            string strResult = "";
            if( Items.Count <= 0 )
                return strResult;

            ExportType type = ExportType.HeartRate | ExportType.Temperature | ExportType.Cadence | ExportType.GalvanicSkinResponse;

            await Task.Run( () =>
            {
                try
                {
                    var tcx = new Tcx();
                    XmlDocument doc = new XmlDocument();

                    if( ( (EventType)WorkoutType == EventType.Running || (EventType)WorkoutType == EventType.Hike || (EventType)WorkoutType == EventType.Sleeping ||
                            (EventType)WorkoutType == EventType.Biking ) && Items.Count > 0 )
                    {
                        ExportType supportedType = type;
                        switch( (EventType)WorkoutType )
                        {
                            case EventType.Hike:
                            case EventType.Running:
                                supportedType &= ExportType.HeartRate | ExportType.Temperature | ExportType.Cadence | ExportType.GalvanicSkinResponse;
                                break;
                            case EventType.Biking:
                                supportedType &= ExportType.HeartRate | ExportType.Temperature | ExportType.GalvanicSkinResponse;
                                break;
                            default:
                                supportedType &= ExportType.HeartRate;
                                break;
                        }

                        TrainingCenterDatabase_t tcxDatabase = new TrainingCenterDatabase_t();
                        tcxDatabase.Activities = new ActivityList_t();
                        tcxDatabase.Activities.Activity = new Activity_t[1];
                        tcxDatabase.Activities.Activity[0] = new Activity_t();

                        tcxDatabase.Activities.Activity[0].Id = Start;
                        tcxDatabase.Activities.Activity[0].Notes = Notes;
                        tcxDatabase.Activities.Activity[0].Sport = ( (EventType)WorkoutType == EventType.Biking ? Sport_t.Biking : Sport_t.Running );

                        tcxDatabase.Activities.Activity[0].Lap = new ActivityLap_t[1];
                        tcxDatabase.Activities.Activity[0].Lap[0] = new ActivityLap_t();

                        double averageMeterPerSecond = 0;
                        string strWorkoutType;
                        tcxDatabase.Activities.Activity[0].Sport = ( (EventType)WorkoutType == EventType.Biking ? Sport_t.Biking : Sport_t.Running );

                        // summary
                        if( AvgHR != 0 )
                        {
                            if( AvgHR <= 120 )
                            {
                                tcxDatabase.Activities.Activity[0].Sport = Sport_t.Other;
                                strWorkoutType = "Walking";
                            }
                            else if( AvgHR < 140 )
                                strWorkoutType = "WarmUp";
                            else if( AvgHR < 145 )
                                strWorkoutType = "Light";
                            else if( AvgHR < 151 )
                                strWorkoutType = "Moderate";
                            else if( AvgHR < 158 )
                                strWorkoutType = "Hard";
                            else
                                strWorkoutType = "Maximum";

                            if( ( type & ExportType.HeartRate ) == ExportType.HeartRate )
                            {
                                tcxDatabase.Activities.Activity[0].Lap[0].AverageHeartRateBpm = new HeartRateInBeatsPerMinute_t();
                                tcxDatabase.Activities.Activity[0].Lap[0].AverageHeartRateBpm.Value = AvgHR;
                                tcxDatabase.Activities.Activity[0].Lap[0].MaximumHeartRateBpm = new HeartRateInBeatsPerMinute_t();
                                tcxDatabase.Activities.Activity[0].Lap[0].MaximumHeartRateBpm.Value = MaxHR;
                            }
                        }
                        else
                        {
                            strWorkoutType = ( (EventType)WorkoutType == EventType.Biking ? "Biking" : ( (EventType)WorkoutType == EventType.Running ? "Running" : "Walking" ) );
                        }

                        tcxDatabase.Activities.Activity[0].Lap[0].MaximumSpeed = MaxSpeed;
                        tcxDatabase.Activities.Activity[0].Lap[0].MaximumSpeedSpecified = true;
                        tcxDatabase.Activities.Activity[0].Lap[0].TotalTimeSeconds = DurationSec;
                        tcxDatabase.Activities.Activity[0].Lap[0].Calories = (ushort)Calories;
                        tcxDatabase.Activities.Activity[0].Lap[0].DistanceMeters = DistanceMeters;
                        tcxDatabase.Activities.Activity[0].Lap[0].Intensity = Intensity_t.Active;

                        averageMeterPerSecond = (double)DistanceMeters / (double)DurationSec;
                        double averageMinPerKm = ( 1000 / averageMeterPerSecond ) / 60;
                        var secDecimal = ( averageMinPerKm % 1 );
                        var seconds = 0.6 * secDecimal;
                        averageMinPerKm -= secDecimal;
                        averageMinPerKm += seconds;

                        FilenameTCX =
                            Start.Year.ToString( "D4" ) + Start.Month.ToString( "D2" ) + Start.Day.ToString( "D2" ) + "_" +
                            Start.Hour.ToString( "D2" ) + Start.Minute.ToString( "D2" ) + "_" +
                            strWorkoutType + "_" + ( (double)DistanceMeters / 1000 ).ToString( "F2", CultureInfo.InvariantCulture ) + "_" +
                            averageMinPerKm.ToString( "F2", CultureInfo.InvariantCulture ) + "_" + AvgHR.ToString( "F0" ) + ".tcx";

                        tcxDatabase.Activities.Activity[0].Lap[0].StartTime = Start;
                        tcxDatabase.Activities.Activity[0].Lap[0].TriggerMethod = TriggerMethod_t.Manual;

                        tcxDatabase.Activities.Activity[0].Lap[0].Track = new Trackpoint_t[Items.Count];

                        int iIndex = 0;
                        foreach( var trackPoint in Items )
                        {
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex] = new Trackpoint_t();
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Time = Start.AddSeconds( trackPoint.SecFromStart );
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].SensorState = SensorState_t.Present;
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].SensorStateSpecified = true;

                            // heart rate
                            if( ( type & ExportType.HeartRate ) == ExportType.HeartRate )
                            {
                                tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].HeartRateBpm = new HeartRateInBeatsPerMinute_t();
                                tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].HeartRateBpm.Value = (byte)trackPoint.Heartrate;
                            }

                            // cadence
                            if( ( type & ExportType.Cadence ) == ExportType.Cadence && (EventType)WorkoutType != EventType.Biking )
                            {
                                tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Cadence = (byte)trackPoint.Cadence;
                                tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].CadenceSpecified = true;
                            }

                            // elevation
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].AltitudeMeters = trackPoint.Elevation;
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].AltitudeMetersSpecified = true;

                            // GPS point
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Position = new Position_t();

                            double latitude = (double)( (double)( LatitudeStart + trackPoint.LatDelta ) / 10000000 );
                            double longitude = (double)( (double)( LongitudeStart + trackPoint.LongDelta ) / 10000000 );
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Position.LatitudeDegrees = latitude;
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Position.LongitudeDegrees = longitude;

                            iIndex++;
                        }
                        string strBuffer = tcx.GenerateTcx( tcxDatabase );
                        if( strBuffer != null && strBuffer.Length > 0 )
                            strResult = strBuffer.Replace( "\"utf-16\"", "\"UTF-8\"" );
                    }
                }
                catch( Exception ex )
                {
                    strResult = "";
                }
            } );

            return strResult;
        }
    }

    public class TracksLoadedEventArgs : EventArgs
    {
        public WorkoutItem Workout { get; private set; }

        public TracksLoadedEventArgs( WorkoutItem workout )
            : base()
        {
            this.Workout = workout;
        }
    }

    /// <summary>
    /// Creates a collection of workouts and items with content read from a database or TCX file.
    /// 
    /// WorkoutDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class WorkoutDataSource
    {
        private const string WorkoutDbName = "workouts.db";
        private const string WorkoutFolderName = "Workouts";
        private const string WorkoutDbFolderName = "WorkoutDB";
        private static WorkoutDataSource _sampleDataSource = new WorkoutDataSource();

        private ObservableCollection<WorkoutItem> _workouts = new ObservableCollection<WorkoutItem>();
        public ObservableCollection<WorkoutItem> Workouts
        {
            get { return this._workouts; }
            set { this._workouts = value; }
        }

        public StorageFolder WorkoutsFolder { get; private set; }
        public static WorkoutDataSource DataSource { get { return _sampleDataSource; } }
        public StorageFolder DatabaseFolder { get; private set; }
        public SensorLog SensorLogEngine { get; private set; }
        public static bool DbInitialized { get; private set; }
        public StorageFolder WorkoutDbFolder { get; private set; }


        //--------------------------------------------------------------------------------------------------------------------
        public WorkoutDataSource()
        //--------------------------------------------------------------------------------------------------------------------
        {
            PreloadTracks += WorkoutTracks_Preload;
        }


        //--------------------------------------------------------------------------------------------------------------------
        public static async Task<IEnumerable<WorkoutItem>> GetWorkoutsAsync( bool bForceReload = false )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( !DbInitialized )
                DbInitialized = await _sampleDataSource.InitDatabase( /* true */ );

            await _sampleDataSource.GetSampleDataAsync( bForceReload );

            return _sampleDataSource.Workouts;
        }


        public static ObservableCollection<WorkoutItem> WorkoutList { get { return _sampleDataSource.Workouts; } }

        //--------------------------------------------------------------------------------------------------------------------
        public static async Task<List<WorkoutItem>> ImportFromSensorlog( StorageFolder sensorLogFolder,
                                                                         Action<string> Status,
                                                                         Action<UInt64, UInt64> Progress )
        //--------------------------------------------------------------------------------------------------------------------
        {
            var workouts = await _sampleDataSource.ReadWorkoutsFromSensorLogs( sensorLogFolder );
            if( workouts != null && workouts.Count > 0 )
            {
                var iTrackCount = 0;
                foreach( var workout in workouts )
                    if( workout.TrackPoints != null )
                        iTrackCount += workout.TrackPoints.Count;

                ulong stepLength = (ulong)_sampleDataSource.SensorLogEngine.BufferSize / (ulong)iTrackCount;
                _sampleDataSource.SensorLogEngine.StepLength = stepLength;

                return await _sampleDataSource.AddWorkouts( workouts, false, Status, Progress, stepLength );
            }
            return new List<WorkoutItem>();
        }

        //--------------------------------------------------------------------------------------------------------------------
        public static async Task<List<WorkoutItem>> ImportFromSensorlog( byte[] btSensorLog,
                                                                         Action<string> Status,
                                                                         Action<UInt64, UInt64> Progress )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( !DbInitialized )
                DbInitialized = await _sampleDataSource.InitDatabase();

            var workouts = await _sampleDataSource.ReadWorkoutsFromSensorLogBuffer( btSensorLog );
            if( workouts != null && workouts.Count > 0 )
            {
                var iTrackCount = 0;
                foreach( var workout in workouts )
                    if( workout.TrackPoints != null )
                        iTrackCount += workout.TrackPoints.Count;

                ulong stepLength = (ulong)_sampleDataSource.SensorLogEngine.BufferSize / (ulong)iTrackCount;
                DataSource.SensorLogEngine.StepLength = stepLength;

                return await _sampleDataSource.AddWorkouts( workouts, false, Status, Progress, stepLength );
            }
            return new List<WorkoutItem>();
        }

        //--------------------------------------------------------------------------------------------------------------------
        public async Task<bool> InitDatabase( bool bDeleteOldDb = false )
        //--------------------------------------------------------------------------------------------------------------------
        {
            WorkoutsFolder = await KnownFolders.DocumentsLibrary.CreateFolderAsync( WorkoutFolderName, CreationCollisionOption.OpenIfExists );
            WorkoutDbFolder = await KnownFolders.DocumentsLibrary.CreateFolderAsync( WorkoutDbFolderName, CreationCollisionOption.OpenIfExists );
            DatabaseFolder = ApplicationData.Current.LocalFolder;
            if( WorkoutsFolder == null || DatabaseFolder == null )
                return false;

            try
            {
                var oldDb = await DatabaseFolder.GetFileAsync( WorkoutDbName );
                if( oldDb != null && bDeleteOldDb )
                    await oldDb.DeleteAsync();
            }
            catch
            {
                // look if there is a database in the WorkoutDB folder
                try
                {
                    var databaseFile = await WorkoutDbFolder.GetFileAsync( WorkoutDbName );
                    if( databaseFile != null && !bDeleteOldDb )
                    {
                        await databaseFile.CopyAsync( DatabaseFolder );
                        return true;
                    }
                }
                catch
                {
                    // ok, we need to create a new file
                }
            }

            await DatabaseFolder.CreateFileAsync( WorkoutDbName, CreationCollisionOption.OpenIfExists );
            string dbpath = Path.Combine( DatabaseFolder.Path, WorkoutDbName );
            using( SqliteConnection db =
               new SqliteConnection( $"Filename={dbpath}" ) )
            {
                await db.OpenAsync();

                try
                {
                    String tableCommand =
                        "CREATE TABLE IF NOT EXISTS Tracks (" +
                        "TrackId INTEGER PRIMARY KEY AUTOINCREMENT, " + "WorkoutId INTEGER NOT NULL, " +
                        "SecFromStart INT, " + "LongDelta INT, " + "LatDelta INT, " + "Elevation INT, " + "Heartrate TINYINT, " + "Barometer INT, " +
                        "Cadence TINYINT, " + "SkinTemp TINYINT, " + "GSR INT, " + "UVExposure INT )";

                    SqliteCommand createTrackTable = new SqliteCommand( tableCommand, db );
                    var reader = await createTrackTable.ExecuteReaderAsync();
                    reader.Close();

                    /*
                    - SleepId (PK) (guid)
                    - SecFromStart (int16)
                    - SegmentType (Byte) - Unknown, Run, FreePlay, Doze, Sleep, Snooze, Awake, GuidedWorkout, Bike, Pause, 
                                           Resume, DistanceBasedInterval, TimeBasedInterval, GolfHole, GolfShot, NotWorn, Hike
                    - SleepType (Byte) - Unknown, UndifferentiatedSleep, RestlessSleep, RestfulSleep
                    - Heartrate (byte)
                    */

                    tableCommand =
                        "CREATE TABLE IF NOT EXISTS Sleep (" +
                        "SleepId INTEGER PRIMARY KEY AUTOINCREMENT, " + "SleepActivityId INTEGER NOT NULL, " +
                        "SecFromStart INT, " + "SegmentType TINYINT, " + "SleepType TINYINT, " + "Heartrate TINYINT )";

                    SqliteCommand createSleepTable = new SqliteCommand( tableCommand, db );
                    reader = await createSleepTable.ExecuteReaderAsync();
                    reader.Close();

                    tableCommand =
                        "CREATE TABLE IF NOT EXISTS Workouts (" +
                        "WorkoutId INTEGER PRIMARY KEY AUTOINCREMENT, " + "WorkoutType TINYINT, " +
                        "Title NVARCHAR(128) NULL, " + "Notes NVARCHAR(2048) NULL, " + "Start DATETIME, " + "End DATETIME, " + "AvgHR TINYINT, " +
                        "MaxHR TINYINT, " + "Calories INT, " + "AvgSpeed INT, " + "MaxSpeed INT, " + "DurationSec INT, " + "LongitudeStart INT8, " +
                        "LatitudeStart INT8, " + "DistanceMeters INT8, " + "LongDeltaRectSW INT, " + "LatDeltaRectSW INT, " + "LongDeltaRectNE INT, " +
                        "LatDeltaRectNE INT )";

                    SqliteCommand createWorkoutTable = new SqliteCommand( tableCommand, db );
                    reader = await createWorkoutTable.ExecuteReaderAsync();
                    reader.Close();

                    tableCommand =
                        "CREATE TABLE IF NOT EXISTS SleepActivity (" +
                        "SleepActivityId INTEGER PRIMARY KEY AUTOINCREMENT, " + "Title NVARCHAR(1024) NULL, " +
                        "Start DATETIME, " + "End DATETIME, " + "AvgHR TINYINT, " + "Calories INT, " + "AwakeDuration INT, " +
                        "SleepDuration INT, " + "NumberOfWakeups TINYINT, " + "FallAsleepDuration INT, " +
                        "SleepEfficiencyPercentage TINYINT, " + "TotalRestlessSleepDuration INT, " +
                        "RestingHeartRate TINYINT, " + "FallAsleepTime INT, " + "WakeupTime INT )";

                    SqliteCommand createSleepActivityTable = new SqliteCommand( tableCommand, db );
                    reader = await createSleepActivityTable.ExecuteReaderAsync();
                    reader.Close();
                }
                catch( Exception ex )
                {

                }
            }
            return true;
        }


        //--------------------------------------------------------------------------------------------------------------------
        public static async Task<List<long>> StoreWorkouts( List<WorkoutItem> workouts,
                                                            Action<UInt64, UInt64> Progress = null,
                                                            ulong ulStepLength = 0 )
        //--------------------------------------------------------------------------------------------------------------------
        {
            List<long> listResult = new List<long>();
            string dbpath = Path.Combine( ApplicationData.Current.LocalFolder.Path, "workouts.db" );

            using( SqliteConnection db = new SqliteConnection( $"Filename={dbpath}" ) )
            {
                await db.OpenAsync();

                foreach( var workout in workouts )
                {
                    listResult.Add( await workout.StoreWorkout( db, Progress, ulStepLength ) );
                }
            }
            return listResult;
        }




        //--------------------------------------------------------------------------------------------------------------------
        public static async Task<bool> BackupDatabase( StorageFolder targetFolder = null )
        //--------------------------------------------------------------------------------------------------------------------
        {
            bool bResult = false;
            StorageFile database = await ApplicationData.Current.LocalFolder.GetFileAsync( WorkoutDbName );

            if( database != null )
            {
                if( targetFolder == null )
                    targetFolder = await KnownFolders.DocumentsLibrary.CreateFolderAsync( WorkoutDbFolderName, CreationCollisionOption.OpenIfExists );

                bResult = ( await database.CopyAsync( targetFolder, database.Name, NameCollisionOption.ReplaceExisting ) != null );
            }
            return bResult;
        }


        //--------------------------------------------------------------------------------------------------------------------
        public async Task<List<Workout>> ReadWorkoutsFromSensorLogBuffer( byte[] btSensorLog )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( SensorLogEngine == null || btSensorLog == null )
                return null;

            try
            {
                using( MemoryStream memStream = new MemoryStream( btSensorLog ) )
                {
                    SensorLogEngine.BufferSize = (ulong) btSensorLog.Length;
                    await SensorLogEngine.Read( memStream );
                }
            }
            catch( Exception )
            {
            }
            try
            {
                // create workouts
                if( SensorLogEngine.Sequences.Count > 0 )
                {
                    return
                        await SensorLogEngine.CreateWorkouts(
                            ExportType.HeartRate | ExportType.Cadence | ExportType.Temperature |
                            ExportType.GalvanicSkinResponse );
                }
            }
            catch( Exception ex )
            {
            }
            return null;
        }


        //--------------------------------------------------------------------------------------------------------------------
        public async Task<List<Workout>> ReadWorkoutsFromSensorLogs( StorageFolder SensorLogFolder )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( SensorLogEngine == null || SensorLogFolder == null )
                return null;

            var strDevicePath = SensorLogFolder.Path;
            var strTempDir = strDevicePath;
            SensorLogEngine.BufferSize = 0;

            try
            {
                Dictionary<DateTime, string> dictFiles = new Dictionary<DateTime, string>();

                IReadOnlyList<StorageFile> dataFiles = await SensorLogFolder.GetFilesAsync();

                foreach( StorageFile file in dataFiles )
                {
                    BasicProperties filesize = await file.GetBasicPropertiesAsync();
                    SensorLogEngine.BufferSize += filesize.Size;

                    string currentFile = file.Path;
                    string fileName = currentFile.Substring( strTempDir.Length + 1 );
                    var iMilliSeconds = Int32.Parse( fileName.Substring( fileName.Length - 12, 3 ) );
                    var iSeconds = Int32.Parse( fileName.Substring( fileName.Length - 14, 2 ) );
                    var iMinutes = Int32.Parse( fileName.Substring( fileName.Length - 16, 2 ) );
                    var iHours = Int32.Parse( fileName.Substring( fileName.Length - 18, 2 ) );
                    var iDay = Int32.Parse( fileName.Substring( fileName.Length - 20, 2 ) );
                    var iMonth = Int32.Parse( fileName.Substring( fileName.Length - 22, 2 ) );
                    var iYear = Int32.Parse( fileName.Substring( fileName.Length - 24, 2 ) );

                    DateTime dtFile = new DateTime( iYear, iMonth, iDay, iHours, iMinutes, iSeconds );
                    dictFiles.Add( dtFile, fileName );
                }

                var dateTimesAscending = dictFiles.Keys.OrderBy( d => d );
                foreach( var dtCurrent in dateTimesAscending )
                {
                    var strFileName = dictFiles[dtCurrent];

                    using( var fileStream = await SensorLogFolder.OpenStreamForReadAsync( strFileName ) )
                    {
                        await SensorLogEngine.Read( fileStream );
                    }
                }
            }
            catch( Exception ex )
            {
            }

            try
            {
                // create workouts
                if( SensorLogEngine.Sequences.Count > 0 )
                {
                    return 
                        await SensorLogEngine.CreateWorkouts( 
                            ExportType.HeartRate | ExportType.Cadence | ExportType.Temperature | 
                            ExportType.GalvanicSkinResponse );
                }
            }
            catch( Exception ex )
            {
            }
            return null;
        }

        
        //--------------------------------------------------------------------------------------------------------------------
        public async Task<List<WorkoutItem>> AddWorkouts( List<Workout> Workouts, 
                                                          bool bAddToDb = false,
                                                          Action<string> Status = null,
                                                          Action<UInt64, UInt64> Progress = null,
                                                          ulong ulStepLength = 0 )
        //--------------------------------------------------------------------------------------------------------------------
        {
            List<WorkoutItem> listWorkouts = new List<WorkoutItem>();
            bool bResult = Workouts.Count > 0;
            ExportType type = ExportType.HeartRate | ExportType.Temperature | ExportType.Cadence | ExportType.GalvanicSkinResponse;

            try
            {
                string dbpath = Path.Combine( ApplicationData.Current.LocalFolder.Path, WorkoutDbName );

                int iIndex = 0;
                foreach( var workout in Workouts )
                {
                    int minLatDelta = 0;
                    int minLongDelta = 0;
                    int maxLatDelta = 0;
                    int maxLongDelta = 0;

                    WorkoutItem workoutData =
                        new WorkoutItem()
                        {
                            WorkoutType = (byte)workout.Type,
                            LongDeltaRectSW = 0,
                            LatDeltaRectSW = 0,
                            LongDeltaRectNE = 0,
                            LatDeltaRectNE = 0,
                            Items = new ObservableCollection<TrackItem>(),
                            DbPath = dbpath,
                            Parent = WorkoutList,
                            Index = iIndex++
                        };
                    WorkoutList.Add( workoutData );

                    if( ( workout.Type == EventType.Running || workout.Type == EventType.Hike || workout.Type == EventType.Walking || 
                          workout.Type == EventType.Sleeping || workout.Type == EventType.Biking ) && workout.TrackPoints.Count > 0 )
                    {
                        ExportType supportedType = type;
                        switch( workout.Type )
                        {
                            case EventType.Hike:
                            case EventType.Running:
                            case EventType.Walking:
                                supportedType &= ExportType.HeartRate | ExportType.Temperature | ExportType.Cadence | ExportType.GalvanicSkinResponse;
                                break;
                            case EventType.Biking:
                                supportedType &= ExportType.HeartRate | ExportType.Temperature | ExportType.GalvanicSkinResponse;
                                break;
                            default:
                                supportedType &= ExportType.HeartRate;
                                break;
                        }

                        workoutData.Start = workout.StartTime;
                        workoutData.End = workout.EndTime;
                        workoutData.Notes = workout.Notes;
                        workoutData.WorkoutType = (byte)workout.Type;

                        // summary
                        if( workout.Summary != null )
                        {
                            double averageMeterPerSecond = 0;
                            string strWorkoutType;

                            if( workout.Type == EventType.Biking )
                            {
                                strWorkoutType = "Biking";
                            }
                            else if( workout.Type == EventType.Hike )
                            {
                                strWorkoutType = "Hiking";
                            }
                            else
                            {
                                if( workout.Summary.HFAverage <= 120 )
                                {
                                    workout.Type = EventType.Walking;
                                    strWorkoutType = "Walking";
                                }
                                else if( workout.Summary.HFAverage < 140 )
                                    strWorkoutType = "WarmUp run";
                                else if( workout.Summary.HFAverage < 145 )
                                    strWorkoutType = "Light run";
                                else if( workout.Summary.HFAverage < 151 )
                                    strWorkoutType = "Moderate run";
                                else if( workout.Summary.HFAverage < 160 )
                                    strWorkoutType = "Hard run";
                                else
                                    strWorkoutType = "Maximum run";
                            }

                            averageMeterPerSecond = workout.Summary.Distance / workout.Summary.Duration;
                            double averageMinPerKm = ( 1000 / averageMeterPerSecond ) / 60;
                            var secDecimal = ( averageMinPerKm % 1 );
                            var seconds = 0.6 * secDecimal;
                            averageMinPerKm -= secDecimal;
                            averageMinPerKm += seconds;

                            // Speed = min/km * 1000
                            workoutData.AvgSpeed = (int)Math.Ceiling( (double)( averageMinPerKm * 1000 ) );
                            workoutData.MaxSpeed = (int)workout.Summary.MaximumSpeed;
                            workoutData.Calories = (int)workout.Summary.CaloriesBurned;
                            workoutData.DurationSec = (int)workout.Summary.Duration;
                            workoutData.AvgHR = (byte)workout.Summary.HFAverage;
                            workoutData.MaxHR = (byte)workout.Summary.HFMax;
                            workoutData.DistanceMeters = (long)workout.Summary.Distance;

                            workoutData.Title =
                                workout.StartTime.ToString( new CultureInfo( "en-US" ) ) + " " + strWorkoutType + " " +
                                ( (double) ( ( (double) workoutData.DistanceMeters ) / (double) 1000 ) ).ToString( "F2", CultureInfo.InvariantCulture ) + " km " +
                                averageMinPerKm.ToString( "F2", CultureInfo.InvariantCulture ) + " min/km " + workoutData.AvgHR.ToString( "F0" ) + " bpm";

                            if( workoutData.Notes == null || workoutData.Notes.Length == 0 )
                                workoutData.Notes = "Sensor log import " + DateTime.Now.ToString( new CultureInfo( "en-US" ) );
                        }
                        else
                        {
                            workoutData.Title =
                                workout.StartTime.ToString( new CultureInfo( "en-US" ) ) + " Workout";
                        }

                        foreach( var trackPoint in workout.TrackPoints )
                        {
                            /*
                            public int Barometer { get; set; }
                            public int SkinTemp { get; set; }
                            public int GSR { get; set; }
                            public int UV { get; set; }
                            */

                            TrackItem trackData = new TrackItem() { WorkoutId = workoutData.WorkoutId };

                            trackData.SecFromStart = (int)( trackPoint.Time - workoutData.Start ).TotalSeconds;
                            trackData.Heartrate = (byte)trackPoint.HeartRateBpm;
                            trackData.Elevation = (int)trackPoint.Elevation;
                            trackData.Cadence = (byte)trackPoint.Cadence;

                            if( workoutData.LongitudeStart == 0 )
                            {
                                workoutData.LongitudeStart = (long)( trackPoint.Position.LongitudeDegrees * 10000000 );
                                trackData.LongDelta = 0;
                            }
                            else
                                trackData.LongDelta = (int)( ( trackPoint.Position.LongitudeDegrees * 10000000 ) - workoutData.LongitudeStart );

                            if( workoutData.LatitudeStart == 0 )
                            {
                                workoutData.LatitudeStart = (long)( trackPoint.Position.LatitudeDegrees * 10000000 );
                                trackData.LatDelta = 0;
                            }
                            else
                                trackData.LatDelta = (int)( ( trackPoint.Position.LatitudeDegrees * 10000000 ) - workoutData.LatitudeStart );

                            workoutData.Items.Add( trackData );

                            minLatDelta = Math.Min( minLatDelta, trackData.LatDelta );
                            minLongDelta = Math.Min( minLongDelta, trackData.LongDelta );
                            maxLatDelta = Math.Max( maxLatDelta, trackData.LatDelta );
                            maxLongDelta = Math.Max( maxLongDelta, trackData.LongDelta );


                        }
                    }

                    workoutData.LatDeltaRectNE = maxLatDelta;
                    workoutData.LatDeltaRectSW = minLatDelta;
                    workoutData.LongDeltaRectNE = maxLongDelta;
                    workoutData.LongDeltaRectSW = minLongDelta;

                    if( workoutData != null && bAddToDb )
                        bResult = await workoutData.StoreWorkout();

                    listWorkouts.Add( workoutData );
                }
            }
            catch( Exception ex )
            {
            }
            return listWorkouts;
        }

        //--------------------------------------------------------------------------------------------------------------------
        public static async Task<WorkoutItem> GetWorkoutAsync( int workoutId )
        //--------------------------------------------------------------------------------------------------------------------
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Workouts.Where( ( workout ) => workout.WorkoutId == workoutId );
            if( matches.Count() > 0 )
            {
                var workoutItem = matches.First();
                //_sampleDataSource.OnPreloadTracks( workoutItem );
                return workoutItem;
            }
            return null;
        }


        //--------------------------------------------------------------------------------------------------------------------
        public static async Task<TrackItem> GetItemAsync( string uniqueId )
        //--------------------------------------------------------------------------------------------------------------------
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Workouts.SelectMany( workout => workout.Items ).Where( ( item ) => item.UniqueId.Equals( uniqueId ) );
            if( matches.Count() == 1 ) return matches.First();
            return null;
        }


        //--------------------------------------------------------------------------------------------------------------------
        private async Task GetSampleDataAsync( bool bForceReload = false )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( !bForceReload && Workouts.Count != 0 )
                return;

            if( bForceReload )
                Workouts.Clear();

            if( SensorLogEngine == null )
                SensorLogEngine = new SensorLog();

            // load workouts from the database
            Workouts = await WorkoutItem.ReadWorkouts();
        }

        private EventRegistrationTokenTable<EventHandler<TracksLoadedEventArgs>>
            m_currentWorkout = null;

        //--------------------------------------------------------------------------------------------------------------------
        public event EventHandler<TracksLoadedEventArgs> PreloadTracks
        //--------------------------------------------------------------------------------------------------------------------
        {
            add
            {
                EventRegistrationTokenTable<EventHandler<TracksLoadedEventArgs>>
                    .GetOrCreateEventRegistrationTokenTable( ref m_currentWorkout )
                    .AddEventHandler( value );
            }
            remove
            {
                EventRegistrationTokenTable<EventHandler<TracksLoadedEventArgs>>
                    .GetOrCreateEventRegistrationTokenTable( ref m_currentWorkout )
                    .RemoveEventHandler( value );
            }
        }

        //--------------------------------------------------------------------------------------------------------------------
        internal void OnPreloadTracks( WorkoutItem workout )
        //--------------------------------------------------------------------------------------------------------------------
        {
            EventHandler<TracksLoadedEventArgs> temp =
                EventRegistrationTokenTable<EventHandler<TracksLoadedEventArgs>>
                .GetOrCreateEventRegistrationTokenTable( ref m_currentWorkout )
                .InvocationList;
            if( temp != null )
            {
                temp( this, new TracksLoadedEventArgs( workout ) );
            }
        }


        //--------------------------------------------------------------------------------------------------------------------
        private async void WorkoutTracks_Preload( object sender, TracksLoadedEventArgs e )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( e.Workout != null )
            {
            }
        }
    }

    /// <summary>
    /// Track item data model.
    /// </summary>
    public class TrackItem
    {
        public TrackItem( String uniqueId, String title, String subtitle, String imagePath, String description, String content )
        {
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Content = content;
        }

        public TrackItem()
        {
        }

        public string UniqueId { get { return TrackId.ToString( "B" ); } }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Content { get; private set; }

        public int TrackId { get; set; }
        public int WorkoutId { get; set; }
        public int SecFromStart { get; set; }
        public int LongDelta { get; set; }
        public int LatDelta { get; set; }
        public int Elevation { get; set; }
        public byte Heartrate { get; set; }
        public int Barometer { get; set; }
        public byte Cadence { get; set; }
        public byte SkinTemp { get; set; }
        public int GSR { get; set; }
        public int UV { get; set; }
        public override string ToString()
        {
            return this.Title;
        }
    }
}