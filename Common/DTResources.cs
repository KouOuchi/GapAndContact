using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class DTResources
    {
        public const string XML_FRONTEND_PATH = @"\Dentics\Dentics CAM\XmlFrontend";
        public const string XML_RHINO_PATH = @"\Dentics\Dentics CAM\XmlRhino";
        public const string XML_LOG_PATH = @"\Dentics\Dentics CAM\XmlLog";
        public const string CONFIG_PATH = @"\Dentics\Dentics CAM\Configs";

        public const string LOC_FILENAME = @"\Location.xml";
        public const string LOCITEM_FILENAME = @"\LocationItem.xml";
        public const string GAP_CONDITION_FILENAME = @"\GapContact_Condition.xml";

        public static string MYDOC_PATH
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }
    }
}
