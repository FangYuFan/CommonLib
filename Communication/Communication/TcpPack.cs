using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Communication
{
    /// <summary>
    /// TCP数据包
    /// </summary>
    public class TcpPack
    {

        NVPair _para = new NVPair();

        private bool ParsefromBuffer(byte[] data)
        {
            return true;
        }

        private bool ParseToBuffer()
        {
            return true;
        }

        private bool SendRequest(NetworkStream clientStream)
        {
            try
            {
                if (clientStream == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

    }
}
