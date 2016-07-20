using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Sodu.Core.Util
{
    public class SerializeHelper
    {
        /// <summary>
        /// 序列化到文件
        /// </summary>
        public static void SerializeToFile(object obj, string filename)
        {
            string path = filename.Substring(0, filename.LastIndexOf(@"\"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer x = new XmlSerializer(obj.GetType());
                try
                {
                    x.Serialize(stream, obj);

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 从文件返序列化
        /// </summary>
        public static T DeserializeFromFile<T>(string filename)
        {
            string path = filename.Substring(0, filename.LastIndexOf(@"\"));
            if (!Directory.Exists(path))
            {
                return default(T);
            }

            if (!File.Exists(filename))
            {
                return default(T);
            }
            XmlSerializer x = new XmlSerializer(typeof(T));
            StreamReader reader = null;
            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite))
                {
                    reader = new StreamReader(stream);
                    return (T)x.Deserialize(reader);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                reader.Dispose();
            }
        }


        /// <summary>
        /// 序列化到文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<bool> WriteAsync<T>(T data, string fileName)
        {
            bool result = false;
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                using (IRandomAccessStream rastream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                {
                    using (IOutputStream outStream = rastream.GetOutputStreamAt(0))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                        serializer.WriteObject(outStream.AsStreamForWrite(), data);
                        await outStream.FlushAsync();
                        result = true;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }


        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<T> ReadAsync<T>(string fileName)
        {
            T sessionState = default(T);
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            if (file == null) return sessionState;
            using (IInputStream rastream = await file.OpenSequentialReadAsync())
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                sessionState = (T)serializer.ReadObject(rastream.AsStreamForRead());
            }
            return sessionState;
        }
    }
}
