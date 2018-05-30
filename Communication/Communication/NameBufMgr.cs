using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Communication
{
    public class NameBufMgr
    {
        Dictionary<string, byte[]> _nameBufList = new Dictionary<string, byte[]>();

        private void Clear()
        {
            _nameBufList.Clear();
        }

        string strMD5 = string.Empty;

        /// <summary>
        /// 添加K-V对
        /// </summary>        
        private bool SetValueByName(string name, byte[] retBuf)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || retBuf == null || retBuf.Length <= 0)
                    return false;
                _nameBufList[name.ToLower()] = retBuf;
                return true;
            }
            catch (Exception)
            {
                return false;                
            }
        }

        /// <summary>
        /// 查找K-V对
        /// </summary>        
        private byte[] GetValueByName(string name)
        {
            byte[] retBuf;
            if (!GetValueByName(name, out retBuf))
            {
                retBuf = new byte[0];
            }
            return retBuf;
        }

        private bool GetValueByName(string name, out byte[] retBuf)
        {
            retBuf = null;
            try
            {
                if (string.IsNullOrEmpty(name))
                    return false;
                if (_nameBufList.ContainsKey(name.ToLower()))
                    retBuf = _nameBufList[name.ToLower()];
                return true;
            }
            catch (Exception)
            {
                return false;                
            }


            

        }

        /// <summary>
        /// 序列化K-V对
        /// </summary>        
        private byte[] ParseToBuffer()
        {
            byte[] retBuf = null;
            if (!ParseToBuffer(out retBuf))
            {
                retBuf = new byte[0];
            }
            return retBuf;
        }
        
        private bool ParseToBuffer(out byte[] buf)
        {
            buf = null;
            List<byte> bufList = new List<byte>();

            try
            {
                foreach (var item in _nameBufList)
                {
                    string name = item.Key;
                    byte[] itemBuf = item.Value;

                    //序列化name
                    byte[] nameBuf = Encoding.Default.GetBytes(name);
                    byte[] nameHeaderBuf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(nameBuf.Length));
                    bufList.AddRange(nameHeaderBuf);
                    bufList.AddRange(nameBuf);

                    //序列化value
                    byte[] valueBuf = itemBuf;
                    byte[] valueHeaderBuf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(itemBuf.Length));
                    bufList.AddRange(valueHeaderBuf);
                    bufList.AddRange(valueBuf);                                        
                }

                //计算整个Key-Value包的大小
                bufList.InsertRange(0, BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bufList.Count)));
                buf = bufList.ToArray();
                return true;
            }
            catch (Exception)
            {
                return false;    
            }

        }





    }
}
