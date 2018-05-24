using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Communication
{
    public class TcpTools
    {

        public static bool OffsetRead(byte[] destBuf, NetworkStream netStream, int start, int toReadLen)
        {
            try
            {
                if (destBuf == null || start < 0 || toReadLen <= 0 || destBuf.Length < start + toReadLen)
                    return false;
                int totalSize = 0;

                while (toReadLen > totalSize)
                {
                    int readLen = netStream.Read(destBuf, start + totalSize, toReadLen - totalSize);
                    if (readLen == 0)
                        return false;
                    totalSize += readLen;
                }

                System.Diagnostics.Debug.Assert(totalSize != toReadLen, "toReadLen 不等于 totalSize");
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static bool SendPackData(byte[] sendData, NetworkStream clientStream)
        {
            try
            {
                if (clientStream == null || sendData.Length <= 0)
                    return false;

                byte[] reqHeader = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(sendData.Length));
                byte[] sendBuf = new byte[reqHeader.Length + sendData.Length];
                reqHeader.CopyTo(sendBuf, 0);
                sendData.CopyTo(sendBuf, reqHeader.Length);

                clientStream.Write(sendBuf, 0, sendBuf.Length);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool RevPackData(out byte[] revBuffer, NetworkStream clientStream)
        {
            revBuffer = null;
            try
            {
                byte[] headerBuf = new byte[4];

                if (!TcpTools.OffsetRead(headerBuf, clientStream, 0, 4))
                    return false;
                Int32 dataLen = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerBuf, 0));
                if (dataLen <= 0)
                  
                    return false;
                byte[] buffer = new byte[dataLen];
                if (!TcpTools.OffsetRead(buffer, clientStream, 0, dataLen))
                    return false;
                revBuffer = buffer;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

    }
}
