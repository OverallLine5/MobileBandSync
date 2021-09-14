using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;

namespace MobileBandSync.OpenTcx
{
    public class Tcx
    {
        /// <summary>
        /// AnalyzeTcxFile
        /// </summary>
        /// <param name="tckFile"></param>
        /// <returns></returns>
        public Entities.TrainingCenterDatabase_t AnalyzeTcxFile(string tckFile)
        {
            Entities.TrainingCenterDatabase_t data = null;
            XmlSerializer xs = new XmlSerializer(typeof(Entities.TrainingCenterDatabase_t));
            //using (FileStream fs = new FileStream(tckFile, FileMode.Open, FileAccess.Read))
            //{
            //    data = xs.Deserialize(fs) as Entities.TrainingCenterDatabase_t;

            //}
            return data;
        }

        /// <summary>
        /// AnalyzeTcxStream
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public Entities.TrainingCenterDatabase_t AnalyzeTcxStream(Stream fs)
        {
            Entities.TrainingCenterDatabase_t data = null;
            XmlSerializer xs = new XmlSerializer(typeof(Entities.TrainingCenterDatabase_t));
            data = xs.Deserialize(fs) as Entities.TrainingCenterDatabase_t;
            return data;
        }

        /// <summary>
        /// GenerateTcx
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GenerateTcx(Entities.TrainingCenterDatabase_t data)
        {
            string tck = null;

            try
            {
                XmlSerializer xs = new XmlSerializer( typeof( Entities.TrainingCenterDatabase_t ) );
                using( StringWriter sw = new StringWriter() )
                {
                    xs.Serialize( sw, data );
                    tck = sw.GetStringBuilder().ToString();
                }
            }
            catch( Exception ex )
            {

            }
            return tck;

        }

        /// <summary>
        /// ValidateTcx
        /// </summary>
        /// <param name="tckFile"></param>
        /// <returns></returns>
        public bool ValidateTcx(string tckFile)
        {
            try
            {
                Validator vad = new Validator();
                return vad.Validate(tckFile, false);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
