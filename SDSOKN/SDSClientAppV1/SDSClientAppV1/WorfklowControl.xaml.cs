using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Presentation;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.Toolbox;
using System.Activities.Statements;
using System.ComponentModel;
using System.Reflection;

namespace SDSClientAppV1
{

    /// <summary>
    /// Interaction logic for WorfklowControl.xaml
    /// </summary>
    public partial class WorfklowControl : UserControl
    {
        private WorkflowDesigner wd;
        public WorfklowControl()
        {
            InitializeComponent();
            // Register the metadata.
            RegisterMetadata();

            // Add the WFF Designer.
            AddDesigner();
            this.AddToolBox();
            this.AddPropertyInspector();
        }
    

        private void AddDesigner()
        {
            // Create an instance of WorkflowDesigner class.
            this.wd = new WorkflowDesigner();

            // Place the designer canvas in the middle column of the grid.
            Grid.SetColumn(this.wd.View, 1);

            // Load a new Sequence as default.
            this.wd.Load(new Sequence());

            // Add the designer canvas to the grid.
            grid1.Children.Add(this.wd.View);
        }
        private void RegisterMetadata()
        {
            var dm = new DesignerMetadata();
            dm.Register();
        }
        private void AddToolBox()
        {
            ToolboxControl tc = GetToolboxControl();
            Grid.SetColumn(tc, 0);
            grid1.Children.Add(tc);
        }
        private ToolboxControl GetToolboxControl()
        {
            var ctrl = new ToolboxControl();

            // Create a category.
            var category = new ToolboxCategory("category1");

            // Create Toolbox items.
            var tool1 =
                new ToolboxItemWrapper("System.Activities.Statements.Assign",
                    typeof(Assign).Assembly.FullName, null, "Assign");

            var tool2 = new ToolboxItemWrapper("System.Activities.Statements.Sequence",
                typeof(Sequence).Assembly.FullName, null, "Sequence");


            // Add the Toolbox items to the category.
            category.Add(tool1);
            category.Add(tool2);

            // Add the category to the ToolBox control.
            ctrl.Categories.Add(category);
            return ctrl;
        }
        private void AddPropertyInspector()
        {
            Grid.SetColumn(wd.PropertyInspectorView, 2);
            grid1.Children.Add(wd.PropertyInspectorView);
        }
    }
}
