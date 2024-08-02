using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace ASI.Lib.Comm.Network
{
    /// <summary>
    /// 
    /// </summary>
    public class NetworkLib
    {
        /// <summary>
        /// 嘗試ping指定IP，超過可允許延遲時間未回應即判定為失敗
        /// </summary>
        /// <param name="pIP">指定IP</param>
        /// <param name="pTimeout">可允許延遲時間(毫秒)</param>
        /// <returns>true:成功/false:失敗</returns>
        public static bool Ping(string pIP,int pTimeout)
        {
            System.Net.NetworkInformation.Ping oPing = null;
            System.Net.NetworkInformation.PingOptions oPingOptions = null;
            System.Net.NetworkInformation.PingReply oPingReply = null;

            try
            {
                oPing = new System.Net.NetworkInformation.Ping();
                oPingOptions = new System.Net.NetworkInformation.PingOptions();

                // Use the default Ttl value which is 128,
                // but change the fragmentation behavior.
                oPingOptions.DontFragment = true;

                // Create a buffer of 32 bytes of data to be transmitted.
                string sData = "Test";
                byte[] bBuffer = Encoding.ASCII.GetBytes(sData);

                oPingReply = oPing.Send(pIP, pTimeout, bBuffer, oPingOptions);
                if (oPingReply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
            finally
            {
                oPing = null;
                oPingOptions = null;
                oPingReply = null;
            }
        }

        /// <summary>
		/// 嘗試ping指定IP，超過指定的失敗次數即判定為斷線
		/// </summary>
		/// <param name="ip">指定IP</param>
		/// <param name="timeOut">可允許延遲時間(毫秒)</param>
        /// <param name="failTimes">指定失敗次數</param>
		/// <returns>true:成功/false:超過指定失敗次數，判定為斷線</returns>
		public static bool TryPing(string ip, int timeOut,int failTimes)
        {
            bool bReturn = false;
            for (int ii = 1; ii <= failTimes; ii++)
            {
                if (Ping(ip, timeOut) == true)
                {
                    bReturn = true;
                    break;
                }

                if (ii >= failTimes)
                {
                    //ping超過指定失敗次數，判定為斷線
                    bReturn = false;
                }

                System.Threading.Thread.Sleep(100);
            }

            return bReturn;
        }

        /// <summary>
        /// 使用SMTP寄送郵件
        /// </summary>
        /// <param name="msgFromAddress">格式:xxx@xxx.com；寄件者郵件地址</param>
        /// <param name="msgFromPwd">寄件者密碼</param>
        /// <param name="msgFromName">寄件者名稱</param>
        /// <param name="smtpServer">設定SMTP Server</param>
        /// <param name="msgTo">格式:xxx@xxx.com；可以發送給多人</param>
        /// <param name="ccTo">格式:xxx@xxx.com；可以抄送副本給多人，沒有則為null</param>
        /// <param name="subject">郵件主旨</param>
        /// <param name="mailContent">郵件內容</param>
        /// <param name="encoding">編碼</param>
        /// <param name="attachment">附件，沒有則為null</param>
        /// <returns></returns>
        public static bool SendMail(string msgFromAddress, string msgFromPwd, string msgFromName, 
            string smtpServer, List<string> msgTo, List<string> ccTo,             
            string subject, string mailContent, System.Text.Encoding encoding, Attachment attachment)
        {
            System.Net.Mail.MailMessage oMailMessage = new System.Net.Mail.MailMessage();
            SmtpClient oSmtpClient = new SmtpClient();
            try
            {             
                // 3個參數分別是發件人地址（可以隨便寫），發件人姓名，編碼
                oMailMessage.From = new MailAddress(msgFromAddress, msgFromName, encoding);

                //msg.To.Add("b@b.com");可以發送給多人
                if (msgTo != null)
                {
                    foreach (string sMsgTo in msgTo)
                    {
                        oMailMessage.To.Add(sMsgTo);
                    }
                }

                //msg.CC.Add("c@c.com");可以抄送副本給多人
                if (ccTo != null)
                {
                    foreach (string sCcTo in ccTo)
                    {
                        oMailMessage.CC.Add(sCcTo);
                    }
                }

                oMailMessage.Subject = subject;//郵件標題
                oMailMessage.SubjectEncoding = encoding;//郵件標題編碼
                oMailMessage.Body = mailContent; //郵件內容
                oMailMessage.BodyEncoding = encoding;//郵件內容編碼 

                //附件
                if (attachment != null)
                {
                    oMailMessage.Attachments.Add(attachment);
                }

                oMailMessage.IsBodyHtml = true;//是否是HTML郵件 
                //msg.Priority = MailPriority.High;//郵件優先級 

                oSmtpClient.Credentials = new System.Net.NetworkCredential(msgFromAddress, msgFromPwd); //這裡要填正確的帳號跟密碼
                oSmtpClient.Host = smtpServer; //設定smtp Server
                oSmtpClient.Port = 25; //設定Port
                oSmtpClient.EnableSsl = true; //gmail預設開啟驗證
                oSmtpClient.Send(oMailMessage); //寄出信件
                
                return true;
            }
            catch (Exception ex)
            {
                ASI.Lib.Log.ErrorLog.Log("ASI.Lib.Comm.Network.NetworkLib", ex);
                return false;
            }
            finally
            {
                oSmtpClient.Dispose();
                oMailMessage.Dispose();
            }
        }
    }
}
