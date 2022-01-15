using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

namespace MobileBandSync.Common
{
    public class WorkoutFilterData
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public string strSearchText;
        public bool? IsRunningWorkout { get; set; }
        public bool? IsBikingWorkout { get; set; }
        public bool? IsWalkingWorkout { get; set; }
        public bool? IsSleepingWorkout { get; set; }
        
        public GeoboundingBox MapBoundingBox;
        public bool MapSelected = false;

        //--------------------------------------------------------------------------------------------------------------------
        public WorkoutFilterData()
        //--------------------------------------------------------------------------------------------------------------------
        {

        }

        //--------------------------------------------------------------------------------------------------------------------
        public GeoboundingBox SetBounds( MapControl map )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( map == null )
            {
                MapBoundingBox = null;
            }
            else
            {
                Geopoint topLeft = null;

                try
                {
                    map.GetLocationFromOffset( new Windows.Foundation.Point( 0, 0 ), out topLeft );
                }
                catch
                {
                    var topOfMap = new Geopoint( new BasicGeoposition()
                    {
                        Latitude = 85,
                        Longitude = 0
                    } );

                    Windows.Foundation.Point topPoint;
                    map.GetOffsetFromLocation( topOfMap, out topPoint );
                    map.GetLocationFromOffset( new Windows.Foundation.Point( 0, topPoint.Y ), out topLeft );
                }

                Geopoint bottomRight = null;
                try
                {
                    map.GetLocationFromOffset( new Windows.Foundation.Point( map.ActualWidth, map.ActualHeight ), out bottomRight );
                }
                catch
                {
                    var bottomOfMap = new Geopoint( new BasicGeoposition()
                    {
                        Latitude = -85,
                        Longitude = 0
                    } );

                    Windows.Foundation.Point bottomPoint;
                    map.GetOffsetFromLocation( bottomOfMap, out bottomPoint );
                    map.GetLocationFromOffset( new Windows.Foundation.Point( 0, bottomPoint.Y ), out bottomRight );
                }

                if( topLeft != null && bottomRight != null )
                {
                    MapBoundingBox = new GeoboundingBox( topLeft.Position, bottomRight.Position );
                    return MapBoundingBox;
                }
            }

            return null;
        }
    }
}
