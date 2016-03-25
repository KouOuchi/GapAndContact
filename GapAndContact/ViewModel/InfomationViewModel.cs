using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Common;
using GapCondition.Model;
using GapCondition.Util;
using GapAndContact.Utilities;
using v2scheduler.ViewModel.Utilities;

namespace GapCondition.ViewModel
{
    class InfomationViewModel : INotifyPropertyChanged
    {
        private string link = DTResources.MYDOC_PATH +
                DTResources.CONFIG_PATH +
                DTResources.GAP_CONDITION_FILENAME;

        private MSColorConverter color_converter = new MSColorConverter();

        private ObservableCollection<Infomation> infos;
        public ObservableCollection<Infomation> Infos
        {
            get { return infos; }
            set { infos = value; }
        }

        public InfomationViewModel()
        {
            if (Data.Infos != null)
            {
                Infos = Data.Infos;
                return;
            }
            XmlUtils ti = new XmlUtils();
            GapConditions data = new GapConditions();

            ti.DeSerialize<GapConditions>(link, out data);
            Infos = new ObservableCollection<Infomation>();
            int i = 0;
            foreach (var condition in data.HouseNo)
            {
                Infos.Add(new Infomation
                {
                    Colors = color_converter.FromHtmlColorToColor(
                        condition.ColorString),
                    Visible = "Visible",
                    Status = condition.Visible,
                    MinBound = condition.Minor,
                    MaxBound = condition.Large
                });
            }

            //Condition last = data.HouseNo.Last();
            //Infomation inf = new Infomation
            //{
            //    Visible = "Hidden",
            //    MaxBound = last.Large
            //};

            //Infos.Add(inf);
            Data.Infos = Infos;
        }

        public Color FromRgb(int argb)
        {
            var b = (byte)(argb & 0xFF);
            var g = (byte)((argb >> 8) & 0xFF);
            var r = (byte)((argb >> 16) & 0xFF);
            var a = (byte)((argb >> 24) & 0xFF);
            int value = (a << 24) + (r << 16) + (g << 8) + b;
            return Color.FromRgb(r, g, b);
            //return string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
        }

        public int ToRgb(Color c)
        {
            return ((c.R & 0x0ff) << 16) | ((c.G & 0x0ff) << 8) | (c.B & 0x0ff);
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion


        internal void Save()
        {
            XmlUtils ti = new XmlUtils();
            GapConditions data = new GapConditions();

            foreach (var i in infos)
            {
                data.HouseNo.Add(new Condition
{
    Large = Convert.ToSingle(i.MaxBound),
    Minor = Convert.ToSingle(i.MinBound),
    ColorString = color_converter.FromColorToHtmlColor(i.Colors),
    Visible = !(i.Visible.Equals("Hidden"))
}
);
            }

            ti.Serialize<GapConditions>(link, data);
        }
    }
}
