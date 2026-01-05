using System;

public static class BytesExtensions 
{
    /// <summary>
    /// bytes 转 字符串
    /// </summary>
    public static string ToStr(this byte[] buffer)
    {
        string str = "";
        if (buffer != null)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                str += buffer[i].ToString("X2");
            }
        }
        return str;
    }
    
    
    
    
}
