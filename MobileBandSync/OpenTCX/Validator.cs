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
                Stream schemaFile = null;

                XmlReaderSettings settings = new XmlReaderSettings();
                ValidationEventHandler SchemaValidationEventHandler = new ValidationEventHandler(ValidationCallBack);

                //settings.ValidationType = ValidationType.Schema;
                //settings.ValidationFlags |= XmlSchemaValidationFlags.AllowXmlAttributes;
                //settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                //settings.ValidationEventHandler += SchemaValidationEventHandler;

                if (LocationDefined == true)
                {
                    //settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                    //settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                }
                else
                {
                    //Assembly assembly = GetType().Assembly;
                    //schemaFile = assembly.GetManifestResourceStream("OpenTcx.TrainingCenterDatabasev2.xsd");

                    //XmlSchema tmsSchema = XmlSchema.Read(schemaFile, SchemaValidationEventHandler);

                    //settings.Schemas.Add(tmsSchema);
                }

                using (XmlReader reader = XmlReader.Create(XMLFile, settings))
                {
                    string test;

                    while (reader.Read() && isValid == true)
                    {
                        test = reader.Name;
                    }
                };

                if (schemaFile != null)
                {
                    //schemaFile.Close();
                }
            }
            catch (Exception e)
            {
                validationErrMsg += "Exception occured when validating. " + e.Message;

                isValid = false;
            }

            return isValid;
        }

        /**/
        /// <summary>
        /// Display any warnings or errors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            //if (args.Message == XmlSeverityType.Warning)
            //{
            //    validationErrMsg += "Matching schema not found. No validation occurred." + args.Message;
            //    validationErrMsg = args.Message;
            //}
            //else
            {
                validationErrMsg += "\nValidation error: " + args.Message;

                validationErrMsg = args.Message;
            }
        }
    }
}
