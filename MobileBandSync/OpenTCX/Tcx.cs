using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using Windows.Storage;
using System.Threading.Tasks;

namespace MobileBandSync.OpenTcx
{
    public class Tcx
    {
        /// <summary>
        /// AnalyzeTcxFile
        /// </summary>
        /// <param name="tckFile"></param>
        /// <returns></returns>
        public async Task<Entities.TrainingCenterDatabase_t> AnalyzeTcxFile( string tcxFile )
        {
            var TempFolder = ApplicationData.Current.LocalFolder;
            var localPath = await CopyLocally( tcxFile );

            Entities.TrainingCenterDatabase_t data = null;

#if WINDOWS_UWP
            XmlSerializer xs = new XmlSerializer( typeof( Entities.TrainingCenterDatabase_t ) );
            using( FileStream fs = new FileStream( localPath, FileMode.Open, FileAccess.Read ) )
            {
                data = xs.Deserialize( fs ) as Entities.TrainingCenterDatabase_t;
            }
            var file = await StorageFile.GetFileFromPathAsync( localPath );
            if( file != null )
                await file.DeleteAsync();
#endif
            return data;
        }

        public async Task<string> CopyLocally( string tcxFile )
        {
#if WINDOWS_UWP
            try
            {
                var file = await StorageFile.GetFileFromPathAsync( tcxFile );
                var TempFolder = ApplicationData.Current.LocalFolder;
                await file.CopyAsync( TempFolder, file.Name, NameCollisionOption.ReplaceExisting );

                var localTcx = await TempFolder.TryGetItemAsync( file.Name ) as StorageFile;
                return localTcx.Path;
            }
            catch( Exception )
            {
                return "";
            }
#else
            return "";
#endif
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
            catch( Exception )
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
