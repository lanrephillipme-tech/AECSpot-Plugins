using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricalSystemCircuits
{
    public class ChangeElementLineStyleModel
    {
        public static bool ChangeLineStyle(Element Element, ElementId GraphicStyleId)
        {
            if (null != Element)
            {
                GetElementEdges(Element).ForEach(cr => cr.SetGraphicsStyleId(GraphicStyleId));
                return true;
            }
            return false;
        }

        private static List<Curve> GetElementEdges(Element element)
        {
            List<Curve> GeometryEdges = new List<Curve>();
            List<XYZ> GeometryPoints = new List<XYZ>();

            Options options = new Options();
            GeometryElement geometryElement = element.get_Geometry(options);
            if (null != geometryElement)
            {
                try
                {
                    foreach (Solid solid in geometryElement)
                    {
                        foreach (Face face in solid.Faces)
                        {
                            if (null != face)
                            {
                                EdgeArrayArray edgeArrayArray = face.EdgeLoops;
                                foreach (EdgeArray edgesArray in edgeArrayArray)
                                {
                                    foreach (Edge edge in edgesArray)
                                    {
                                        GeometryEdges.Add(edge.AsCurve());
                                        GeometryPoints.Add(edge.Evaluate(0));
                                        GeometryPoints.Add(edge.Evaluate(1));
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    try
                    {
                        foreach (GeometryInstance solid in geometryElement)
                        {
                            try
                            {
                                foreach (GeometryObject GeoInstanceObj in solid.GetInstanceGeometry())
                                {
                                    Type objType = GeoInstanceObj.GetType();

                                    if (GeoInstanceObj.GetType() == typeof(Autodesk.Revit.DB.Line))
                                    {
                                        Curve line = (Line)GeoInstanceObj;
                                        GeometryEdges.Add(line);
                                        GeometryPoints.Add(line.GetEndPoint(0));
                                        GeometryPoints.Add(line.GetEndPoint(1));
                                    }
                                    else if (GeoInstanceObj.GetType() == typeof(Autodesk.Revit.DB.Arc))
                                    {
                                        Arc line = (Arc)GeoInstanceObj;
                                        line.MakeBound(0, 1);
                                        GeometryEdges.Add(line);
                                        GeometryPoints.Add(line.GetEndPoint(0));
                                        GeometryPoints.Add(line.Evaluate(0.5, true));
                                        GeometryPoints.Add(line.GetEndPoint(1));
                                    }
                                    else
                                    {
                                        Solid sold = (Solid)GeoInstanceObj;
                                        foreach (Face face in sold.Faces)
                                        {
                                            if (null != face)
                                            {
                                                EdgeArrayArray edgeArrayArray = face.EdgeLoops;
                                                foreach (EdgeArray edgesArray in edgeArrayArray)
                                                {
                                                    foreach (Edge edge in edgesArray)
                                                    {
                                                        foreach (XYZ pnt in edge.Tessellate())
                                                        {
                                                            GeometryPoints.Add((XYZ)pnt);
                                                        }
                                                        GeometryEdges.Add(edge.AsCurve());
                                                        GeometryPoints.Add(edge.Evaluate(0));
                                                        GeometryPoints.Add(edge.Evaluate(1));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    catch (Exception e) { System.Windows.MessageBox.Show($"You Can't Calculate The Distance Because : {e.Message}"); }
                }
            }
            return GeometryEdges;
        }
    }
}