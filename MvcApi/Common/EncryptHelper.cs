using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MvcApi.Common
{
    public class EncryptHelper
    {
        public static string ToMd5(string strPwd)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(strPwd);
            byte[] md5data = md5.ComputeHash(data); md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length - 1; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return str;
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt32(string password)
        {
            string cl = password;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 

                //pwd = pwd + s[i].ToString("X");
                string temp = s[i].ToString("X");
                if (temp.Length == 1)
                    pwd = pwd + "0";
                pwd = pwd + temp;
            }
            return pwd;
        }

        /// <summary>
        /// 随机创建Key
        /// </summary>
        /// <param name="num">字节位数</param>
        /// <returns></returns>
        public static byte[] CreateKey(int num)
        {
            byte[] result = new byte[num];
            Random rand = new Random();
            for (int i = 0; i < num; i++)
            {
                result[i] = (Byte)rand.Next(1, 256);
            }
            return result;

        }

        #region RSA-------------------------------加密与解密
        public static string EncryptRSA(string data, string publickey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(data), false);
            return Convert.ToBase64String(cipherbytes);

        }

        public static string DecryptRSA(string data, string privatekey)
        {

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes; rsa.FromXmlString(privatekey);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(data), false);
            return Encoding.UTF8.GetString(cipherbytes);
        }

        #endregion

        public static string KeyAES = "XCVdefgh12345678CDEFGHTR12345678";

        public static string EncryptAES(string plainText, string AESKey)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plainText);//得到需要加密的字节数组
            rijndaelCipher.Key = Convert.FromBase64String(AESKey);//加解密双方约定好密钥：AESKey
            rijndaelCipher.GenerateIV();
            byte[] keyIv = rijndaelCipher.IV;
            byte[] cipherBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, rijndaelCipher.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cipherBytes = ms.ToArray();//得到加密后的字节数组
                    cs.Close();
                    ms.Close();
                }
            }
            var allEncrypt = new byte[keyIv.Length + cipherBytes.Length];
            Buffer.BlockCopy(keyIv, 0, allEncrypt, 0, keyIv.Length);
            Buffer.BlockCopy(cipherBytes, 0, allEncrypt, keyIv.Length * sizeof(byte), cipherBytes.Length);
            return Convert.ToBase64String(allEncrypt);
        }

        public static string DecryptAES(string showText, string AESKey)
        {
            string result = string.Empty;
            try
            {
                byte[] cipherText = Convert.FromBase64String(showText);
                int length = cipherText.Length;
                SymmetricAlgorithm rijndaelCipher = Rijndael.Create();
                rijndaelCipher.Key = Convert.FromBase64String(AESKey);//加解密双方约定好的密钥
                byte[] iv = new byte[16];
                Buffer.BlockCopy(cipherText, 0, iv, 0, 16);
                rijndaelCipher.IV = iv;
                byte[] decryptBytes = new byte[length - 16];
                byte[] passwdText = new byte[length - 16];
                Buffer.BlockCopy(cipherText, 16, passwdText, 0, length - 16);
                using (MemoryStream ms = new MemoryStream(passwdText))
                {
                    using (CryptoStream cs = new CryptoStream(ms, rijndaelCipher.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        cs.Read(decryptBytes, 0, decryptBytes.Length);
                        cs.Close();
                        ms.Close();
                    }
                }
                result = Encoding.UTF8.GetString(decryptBytes).Replace("\0", "");  ///将字符串后尾的'\0'去掉
            }
            catch { }
            return result;
        }

        public static string encode(string str)
        {
            string htext = "";

            for (int i = 0; i < str.Length; i++)
            {
                htext = htext + (char)(str[i] + 10 - 1 * 2);
            }
            return htext;
        }

        public static string decode(string str)
        {
            string dtext = "";

            for (int i = 0; i < str.Length; i++)
            {
                dtext = dtext + (char)(str[i] - 10 + 1 * 2);
            }
            return dtext;
        }
    }
}