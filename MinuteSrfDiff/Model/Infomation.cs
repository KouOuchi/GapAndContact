using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace GapCondition.Model
{
    public class Infomation 
    {
        private bool status;
        private Color colors;
        private double maxbound;
        private double minbound;
        private string visible;

        public string Visible
        {
            get { return visible; }
            set
            {
                visible = value; 
                //OnPropertyChanged("Visible");
            }
        }

        public bool Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                //OnPropertyChanged("Status");
            }
        }

        public SolidColorBrush ColorsToDisplay
        {
            get
            {
                return new SolidColorBrush(colors);
            }
            set
            {
                colors = value.Color;
            }
        }
        public Color Colors
        {
            get
            {
                return colors;
            }
            set
            {
                colors = value;
                //OnPropertyChanged("ColorsToDisplay");
            }
        }

        public double MaxBound
        {
            get { return maxbound; }
            set
            {
                maxbound = value;
                //OnPropertyChanged("MaxBound");
            }
        }

        public double MinBound
        {
            get { return minbound; }
            set
            {
                minbound = value;
                //OnPropertyChanged("MinBound");
            }
        }



    }
}
