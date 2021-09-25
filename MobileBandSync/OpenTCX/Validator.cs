using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Schema;
using System.Xml;

namespace MobileBandSync.OpenTcx
{
    sealed class Validator
    {
        private string errMsg;

        /**/
        /// <summary>
        /// validation Error Msg
        /// </summary>
        public string validationErrMsg
        {
            get { return errMsg; }
            set { errMsg = value; }
        }


        /**/
        /// <summary>
        /// Validate XML against schema
        /// </summary>
        /// <param name="XSD"></param>
        /// <param name="XMLFile"></param>
        /// <param name="LocationDefined"></param>
        /// <returns></returns>
        public bool Validate(string XMLFile, bool LocationDefined)
        {
            bool isValid = true;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

                using (XmlReader reader = XmlReader.Create(XMLFile, settings))
                {
                    string test;

                    while (reader.Read() && isValid == true)
                    {
                        test = reader.Name;
                    }
                };
            }
            catch (Exception e)
            {
                validationErrMsg += "Exception occured when validating. " + e.Message;

                isValid = false;
            }

            return isValid;
        }
    }
}
