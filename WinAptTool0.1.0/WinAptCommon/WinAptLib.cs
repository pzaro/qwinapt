using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;

namespace WinApt.Common
{
    public class WinAptLib
    {
        public static int NoFile = 0;
        public static int Downloaded = 1;
        public static int NewVersion = 2;
        //public enum State
        //{
        //    NoFile = 0,
        //    Downloaded = 1,
        //    NewVersion = 2
        //}
        public static void WriteToFile(object o, string fileName)
        {
            XmlSerializer x = new XmlSerializer(o.GetType());
            TextWriter writer = new StreamWriter(fileName);
            x.Serialize(writer, o);
            writer.Close();
        }

        public static object ReadFromFile(Type t, string fileName)
        {
            XmlSerializer x = new XmlSerializer(t);
            FileStream reader = new FileStream(fileName, FileMode.Open);
            object o = x.Deserialize(reader);
            reader.Close();
            return o;
        }

        public static string getPageContent(string url)
        {
            HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader srd = new StreamReader(response.GetResponseStream());
            string ret = srd.ReadToEnd();
            srd.Close();
            return ret;
        }

        public static object ReadFromStream(Type t, string content)
        {
            XmlSerializer x = new XmlSerializer(t);
            StringReader reader = new StringReader(content);
            object o = x.Deserialize(reader);
            reader.Close();
            return o;
        }
        public static bool DownloadDbFile(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                SaveBinaryFile(response, "tmpDB.zip");
                UnZip("tmpDB.zip", @".\");
                File.Delete("tmpDB.zip");
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static void UnZip(string zipFileName,string dir)
        {
            ZipInputStream s = new ZipInputStream(File.OpenRead(zipFileName));
            try
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(dir);
                    string fileName = Path.GetFileName(theEntry.Name);

                    Directory.CreateDirectory(directoryName);

                    if (fileName != String.Empty)
                    {

                        FileStream streamWriter = File.Create(dir + fileName);

                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        streamWriter.Close();
                    }
                }
                s.Close();
            }
            catch (Exception eu)
            {
                throw eu;
            }
            finally
            {
                s.Close();
            }

        }//end UnZip

        private static bool SaveBinaryFile(WebResponse response, string FileName)
        {
            bool Value = true;
            byte[] buffer = new byte[2048];

            try
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
                Stream outStream = System.IO.File.Create(FileName);
                Stream inStream = response.GetResponseStream();

                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);

                outStream.Close();
                inStream.Close();
            }
            catch
            {
                Value = false;
            }
            return Value;
        }
    }
}