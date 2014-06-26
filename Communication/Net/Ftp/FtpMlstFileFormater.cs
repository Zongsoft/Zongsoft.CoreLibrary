using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zongsoft.Communication.Net.Ftp
{
    internal static class FtpMlstFileFormater
    {
        private const string NEWLINE = "\r\n";
        private const char DELIM = ' ';

        public static string Format(FtpCommandContext context, FileSystemInfo fileInfo)
        {
            var output = new StringBuilder();
            Format(context, fileInfo, output);
            return output.ToString();
        }

        public static void Format(FtpCommandContext context, FileSystemInfo fileInfo, StringBuilder output)
        {
            var isFile = fileInfo is FileInfo;

            //Size
            output.AppendFormat("size={0};", isFile ? ((FileInfo)fileInfo).Length : 0);

            //Permission
            output.AppendFormat("perm={0}{1};",
                                /* Can read */ isFile ? "r" : "el",
                                /* Can write */ isFile ? "adfw" : "fpcm");
            
            //Type
            output.AppendFormat("type={0};", isFile ? "file" : "dir");

            //Create
            output.AppendFormat("create={0};", FtpDateUtils.FormatFtpDate(fileInfo.CreationTimeUtc));

            //Modify
            output.AppendFormat("modify={0};", FtpDateUtils.FormatFtpDate(fileInfo.LastWriteTimeUtc));

            //File name
            output.Append(DELIM);
            output.Append(fileInfo.Name);

            output.Append(NEWLINE);
        }
    }
}
