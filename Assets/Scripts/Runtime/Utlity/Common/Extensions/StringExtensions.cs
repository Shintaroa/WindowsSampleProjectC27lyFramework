using System;
using System.Text;
using System.Text.RegularExpressions;

public static class StringExtensions
{
#region 运算操作
    /// <summary>
    /// 字符串 转 枚举
    /// </summary>
    public static T ToEnum<T>(this string str)
    {
        try
        {
            return (T)Enum.Parse(typeof(T), str);
        }
        catch
        {
            return default;
        }
    }
    
    /// <summary>
    /// 字符串 转 bytes
    /// </summary>
    public static byte[] ToBytes(this string str)
    {
        byte[] Data = new byte[str.Length];
        int count = 0;
        string strSource = str.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");

        try
        {
            //取余3运算作用是防止用户输入的字符为奇数个
            for (int i = 0; i < (strSource.Length - strSource.Length % 2) / 2; i++)
            {
                Data[count] = Convert.ToByte(strSource.Substring(i * 2, 2), 16);
                count++;
            }
            //剩下一位单独处理
            if (strSource.Length % 2 != 0)
            {
                //单独处理B（0B）
                Data[count] = Convert.ToByte(strSource.Substring(strSource.Length - 1, 1), 16);
                count++;
            }
        }
        catch (Exception)
        {
            return null;
        }

        byte[] buf = new byte[count];
        Array.Copy(Data, 0, buf, 0, count); //复制原始数据    
        return buf;
    }
    
    /// <summary>
    /// 字符串 转 Base64
    /// </summary>
    public static string ToBase64(this string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// 字符串 转 整型
    /// </summary>
    public static int ToInt(this string str)
    {
        return Convert.ToInt32(str);
    }

    
#endregion

#region IO操作
    /// <summary>
    /// 判定文件是否存在
    /// </summary>
    public static bool IsFileExist(this string path)
    {
        return System.IO.File.Exists(path);
    }
    
    /// <summary>
    /// 创建文件
    /// </summary>
    public static void CreateFile(this string path)
    {
        if (!System.IO.File.Exists(path))
        {
            System.IO.File.Create(path);
        }
    }
    
    /// <summary>
    /// 删除文件
    /// </summary>
    public static void DelFile(this string path)
    {
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
    }
    
    /// <summary>
    /// 判定文件夹是否存在
    /// </summary>
    public static bool IsFolderExist(this string path)
    {
        return System.IO.Directory.Exists(path);
    }
    
    /// <summary>
    /// 创建文件夹
    /// </summary>
    public static void CreateFolder(this string path)
    {
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }
    }
    
    /// <summary>
    /// 删除文件夹
    /// </summary>
    public static void DelFolder(this string path)
    {
        if (System.IO.Directory.Exists(path))
        {
            System.IO.Directory.Delete(path, true);
        }
    }
#endregion

#region 格式判定

    /// <summary>
    /// 判定邮箱格式
    /// </summary>
    public static bool IsEmail(this string str)
    {
        return Regex.IsMatch(str, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
    }
    
    /// <summary>
    /// 判定手机号格式
    /// </summary>
    public static bool IsMobile(this string str)
    {
        return Regex.IsMatch(str, @"^1\d{10}$");
    }
    
    /// <summary>
    /// 字符串掩码
    /// </summary>
    /// <param name="s">字符串</param>
    /// <param name="mask">掩码符</param>
    /// <returns>参考样例："13123456789".Mask(); // 131****5678 </returns>
    public static string Mask(this string s, char mask = '*')
    {
        if (string.IsNullOrWhiteSpace(s?.Trim()))
        {
            return s;
        }

        s = s.Trim();
        string masks = mask.ToString().PadLeft(4, mask);
        return s.Length switch
        {
            >= 11 => Regex.Replace(s, "(.{3}).*(.{4})", $"$1{masks}$2"),
            10 => Regex.Replace(s, "(.{3}).*(.{3})", $"$1{masks}$2"),
            9 => Regex.Replace(s, "(.{2}).*(.{3})", $"$1{masks}$2"),
            8 => Regex.Replace(s, "(.{2}).*(.{2})", $"$1{masks}$2"),
            7 => Regex.Replace(s, "(.{1}).*(.{2})", $"$1{masks}$2"),
            6 => Regex.Replace(s, "(.{1}).*(.{1})", $"$1{masks}$2"),
            _ => Regex.Replace(s, "(.{1}).*", $"$1{masks}")
        };
    }
    

#endregion
    
    
    
}