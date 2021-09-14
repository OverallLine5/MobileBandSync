using MobileBandSync.Data;
using MobileBandSync.OpenTcx;
using MobileBandSync.OpenTcx.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Windows.Data.Xml.Dom;

namespace MobileBandSync.Common
{
    //=========================================================================================================
    public enum SensorLogType
    //=========================================================================================================
    {
        Timestamp = 0x00,
        SequenceID = 0x0F,
        UtcOffset = 0x13,
        SkinTemperature = 0x42,
        Waypoint = 0x53,
        Sensor = 0x68,
        HeartRate = 0x80,
        Steps = 0x82,
        DailySummary = 0xE6,
        WorkoutMarker = 0xD0,
        WorkoutMarker2 = 0xD2,
        WorkoutSummary = 0xA1,
        Counter = 0xA4,
        Unknown = 0xFF
    }

    //=========================================================================================================
    public enum ExportType
    //=========================================================================================================
    {
        Unknown = 0x00,
        GPX = 0x01, // not supported yet
        TCX = 0x02,
        FIT = 0x04,
        HeartRate = 0x08,
        Cadence = 0x10,
        Temperature = 0x20,
        GalvanicSkinResponse = 0x40
    }

    //=========================================================================================================
    public enum DistanceAnnotationType
    //=========================================================================================================
    {
        Unknown,
        Start,
        End,
        Split,
        Pause,
        UserGenerated,
        ElevationMax,
        ElevationMin,
        TimeMidPoint,
        Sunrise,
        Sunset
    }

    //=========================================================================================================
    public enum EventType
    //=========================================================================================================
    {
        Unknown = -2,
        None = 0,
        Running = 4,
        Biking = 6,
        Walking = 16,
        Sleeping = 21,
        Hike = 32,
        Workout = 99
    }

    //=========================================================================================================
    public class HeartRate
    //=========================================================================================================
    {
        public int Bpm { get; set; }
        public int Accuracy { get; set; }
        public System.DateTime TimeStamp { get; set; }
    }

    //=========================================================================================================
    public class Steps
    //=========================================================================================================
    {
        public uint Counter { get; set; }
        public System.DateTime TimeStamp { get; set; }
    }

    //=========================================================================================================
    public class WorkoutMarker
    //=========================================================================================================
    {
        public DistanceAnnotationType Action { get; set; }
        public EventType WorkoutType { get; set; }
        public int Value2 { get; set; }
        public System.DateTime TimeStamp { get; set; }
    }

    //=========================================================================================================
    public class WorkoutMarker2
    //=========================================================================================================
    {
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public System.DateTime TimeStamp { get; set; }
    }

    //=========================================================================================================
    public class Waypoint
    //=========================================================================================================
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double SpeedOverGround { get; set; }
        public double ElevationFromMeanSeaLevel { get; set; }
        public double EstimatedHorizontalError { get; set; }
        public double EstimatedVerticalError { get; set; }
        public System.DateTime TimeStamp { get; set; }
    }

    //=========================================================================================================
    public class Counter
    //=========================================================================================================
    {
        public double Value1 { get; set; }
        public double Value2 { get; set; }
    }

    //=========================================================================================================
    public class Sensor
    //=========================================================================================================
    {
        public uint GalvanicSkinResponse { get; set; }
        public uint Value1 { get; set; }
        public uint Value2 { get; set; }
        public uint Value3 { get; set; }
        public System.DateTime TimeStamp { get; set; }
    }

    //=========================================================================================================
    public class SensorValueCollection1
    //=========================================================================================================
    {
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public int Value3 { get; set; }
        public int Value4 { get; set; }
        public int Value5 { get; set; }
    }

    //=========================================================================================================
    public class SensorValueCollection2
    //=========================================================================================================
    {
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public int Value3 { get; set; }
        public int Value4 { get; set; }
        public int Value5 { get; set; }
        public int Value6 { get; set; }
    }

    //=========================================================================================================
    public class SensorValueCollection3
    //=========================================================================================================
    {
        public int Value1 { get; set; }
        public int Value2 { get; set; }
    }

    //=========================================================================================================
    public class WorkoutSummary
    //=========================================================================================================
    {
        public System.DateTime StartDate { get; set; }
        public int UnknownValue1 { get; set; }
        public double Duration { get; set; }
        public double Distance { get; set; }
        public double AverageSpeed { get; set; }
        public double MaximumSpeed { get; set; }
        public int CaloriesBurned { get; set; }
        public int HFAverage { get; set; }
        public int HFMax { get; set; }
        public System.DateTime IntermediateDate { get; set; }
        public int UtcDiffHrs { get; set; }
        public double TotalElevation { get; set; }
        public int UnknownValue2 { get; set; }
        public int UnknownValue3 { get; set; }
        public int UnknownValue4 { get; set; }
        public int UnknownValue5 { get; set; }
        public int UnknownValue6 { get; set; }
        public int UnknownValue7 { get; set; }
    }

    //=========================================================================================================
    public class DailySummary
    //=========================================================================================================
    {
        public uint Flag { get; set; }
        public System.DateTime Date { get; set; }
    }

    //=========================================================================================================
    public class SkinTemperature
    //=========================================================================================================
    {
        public double DegreeCelsius { get; set; }
        public System.DateTime TimeStamp { get; set; }
    }


    //=========================================================================================================
    public class UnknownData
    //=========================================================================================================
    {
        //------------------------------------------------------------------------------------------------------
        public UnknownData( int iID, byte[] buffer, int iLength )
        //------------------------------------------------------------------------------------------------------
        {
            ID = iID;
            Content = new byte[iLength];
            buffer.CopyTo( Content, 0 );

            ValueInt16 = new int[100];
            ValueInt32 = new int[100];

            // int16
            int iIndex = 0;
            for( int i = 0; i < iLength; i += 2 )
            {
                if( i + 1 >= iLength )
                    ValueInt16[iIndex] = (int)buffer[i];
                else
                    ValueInt16[iIndex] = BitConverter.ToInt16( buffer, i );

                if( ( i + 3 ) < iLength )
                    ValueInt32[iIndex] = BitConverter.ToInt32( buffer, i );

                iIndex++;
            }
        }

        public byte[] Content { get; }
        public int ID { get; set; }

        public int[] ValueInt16;
        public int[] ValueInt32;
    }


    //=========================================================================================================
    public class SensorLogSequence
    //=========================================================================================================
    {
        //------------------------------------------------------------------------------------------------------
        public SensorLogSequence( long lFileTime )
        //------------------------------------------------------------------------------------------------------
        {
            if( lFileTime > 0 )
                TimeStamp = System.DateTime.FromFileTime( lFileTime );
            else
                TimeStamp = System.DateTime.MinValue;

            HeartRates = new List<HeartRate>();
            Waypoints = new List<Waypoint>();
            AdditionalData = new List<UnknownData>();
            Counters = new List<Counter>();
            Temperatures = new List<SkinTemperature>();
            WorkoutMarkers = new List<WorkoutMarker>();
            WorkoutMarkers2 = new List<WorkoutMarker2>();
            SensorValues1 = new List<SensorValueCollection1>();
            SensorValues2 = new List<SensorValueCollection2>();
            SensorValues3 = new List<SensorValueCollection3>();
            StepSnapshots = new List<Steps>();
            WorkoutSummaries = new List<WorkoutSummary>();
            DailySummaries = new List<DailySummary>();
            IdOccurencies = new Dictionary<uint, uint>();
            SensorList = new List<Sensor>();
        }

        public System.DateTime? TimeStamp { get; set; }
        public TimeSpan? Duration { get; set; }
        public int ID { get; set; }
        public int UtcOffset { get; set; }
        public List<HeartRate> HeartRates { get; }
        public List<Waypoint> Waypoints { get; }
        public List<Counter> Counters { get; }
        public List<SkinTemperature> Temperatures { get; }
        public List<Sensor> SensorList { get; }
        public List<Steps> StepSnapshots { get; }
        public List<WorkoutMarker> WorkoutMarkers { get; }
        public List<WorkoutMarker2> WorkoutMarkers2 { get; }
        public List<SensorValueCollection1> SensorValues1 { get; }
        public List<SensorValueCollection2> SensorValues2 { get; }
        public List<SensorValueCollection3> SensorValues3 { get; }
        public List<UnknownData> AdditionalData { get; }
        public List<WorkoutSummary> WorkoutSummaries { get; }
        public List<DailySummary> DailySummaries { get; }
        public Dictionary<uint, uint> IdOccurencies { get; }
    }

    //=========================================================================================================
    public class GpsPosition
    //=========================================================================================================
    {
        public double LatitudeDegrees { get; set; }
        public double LongitudeDegrees { get; set; }
    }

    //=========================================================================================================
    public class WorkoutPoint
    //=========================================================================================================
    {
        public System.DateTime Time { get; set; }
        public int HeartRateBpm { get; set; }
        public GpsPosition Position { get; set; }
        public double Elevation { get; set; }
        public uint GalvanicSkinResponse { get; set; }
        public double SkinTemperature { get; set; }
        public uint Cadence { get; set; }
    }

    //=========================================================================================================
    public class TrackPoint : WorkoutPoint
    //=========================================================================================================
    {
        public double AltitudeMeters { get; set; }
    }

    //=========================================================================================================
    public class Workout
    //=========================================================================================================
    {
        //------------------------------------------------------------------------------------------------------
        public Workout()
        //------------------------------------------------------------------------------------------------------
        {
            Type = EventType.Workout;
            TrackPoints = new List<WorkoutPoint>();
        }

        public System.DateTime StartTime { get; set; }
        public System.DateTime LastSplitTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public List<WorkoutPoint> TrackPoints { get; }
        public int LastHR { get; set; }

        public WorkoutSummary Summary;
        public string Notes { get; set; }
        public EventType Type { get; set; }
        public String TCXBuffer { get; set; }
        public String Filename { get; set; }
        public ulong Filesize { get; set; }
    }


    //=========================================================================================================
    public class SensorLog
    //=========================================================================================================
    {
        private Stream DataStream;

        //------------------------------------------------------------------------------------------------------
        public SensorLog()
        //------------------------------------------------------------------------------------------------------
        {
            Sequences = new List<SensorLogSequence>();
        }


        //------------------------------------------------------------------------------------------------------
		public static bool IsSensorLog( Stream stream, 
                                        out System.DateTime dtStartDate )
        //------------------------------------------------------------------------------------------------------
        {
            Stream logStream = stream;
            bool bIsSensorLog = false;
            dtStartDate = System.DateTime.MinValue;

            if( logStream.CanRead && logStream.CanSeek )
            {
                logStream.Seek( 0, SeekOrigin.Begin );
                int iID = logStream.ReadByte();
                if( (SensorLogType)iID == SensorLogType.Timestamp )
                {
                    int iLength = logStream.ReadByte();
                    if( iLength == 8 )
                    {
                        // seems to be a valid sensor log stream
                        var buffer = new byte[iLength];

                        logStream.Read( buffer, 0, iLength );
                        var lTimeStamp = BitConverter.ToInt64( buffer, 0 );

                        if( lTimeStamp > 0 )
                        {
                            dtStartDate = System.DateTime.FromFileTime( lTimeStamp );
                            bIsSensorLog = true;
                        }
                    }
                }
            }
            return bIsSensorLog;
        }


        //------------------------------------------------------------------------------------------------------
		public async Task<bool> Read( Stream stream )
		//------------------------------------------------------------------------------------------------------
		{
            DataStream = stream;
            System.DateTime LastTimeStamp = System.DateTime.MinValue;

            if( DataStream.CanSeek )
            {
                await Task.Run( () =>
                {
                    SensorLogSequence currentSequence = null;
                    int iID = -1;
                    System.DateTime currentDateTime = System.DateTime.Now;

                    DataStream.Seek( 0, SeekOrigin.Begin );

                    do
                    {
                        try
                        {
                            iID = DataStream.ReadByte();
                            if( iID == -1 )
                                break;

                            int iLength = DataStream.ReadByte();
                            SensorLogType type = (SensorLogType)iID;

                            var buffer = new byte[iLength];

                            DataStream.Read( buffer, 0, iLength );

                            switch( type )
                            {
                                case SensorLogType.Timestamp:
                                    {
                                        var lTimeStamp = BitConverter.ToInt64( buffer, 0 );

                                        if( lTimeStamp > 0 )
                                        {
                                            // timestamp is valid
                                            if( currentSequence == null )
                                            {
                                                currentSequence = new SensorLogSequence( lTimeStamp );
                                                currentDateTime = System.DateTime.FromFileTime( lTimeStamp );

                                                if( Sequences.Count > 0 )
                                                {
                                                    var previousSequence = Sequences[Sequences.Count - 1];
                                                    if( previousSequence != null )
                                                    {
                                                        // add Duration
                                                        previousSequence.Duration = currentDateTime - previousSequence.TimeStamp;
                                                    }
                                                }
                                            }
                                            else // timestamp of a marker
                                            {
                                                LastTimeStamp = System.DateTime.FromFileTime( lTimeStamp );
                                            }
                                        }
                                    }
                                    break;

                                case SensorLogType.UtcOffset:
                                    if( currentSequence != null )
                                    {
                                        currentSequence.UtcOffset = BitConverter.ToInt16( buffer, 0 );
                                    }
                                    break;

                                case SensorLogType.SequenceID:
                                    if( currentSequence != null )
                                    {
                                        currentSequence.ID = BitConverter.ToInt32( buffer, 0 );
                                        // end of chunk, add new sequence and reset current
                                        Sequences.Add( currentSequence );
                                        currentSequence = null;
                                    }
                                    break;

                                case SensorLogType.Steps:
                                    if( currentSequence != null )
                                    {
                                        currentSequence.StepSnapshots.Add(
                                            new Steps()
                                            {
                                                Counter = BitConverter.ToUInt32( buffer, 0 ),
                                                TimeStamp = currentDateTime
                                            } );
                                    }
                                    break;

                                case SensorLogType.HeartRate:
                                    if( currentSequence != null )
                                    {
                                        currentSequence.HeartRates.Add(
                                            new HeartRate()
                                            {
                                                Bpm = (int)buffer[0],
                                                Accuracy = (int)buffer[1],
                                                TimeStamp = currentDateTime
                                            } );
                                    }
                                    break;

                                case SensorLogType.WorkoutMarker:
                                    if( currentSequence != null )
                                    {
                                        int iEventType = (int)buffer[1];
                                        if( iEventType != 4 && iEventType != 6 && iEventType != 21 && iEventType != 32 )
                                            iEventType = 99;

                                        currentSequence.WorkoutMarkers.Add(
                                            new WorkoutMarker()
                                            {
                                                Action = (DistanceAnnotationType)(int)buffer[0],
                                                WorkoutType = (EventType)iEventType,
                                                Value2 = (int)buffer[2],
                                                TimeStamp = LastTimeStamp
                                            } );
                                    }
                                    break;

                                case SensorLogType.WorkoutMarker2:
                                    if( currentSequence != null )
                                    {
                                        currentSequence.WorkoutMarkers2.Add(
                                            new WorkoutMarker2()
                                            {
                                                Value1 = BitConverter.ToInt16( buffer, 0 ),
                                                Value2 = BitConverter.ToInt32( buffer, 2 ),
                                                TimeStamp = LastTimeStamp
                                            } );
                                    }
                                    break;

                                case SensorLogType.Sensor:
                                    if( currentSequence != null )
                                    {
                                        currentSequence.SensorList.Add(
                                            new Sensor()
                                            {
                                                Value1 = BitConverter.ToUInt32( buffer, 0 ),
                                                GalvanicSkinResponse = BitConverter.ToUInt32( buffer, 4 ),
                                                Value2 = BitConverter.ToUInt32( buffer, 8 ),
                                                Value3 = BitConverter.ToUInt32( buffer, 12 ),
                                                TimeStamp = currentDateTime
                                            } );
                                    }
                                    break;

                                case SensorLogType.SkinTemperature:
                                    if( currentSequence != null )
                                    {
                                        double rawTemp = BitConverter.ToInt16( buffer, 0 );
                                        currentSequence.Temperatures.Add(
                                            new SkinTemperature()
                                            {
                                                DegreeCelsius = rawTemp > 0 ? rawTemp / 100 : 0,
                                                TimeStamp = currentDateTime
                                            } );
                                    }
                                    break;

                                case SensorLogType.Waypoint:
                                    if( currentSequence != null )
                                    {
                                        double rawLatitude = BitConverter.ToInt32( buffer, 2 );
                                        double rawLongitude = BitConverter.ToInt32( buffer, 6 );
                                        double rawElevation = BitConverter.ToInt32( buffer, 10 );
                                        double rawSpeedOverGround = BitConverter.ToInt16( buffer, 0 );
                                        double rawEstimatedHorizontalError = BitConverter.ToInt32( buffer, 14 );
                                        double rawEstimatedVerticalError = BitConverter.ToInt32( buffer, 18 );

                                        currentSequence.Waypoints.Add(
                                            new Waypoint()
                                            {
                                                SpeedOverGround = rawSpeedOverGround > 0 ? rawSpeedOverGround / 100 : 0,
                                                Latitude = rawLatitude > 0 ? rawLatitude / 10000000 : 0,
                                                Longitude = rawLongitude > 0 ? rawLongitude / 10000000 : 0,
                                                ElevationFromMeanSeaLevel = rawElevation / 100,
                                                EstimatedHorizontalError = rawEstimatedHorizontalError > 0 ? rawEstimatedHorizontalError / 100 : 0,
                                                EstimatedVerticalError = rawEstimatedVerticalError > 0 ? rawEstimatedVerticalError / 100 : 0,
                                                TimeStamp = currentDateTime
                                            } );
                                    }
                                    break;

                                case SensorLogType.Counter:
                                    if( currentSequence != null )
                                    {
                                        currentSequence.Counters.Add(
                                            new Counter()
                                            {
                                                Value1 = BitConverter.ToInt32( buffer, 0 ),
                                                Value2 = BitConverter.ToInt32( buffer, 4 )
                                            } );

                                        // increase current DateTime by 1 second
                                        currentDateTime = currentDateTime.AddSeconds( 1.0 );
                                    }
                                    break;

                                case SensorLogType.WorkoutSummary:
                                    if( currentSequence != null )
                                    {
                                        var lTimeStamp1 = BitConverter.ToInt64( buffer, 0 );
                                        var lTimeStamp2 = BitConverter.ToInt64( buffer, 38 );
                                        var rawStartDate = lTimeStamp1 > 0 ? System.DateTime.FromFileTime( lTimeStamp1 ) : System.DateTime.MinValue;
                                        var rawIntermediateDate = lTimeStamp2 > 0 ? System.DateTime.FromFileTime( lTimeStamp2 ) : System.DateTime.MinValue;
                                        var rawDuration = (double)BitConverter.ToInt32( buffer, 10 );
                                        var rawDistance = (double)BitConverter.ToInt32( buffer, 14 );
                                        var rawAverageSpeed = (double)BitConverter.ToInt32( buffer, 18 );
                                        var rawMaximumSpeed = (double)BitConverter.ToInt32( buffer, 22 );
                                        var rawCaloriesBurned = BitConverter.ToInt32( buffer, 26 );
                                        var rawHFAverage = BitConverter.ToInt32( buffer, 30 );
                                        var rawHFMax = BitConverter.ToInt32( buffer, 34 );
                                        var rawUtcDiffHrs = BitConverter.ToInt32( buffer, 46 );
                                        var rawTotalElevation = (double)BitConverter.ToInt32( buffer, 50 );

                                        currentSequence.WorkoutSummaries.Add(
                                            new WorkoutSummary()
                                            {
                                                StartDate = rawStartDate,
                                                IntermediateDate = rawIntermediateDate,
                                                Duration = rawDuration / 1000.0,
                                                Distance = rawDistance / 100.0,
                                                AverageSpeed = rawAverageSpeed / 100.0,
                                                MaximumSpeed = rawMaximumSpeed / 100.0,
                                                CaloriesBurned = rawCaloriesBurned,
                                                HFAverage = rawHFAverage,
                                                HFMax = rawHFMax,
                                                UtcDiffHrs = rawUtcDiffHrs,
                                                TotalElevation = rawTotalElevation / 100.0,
                                                UnknownValue1 = BitConverter.ToInt16( buffer, 8 ),
                                                UnknownValue2 = BitConverter.ToInt32( buffer, 54 ),
                                                UnknownValue3 = BitConverter.ToInt32( buffer, 58 ),
                                                UnknownValue4 = BitConverter.ToInt32( buffer, 62 ),
                                                UnknownValue5 = BitConverter.ToInt32( buffer, 66 ),
                                                UnknownValue6 = BitConverter.ToInt32( buffer, 70 ),
                                                UnknownValue7 = BitConverter.ToInt32( buffer, 74 )
                                            } );
                                    }
                                    break;

                                case SensorLogType.DailySummary:
                                    if( currentSequence != null )
                                    {
                                        var lTimeStamp = BitConverter.ToInt64( buffer, 1 );

                                        currentSequence.DailySummaries.Add(
                                            new DailySummary()
                                            {
                                                Flag = (uint)buffer[0],
                                                Date = lTimeStamp > 0 ? System.DateTime.FromFileTime( lTimeStamp ) : System.DateTime.MinValue
                                            } );
                                    }
                                    break;

                                default: // Unknown
                                    if( currentSequence != null )
                                    {
                                        uint iCount = 0;
                                        if( currentSequence.IdOccurencies.ContainsKey( (uint)iID ) )
                                            iCount = currentSequence.IdOccurencies[(uint)iID];

                                        currentSequence.IdOccurencies[(uint)iID] = iCount + 1;
                                        currentSequence.AdditionalData.Add( new UnknownData( iID, buffer, iLength ) );
                                    }
                                    break;
                            }
                        }
                        catch( Exception )
                        {
                            break;
                        }
                    }
                    while( iID >= 0 );
                } );
            }

            if( Sequences.Count > 0 )
            {
                var lastSequence = Sequences[Sequences.Count - 1];
                if( lastSequence != null && LastTimeStamp != System.DateTime.MinValue &&
                    LastTimeStamp > lastSequence.TimeStamp )
                {
                    // add Duration
                    lastSequence.Duration = LastTimeStamp - lastSequence.TimeStamp;
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------------------------------
        public async Task<List<Workout>> CreateWorkouts( ExportType type = ExportType.TCX | 
                                                         ExportType.HeartRate | ExportType.Cadence )
        //------------------------------------------------------------------------------------------------------
        {
            List<Workout> Workouts = new List<Workout>();

            await Task.Run( () =>
            {
                Workout currentWorkout = null;

                Dictionary<System.DateTime, int> heartRates = new Dictionary<System.DateTime, int>();
                Dictionary<System.DateTime, double> elevationList = new Dictionary<System.DateTime, double>();
                Dictionary<System.DateTime, GpsPosition> positionList = new Dictionary<System.DateTime, GpsPosition>();
                Dictionary<System.DateTime, uint> galvanicList = new Dictionary<System.DateTime, uint>();
                Dictionary<System.DateTime, double> temperatureList = new Dictionary<System.DateTime, double>();
                Dictionary<System.DateTime, uint> stepsList = new Dictionary<System.DateTime, uint>();
                System.DateTime lastDateTime = System.DateTime.Now;
                double dSleepFakeLat = 48.6721393;
                double dSleepFakeLong = 9.24037;
                double dSleepFakeDelta = 0.00005;
                double dTotalSleepFakeLat = dSleepFakeLat;
                double dTotalSleepFakeLong = dSleepFakeLong;

                foreach( var sequence in Sequences )
                {
                    if( sequence.WorkoutSummaries.Count > 0 && Workouts.Count > 0 )
                    {
                        var tempWorkout = ( currentWorkout == null ? Workouts[Workouts.Count - 1] : currentWorkout );
                        tempWorkout.Summary = sequence.WorkoutSummaries[sequence.WorkoutSummaries.Count - 1];

                        dTotalSleepFakeLat = dSleepFakeLat;
                        dTotalSleepFakeLong = dSleepFakeLong;
                    }

                    if( sequence.WorkoutMarkers.Count > 0 )
                    {
                        foreach( var workoutMarker in sequence.WorkoutMarkers )
                        {
                            if( workoutMarker.Action == DistanceAnnotationType.Start )
                            {
                                if( currentWorkout != null )
                                {
                                    currentWorkout.EndTime = lastDateTime;

                                    AddWorkoutData( ref currentWorkout, heartRates, elevationList, positionList, galvanicList, temperatureList, stepsList );
                                    if( !Workouts.Contains( currentWorkout ) )
                                        Workouts.Add( currentWorkout );
                                    currentWorkout = null;
                                    heartRates.Clear();
                                    elevationList.Clear();
                                    positionList.Clear();
                                    temperatureList.Clear();
                                    galvanicList.Clear();
                                    stepsList.Clear();
                                }

                                currentWorkout = new Workout();
                                currentWorkout.LastSplitTime = currentWorkout.StartTime = workoutMarker.TimeStamp;
                                currentWorkout.Type = workoutMarker.WorkoutType;
                                if( !Workouts.Contains( currentWorkout ) )
                                    Workouts.Add( currentWorkout );

                                currentWorkout.Filename =
                                    currentWorkout.Type.ToString() + "-" + currentWorkout.StartTime.Year.ToString( "D4" ) + currentWorkout.StartTime.Month.ToString( "D2" ) +
                                    currentWorkout.StartTime.Day.ToString( "D2" ) + currentWorkout.StartTime.Hour.ToString( "D2" ) + currentWorkout.StartTime.Minute.ToString( "D2" ) +
                                    currentWorkout.StartTime.Second.ToString( "D2" ) + ".tcx";

                                currentWorkout.Notes = "Generated from Microsoft Band Sensor Log Workout on " + currentWorkout.StartTime.ToString();
                            }
                            else if( workoutMarker.Action == DistanceAnnotationType.Split )
                            {
                                if( currentWorkout == null && Workouts.Count > 0 )
                                {
                                    currentWorkout = Workouts[Workouts.Count - 1];
                                    Workouts.RemoveAt( Workouts.Count - 1 );
                                }
                                if( currentWorkout != null )
                                    currentWorkout.LastSplitTime = workoutMarker.TimeStamp;
                            }
                            else if( workoutMarker.Action == DistanceAnnotationType.Pause )
                            {
                                if( currentWorkout != null )
                                {
                                    lastDateTime = currentWorkout.EndTime = workoutMarker.TimeStamp;

                                    AddWorkoutData( ref currentWorkout, heartRates, elevationList, positionList, galvanicList, temperatureList, stepsList );
                                    if( !Workouts.Contains( currentWorkout ) )
                                        Workouts.Add( currentWorkout );

                                    if( currentWorkout.Type == EventType.Sleeping )
                                    {
                                        dTotalSleepFakeLat = dSleepFakeLat;
                                        dTotalSleepFakeLong = dSleepFakeLong;
                                    }

                                    currentWorkout = null;

                                    heartRates.Clear();
                                    elevationList.Clear();
                                    positionList.Clear();
                                    temperatureList.Clear();
                                    galvanicList.Clear();
                                    stepsList.Clear();
                                }
                            }
                            else
                            {
                                lastDateTime = workoutMarker.TimeStamp;
                                if( currentWorkout != null && currentWorkout.EndTime == System.DateTime.MinValue )
                                    currentWorkout.EndTime = lastDateTime;
                            }
                        }
                    }

                    if( currentWorkout != null )
                    {
                        if( ( type & ExportType.HeartRate ) == ExportType.HeartRate && sequence.HeartRates.Count > 0 )
                        {
                            if( currentWorkout.Type == EventType.Sleeping )
                            {
                                List<HeartRate> accuracyList = new List<HeartRate>();
                                foreach( var heartRate in sequence.HeartRates )
                                {
                                    if( heartRate.Accuracy >= 9 )
                                        accuracyList.Add( heartRate );
                                }

                                double seconds = 0.0;
                                if( sequence.Duration.Value != null && sequence.Duration > TimeSpan.FromSeconds( 1 ) )
                                    seconds = sequence.Duration.Value.TotalSeconds / accuracyList.Count;

                                double currentSeconds = 0.0;
                                var lastTimeStamp = (System.DateTime)sequence.TimeStamp;

                                foreach( var heartRate in accuracyList )
                                {
                                    var timeStamp = (System.DateTime)sequence.TimeStamp + TimeSpan.FromSeconds( currentSeconds );

                                    if( currentSeconds == 0 || timeStamp >= ( lastTimeStamp + TimeSpan.FromSeconds( 60 ) ) )
                                    {
                                        heartRates[timeStamp] = heartRate.Bpm;

                                        // add fake waypoints
                                        positionList[timeStamp] = new GpsPosition() { LatitudeDegrees = dTotalSleepFakeLat, LongitudeDegrees = dTotalSleepFakeLong };
                                        elevationList[timeStamp] = 360.0;
                                        dTotalSleepFakeLat += dSleepFakeDelta;
                                        dTotalSleepFakeLong += dSleepFakeDelta;

                                        lastTimeStamp = timeStamp;
                                    }
                                    currentSeconds += seconds;
                                }
                            }
                            else
                            {
                                foreach( var heartRate in sequence.HeartRates )
                                {
                                    if( heartRate.TimeStamp >= currentWorkout.LastSplitTime - TimeSpan.FromSeconds( 10 ) )
                                    {
                                        // allow lower HR accuracy for the first 60 seconds after starting the workout
                                        if( heartRate.Accuracy >= 8 ||
                                            ( heartRate.Accuracy >= 5 && heartRate.TimeStamp <= currentWorkout.LastSplitTime + TimeSpan.FromSeconds( 120 ) ) )
                                        {
                                            heartRates[heartRate.TimeStamp] = heartRate.Bpm;
                                        }
                                    }
                                }
                            }
                        }
                        if( sequence.SensorList.Count > 0 )
                        {
                            foreach( var sensor in sequence.SensorList )
                            {
                                galvanicList[sensor.TimeStamp] = sensor.GalvanicSkinResponse;
                            }
                        }
                        if( sequence.StepSnapshots.Count > 0 )
                        {
                            foreach( var steps in sequence.StepSnapshots )
                            {
                                stepsList[steps.TimeStamp] = steps.Counter;
                            }
                        }
                        if( sequence.Temperatures.Count > 0 )
                        {
                            foreach( var temperature in sequence.Temperatures )
                            {
                                temperatureList[temperature.TimeStamp] = temperature.DegreeCelsius;
                            }
                        }
                        if( sequence.Waypoints.Count > 0 )
                        {
                            System.DateTime lastGoodPos = currentWorkout.LastSplitTime - TimeSpan.FromSeconds( 20 );
                            System.DateTime lastKnownPos = lastGoodPos;

                            // Hiking for example uses low energy GPS and has a lot more estimated error distances
                            int iExactThreshold = ( ( currentWorkout.Type == EventType.Running || currentWorkout.Type == EventType.Biking ) ? 20 : 150 );
                            int iTolerantThreshold = ( ( currentWorkout.Type == EventType.Running || currentWorkout.Type == EventType.Biking ) ? 135 : 300 );

                            foreach( var wayPoint in sequence.Waypoints )
                            {
                                if( wayPoint.TimeStamp >= currentWorkout.LastSplitTime - TimeSpan.FromSeconds( 10 ) )
                                {
                                    if( ( wayPoint.EstimatedHorizontalError <= iExactThreshold || wayPoint.EstimatedVerticalError <= iExactThreshold ) )
                                    {
                                        if( ( wayPoint.TimeStamp - lastGoodPos ) >= TimeSpan.FromSeconds( 3 ) )
                                        {
                                            lastKnownPos = lastGoodPos = wayPoint.TimeStamp;
                                            positionList[wayPoint.TimeStamp] =
                                                new GpsPosition() { LatitudeDegrees = wayPoint.Latitude, LongitudeDegrees = wayPoint.Longitude };
                                            elevationList[wayPoint.TimeStamp] = wayPoint.ElevationFromMeanSeaLevel;
                                        }
                                    }
                                    else if( ( wayPoint.EstimatedHorizontalError <= iTolerantThreshold || wayPoint.EstimatedVerticalError <= iTolerantThreshold ) )

                                    {
                                        if( ( wayPoint.TimeStamp - lastKnownPos ) >= TimeSpan.FromSeconds( 3 ) )
                                        {
                                            // better than dropping the GPS point
                                            lastKnownPos = wayPoint.TimeStamp;
                                            positionList[wayPoint.TimeStamp] =
                                                new GpsPosition() { LatitudeDegrees = wayPoint.Latitude, LongitudeDegrees = wayPoint.Longitude };
                                            elevationList[wayPoint.TimeStamp] = wayPoint.ElevationFromMeanSeaLevel;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if( ( type & ExportType.TCX ) == ExportType.TCX )
                {
                    AddTcxExport( ref Workouts, type );
                }
            } );

            return Workouts;
        }

        //------------------------------------------------------------------------------------------------------
        public bool AddWorkoutData( ref Workout currenWorkout,
									Dictionary<System.DateTime, int> heartRateList,
									Dictionary<System.DateTime, double> elevationList,
									Dictionary<System.DateTime, GpsPosition> positionList,
									Dictionary<System.DateTime, uint> galvanicList,
									Dictionary<System.DateTime, double> temperatureList,
									Dictionary<System.DateTime, uint> stepsList )
        //------------------------------------------------------------------------------------------------------
        {
            uint lastGalvanic = 0;
            uint lastSteps = 0;
            uint lastStepCount = 0;
            System.DateTime lastStepCountDate = System.DateTime.MinValue;
            uint lastCadence = 0;
            double lastTemperature = 0.0;

            foreach( var date in positionList.Keys )
            {
                if( positionList.ContainsKey( date ) &&
                    elevationList.ContainsKey( date ) )
                {
                    var heartListDate = System.DateTime.MinValue;
                    if( !heartRateList.ContainsKey( date ) )
                    {
                        // go back 10 seconds
                        for( var tempDate = date; tempDate >= ( date - TimeSpan.FromSeconds( 10 ) ); tempDate -= TimeSpan.FromSeconds( 1 ) )
                        {
                            if( heartRateList.ContainsKey( tempDate ) )
                            {
                                heartListDate = tempDate;
                                break;
                            }
                        }
                    }
                    else
                        heartListDate = date;

                    if( !galvanicList.ContainsKey( date ) )
                    {
                        // go back 10 seconds
                        for( var tempDate = date; tempDate >= ( date - TimeSpan.FromSeconds( 10 ) ); tempDate -= TimeSpan.FromSeconds( 1 ) )
                        {
                            if( galvanicList.ContainsKey( tempDate ) )
                            {
                                lastGalvanic = galvanicList[tempDate];
                                break;
                            }
                        }
                    }
                    else
                        lastGalvanic = galvanicList[date];

                    if( !stepsList.ContainsKey( date ) )
                    {
                        // go back 10 seconds
                        for( var tempDate = date; tempDate >= ( date - TimeSpan.FromSeconds( 50 ) ); tempDate -= TimeSpan.FromSeconds( 1 ) )
                        {
                            if( stepsList.ContainsKey( tempDate ) )
                            {
                                lastSteps = stepsList[tempDate];
                                break;
                            }
                        }
                    }
                    else
                        lastSteps = stepsList[date];

                    if( !temperatureList.ContainsKey( date ) )
                    {
                        // go back 10 seconds
                        for( var tempDate = date; tempDate >= ( date - TimeSpan.FromSeconds( 10 ) ); tempDate -= TimeSpan.FromSeconds( 1 ) )
                        {
                            if( temperatureList.ContainsKey( tempDate ) )
                            {
                                lastTemperature = temperatureList[tempDate];
                                break;
                            }
                        }
                    }
                    else
                        lastTemperature = temperatureList[date];

                    // determine cadence in steps per minute
                    if( lastSteps > lastStepCount )
                    {
                        if( lastStepCount > 0 )
                        {
                            var stepsDiff = lastSteps - lastStepCount;
                            var stepsSpanFactor = ( ( date - lastStepCountDate ).TotalSeconds / 60 );

                            lastCadence = (uint)( stepsDiff / stepsSpanFactor );
                        }
                        lastStepCount = lastSteps;
                        lastStepCountDate = date;
                    }

                    if( heartRateList.Count > 0 && heartRateList.ContainsKey( heartListDate ) )
                        currenWorkout.LastHR = heartRateList[heartListDate];

                    currenWorkout.TrackPoints.Add(
                        new WorkoutPoint()
                        {
                            Time = date,
                            Position = positionList[date],
                            Elevation = elevationList[date],
                            HeartRateBpm = currenWorkout.LastHR,
                            GalvanicSkinResponse = lastGalvanic,
                            SkinTemperature = lastTemperature,
                            Cadence = lastCadence
                        } );
                }
            }
            return true;
        }

        //------------------------------------------------------------------------------------------------------
        public bool AddTcxExport( ref List<Workout> Workouts,
                                  ExportType type )
        //------------------------------------------------------------------------------------------------------
        {
            var tcx = new Tcx();
            XmlDocument doc = new XmlDocument();
            bool bResult = Workouts.Count > 0;

            try
            {
                foreach( var workout in Workouts )
                {
                    if( ( workout.Type == EventType.Running || workout.Type == EventType.Hike || workout.Type == EventType.Sleeping || workout.Type == EventType.Biking ) && workout.TrackPoints.Count > 0 )
                    {
                        ExportType supportedType = type;
                        switch( workout.Type )
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

                        tcxDatabase.Activities.Activity[0].Id = workout.StartTime;
                        tcxDatabase.Activities.Activity[0].Notes = workout.Notes;
						tcxDatabase.Activities.Activity[0].Sport = workout.Type == EventType.Biking ? Sport_t.Biking : Sport_t.Running;

                        tcxDatabase.Activities.Activity[0].Lap = new ActivityLap_t[1];
                        tcxDatabase.Activities.Activity[0].Lap[0] = new ActivityLap_t();

                        // summary
                        if( workout.Summary != null )
                        {
                            double averageMeterPerSecond = 0;
                            string strWorkoutType;
                            tcxDatabase.Activities.Activity[0].Sport = Sport_t.Running;

                            if( workout.Type == EventType.Biking )
                            {
                                strWorkoutType = "Biking";
                                tcxDatabase.Activities.Activity[0].Sport = Sport_t.Biking;
                            }
                            else if( workout.Type == EventType.Hike )
                            {
                                strWorkoutType = "Hiking";
                            }
                            else
                            {
                                if( workout.Summary.HFAverage <= 120 )
                                    strWorkoutType = "Walking";
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

                            if( ( type & ExportType.HeartRate ) == ExportType.HeartRate )
                            {
                                tcxDatabase.Activities.Activity[0].Lap[0].AverageHeartRateBpm = new HeartRateInBeatsPerMinute_t();
                                tcxDatabase.Activities.Activity[0].Lap[0].AverageHeartRateBpm.Value = (byte)workout.Summary.HFAverage;
                                tcxDatabase.Activities.Activity[0].Lap[0].MaximumHeartRateBpm = new HeartRateInBeatsPerMinute_t();
                                tcxDatabase.Activities.Activity[0].Lap[0].MaximumHeartRateBpm.Value = (byte)workout.Summary.HFMax;
                            }
                            tcxDatabase.Activities.Activity[0].Lap[0].MaximumSpeed = workout.Summary.MaximumSpeed;
                            tcxDatabase.Activities.Activity[0].Lap[0].MaximumSpeedSpecified = true;
                            tcxDatabase.Activities.Activity[0].Lap[0].TotalTimeSeconds = workout.Summary.Duration;
                            tcxDatabase.Activities.Activity[0].Lap[0].Calories = (ushort)workout.Summary.CaloriesBurned;
                            tcxDatabase.Activities.Activity[0].Lap[0].DistanceMeters = workout.Summary.Distance;
                            tcxDatabase.Activities.Activity[0].Lap[0].Intensity = Intensity_t.Active;

                            averageMeterPerSecond = workout.Summary.Distance / workout.Summary.Duration;
                            double averageMinPerKm = ( 1000 / averageMeterPerSecond ) / 60;
                            var secDecimal = ( averageMinPerKm % 1 );
                            var seconds = 0.6 * secDecimal;
                            averageMinPerKm -= secDecimal;
                            averageMinPerKm += seconds;

                            workout.Filename =
                                workout.StartTime.Year.ToString( "D4" ) + workout.StartTime.Month.ToString( "D2" ) +
                                workout.StartTime.Day.ToString( "D2" ) + "_" +
                                workout.StartTime.Hour.ToString( "D2" ) + workout.StartTime.Minute.ToString( "D2" ) + "_" +
                                strWorkoutType + "_" + ( workout.Summary.Distance / 1000 ).ToString( "F2", CultureInfo.InvariantCulture ) + "_" +
                                averageMinPerKm.ToString( "F2", CultureInfo.InvariantCulture ) + "_" + workout.Summary.HFAverage.ToString( "F0" ) + ".tcx";
                        }
                        else
                            tcxDatabase.Activities.Activity[0].Sport = workout.Type == EventType.Biking ? Sport_t.Biking : Sport_t.Running;

                        tcxDatabase.Activities.Activity[0].Lap[0].StartTime = workout.StartTime;
                        tcxDatabase.Activities.Activity[0].Lap[0].TriggerMethod = TriggerMethod_t.Manual;

                        tcxDatabase.Activities.Activity[0].Lap[0].Track = new Trackpoint_t[workout.TrackPoints.Count];

                        int iIndex = 0;
                        foreach( var trackPoint in workout.TrackPoints )
                        {
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex] = new Trackpoint_t();
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Time = trackPoint.Time;
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].SensorState = SensorState_t.Present;
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].SensorStateSpecified = true;

                            // heart rate
                            if( ( type & ExportType.HeartRate ) == ExportType.HeartRate )
                            {
                                tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].HeartRateBpm = new HeartRateInBeatsPerMinute_t();
                                tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].HeartRateBpm.Value = (byte)trackPoint.HeartRateBpm;
                            }

                            // cadence
                            if( ( type & ExportType.Cadence ) == ExportType.Cadence && workout.Type != EventType.Biking )
                            {
                                tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Cadence = (byte)trackPoint.Cadence;
                                tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].CadenceSpecified = true;
                            }

                            // elevation
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].AltitudeMeters = trackPoint.Elevation;
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].AltitudeMetersSpecified = true;

                            if( ( type & ExportType.Temperature ) == ExportType.Temperature ||
                                ( type & ExportType.GalvanicSkinResponse ) == ExportType.GalvanicSkinResponse )
                            {
								//tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Extensions = new Extensions_t();
								//tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Extensions.Any = new XmlElement[1];
								//tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Extensions.Any[0] =
								//	doc.CreateElement( "TPX" );
								//if( ( type & ExportType.GalvanicSkinResponse ) == ExportType.GalvanicSkinResponse )
								//{
								//	XmlElement elem = doc.CreateElement( "Galvanic" );
								//	XmlText text = doc.CreateTextNode( String.Format( new CultureInfo( "en-US" ), "{0:0}", trackPoint.GalvanicSkinResponse ) );
								//	elem.AppendChild( text );
								//	tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Extensions.Any[0].AppendChild( elem );
								//}
								//if( ( type & ExportType.Temperature ) == ExportType.Temperature )
								//{
								//	XmlElement elem = doc.CreateElement( "Temp" );
								//	XmlText text = doc.CreateTextNode( String.Format( new CultureInfo( "en-US" ), "{0:0.00}", trackPoint.SkinTemperature ) );
								//	elem.AppendChild( text );
								//	tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Extensions.Any[0].AppendChild( elem );
								//}
                                }

                            // GPS point
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Position = new Position_t();
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Position.LatitudeDegrees = trackPoint.Position.LatitudeDegrees;
                            tcxDatabase.Activities.Activity[0].Lap[0].Track[iIndex].Position.LongitudeDegrees = trackPoint.Position.LongitudeDegrees;


                            iIndex++;
                        }
                        string strBuffer = tcx.GenerateTcx( tcxDatabase );
                        if( strBuffer != null && strBuffer.Length > 0 )
						    workout.TCXBuffer = strBuffer.Replace( "\"utf-16\"", "\"UTF-8\"" );
                    }
                }
            }
			catch( Exception ex )
            {
                bResult = false;
            }
            return bResult;
        }

        public List<SensorLogSequence> Sequences { get; }
        public ulong BufferSize { get; set; }
        public ulong StepLength { get; internal set; }
    }
}
