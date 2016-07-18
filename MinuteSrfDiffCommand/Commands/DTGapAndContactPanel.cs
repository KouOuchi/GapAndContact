using GapCondition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GapAndContact.Commands
{
    public class DTGapAndContactPanel
    {
        private static NewDesign panel_instance = null;
        public static NewDesign GetPanelInstance()
        {
            if (panel_instance == null)
            {
                panel_instance = new NewDesign();
            }

            return panel_instance;
        }

        public static void Show()
        {
            if (!DTGapAndContactPanel.GetPanelInstance().IsVisible)
            {
                // show panel
                DTGapAndContactPanel.GetPanelInstance().Show();
            }
        }
        public static void ShowDialog()
        {
            if (!DTGapAndContactPanel.GetPanelInstance().IsVisible)
            {
                // show panel
                DTGapAndContactPanel.GetPanelInstance().ShowDialog();
            }
        }

        public static void DestroyPanelInstance()
        {
            if (panel_instance != null)
            {
                if (panel_instance.IsVisible)
                {
                    panel_instance.Close();
                }
                panel_instance = null;
            }
        }

    }
}
