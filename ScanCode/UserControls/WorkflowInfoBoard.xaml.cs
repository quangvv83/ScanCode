using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScanCode.UserControls
{
    /// <summary>
    /// Interaction logic for WorkflowInfoBoard.xaml
    /// </summary>
    public partial class WorkflowInfoBoard : UserControl
    {
        public WorkflowInfoBoard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(WorkflowInfoBoard));
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(WorkflowInfoBoard));
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(string), typeof(WorkflowInfoBoard));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(DrawingImage), typeof(WorkflowInfoBoard));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
        public string Data
        {
            get => (string)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
        public DrawingImage Icon
        {
            get => (DrawingImage)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}
