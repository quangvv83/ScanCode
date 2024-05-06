using System.Windows;
using System.Windows.Interactivity;

namespace ScanCode.Behaviors
{
    public class DropBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty DroppedDataProperty =
            DependencyProperty.Register(
                nameof(DroppedData),
                typeof(object),
                typeof(DropBehavior),
                new PropertyMetadata(null));

        public object DroppedData
        {
            get { return GetValue(DroppedDataProperty); }
            set { SetValue(DroppedDataProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.Drop += AssociatedObject_Drop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Drop -= AssociatedObject_Drop;
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                // For simplicity, just taking the first dropped file
                DroppedData = files[0];
            }
        }
    }
}
