using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Document = Autodesk.Revit.DB.Document;

namespace AECSpot
{
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class EODCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region Current Application Session
            ///geting the current  ui document
            UIDocument _uiDoc = commandData.Application.ActiveUIDocument;
            ///geting the current document
            Document _currentDoc = commandData.Application.ActiveUIDocument.Document;
            var selectedElements = _uiDoc.Selection.GetElementIds();
            if (selectedElements.Count == 0)
            {
                selectedElements = new List<ElementId>();
                try
                {
                    var selectedElementRefrence = _uiDoc.Selection.PickObjects(ObjectType.Element, "Select Objects To Apply Override To");
                    if (selectedElementRefrence == null || selectedElementRefrence?.Count == 0) return Result.Succeeded;
                    foreach (var reference in selectedElementRefrence)
                    {
                        selectedElements.Add(reference.ElementId);
                    }
                }
                catch
                {

                    message = "Operation Aborted.";
                    return Result.Failed;
                }
            }
            var elementOverrides = new OverrideGraphicSettings();

            #endregion Current Application Session
            var dialog = new TransferElementsGraphics(_uiDoc, selectedElements);
            if (dialog.ShowDialog() == true)
            {

            }

            return Result.Succeeded;
        }
    }
    [System.Serializable]
    public class OverrideElementModel
    {
        [XmlAttribute]
        public Guid Guid { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public bool Visible { get; set; }
        [XmlAttribute]
        public bool Halftone { get; set; }
        [XmlAttribute]
        public string ProjectionLineName { get; set; }
        [XmlAttribute]
        public string ProjectionLineWeight { get; set; }
        [XmlIgnore]
        public Autodesk.Revit.DB.Color ProjectionLineColor { get; set; }
        [XmlAttribute]
        public string ProjectionLineColorString { get; set; }
        [XmlAttribute]
        public bool SurfaceForegroundVisible { get; set; }
        [XmlAttribute]
        public string SurfaceForegroundPatternName { get; set; }
        [XmlIgnore]
        public Autodesk.Revit.DB.Color SurfaceForegroundColor { get; set; }
        [XmlAttribute]
        public string SurfaceForegroundColorString { get; set; }


        [XmlAttribute]
        public bool SurfaceBackgroundVisible { get; set; }
        [XmlAttribute]
        public string SurfaceBckgroundPatternName { get; set; }
        [XmlIgnore]
        public Autodesk.Revit.DB.Color SurfaceBckgroundColor { get; set; }
        [XmlAttribute]
        public string SurfaceBckgroundColorString { get; set; }
        [XmlAttribute]
        public string CutLineName { get; set; }
        [XmlAttribute]
        public string CutLineWeight { get; set; }
        [XmlIgnore]
        public Autodesk.Revit.DB.Color CutLineColor { get; set; }
        [XmlAttribute]
        public string CutLineColorString { get; set; }
        [XmlAttribute]
        public bool CutForegroundVisible { get; set; }
        [XmlAttribute]
        public string CutForegroundPatternName { get; set; }
        [XmlIgnore]
        public Autodesk.Revit.DB.Color CutForegroundColor { get; set; }
        [XmlAttribute]
        public string CutForegroundColorString { get; set; }
        [XmlAttribute]
        public bool CutBackgroundVisible { get; set; }
        [XmlAttribute]
        public string CutBckgroundPatternName { get; set; }
        [XmlIgnore]
        public Autodesk.Revit.DB.Color CutBckgroundColor { get; set; }
        [XmlAttribute]
        public string CutBckgroundColorString { get; set; }
        [XmlAttribute]
        public int Transparency { get; set; }
        public void ConvertColorsToString()
        {
            ProjectionLineColorString = ConvertColorToString(ProjectionLineColor);
            SurfaceBckgroundColorString = ConvertColorToString(SurfaceBckgroundColor);//SurfaceBckgroundColor
            CutLineColorString = ConvertColorToString(CutLineColor);// CutLineColor
            CutForegroundColorString = ConvertColorToString(CutForegroundColor);//CutForegroundColor
            SurfaceForegroundColorString = ConvertColorToString(SurfaceForegroundColor);// SurfaceForegroundColor
            CutBckgroundColorString = ConvertColorToString(CutBckgroundColor);//CutBckgroundColor
        }
        public void ConvertStringToColors()
        {
            ProjectionLineColor = ConvertStringToColor(ProjectionLineColorString);
            SurfaceBckgroundColor = ConvertStringToColor(SurfaceBckgroundColorString);//SurfaceBckgroundColor
            CutLineColor = ConvertStringToColor(CutLineColorString);// CutLineColor
            CutForegroundColor = ConvertStringToColor(CutForegroundColorString);//CutForegroundColor
            SurfaceForegroundColor = ConvertStringToColor(SurfaceForegroundColorString);// SurfaceForegroundColor
            CutBckgroundColor = ConvertStringToColor(CutBckgroundColorString);//CutBckgroundColor
        }
        private string ConvertColorToString(Autodesk.Revit.DB.Color color)
        {
            if (color == null) return "";
            if (color.IsValid)
            {
                return $"{color.Red},{color.Green},{color.Blue}";
            }
            return "";
        }
        private Autodesk.Revit.DB.Color ConvertStringToColor(string color)
        {
            if (color == "" || color == null)
            {
                return Color.InvalidColorValue;

            }

            var colors = color.Split(',');
            var Red = colors[0];
            var Green = colors[1];
            var Blue = colors[2];
            return new Autodesk.Revit.DB.Color(byte.Parse(Red), byte.Parse(Green), byte.Parse(Blue));
        }

    }
    public class SerializerManager
    {
        #region XML Serializer

        /// <summary>
        /// append the new element to the end of the parent elment
        /// </summary>
        /// <param name="MainContainertObjectToSerialze">
        /// Host elment that contain the childes elments
        /// </param>
        /// <param name="ObjectToSerialze">              
        /// The elment that will bew serialized to the host
        /// </param>
        /// <param name="XMLFilePath">                    The document path </param>
        public static void MultpleElementsXMLSerializer(object MainContainertObjectToSerialze, object ObjectToSerialze, string XMLFilePath)
        {
            ///Create the document file
            ///Get the container element to append all other children
            ///append child ,then save the file

            #region Initial File Creation

            CreateRootFile(MainContainertObjectToSerialze, XMLFilePath);

            #endregion Initial File Creation

            #region Step-01 Initialize the document setup

            XmlDocument document = new XmlDocument();
            document.Load(XMLFilePath);

            #endregion Step-01 Initialize the document setup

            #region Step-02 Get the root element

            var rootNode = document.GetElementsByTagName(MainContainertObjectToSerialze.GetType().Name)[0];
            var nav = rootNode.CreateNavigator();

            #endregion Step-02 Get the root element

            #region Append The New Child

            using (var writer = nav.AppendChild())
            {
                if (null != ObjectToSerialze)
                {
                    var serializer = new XmlSerializer(ObjectToSerialze.GetType());
                    writer.WriteWhitespace("");
                    serializer.Serialize(writer, ObjectToSerialze);
                    writer.Close();
                }
            }
            document.Save(XMLFilePath);

            #endregion Append The New Child
        }

        /// <summary>
        /// Sreialze object to Specific location path
        /// </summary>
        /// <param name="ObjectToSerialze"> Object to serialze </param>
        /// <param name="XMLFilePath">      XML file locations </param>
        public static void SingelXMLFileSerialization(object ObjectToSerialze, string XMLFilePath)
        {
            
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialze.GetType());
            using (var writer = new StreamWriter(XMLFilePath,false))
            {
                xmlSerializer.Serialize(writer, ObjectToSerialze, new XmlSerializerNamespaces());
            }
        }

        /// <summary>
        /// DeSerialize objects from xml file to entities
        /// </summary>
        /// <param name="ReturnedParenttype"> The container that contain the elmement : FamiliesSerilizeContainer </param>
        /// <param name="XMLFilePath">        The stream source </param>
        /// <returns> </returns>
        public static object DeSerializationFromXML(object serialiazationParent, string XMLFilePath)
        {
            if (File.Exists(XMLFilePath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(serialiazationParent.GetType());
                using (var Reader = new StreamReader(XMLFilePath))
                {
                    serialiazationParent = (xmlSerializer.Deserialize(Reader));
                    return serialiazationParent;
                }
            }
            return null;
        }

        #region Private Initial Create

        private static void CreateRootFile(object MainContainertObjectToSerialze, string XMLFilePath)
        {
            if (!File.Exists(XMLFilePath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(MainContainertObjectToSerialze.GetType());
                using (var writer = new StreamWriter(XMLFilePath, false))
                {
                    xmlSerializer.Serialize(writer, MainContainertObjectToSerialze);
                }
            }
        }

        #endregion Private Initial Create

        #endregion XML Serializer
    }

}