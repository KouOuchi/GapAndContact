using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GapCondition;
using GapCondition.Model;
using GapCondition.ViewModel;


namespace GapCondition
{
    /// <summary>
    /// Interaction logic for NewDesign.xaml
    /// </summary>
    public partial class NewDesign : Window
    {
        private InfomationViewModel viewmodel;

        //Scroll
        ScrollViewer scrollViewer1;

        public NewDesign()
        {
            InitializeComponent();

            viewmodel = new InfomationViewModel();
            this.DataContext = viewmodel;
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ColorPicker pic = new ColorPicker();
            pic.ShowDialog();
            if(pic.IsCanceled)
                return;
            Rectangle rec = sender as Rectangle;
            rec.Fill = new SolidColorBrush(pic.ColorSel);
            //var abc = viewmodel.Infos;
        }

        private void NewDesign_OnLoaded(object sender, RoutedEventArgs e)
        {
        }
        
        private T FindChildOfType<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindChildOfType<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        private void Gridview_OnLoaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer viewer = (ScrollViewer)FindChildOfType<ScrollViewer>(gridview);
            if (viewer != null)
            {
                scrollViewer1 = viewer;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                viewmodel.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public SortedDictionary<double, System.Drawing.Color> GetMinDistanceRanges()
        {
            SortedDictionary<double, System.Drawing.Color> ret = new SortedDictionary<double, System.Drawing.Color>();

            foreach(var m in viewmodel.Infos)
            {
                System.Drawing.Color c = 
                   System.Drawing.Color.FromArgb(m.Colors.R, m.Colors.G, m.Colors.B);
                ret.Add(m.MinBound, c);
            }

            return ret;
        }
    }
}
