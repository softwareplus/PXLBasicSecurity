using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Xml;
using BasicSecurity.Models;

namespace BasicSecurity.Helpers
{
    public class UsersDAL
    {
        private string strFileName;

        public UsersDAL()
        {
            string relativePath = ConfigurationManager.AppSettings["DataBasePath"];
            strFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        }

        public bool Create(User user)
        {
            try
            {
                Delete(user.Name);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strFileName);


                XmlElement subNode = xmlDoc.CreateElement("User");

            
                string strId = CommonMethods.GetMaxValue(xmlDoc, "Users" + "/" + "User" + "/" + "Id").ToString();

        
                subNode.AppendChild(CommonMethods.CreateXMLElement(xmlDoc, "Id", strId));
                xmlDoc.DocumentElement.AppendChild(subNode);


                subNode.AppendChild(CommonMethods.CreateXMLElement(xmlDoc, "Name", user.Name));
                xmlDoc.DocumentElement.AppendChild(subNode);

                subNode.AppendChild(CommonMethods.CreateXMLElement(xmlDoc, "PublicKey", user.publicKey));
                xmlDoc.DocumentElement.AppendChild(subNode);

                xmlDoc.Save(strFileName);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool Delete(string id)
        {

            try
            {

                    XmlDocument objXmlDocument = new XmlDocument();
                    objXmlDocument.Load(strFileName);

                    XmlNode node = objXmlDocument.SelectSingleNode("//User[Name='" + id + "']");

                    if (node != null)
                    {
                        objXmlDocument.ChildNodes[1].RemoveChild(node);
                    }
                    objXmlDocument.Save(strFileName);

                    return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}