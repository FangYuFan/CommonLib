using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace Communication
{
    public class MGZipTool
    {
        /// <summary>
        /// zip压缩
        /// </summary>        
        public static string Compress(string input)
        {
            byte[] inputBytes = Encoding.Default.GetBytes(input);
            byte[] outBytes = Compress(inputBytes);
            return Convert.ToBase64String(outBytes);
        }

        /// <summary>
        /// zip解压
        /// </summary>        
        public static string Decompress(string input)
        {
            byte[] inputBytes = Convert.FromBase64String(input);
            byte[] decompressBytes = Decompress(inputBytes);
            return Encoding.Default.GetString(decompressBytes);
        }

        /// <summary>
        /// zip压缩
        /// </summary>        
        public static byte[] Compress(byte[] inputBytes)
        {
            using (MemoryStream outStream=new MemoryStream())
            {
                using (GZipStream zipStream=new GZipStream(outStream,CompressionMode.Compress,true))
                {
                    zipStream.Write(inputBytes, 0, inputBytes.Length);
                    zipStream.Close();   //关闭后才能解压
                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// zip解压
        /// </summary>        
        private static byte[] Decompress(byte[] inputBytes)
        {
            try
            {
                using (MemoryStream inputStream=new MemoryStream(inputBytes))
                {
                    using (MemoryStream outStream=new MemoryStream())
                    {
                        using (GZipStream zipStream=new GZipStream(inputStream,CompressionMode.Decompress))
                        {
                            byte[] blockRead = new byte[1024 * 100];  //100 kb的缓冲区
                            while (true)
                            {
                                int byteRead = zipStream.Read(blockRead, 0, blockRead.Length);
                                if (byteRead<=0)
                                {
                                    break;
                                }
                                else
                                {
                                    outStream.Write(blockRead, 0, byteRead);
                                }                                                                   
                            }
                            return outStream.ToArray();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取MD5码
        /// </summary>        
        public bool GetMD5(byte[] srcData, out string strMD5)
        {
            strMD5 = string.Empty;
            try
            {
                if (srcData == null || srcData.Length <= 0)
                    return false;
                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                byte[] data = md5.ComputeHash(srcData);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }

                strMD5 = sb.ToString();
                return true;
            }
            catch (Exception)
            {
                return false;                
            }
            
        }

    }
}
