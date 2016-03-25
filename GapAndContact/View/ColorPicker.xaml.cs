using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Xml;

namespace GapCondition
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window
    {
        private List<Color> listWPFColors;
        private bool BlockEvent;
        private int _IndexSel;                      // Index of a selected color (0..140), -1 if no item is selected.
        private Color _ColorSel;
        private Color _ColorSelOld;                 // Previous selected color.
        private bool isCanceled;                    // =true: user has canceled.

        public bool IsCanceled
        {
            get { return isCanceled; }
            set { isCanceled = value; }
        }

        public ColorPicker()
        {
            

            InitializeComponent();

            listWPFColors = new List<Color>();
            BlockEvent = false;
            _IndexSel = 0;
            isCanceled = false;

            ListBoxColors.BorderBrush = Brushes.DarkGray;
            ListBoxColors.BorderThickness = new Thickness(1);
            ListBoxColors.Padding = new Thickness(4);
            ListBoxColors.FontFamily = new FontFamily("Verdana");
            ListBoxColors.FontSize = 12.0;
            ListBoxColors.SelectionMode = SelectionMode.Single;

            buttonUp.Content = ImageFromResource("Up_16.png", 16);
            buttonUp.Focusable = false;

            buttonDown.Content = ImageFromResource("Down_16.png", 16);
            buttonDown.Focusable = false;

            txtHexColor.BorderThickness = new Thickness(1);
            txtHexColor.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            txtHexColor.Margin = new System.Windows.Thickness(0);
            txtHexColor.Padding = new Thickness(3);
            txtHexColor.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            txtHexColor.TextWrapping = TextWrapping.NoWrap;
            txtHexColor.FontFamily = new FontFamily("Lucida Console");
            txtHexColor.FontSize = 14;
            txtHexColor.AcceptsReturn = false;
            txtHexColor.Text = "";

            GetColorList();
        }


        // ---------------------------------------------------------------
        // Date      261011
        // Purpose   Get an Image from /Resources (build action = resource).
        // Entry     sFileName - The filename of the image (no path).
        //           iImageWidth - The width of the image in pixels.
        // Return    The Image.
        // Comments  1) Returns a System.Windows.Controls.Image.
        //           2) Supports .bmp, .gif, .ico, .jpg, .png, .wdp, .tiff.
        // ---------------------------------------------------------------
        private Image ImageFromResource(string sFileName, int iImageWidth)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            Uri uri = new Uri("pack://application:,,,/Resources/" + sFileName,
                           UriKind.Absolute);
            bi.UriSource = uri;
            // In order to preserve aspect ratio, set DecodePixelWidth
            // or DecodePixelHeight but not both.
            bi.DecodePixelWidth = iImageWidth;
            try
            {
                if (Application.ResourceAssembly == null)
                    Application.ResourceAssembly = typeof(NewDesign).Assembly;
                bi.EndInit();
            }
            catch (Exception ex)
            {
                
            }
            
            // Make Image.
            Image oImage = new Image();
            oImage.Width = iImageWidth;
            oImage.Margin = new Thickness(0);
            oImage.SnapsToDevicePixels = true;
            oImage.Source = bi;
            return oImage;
        }

        // Property - The selected color.
        // Can be set before the window is loaded.
        public Color ColorSel
        {
            get { return _ColorSel; }
            set 
            { 
                _ColorSel = value;
                _ColorSelOld = _ColorSel;
                // Show color in UI.
                BlockEvent = true;
                SetColorUI(_ColorSel);
                BlockEvent = false;
            }
        }

        // Set a color in the user interface.
        // NewColor - The color to show.
        private void SetColorUI(Color NewColor)
        {
            // Set hex color input box.
            txtHexColor.Text = NewColor.ToString();

            // Set color panel.
            recColor.Fill = new SolidColorBrush(NewColor);

            // Select item in list if this is a predefined color.
            int iCount = -1;
            bool IsPredefinedColor = false;
            foreach (Color color in listWPFColors)
            {
                iCount++;
                if (NewColor.Equals(color))
                {
                    _IndexSel = iCount;
                    _ColorSel = color;
                    ListBoxColors.SelectedIndex = iCount;
                    ListBoxColors.ScrollIntoView(ListBoxColors.Items[iCount]);
                    //txtColorNum.Text = (iCount + 1).ToString();
                    IsPredefinedColor = true;
                    break;
                }
            }

            if (IsPredefinedColor == false)
            {
                ListBoxColors.UnselectAll();
                _IndexSel = -1;
                //txtColorNum.Text = "";
            }
        }

        #region Event

        // The user confirms selection of a color.
        // The color is available in property ColorSel.
        // Calls ColorDialog_Closing().
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            isCanceled = false;
            this.Close();
        }

        // The user cancels.
        // Calls ColorDialog_Closing().
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            isCanceled = true;
            
            this.Close();
        }

        // OK 311011
        // The user selects a new color in the list,
        // or the Up/Down buttons are used.
        private void ListBoxColors_SelectionChanged(object sender,
                                        SelectionChangedEventArgs e)
        {
            if (BlockEvent == true) { return; }
            BlockEvent = true;

            ListBox oListBox = (ListBox)sender;
            if (oListBox.SelectedItem != null)
            {
                _ColorSel = listWPFColors[oListBox.SelectedIndex];
                SetColorUI(_ColorSel);
            }

            BlockEvent = false;
        }

        // Last things to do.
        private void ColorPicker_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isCanceled == true)
            {
                // Restore original color.
                _ColorSel = _ColorSelOld;
            }
        }

        // Previous predefined color is selected.
        // Calls listPredefColor_SelectionChanged().
        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            _IndexSel -= 1;
            if (_IndexSel < 0)
            {
                _IndexSel = 0;
            }
            ListBoxColors.SelectedIndex = _IndexSel;
        }

        // Input box 'Hex' is changed.
        // Format: "#AARRGGBB"
        private void txtHexColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BlockEvent == true) { return; }
            BlockEvent = true;

            try
            {
                // Get color from hex color string.
                TextBox TBox = (TextBox)sender;
                string sHexColor = TBox.Text.Trim();
                _ColorSel = (Color)ColorConverter.ConvertFromString(sHexColor);
                SetColorUI(_ColorSel);
            }
            catch
            {
                // Unselect item in list.
                ListBoxColors.UnselectAll();
                _IndexSel = -1;
                // Clear color field.
                recColor.Fill = Brushes.Transparent;
            }

            BlockEvent = false;
        }
        #endregion Event

        private void GetColorList()
        {
            string sXml;

            Uri uri = new Uri("pack://application:,,,/Resources/WPFColors_02.xml",
                                                                UriKind.Absolute);
            
            //Uri uri = new System.Uri(@"D:\ListViewTest\ListViewTest\Resources\WPFColors_02.xml", UriKind.Absolute);
            //if(!File.Exists(uri.AbsolutePath))
                //return;
            try
            {
                StreamResourceInfo sr1 = Application.GetResourceStream(uri);
            }
            catch (Exception ex)
            {
                
                
            }
            StreamResourceInfo sr = Application.GetResourceStream(uri);
            using (StreamReader reader = new StreamReader(sr.Stream))
            {
                sXml = reader.ReadToEnd();
            }
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(sXml);

            string sColorName;
            string sHexaColor;
            Color predefColor;

            BlockEvent = true;

            XmlElement root = xDoc.DocumentElement;             // <table>
            XmlNodeList list = root.ChildNodes;                 // All <tr>..</tr>
            foreach (XmlNode tr in list)
            {
                sColorName = tr.ChildNodes[0].InnerText;        // <td>colorname<td>
                sHexaColor = tr.ChildNodes[1].InnerText;        // <td>hexcolor<td>
                // "#AARRGGBB" -> A,R,G,B.
                byte A = Convert.ToByte(sHexaColor.Substring(1, 2), 16);
                byte R = Convert.ToByte(sHexaColor.Substring(3, 2), 16);
                byte G = Convert.ToByte(sHexaColor.Substring(5, 2), 16);
                byte B = Convert.ToByte(sHexaColor.Substring(7, 2), 16);
                predefColor = Color.FromArgb(A, R, G, B);
                // Store color in list for later use.
                listWPFColors.Add(predefColor);
                // Add color name to ListBox in UI.
                ListBoxColors.Items.Add(sColorName);
            }

            BlockEvent = false;

            // Select first color.
            ListBoxColors.SelectedIndex = 0;
            _IndexSel = 0;
            _ColorSel = listWPFColors[_IndexSel];
        }

        private void ColorPicker_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Remember original color.
            _ColorSelOld = _ColorSel;
            isCanceled = true;
        }

        private void buttonDown_Click(object sender, RoutedEventArgs e)
        {
            _IndexSel += 1;
            if (_IndexSel > (listWPFColors.Count - 1))
            {
                _IndexSel = listWPFColors.Count - 1;
            }
            ListBoxColors.SelectedIndex = _IndexSel;
        }
    }
}
