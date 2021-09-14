namespace MobileBandSync.OpenTcx.Entities
{
    using System.Xml.Serialization;
    using Windows.Data.Xml.Dom;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    [System.Xml.Serialization.XmlRootAttribute("TrainingCenterDatabase", Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2", IsNullable=false)]
    public partial class TrainingCenterDatabase_t {
        
        private Folders_t foldersField;
        
        private ActivityList_t activitiesField;
        
        private Workout_t[] workoutsField;
        
        private Course_t[] coursesField;
        
        private AbstractSource_t authorField;
        
        //private Extensions_t extensionsField;
        
        /// <remarks/>
        public Folders_t Folders {
            get {
                return this.foldersField;
            }
            set {
                this.foldersField = value;
            }
        }
        
        /// <remarks/>
        public ActivityList_t Activities {
            get {
                return this.activitiesField;
            }
            set {
                this.activitiesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Workout", IsNullable=false)]
        public Workout_t[] Workouts {
            get {
                return this.workoutsField;
            }
            set {
                this.workoutsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Course", IsNullable=false)]
        public Course_t[] Courses {
            get {
                return this.coursesField;
            }
            set {
                this.coursesField = value;
            }
        }
        
        /// <remarks/>
        public AbstractSource_t Author {
            get {
                return this.authorField;
            }
            set {
                this.authorField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Folders_t {
        
        private History_t historyField;
        
        private Workouts_t workoutsField;
        
        private Courses_t coursesField;
        
        /// <remarks/>
        public History_t History {
            get {
                return this.historyField;
            }
            set {
                this.historyField = value;
            }
        }
        
        /// <remarks/>
        public Workouts_t Workouts {
            get {
                return this.workoutsField;
            }
            set {
                this.workoutsField = value;
            }
        }
        
        /// <remarks/>
        public Courses_t Courses {
            get {
                return this.coursesField;
            }
            set {
                this.coursesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class History_t {
        
        private HistoryFolder_t runningField;
        
        private HistoryFolder_t bikingField;
        
        private HistoryFolder_t otherField;
        
        private MultiSportFolder_t multiSportField;
        
        //private Extensions_t extensionsField;
        
        /// <remarks/>
        public HistoryFolder_t Running {
            get {
                return this.runningField;
            }
            set {
                this.runningField = value;
            }
        }
        
        /// <remarks/>
        public HistoryFolder_t Biking {
            get {
                return this.bikingField;
            }
            set {
                this.bikingField = value;
            }
        }
        
        /// <remarks/>
        public HistoryFolder_t Other {
            get {
                return this.otherField;
            }
            set {
                this.otherField = value;
            }
        }
        
        /// <remarks/>
        public MultiSportFolder_t MultiSport {
            get {
                return this.multiSportField;
            }
            set {
                this.multiSportField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class HistoryFolder_t {
        
        private HistoryFolder_t[] folderField;
        
        private ActivityReference_t[] activityRefField;
        
        private Week_t[] weekField;
        
        private string notesField;
        
        //private Extensions_t extensionsField;
        
        private string nameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Folder")]
        public HistoryFolder_t[] Folder {
            get {
                return this.folderField;
            }
            set {
                this.folderField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ActivityRef")]
        public ActivityReference_t[] ActivityRef {
            get {
                return this.activityRefField;
            }
            set {
                this.activityRefField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Week")]
        public Week_t[] Week {
            get {
                return this.weekField;
            }
            set {
                this.weekField = value;
            }
        }
        
        /// <remarks/>
        public string Notes {
            get {
                return this.notesField;
            }
            set {
                this.notesField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class ActivityReference_t {
        
        private System.DateTime idField;
        
        /// <remarks/>
        public System.DateTime Id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class CoursePoint_t {
        
        private string nameField;
        
        private System.DateTime timeField;
        
        private Position_t positionField;
        
        private double altitudeMetersField;
        
        private bool altitudeMetersFieldSpecified;
        
        private CoursePointType_t pointTypeField;
        
        private string notesField;
        
        //private Extensions_t extensionsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime Time {
            get {
                return this.timeField;
            }
            set {
                this.timeField = value;
            }
        }
        
        /// <remarks/>
        public Position_t Position {
            get {
                return this.positionField;
            }
            set {
                this.positionField = value;
            }
        }
        
        /// <remarks/>
        public double AltitudeMeters {
            get {
                return this.altitudeMetersField;
            }
            set {
                this.altitudeMetersField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AltitudeMetersSpecified {
            get {
                return this.altitudeMetersFieldSpecified;
            }
            set {
                this.altitudeMetersFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public CoursePointType_t PointType {
            get {
                return this.pointTypeField;
            }
            set {
                this.pointTypeField = value;
            }
        }
        
        /// <remarks/>
        public string Notes {
            get {
                return this.notesField;
            }
            set {
                this.notesField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Position_t {
        
        private double latitudeDegreesField;
        
        private double longitudeDegreesField;
        
        /// <remarks/>
        public double LatitudeDegrees {
            get {
                return this.latitudeDegreesField;
            }
            set {
                this.latitudeDegreesField = value;
            }
        }
        
        /// <remarks/>
        public double LongitudeDegrees {
            get {
                return this.longitudeDegreesField;
            }
            set {
                this.longitudeDegreesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public enum CoursePointType_t {
        
        /// <remarks/>
        Generic,
        
        /// <remarks/>
        Summit,
        
        /// <remarks/>
        Valley,
        
        /// <remarks/>
        Water,
        
        /// <remarks/>
        Food,
        
        /// <remarks/>
        Danger,
        
        /// <remarks/>
        Left,
        
        /// <remarks/>
        Right,
        
        /// <remarks/>
        Straight,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("First Aid")]
        FirstAid,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4th Category")]
        Item4thCategory,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("3rd Category")]
        Item3rdCategory,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2nd Category")]
        Item2ndCategory,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1st Category")]
        Item1stCategory,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("Hors Category")]
        HorsCategory,
        
        /// <remarks/>
        Sprint,
    }
    
    /// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    //public partial class Extensions_t {
        
    //    private XmlElement[] anyField;
        
    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAnyElementAttribute()]
    //    public XmlElement[] Any {
    //        get {
    //            return this.anyField;
    //        }
    //        set {
    //            this.anyField = value;
    //        }
    //    }
    //}
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class CourseLap_t {
        
        private double totalTimeSecondsField;
        
        private double distanceMetersField;
        
        private Position_t beginPositionField;
        
        private double beginAltitudeMetersField;
        
        private bool beginAltitudeMetersFieldSpecified;
        
        private Position_t endPositionField;
        
        private double endAltitudeMetersField;
        
        private bool endAltitudeMetersFieldSpecified;
        
        private HeartRateInBeatsPerMinute_t averageHeartRateBpmField;
        
        private HeartRateInBeatsPerMinute_t maximumHeartRateBpmField;
        
        private Intensity_t intensityField;
        
        private byte cadenceField;
        
        private bool cadenceFieldSpecified;
        
        //private Extensions_t extensionsField;
        
        /// <remarks/>
        public double TotalTimeSeconds {
            get {
                return this.totalTimeSecondsField;
            }
            set {
                this.totalTimeSecondsField = value;
            }
        }
        
        /// <remarks/>
        public double DistanceMeters {
            get {
                return this.distanceMetersField;
            }
            set {
                this.distanceMetersField = value;
            }
        }
        
        /// <remarks/>
        public Position_t BeginPosition {
            get {
                return this.beginPositionField;
            }
            set {
                this.beginPositionField = value;
            }
        }
        
        /// <remarks/>
        public double BeginAltitudeMeters {
            get {
                return this.beginAltitudeMetersField;
            }
            set {
                this.beginAltitudeMetersField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BeginAltitudeMetersSpecified {
            get {
                return this.beginAltitudeMetersFieldSpecified;
            }
            set {
                this.beginAltitudeMetersFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public Position_t EndPosition {
            get {
                return this.endPositionField;
            }
            set {
                this.endPositionField = value;
            }
        }
        
        /// <remarks/>
        public double EndAltitudeMeters {
            get {
                return this.endAltitudeMetersField;
            }
            set {
                this.endAltitudeMetersField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EndAltitudeMetersSpecified {
            get {
                return this.endAltitudeMetersFieldSpecified;
            }
            set {
                this.endAltitudeMetersFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public HeartRateInBeatsPerMinute_t AverageHeartRateBpm {
            get {
                return this.averageHeartRateBpmField;
            }
            set {
                this.averageHeartRateBpmField = value;
            }
        }
        
        /// <remarks/>
        public HeartRateInBeatsPerMinute_t MaximumHeartRateBpm {
            get {
                return this.maximumHeartRateBpmField;
            }
            set {
                this.maximumHeartRateBpmField = value;
            }
        }
        
        /// <remarks/>
        public Intensity_t Intensity {
            get {
                return this.intensityField;
            }
            set {
                this.intensityField = value;
            }
        }
        
        /// <remarks/>
        public byte Cadence {
            get {
                return this.cadenceField;
            }
            set {
                this.cadenceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CadenceSpecified {
            get {
                return this.cadenceFieldSpecified;
            }
            set {
                this.cadenceFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class HeartRateInBeatsPerMinute_t : HeartRateValue_t {
        
        private byte valueField;
        
        /// <remarks/>
        public byte Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(HeartRateAsPercentOfMax_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(HeartRateInBeatsPerMinute_t))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public abstract partial class HeartRateValue_t {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class HeartRateAsPercentOfMax_t : HeartRateValue_t {
        
        private byte valueField;
        
        /// <remarks/>
        public byte Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public enum Intensity_t {
        
        /// <remarks/>
        Active,
        
        /// <remarks/>
        Resting,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Course_t {
        
        private string nameField;
        
        private CourseLap_t[] lapField;
        
        private Trackpoint_t[] trackField;
        
        private string notesField;
        
        private CoursePoint_t[] coursePointField;
        
        private AbstractSource_t creatorField;
        
        //private Extensions_t extensionsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Lap")]
        public CourseLap_t[] Lap {
            get {
                return this.lapField;
            }
            set {
                this.lapField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Trackpoint", typeof(Trackpoint_t), IsNullable=false)]
        public Trackpoint_t[] Track {
            get {
                return this.trackField;
            }
            set {
                this.trackField = value;
            }
        }
        
        /// <remarks/>
        public string Notes {
            get {
                return this.notesField;
            }
            set {
                this.notesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CoursePoint")]
        public CoursePoint_t[] CoursePoint {
            get {
                return this.coursePointField;
            }
            set {
                this.coursePointField = value;
            }
        }
        
        /// <remarks/>
        public AbstractSource_t Creator {
            get {
                return this.creatorField;
            }
            set {
                this.creatorField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Trackpoint_t {
        
        private System.DateTime timeField;
        
        private Position_t positionField;
        
        private double altitudeMetersField;
        
        private bool altitudeMetersFieldSpecified;
        
        private double distanceMetersField;
        
        private bool distanceMetersFieldSpecified;
        
        private HeartRateInBeatsPerMinute_t heartRateBpmField;
        
        private byte cadenceField;
        
        private bool cadenceFieldSpecified;
        
        private SensorState_t sensorStateField;
        
        private bool sensorStateFieldSpecified;
        
        //private Extensions_t extensionsField;
        
        /// <remarks/>
        public System.DateTime Time {
            get {
                return this.timeField;
            }
            set {
                this.timeField = value;
            }
        }
        
        /// <remarks/>
        public Position_t Position {
            get {
                return this.positionField;
            }
            set {
                this.positionField = value;
            }
        }
        
        /// <remarks/>
        public double AltitudeMeters {
            get {
                return this.altitudeMetersField;
            }
            set {
                this.altitudeMetersField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AltitudeMetersSpecified {
            get {
                return this.altitudeMetersFieldSpecified;
            }
            set {
                this.altitudeMetersFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public double DistanceMeters {
            get {
                return this.distanceMetersField;
            }
            set {
                this.distanceMetersField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DistanceMetersSpecified {
            get {
                return this.distanceMetersFieldSpecified;
            }
            set {
                this.distanceMetersFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public HeartRateInBeatsPerMinute_t HeartRateBpm {
            get {
                return this.heartRateBpmField;
            }
            set {
                this.heartRateBpmField = value;
            }
        }
        
        /// <remarks/>
        public byte Cadence {
            get {
                return this.cadenceField;
            }
            set {
                this.cadenceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CadenceSpecified {
            get {
                return this.cadenceFieldSpecified;
            }
            set {
                this.cadenceFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public SensorState_t SensorState {
            get {
                return this.sensorStateField;
            }
            set {
                this.sensorStateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SensorStateSpecified {
            get {
                return this.sensorStateFieldSpecified;
            }
            set {
                this.sensorStateFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public enum SensorState_t {
        
        /// <remarks/>
        Present,
        
        /// <remarks/>
        Absent,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Application_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Device_t))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public abstract partial class AbstractSource_t {
        
        private string nameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Application_t : AbstractSource_t {
        
        private Build_t buildField;
        
        private string langIDField;
        
        private string partNumberField;
        
        /// <remarks/>
        public Build_t Build {
            get {
                return this.buildField;
            }
            set {
                this.buildField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string LangID {
            get {
                return this.langIDField;
            }
            set {
                this.langIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string PartNumber {
            get {
                return this.partNumberField;
            }
            set {
                this.partNumberField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Build_t {
        
        private Version_t versionField;
        
        private BuildType_t typeField;
        
        private bool typeFieldSpecified;
        
        private string timeField;
        
        private string builderField;
        
        /// <remarks/>
        public Version_t Version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
        
        /// <remarks/>
        public BuildType_t Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TypeSpecified {
            get {
                return this.typeFieldSpecified;
            }
            set {
                this.typeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string Time {
            get {
                return this.timeField;
            }
            set {
                this.timeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string Builder {
            get {
                return this.builderField;
            }
            set {
                this.builderField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Version_t {
        
        private ushort versionMajorField;
        
        private ushort versionMinorField;
        
        private ushort buildMajorField;
        
        private bool buildMajorFieldSpecified;
        
        private ushort buildMinorField;
        
        private bool buildMinorFieldSpecified;
        
        /// <remarks/>
        public ushort VersionMajor {
            get {
                return this.versionMajorField;
            }
            set {
                this.versionMajorField = value;
            }
        }
        
        /// <remarks/>
        public ushort VersionMinor {
            get {
                return this.versionMinorField;
            }
            set {
                this.versionMinorField = value;
            }
        }
        
        /// <remarks/>
        public ushort BuildMajor {
            get {
                return this.buildMajorField;
            }
            set {
                this.buildMajorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BuildMajorSpecified {
            get {
                return this.buildMajorFieldSpecified;
            }
            set {
                this.buildMajorFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public ushort BuildMinor {
            get {
                return this.buildMinorField;
            }
            set {
                this.buildMinorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BuildMinorSpecified {
            get {
                return this.buildMinorFieldSpecified;
            }
            set {
                this.buildMinorFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public enum BuildType_t {
        
        /// <remarks/>
        Internal,
        
        /// <remarks/>
        Alpha,
        
        /// <remarks/>
        Beta,
        
        /// <remarks/>
        Release,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Device_t : AbstractSource_t {
        
        private uint unitIdField;
        
        private ushort productIDField;
        
        private Version_t versionField;
        
        /// <remarks/>
        public uint UnitId {
            get {
                return this.unitIdField;
            }
            set {
                this.unitIdField = value;
            }
        }
        
        /// <remarks/>
        public ushort ProductID {
            get {
                return this.productIDField;
            }
            set {
                this.productIDField = value;
            }
        }
        
        /// <remarks/>
        public Version_t Version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CustomHeartRateZone_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(PredefinedHeartRateZone_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CustomSpeedZone_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(PredefinedSpeedZone_t))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public abstract partial class Zone_t {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class CustomHeartRateZone_t : Zone_t {
        
        private HeartRateValue_t lowField;
        
        private HeartRateValue_t highField;
        
        /// <remarks/>
        public HeartRateValue_t Low {
            get {
                return this.lowField;
            }
            set {
                this.lowField = value;
            }
        }
        
        /// <remarks/>
        public HeartRateValue_t High {
            get {
                return this.highField;
            }
            set {
                this.highField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class PredefinedHeartRateZone_t : Zone_t {
        
        private string numberField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="positiveInteger")]
        public string Number {
            get {
                return this.numberField;
            }
            set {
                this.numberField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class CustomSpeedZone_t : Zone_t {
        
        private SpeedType_t viewAsField;
        
        private double lowInMetersPerSecondField;
        
        private double highInMetersPerSecondField;
        
        /// <remarks/>
        public SpeedType_t ViewAs {
            get {
                return this.viewAsField;
            }
            set {
                this.viewAsField = value;
            }
        }
        
        /// <remarks/>
        public double LowInMetersPerSecond {
            get {
                return this.lowInMetersPerSecondField;
            }
            set {
                this.lowInMetersPerSecondField = value;
            }
        }
        
        /// <remarks/>
        public double HighInMetersPerSecond {
            get {
                return this.highInMetersPerSecondField;
            }
            set {
                this.highInMetersPerSecondField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public enum SpeedType_t {
        
        /// <remarks/>
        Pace,
        
        /// <remarks/>
        Speed,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class PredefinedSpeedZone_t : Zone_t {
        
        private string numberField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="positiveInteger")]
        public string Number {
            get {
                return this.numberField;
            }
            set {
                this.numberField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(None_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Cadence_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(HeartRate_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Speed_t))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public abstract partial class Target_t {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class None_t : Target_t {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Cadence_t : Target_t {
        
        private double lowField;
        
        private double highField;
        
        /// <remarks/>
        public double Low {
            get {
                return this.lowField;
            }
            set {
                this.lowField = value;
            }
        }
        
        /// <remarks/>
        public double High {
            get {
                return this.highField;
            }
            set {
                this.highField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class HeartRate_t : Target_t {
        
        private Zone_t heartRateZoneField;
        
        /// <remarks/>
        public Zone_t HeartRateZone {
            get {
                return this.heartRateZoneField;
            }
            set {
                this.heartRateZoneField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Speed_t : Target_t {
        
        private Zone_t speedZoneField;
        
        /// <remarks/>
        public Zone_t SpeedZone {
            get {
                return this.speedZoneField;
            }
            set {
                this.speedZoneField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(UserInitiated_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CaloriesBurned_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(HeartRateBelow_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(HeartRateAbove_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Distance_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Time_t))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public abstract partial class Duration_t {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class UserInitiated_t : Duration_t {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class CaloriesBurned_t : Duration_t {
        
        private ushort caloriesField;
        
        /// <remarks/>
        public ushort Calories {
            get {
                return this.caloriesField;
            }
            set {
                this.caloriesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class HeartRateBelow_t : Duration_t {
        
        private HeartRateValue_t heartRateField;
        
        /// <remarks/>
        public HeartRateValue_t HeartRate {
            get {
                return this.heartRateField;
            }
            set {
                this.heartRateField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class HeartRateAbove_t : Duration_t {
        
        private HeartRateValue_t heartRateField;
        
        /// <remarks/>
        public HeartRateValue_t HeartRate {
            get {
                return this.heartRateField;
            }
            set {
                this.heartRateField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Distance_t : Duration_t {
        
        private ushort metersField;
        
        /// <remarks/>
        public ushort Meters {
            get {
                return this.metersField;
            }
            set {
                this.metersField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Time_t : Duration_t {
        
        private ushort secondsField;
        
        /// <remarks/>
        public ushort Seconds {
            get {
                return this.secondsField;
            }
            set {
                this.secondsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Step_t))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Repeat_t))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public abstract partial class AbstractStep_t {
        
        private string stepIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="positiveInteger")]
        public string StepId {
            get {
                return this.stepIdField;
            }
            set {
                this.stepIdField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Step_t : AbstractStep_t {
        
        private string nameField;
        
        private Duration_t durationField;
        
        private Intensity_t intensityField;
        
        private Target_t targetField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public Duration_t Duration {
            get {
                return this.durationField;
            }
            set {
                this.durationField = value;
            }
        }
        
        /// <remarks/>
        public Intensity_t Intensity {
            get {
                return this.intensityField;
            }
            set {
                this.intensityField = value;
            }
        }
        
        /// <remarks/>
        public Target_t Target {
            get {
                return this.targetField;
            }
            set {
                this.targetField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Repeat_t : AbstractStep_t {
        
        private string repetitionsField;
        
        private AbstractStep_t[] childField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="positiveInteger")]
        public string Repetitions {
            get {
                return this.repetitionsField;
            }
            set {
                this.repetitionsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Child")]
        public AbstractStep_t[] Child {
            get {
                return this.childField;
            }
            set {
                this.childField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Workout_t {
        
        private string nameField;
        
        private AbstractStep_t[] stepField;
        
        private System.DateTime[] scheduledOnField;
        
        private string notesField;
        
        private AbstractSource_t creatorField;
        
        //private Extensions_t extensionsField;
        
        private Sport_t sportField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Step")]
        public AbstractStep_t[] Step {
            get {
                return this.stepField;
            }
            set {
                this.stepField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ScheduledOn", DataType="date")]
        public System.DateTime[] ScheduledOn {
            get {
                return this.scheduledOnField;
            }
            set {
                this.scheduledOnField = value;
            }
        }
        
        /// <remarks/>
        public string Notes {
            get {
                return this.notesField;
            }
            set {
                this.notesField = value;
            }
        }
        
        /// <remarks/>
        public AbstractSource_t Creator {
            get {
                return this.creatorField;
            }
            set {
                this.creatorField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public Sport_t Sport {
            get {
                return this.sportField;
            }
            set {
                this.sportField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public enum Sport_t {
        
        /// <remarks/>
        Running,
        
        /// <remarks/>
        Biking,
        
        /// <remarks/>
        Other,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class NextSport_t {
        
        private ActivityLap_t transitionField;
        
        private Activity_t activityField;
        
        /// <remarks/>
        public ActivityLap_t Transition {
            get {
                return this.transitionField;
            }
            set {
                this.transitionField = value;
            }
        }
        
        /// <remarks/>
        public Activity_t Activity {
            get {
                return this.activityField;
            }
            set {
                this.activityField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class ActivityLap_t {
        
        private double totalTimeSecondsField;
        
        private double distanceMetersField;
        
        private double maximumSpeedField;
        
        private bool maximumSpeedFieldSpecified;
        
        private ushort caloriesField;
        
        private HeartRateInBeatsPerMinute_t averageHeartRateBpmField;
        
        private HeartRateInBeatsPerMinute_t maximumHeartRateBpmField;
        
        private Intensity_t intensityField;
        
        private byte cadenceField;
        
        private bool cadenceFieldSpecified;
        
        private TriggerMethod_t triggerMethodField;
        
        private Trackpoint_t[] trackField;
        
        private string notesField;
        
        //private Extensions_t extensionsField;
        
        private System.DateTime startTimeField;
        
        /// <remarks/>
        public double TotalTimeSeconds {
            get {
                return this.totalTimeSecondsField;
            }
            set {
                this.totalTimeSecondsField = value;
            }
        }
        
        /// <remarks/>
        public double DistanceMeters {
            get {
                return this.distanceMetersField;
            }
            set {
                this.distanceMetersField = value;
            }
        }
        
        /// <remarks/>
        public double MaximumSpeed {
            get {
                return this.maximumSpeedField;
            }
            set {
                this.maximumSpeedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool MaximumSpeedSpecified {
            get {
                return this.maximumSpeedFieldSpecified;
            }
            set {
                this.maximumSpeedFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public ushort Calories {
            get {
                return this.caloriesField;
            }
            set {
                this.caloriesField = value;
            }
        }
        
        /// <remarks/>
        public HeartRateInBeatsPerMinute_t AverageHeartRateBpm {
            get {
                return this.averageHeartRateBpmField;
            }
            set {
                this.averageHeartRateBpmField = value;
            }
        }
        
        /// <remarks/>
        public HeartRateInBeatsPerMinute_t MaximumHeartRateBpm {
            get {
                return this.maximumHeartRateBpmField;
            }
            set {
                this.maximumHeartRateBpmField = value;
            }
        }
        
        /// <remarks/>
        public Intensity_t Intensity {
            get {
                return this.intensityField;
            }
            set {
                this.intensityField = value;
            }
        }
        
        /// <remarks/>
        public byte Cadence {
            get {
                return this.cadenceField;
            }
            set {
                this.cadenceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CadenceSpecified {
            get {
                return this.cadenceFieldSpecified;
            }
            set {
                this.cadenceFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        public TriggerMethod_t TriggerMethod {
            get {
                return this.triggerMethodField;
            }
            set {
                this.triggerMethodField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Trackpoint", typeof(Trackpoint_t), IsNullable=false)]
        public Trackpoint_t[] Track {
            get {
                return this.trackField;
            }
            set {
                this.trackField = value;
            }
        }
        
        /// <remarks/>
        public string Notes {
            get {
                return this.notesField;
            }
            set {
                this.notesField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime StartTime {
            get {
                return this.startTimeField;
            }
            set {
                this.startTimeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public enum TriggerMethod_t {
        
        /// <remarks/>
        Manual,
        
        /// <remarks/>
        Distance,
        
        /// <remarks/>
        Location,
        
        /// <remarks/>
        Time,
        
        /// <remarks/>
        HeartRate,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Activity_t {
        
        private System.DateTime idField;
        
        private ActivityLap_t[] lapField;
        
        private string notesField;
        
        private Training_t trainingField;
        
        private AbstractSource_t creatorField;
        
        //private Extensions_t extensionsField;
        
        private Sport_t sportField;
        
        /// <remarks/>
        public System.DateTime Id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Lap")]
        public ActivityLap_t[] Lap {
            get {
                return this.lapField;
            }
            set {
                this.lapField = value;
            }
        }
        
        /// <remarks/>
        public string Notes {
            get {
                return this.notesField;
            }
            set {
                this.notesField = value;
            }
        }
        
        /// <remarks/>
        public Training_t Training {
            get {
                return this.trainingField;
            }
            set {
                this.trainingField = value;
            }
        }
        
        /// <remarks/>
        public AbstractSource_t Creator {
            get {
                return this.creatorField;
            }
            set {
                this.creatorField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public Sport_t Sport {
            get {
                return this.sportField;
            }
            set {
                this.sportField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Training_t {
        
        private QuickWorkout_t quickWorkoutResultsField;
        
        private Plan_t planField;
        
        private bool virtualPartnerField;
        
        /// <remarks/>
        public QuickWorkout_t QuickWorkoutResults {
            get {
                return this.quickWorkoutResultsField;
            }
            set {
                this.quickWorkoutResultsField = value;
            }
        }
        
        /// <remarks/>
        public Plan_t Plan {
            get {
                return this.planField;
            }
            set {
                this.planField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool VirtualPartner {
            get {
                return this.virtualPartnerField;
            }
            set {
                this.virtualPartnerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class QuickWorkout_t {
        
        private double totalTimeSecondsField;
        
        private double distanceMetersField;
        
        /// <remarks/>
        public double TotalTimeSeconds {
            get {
                return this.totalTimeSecondsField;
            }
            set {
                this.totalTimeSecondsField = value;
            }
        }
        
        /// <remarks/>
        public double DistanceMeters {
            get {
                return this.distanceMetersField;
            }
            set {
                this.distanceMetersField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Plan_t {
        
        private string nameField;
        
        //private Extensions_t extensionsField;
        
        private TrainingType_t typeField;
        
        private bool intervalWorkoutField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public TrainingType_t Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool IntervalWorkout {
            get {
                return this.intervalWorkoutField;
            }
            set {
                this.intervalWorkoutField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public enum TrainingType_t {
        
        /// <remarks/>
        Workout,
        
        /// <remarks/>
        Course,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class FirstSport_t {
        
        private Activity_t activityField;
        
        /// <remarks/>
        public Activity_t Activity {
            get {
                return this.activityField;
            }
            set {
                this.activityField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class MultiSportSession_t {
        
        private System.DateTime idField;
        
        private FirstSport_t firstSportField;
        
        private NextSport_t[] nextSportField;
        
        private string notesField;
        
        /// <remarks/>
        public System.DateTime Id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public FirstSport_t FirstSport {
            get {
                return this.firstSportField;
            }
            set {
                this.firstSportField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("NextSport")]
        public NextSport_t[] NextSport {
            get {
                return this.nextSportField;
            }
            set {
                this.nextSportField = value;
            }
        }
        
        /// <remarks/>
        public string Notes {
            get {
                return this.notesField;
            }
            set {
                this.notesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class ActivityList_t {
        
        private Activity_t[] activityField;
        
        private MultiSportSession_t[] multiSportSessionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Activity")]
        public Activity_t[] Activity {
            get {
                return this.activityField;
            }
            set {
                this.activityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("MultiSportSession")]
        public MultiSportSession_t[] MultiSportSession {
            get {
                return this.multiSportSessionField;
            }
            set {
                this.multiSportSessionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class CourseFolder_t {
        
        private CourseFolder_t[] folderField;
        
        private NameKeyReference_t[] courseNameRefField;
        
        private string notesField;
        
        //private Extensions_t extensionsField;
        
        private string nameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Folder")]
        public CourseFolder_t[] Folder {
            get {
                return this.folderField;
            }
            set {
                this.folderField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CourseNameRef")]
        public NameKeyReference_t[] CourseNameRef {
            get {
                return this.courseNameRefField;
            }
            set {
                this.courseNameRefField = value;
            }
        }
        
        /// <remarks/>
        public string Notes {
            get {
                return this.notesField;
            }
            set {
                this.notesField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class NameKeyReference_t {
        
        private string idField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string Id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Courses_t {
        
        private CourseFolder_t courseFolderField;
        
        //private Extensions_t extensionsField;
        
        /// <remarks/>
        public CourseFolder_t CourseFolder {
            get {
                return this.courseFolderField;
            }
            set {
                this.courseFolderField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class WorkoutFolder_t {
        
        private WorkoutFolder_t[] folderField;
        
        private NameKeyReference_t[] workoutNameRefField;
        
        //private Extensions_t extensionsField;
        
        private string nameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Folder")]
        public WorkoutFolder_t[] Folder {
            get {
                return this.folderField;
            }
            set {
                this.folderField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("WorkoutNameRef")]
        public NameKeyReference_t[] WorkoutNameRef {
            get {
                return this.workoutNameRefField;
            }
            set {
                this.workoutNameRefField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Workouts_t {
        
        private WorkoutFolder_t runningField;
        
        private WorkoutFolder_t bikingField;
        
        private WorkoutFolder_t otherField;
        
        //private Extensions_t extensionsField;
        
        /// <remarks/>
        public WorkoutFolder_t Running {
            get {
                return this.runningField;
            }
            set {
                this.runningField = value;
            }
        }
        
        /// <remarks/>
        public WorkoutFolder_t Biking {
            get {
                return this.bikingField;
            }
            set {
                this.bikingField = value;
            }
        }
        
        /// <remarks/>
        public WorkoutFolder_t Other {
            get {
                return this.otherField;
            }
            set {
                this.otherField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class MultiSportFolder_t {
        
        private MultiSportFolder_t[] folderField;
        
        private ActivityReference_t[] multisportActivityRefField;
        
        private Week_t[] weekField;
        
        private string notesField;
        
        //private Extensions_t extensionsField;
        
        private string nameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Folder")]
        public MultiSportFolder_t[] Folder {
            get {
                return this.folderField;
            }
            set {
                this.folderField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("MultisportActivityRef")]
        public ActivityReference_t[] MultisportActivityRef {
            get {
                return this.multisportActivityRefField;
            }
            set {
                this.multisportActivityRefField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Week")]
        public Week_t[] Week {
            get {
                return this.weekField;
            }
            set {
                this.weekField = value;
            }
        }
        
        /// <remarks/>
        public string Notes {
            get {
                return this.notesField;
            }
            set {
                this.notesField = value;
            }
        }
        
        /// <remarks/>
        //public Extensions_t Extensions {
        //    get {
        //        return this.extensionsField;
        //    }
        //    set {
        //        this.extensionsField = value;
        //    }
        //}
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public partial class Week_t {
        
        private string notesField;
        
        private System.DateTime startDayField;
        
        /// <remarks/>
        public string Notes {
            get {
                return this.notesField;
            }
            set {
                this.notesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="date")]
        public System.DateTime StartDay {
            get {
                return this.startDayField;
            }
            set {
                this.startDayField = value;
            }
        }
    }
}
