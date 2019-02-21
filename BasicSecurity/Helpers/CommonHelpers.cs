using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace BasicSecurity.Helpers
{
    public class CommonMethods
    {
        public static XmlElement CreateXMLElement(XmlDocument xmlDoc, string name, string value)
        {
            XmlElement xmlElement = xmlDoc.CreateElement(name);
            XmlText xmlText = xmlDoc.CreateTextNode(value);
            xmlElement.AppendChild(xmlText);
            return xmlElement;
        }
        public static int GetMaxValue(XmlDocument xmlDoc, string nodeNameToSearch)
        {
            int intMaxValue = 0;
            XmlNodeList nodelist = xmlDoc.SelectNodes(nodeNameToSearch);
            foreach (XmlNode node in nodelist)
            {
                if (Convert.ToInt32(node.InnerText) > intMaxValue)
                {
                    intMaxValue = Convert.ToInt32(node.InnerText);
                }
            }
            return (intMaxValue + 1);
        }
    }
}