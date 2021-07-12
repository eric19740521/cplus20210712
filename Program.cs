using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
 

namespace escpos01
{
    class Program
    {

        const char ESC = '\x1b';
        const char FS = '\x1c';
        const char GS = '\x1d';

        const char dd = '\x64';

        private static Byte[]  Const_SetCut = new byte[] { 0x1D, 0x56, 0x30};

        private static bool TcpPrint02(string host, byte[] cmdBytes)
        {
            bool result = false;
            TcpClient tcp = null;
            try
            {

                tcp = new TcpClient(host, 9100);
                tcp.SendTimeout = 1000;
                tcp.ReceiveTimeout = 1000;
                if (tcp.Connected)
                {
                    tcp.Client.Send(cmdBytes);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("打印失败，请检查打印机或网络设置。", ex);
            }
            finally
            {
                if (tcp != null)
                {
                    if (tcp.Client != null)
                    {
                        tcp.Client.Close();
                        tcp.Client = null;
                    }
                    tcp.Close();
                    tcp = null;
                }
            }
            return result;
        }

        static void TcpPrint(string host, byte[] data)
        {
            Socket s = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            s.Connect(host, 9100);
            s.Send(data);
            s.Disconnect(false);
        }
  static byte[] GetPrintData(string host)
        {
            StringBuilder sb = new StringBuilder();

            // Initialize printer
            sb.Append(ESC + "@");

            // Align center
            sb.Append(ESC + "a" + (char)1);

            // Use bold
            sb.Append(ESC + "E" + (char)1);

            // Add header text

            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

 
            //ASCII 碼 ESC ! n

            sb.Append(GS + "!" + (char)1);
            sb.Append("POS" +"\n");
            sb.Append("https://ble.com.tw\n\n");
            sb.Append(GS + "!" + (char)0);

            // Revert to align left and normal weight
            sb.Append(ESC + "a" + (char)0);
            sb.Append(ESC + "E" + (char)0);

            // Format some text
            sb.Append("This sample text is sent from a C# program\n");
            sb.AppendFormat("to Ethernet POS machine '{0}'.\n\n", host);
            sb.Append("Regards,\nThe Active+ Software team\n");

            sb.Append(ESC + "\x64" + (char)5);

            // Feed and cut paper
            sb.Append(GS + "V\x41\0");

 

            return Encoding.Default.GetBytes(sb.ToString());
        }


        static byte[] GetPrintData02(string host)
        {
            StringBuilder sb = new StringBuilder();

            // Initialize printer
            sb.Append(ESC + "@");

            // Align center
            sb.Append(ESC + "a" + (char)1);

            // Use bold
            sb.Append(ESC + "E" + (char)1);

            // Add header text
            //https://harry-lin.blogspot.com/2019/05/net-core-big5.html
            ///https://docs.microsoft.com/zh-tw/dotnet/csharp/language-reference/builtin-types/char
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            //ASCII 碼 ESC ! n

            sb.Append(GS + "!" + (char)1);
            sb.Append("中文化" + "\n");
            sb.Append("https://ble.com.tw\n\n");
            sb.Append(GS + "!" + (char)0);

            // Revert to align left and normal weight
            sb.Append(ESC + "a" + (char)0);
            sb.Append(ESC + "E" + (char)0);

            // Format some text
            sb.Append("This sample text is sent from a C# program\n");
            sb.AppendFormat("to Ethernet POS machine '{0}'.\n\n", host);
            sb.Append("Regards,\nThe Active+ Software team\n");

            sb.Append(ESC + "\x64" + (char)5);

            // Feed and cut paper
            sb.Append(GS + "V\x41\0");



            return Encoding.GetEncoding("big5").GetBytes(sb.ToString());
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


       //Console.Write("Please enter a hostname or IP: ");
            string host = "192.168.1.31";
            Console.WriteLine();
            
            try {
                //byte[] data = GetPrintData(host);
                //TcpPrint(host, data);


                byte[] data = GetPrintData02(host);
                TcpPrint(host, data);



                //TcpPrint02(host, data);


                Console.WriteLine(
                    "{0} bytes successfully sent to Ethernet POS machine '{1}'",
                    data.Length,
                    host);
            }
            catch (Exception e) {
                Console.WriteLine("Error: " + e.Message);
            }            
        }
    }
}
