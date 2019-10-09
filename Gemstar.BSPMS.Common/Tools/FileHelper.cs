using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Gemstar.BSPMS.Common.Tools
{
    public static class FileHelper
    {
        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        public static FileExtension GetFileExtension(string fileName, Stream fileStream)
        {
            bool isSuccess = false;
            FileExtension result = FileExtension.Other;

            //1.根据文件名称 获取扩展名
            var extensionName = Path.GetExtension(fileName);
            if (!string.IsNullOrWhiteSpace(extensionName) && extensionName.Length > 1)
            {
                if (extensionName.StartsWith("."))
                {
                    extensionName = extensionName.Substring(1);
                }
                if(Enum.TryParse<FileExtension>(extensionName.ToUpper(), out result))
                {
                    if (Enum.IsDefined(typeof(FileExtension), result))
                    {
                        isSuccess = true;
                    }
                }
            }

            //2.根据文件名称 获取文件真实类型
            if (isSuccess && fileStream != null)
            {
                try
                {
                    BinaryReader r = new BinaryReader(fileStream);
                    string bx = " ";
                    byte buffer;
                    try
                    {
                        buffer = r.ReadByte();
                        bx = buffer.ToString();
                        buffer = r.ReadByte();
                        bx += buffer.ToString();
                        fileStream.Seek(0, SeekOrigin.Begin);
                    }
                    catch { }
                    if (bx == (FileExtensionByte(result)).ToString() || bx == (FileExtensionByte(FileExtension.ZIP)).ToString())
                    {
                        return result;
                    }
                }
                catch
                {
                    return FileExtension.Other;
                }
            }

            return FileExtension.Other;
        }

        /// <summary>
        /// 获取扩展名对应真实字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int FileExtensionByte(FileExtension value)
        {
            int result = -1;
            switch(value)
            {
                case FileExtension.ASPX:
                    result = 239187;
                    break;
                case FileExtension.BAT:
                    result = 64101;
                    break;
                case FileExtension.BMP:
                    result = 6677;
                    break;
                case FileExtension.BTSEED:
                    result = 10056;
                    break;
                case FileExtension.CHM:
                    result = 7384;
                    break;
                case FileExtension.COM:
                    result = 7790;
                    break;
                case FileExtension.CS:
                    result = 117115;
                    break;
                case FileExtension.DLL:
                    result = 7790;
                    break;
                case FileExtension.DOC:
                    result = 208207;
                    break;
                case FileExtension.DOCX:
                    result = 208207;
                    break;
                case FileExtension.EXE:
                    result = 7790;
                    break;
                case FileExtension.GIF:
                    result = 7173;
                    break;
                case FileExtension.HLP:
                    result = 6395;
                    break;
                case FileExtension.HTML:
                    result = 6033;
                    break;
                case FileExtension.JPG:
                    result = 255216;
                    break;
                case FileExtension.JS:
                    result = 119105;
                    break;
                case FileExtension.LOG:
                    result = 70105;
                    break;
                case FileExtension.PDF:
                    result = 3780;
                    break;
                case FileExtension.PNG:
                    result = 13780;
                    break;
                case FileExtension.PSD:
                    result = 5666;
                    break;
                case FileExtension.RAR:
                    result = 8297;
                    break;
                case FileExtension.RDP:
                    result = 255254;
                    break;
                case FileExtension.REG:
                    result = 8269;
                    break;
                case FileExtension.SQL:
                    result = 255254;
                    break;
                case FileExtension.TXT:
                    result = 210187;
                    break;
                case FileExtension.XLS:
                    result = 208207;
                    break;
                case FileExtension.XLSX:
                    result = 208207;
                    break;
                case FileExtension.XML:
                    result = 6063;
                    break;
                case FileExtension.ZIP:
                    result = 8075;
                    break;
            }
            return result;
        }
    }

    /// <summary>
    /// 文件类型扩展名
    /// </summary>
    public enum FileExtension
    {
        JPG,
        GIF,
        BMP,
        PNG,
        COM,
        EXE,
        DLL,
        RAR,
        ZIP,
        XML,
        HTML,
        ASPX,
        CS,
        JS,
        TXT,
        SQL,
        BAT,
        BTSEED,
        RDP,
        PSD,
        PDF,
        CHM,
        LOG,
        REG,
        HLP,
        DOC,
        XLS,
        DOCX,
        XLSX,
        Other,
    }

    ///// <summary>
    ///// 文件类型扩展名
    ///// </summary>
    //public enum FileExtension
    //{
    //    JPG = 255216,
    //    GIF = 7173,
    //    BMP = 6677,
    //    PNG = 13780,
    //    COM = 7790,
    //    EXE = 7790,
    //    DLL = 7790,
    //    RAR = 8297,
    //    ZIP = 8075,
    //    XML = 6063,
    //    HTML = 6033,
    //    ASPX = 239187,
    //    CS = 117115,
    //    JS = 119105,
    //    TXT = 210187,
    //    SQL = 255254,
    //    BAT = 64101,
    //    BTSEED = 10056,
    //    RDP = 255254,
    //    PSD = 5666,
    //    PDF = 3780,
    //    CHM = 7384,
    //    LOG = 70105,
    //    REG = 8269,
    //    HLP = 6395,
    //    DOC = 208207,
    //    XLS = 208207,
    //    DOCX = 208207,
    //    XLSX = 208207,
    //    Other = 0,
    //}

}
