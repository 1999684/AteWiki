using AtelierWiki.Data;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AtelierWiki.Components.Dock
{
    public partial class DockBar : UserControl
    {
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<DockItemData>), typeof(DockBar));

        public ObservableCollection<DockItemData> Items
        {
            get { return (ObservableCollection<DockItemData>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register("ItemClickCommand", typeof(ICommand), typeof(DockBar));

        public ICommand ItemClickCommand
        {
            get { return (ICommand)GetValue(ItemClickCommandProperty); }
            set { SetValue(ItemClickCommandProperty, value); }
        }

        private const double BaseSize = 50;
        private const double MaxSize = 90;
        private const double EffectRange = 200;

        public DockBar()
        {
            InitializeComponent();
        }

        private void Dock_MouseMove(object sender, MouseEventArgs e)
        {
            var stackPanel = FindVisualChild<StackPanel>(DockItemsControl);
            if (stackPanel == null) return;

            Point mousePos = e.GetPosition(stackPanel);

            foreach (ContentPresenter itemContainer in stackPanel.Children)
            {
                var grid = FindVisualChild<Grid>(itemContainer);
                if (grid == null) continue;

                Point itemPos = itemContainer.TransformToAncestor(stackPanel).Transform(new Point(0, 0));
                double centerX = itemPos.X + (grid.ActualWidth / 2);

                double distance = Math.Abs(mousePos.X - centerX);

                double targetSize = BaseSize;

                if (distance < EffectRange)
                {
                    double ratio = (distance / EffectRange);
                    if (ratio > 1) ratio = 1;


                    double val = Math.Cos(ratio * Math.PI / 2);

                    val = Math.Pow(val, 2);

                    targetSize = BaseSize + (MaxSize - BaseSize) * val;
                }

                grid.Width = targetSize;
                grid.Height = targetSize;
            }
        }

        private void Dock_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetSizes();
        }

        private void ResetSizes()
        {
            var stackPanel = FindVisualChild<StackPanel>(DockItemsControl);
            if (stackPanel == null) return;

            foreach (ContentPresenter itemContainer in stackPanel.Children)
            {
                var grid = FindVisualChild<Grid>(itemContainer);
                if (grid != null)
                {
                    grid.Width = BaseSize;
                    grid.Height = BaseSize;
                }
            }
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild) return typedChild;

                var result = FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        private void DockItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid grid && grid.Tag is DockItemData data)
            {
                if (ItemClickCommand != null && ItemClickCommand.CanExecute(data))
                {
                    ItemClickCommand.Execute(data);
                }
            }
            e.Handled = true; // 阻止事件冒泡
        }
    }
}
