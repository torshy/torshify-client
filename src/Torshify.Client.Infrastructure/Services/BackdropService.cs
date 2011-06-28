using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Infrastructure.Services
{
    public class BackdropService : IBackdropService
    {
        #region Fields

        private Random _randomGen = new Random();
        private const string ApiKey = "590b54eae4a816b5144c09f15a8f3876";

        #endregion Fields

        #region Constructors

        public BackdropService()
        {
            CacheLocation = Environment.CurrentDirectory;
        }

        #endregion Constructors

        #region Properties

        public string CacheLocation
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void GetBackdrop(string artistName, Action<string> foundBackdrop)
        {
            string fileName;

            if (TryGetFromCache(artistName, out fileName))
            {
                foundBackdrop(fileName);
                return;
            }

            string downloadFolder = Path.Combine(CacheLocation, StringToHash(artistName));
            Directory.CreateDirectory(downloadFolder);
            Task.Factory.StartNew(() => StartDownloading(artistName, downloadFolder, foundBackdrop));
        }

        public bool TryGetFromCache(string artistName, out string fileName)
        {
            string[] files;

            if (TryGetFromCache(artistName, out files))
            {
                int randomIndex = _randomGen.Next(0, files.Length);
                fileName = files[randomIndex];
                return true;
            }

            fileName = null;
            return false;
        }

        public bool TryGetFromCache(string artistName, out string[] fileNames)
        {
            string possibleLocation = Path.Combine(CacheLocation, StringToHash(artistName));

            if (Directory.Exists(possibleLocation))
            {
                string[] files = Directory.GetFiles(possibleLocation);
                if (files.Length > 0)
                {
                    fileNames = files;
                    return true;
                }
            }

            fileNames = null;
            return false;
        }

        private static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }

        private static string StringToHash(string text)
        {
            var tmpSource = ASCIIEncoding.ASCII.GetBytes(text);
            var tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            return ByteArrayToString(tmpHash);
        }

        private void StartDownloading(string keywords, string downloadFolder, Action<string> foundBackdrop)
        {
            try
            {
                string replacedSpaces = keywords.Replace(" ", "_");
                Uri siteUri =
                    new Uri(
                        "http://htbackdrops.com/api/" + ApiKey + "/searchXML?keywords=" + replacedSpaces + "&limit=2");

                string result = "";

                HttpWebRequest request = WebRequest.Create(siteUri) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    result = reader.ReadToEnd();
                }

                //load xml from web request result and get the image id's
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);

                XmlNodeList nodelist = doc.SelectNodes("/search/images/image/id");

                if (nodelist.Count == 0)
                {
                    return;
                }

                Parallel.ForEach(nodelist.Cast<XmlNode>(), node => ProcessImage(node.InnerText, downloadFolder));

                if (TryGetFromCache(keywords, out result))
                {
                    foundBackdrop(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ProcessImage(string imageId, string downloadFolder)
        {
            string url = "http://htbackdrops.com/api/" + ApiKey + "/download/" + imageId + "/intermediate";

            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    Stream responseStream = response.GetResponseStream();
                    byte[] buffer = new byte[1024];
                    int bytes;
                    bytes = responseStream.Read(buffer, 0, buffer.Length);

                    string filePath = Path.Combine(downloadFolder, imageId + ".jpg");
                    using (FileStream fileStream = File.OpenWrite(filePath))
                    {
                        while (bytes > 0)
                        {
                            fileStream.Write(buffer, 0, bytes);
                            bytes = responseStream.Read(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion Methods
    }
}