using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AECSpot
{
    public class TransferFilterViewModel : ViewModelBase
    {
        #region Properties
        private string _counter { get; set; } = "0";
        #region Revit documents

        /// <summary>
        /// the document that the standereds will be transfer from
        /// </summary>
        public Document HostDocument { get; internal set; }
        public List<ParameterFilterElement> ViewFilters { get; set; }
        public ParameterFilterElement SelectedFilter { get; set; }

        public string Counter
        {
            get { return _counter; }
            set
            {
                _counter = value;
                OnPropertyChanged(nameof(Counter));
            }
        }



        #endregion Revit documents

        #region Get Open Documents


        #endregion Get Open Documents

        #region Tree Root Element

        private TreeViewElement _Root;

        /// <summary>
        /// List of categories that contain all editible elements
        /// </summary>
        public TreeViewElement Root
        {
            get
            {

                if (HostDocument != null && _Root == null)
                {
                    GetFamiliesData(HostDocument);/* _counter = (int)(_Root?.GetSelected().Count);*/
                }
                    return _Root;
            }
            set
            {
                _Root = value;
                //_counter = (int)(_Root?.GetSelected().Count);
                OnPropertyChanged(nameof(Root));
            }
        }

        #endregion Tree Root Element

        #endregion Properties

        #region Methods

        /// <summary>
        /// Get all families informations from the selected document
        /// </summary>
        /// <param name="doc"> </param>
        private void GetFamiliesData(Document doc)
        {
           
            var root = new TreeViewElement() { Children = new List<TreeViewElement>(), Name = "Views" };

            var categories = new List<Category>() { Category.GetCategory(doc, BuiltInCategory.OST_Views) };
            var views = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).ToList();

            var rootSections = new Dictionary<string, TreeViewElement>();
            var viewsTypes=views.Select(v => ((View)v).ViewType.ToString()).Distinct();
            foreach (var category in categories)
            {
                foreach (var view in viewsTypes)
                {
                    TreeViewElement categorySection = null;
                    if (view=="ThreeD")
                    {
                        CreateRootSectionIfNotExist(root, rootSections, "3D");
                        categorySection = rootSections["3D"];
                    }
                    else
                    {
                        CreateRootSectionIfNotExist(root, rootSections, view);
                        categorySection = rootSections[view];
                    }
                    List<TreeViewElement> CategoryElementsdata = ConvertElemntsToTreeElements(views.Where(v => ((View)v).ViewType.ToString()== view).ToList(), categorySection);
                    categorySection.Children.AddRange(CategoryElementsdata);
                }
            }
            foreach (var item in rootSections) root.Children.Add(item.Value);
            Root = root;
            //_counter = (_Root?.GetSelected().Count.ToString());
        }

        public static List<TreeViewElement> ConvertElemntsToTreeElements(List<Element> families, TreeViewElement parent)
        {
            var elements = new List<TreeViewElement>();
            foreach (var fam in families)
            {
                try
                {
                    var familyItem = CreateElement(fam, parent);
                    familyItem.Children = null;
                    elements.Add(familyItem);
                }
                catch (Exception) { }
            }
            return elements;
        }

        /// <summary>
        /// Create new tree view element using revit element
        /// </summary>
        /// <param name="element"> revit element </param>
        /// <param name="parent">  the ceraed element parent </param>
        /// <returns> </returns>
        private static TreeViewElement CreateElement(Element element, TreeViewElement parent)
        {
            return new TreeViewElement()
            {
                Name = element.Name,
                Id = element.Id,
                CategoryName = element.Name,
                CategoryId = element.Id,
                Parent = parent,
                Children = new List<TreeViewElement>(),
                IsChecked = false
            };
        }

        /// <summary>
        /// Create new tree view element using revit category
        /// </summary>
        /// <param name="element"> revit element </param>
        /// <param name="parent">  the ceraed element parent </param>
        /// <returns> </returns>
        private static TreeViewElement CreateElement(Category category, TreeViewElement parent)
        {
            return new TreeViewElement()
            {
                Name = category.Name,
                Id = null,
                CategoryName = category.Name,
                CategoryId = category.Id,
                Parent = parent,
                Children = new List<TreeViewElement>(),
                IsChecked = false
            };
        }

        /// <summary>
        /// initialize a new section in the root element if this section not exisit [ex] Annotation
        /// section if not exist create the section
        /// </summary>
        /// <param name="parent">       the element parent </param>
        /// <param name="rootSections">
        /// the root element dictionary that contain all sections ,,,, the sections name and section element
        /// </param>
        /// <param name="categoryType"> </param>
        public static void CreateRootSectionIfNotExist(TreeViewElement parent, Dictionary<string, TreeViewElement> rootSections, string categoryType)
        {
            if (!rootSections.ContainsKey(categoryType))
            {
                rootSections.Add(categoryType, new TreeViewElement()
                {
                    Name = categoryType,
                    Children = new List<TreeViewElement>(),
                    Parent = parent
                });
            }
        }

        /// <summary>
        /// Get all elements of specific type
        /// </summary>
        /// <param name="document"> the current open document </param>
        /// <param name="type">     type of class to get it children </param>
        /// <returns> </returns>
        public List<Family> GetEditableFamilies(Document document, Category category)
        {
            try
            {
                FilteredElementCollector BeforeAddFamilieselements = new FilteredElementCollector(document);
                return BeforeAddFamilieselements.OfClass(typeof(Family)).Where(fam =>
                {
                    var IsEditible = ((Family)fam).IsEditable;
                    var IsNotNullCategory = ((Family)fam).FamilyCategory != null;
                    if (IsEditible && IsNotNullCategory)
                    {
                        if (((Family)fam).FamilyCategoryId == category.Id)
                        {
                            return true;
                        }
                    }
                    return false;
                }).Cast<Family>().ToList();
            }
            catch (Exception)
            {
                return new List<Family>();
            }
        }



        #endregion Methods







    }
}
