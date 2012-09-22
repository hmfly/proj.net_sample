using ProjNet.Converters.WellKnownText;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.IO;

namespace prj2mercator
{
    class Program
    {
        static void Main(string[] args)
        {
            var wktWGS84 = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]]";
            var wgs84 = CoordinateSystemWktReader.Parse(wktWGS84) as ICoordinateSystem;
            var wktMercator = "PROJCS[\"World_Mercator\",GEOGCS[\"GCS_WGS_1984\",DATUM[\"WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Mercator_1SP\"],PARAMETER[\"False_Easting\",0],PARAMETER[\"False_Northing\",0],PARAMETER[\"Central_Meridian\",0],PARAMETER[\"latitude_of_origin\",0],UNIT[\"Meter\",1]]";
            var mercator = CoordinateSystemWktReader.Parse(wktMercator) as ICoordinateSystem;

            var transFactory = new CoordinateTransformationFactory();
            var trans = transFactory.CreateFromCoordinateSystems(wgs84, mercator);

            var fromPoints = new List<double[]>();
            using (var sr = new StreamReader("32332.dat"))
            {
                while (!sr.EndOfStream)
                {
                    var val = sr.ReadLine().Split("".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (val[0].Contains("nan") || val[1].Contains("nan"))
                    {
                        continue;
                    }

                    var x = double.Parse(val[0]);
                    var y = double.Parse(val[1]);
                    fromPoints.Add(new double[] { x, y });
                }
            }

            var toPoints = trans.MathTransform.TransformList(fromPoints);

            using (var sw = new StreamWriter("result.dat"))
            {
                foreach (var toPoint in toPoints)
                {
                    sw.WriteLine("{0} {1}", toPoint[0], toPoint[1]);
                }
            }
        }      
    }
}