using System.Net;
using System.Net.Sockets;
namespace SchoolNetConnect
{
    public class GetSystemData //处理函数之类的
    {
        public static string GetIP()
        {
            string localIP = "127.0.0.1";
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("255.255.255.255", 65530);
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
                localIP = endPoint!.Address.ToString();
            }
            return localIP;
        }
    }
}
