using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;


namespace Communication
{
    public class NVPair
    {
        Dictionary<string, string> _nameValueList = new Dictionary<string, string>();

        public void Clear()
        {
            lock (_nameValueList)
                _nameValueList.Clear();
        }

        private bool SetValueByName(string name,string value)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return false;

                lock (_nameValueList)
                {
                    _nameValueList[name.ToLower()] = value == null ? "" : value;
                }
                return true;
            }
            catch (Exception)
            {
                return false;                
            }
        }

        private string GetValueByName(string name)
        {
            string strValue = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(name) && _nameValueList.ContainsKey(name))
                    return _nameValueList[name];
                return strValue;
            }
            catch (Exception)
            {
                return strValue;                
            }
        }

        /// <summary>
        /// 将键值对转换为字节数组
        /// </summary>                
        private byte[] ParseToBuffer()
        {
            byte[] outBuffer;
            if (!ParseToBuffer(out outBuffer))
            {
                outBuffer = new byte[0];
            }
            return outBuffer;
        }
        
        private bool ParseToBuffer(out byte[] outBuffer)
        {
            outBuffer = null;
            List<byte> retBuf = new List<byte>();

            foreach (var item in _nameValueList)
            {
                string name = item.Key;
                string value = item.Value;

                //字节流中用4个字节写入name的长度，再写入name的具体值
                byte[] nameBuf = Encoding.Default.GetBytes(name);
                byte[] nameHeadBuf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(nameBuf.Length));
                retBuf.AddRange(nameHeadBuf);
                retBuf.AddRange(nameBuf);

                //字节流中用4个字节写入value的长度，再写入value的具体值
                byte[] valueBuf = Encoding.Default.GetBytes(name);
                byte[] valueHeadBuf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(valueBuf.Length));
                retBuf.AddRange(valueHeadBuf);
                retBuf.AddRange(valueBuf);
            }

            //字节流中用4个字节写入整个_nameValueList的长度，再写入_nameValueList的具体值
            retBuf.InsertRange(0, BitConverter.GetBytes(IPAddress.HostToNetworkOrder(retBuf.Count)));
            outBuffer = retBuf.ToArray();
            retBuf = null;
            return true;
        }


        /// <summary>
        /// 将字节数组解析并写入到键值对中
        /// </summary>
        private bool ParseFromBuffer(byte[] data)
        {
            int start = 0;
            int parsedCount = 0;
            return ParseFromBuffer(data, start, out parsedCount);

        }

        private bool ParseFromBuffer(byte[] buff, int start, out int parsedCount)
        {
            parsedCount = 0;

            lock (_nameValueList)
            {
                _nameValueList.Clear();

                if (buff == null || buff.Length < sizeof(Int32) || start != 0)
                    return false;

                //数据总长度
                Int32 dataLen = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buff, start));
                int headerLen = sizeof(Int32);

                if (dataLen < 0 || dataLen + sizeof(Int32) > buff.Length)
                    return false;

                int readedSize = 0;
                while (readedSize < dataLen)
                {
                    //从buff中获取name
                    if (readedSize + sizeof(Int32) > dataLen)
                        break;
                    Int32 nameLen = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buff, start + headerLen + readedSize));
                    readedSize += sizeof(Int32);
                    if (readedSize + nameLen > dataLen || nameLen < 0)
                        break;
                    string strName = string.Empty;
                    if (nameLen > 0)
                        strName = Encoding.Default.GetString(buff, start + headerLen + readedSize, nameLen);
                    readedSize += nameLen;

                    //从buff中获取value
                    if (readedSize + sizeof(Int32) > dataLen)
                        break;
                    Int32 valueLen = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buff, start + headerLen + readedSize));
                    readedSize += sizeof(Int32);
                    if (readedSize + valueLen > dataLen || valueLen < 0)
                        break;
                    string strValue = string.Empty;
                    if (valueLen > 0)
                        strValue = Encoding.Default.GetString(buff, start + headerLen + readedSize, valueLen);
                    readedSize += valueLen;

                    //将获取的name和value写入_nameValueList中
                    if (string.IsNullOrEmpty(strName))
                        _nameValueList[strName] = strValue == null ? "" : strValue;
                }

                if (readedSize != dataLen)
                    return false;
                parsedCount = headerLen + dataLen;
            }
            return true;
        }

    }
}
