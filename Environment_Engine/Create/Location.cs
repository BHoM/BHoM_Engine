using BH.oM.Environmental.Elements;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Location Location(string placename, double latitude, double longitude, double elevation)
        {
            return new Location
            {
                Name = placename,
                Latitude = latitude,
                Longitude = longitude,
                Elevation = elevation
            };
        }

        /***************************************************/
    }
}
