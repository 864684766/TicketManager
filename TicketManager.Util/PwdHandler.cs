using System.Security.Cryptography;
using System.Text;

namespace TicketManager.Util
{
    public class PwdHandler
    {
        /// <summary>
        /// MD532位加密方式
        /// </summary>
        /// <param name="str">用户原始密码</param>
        /// <returns></returns>
        public static string MD5EncryptTo32(string str)
        {
            string cl = str;
            string pwd = "";
            var md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("x2");

            }
            return pwd;
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <param name="key">公钥</param>
        /// <returns></returns>
        public static string DESEncode(string content, string key)
        {
            byte[] iv = new byte[8];
            byte[] array;

            using (DES des = DES.Create())
            {
                des.Key = Encoding.UTF8.GetBytes(key);
                des.IV = iv;

                ICryptoTransform cryptoTransform = des.CreateEncryptor(des.Key, des.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                    {
                        array = Encoding.UTF8.GetBytes(content);
                        cryptoStream.Write(array, 0, array.Length);
                        cryptoStream.FlushFinalBlock();

                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <param name="key">公钥</param>
        /// <returns></returns>
        public static string DESDecode(string content, string key)
        {
            byte[] iv = new byte[8];
            byte[] buffer = Convert.FromBase64String(content);

            using (DES des = DES.Create())
            {
                des.Key = Encoding.UTF8.GetBytes(key);
                des.IV = iv;

                ICryptoTransform cryptoTransform = des.CreateDecryptor(des.Key, des.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
