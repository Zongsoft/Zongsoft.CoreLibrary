using System;
using System.Text;
using System.IO;

namespace Zongsoft.Communication.Net.Ftp
{
    internal class FtpListFileFormater
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
            output.Append(GetPermission(context, fileInfo));
            output.Append(DELIM);
            output.Append(DELIM);
            output.Append(DELIM);
            output.Append(1); //LinkCount
            output.Append(DELIM);
            output.Append("ftp"); //OwnerName
            output.Append(DELIM);
            output.Append("ftp"); //GroupName
            output.Append(DELIM);
            output.Append(GetLength(fileInfo));
            output.Append(DELIM);
            output.Append(GetLastModified(fileInfo));
            output.Append(DELIM);
            output.Append(fileInfo.Name);
            output.Append(NEWLINE);
        }

        private static char[] GetPermission(FtpCommandContext context, FileSystemInfo fileInfo)
        {
            var perm = ("----------").ToCharArray();
            perm[0] = fileInfo is DirectoryInfo ? 'd' : '-';
            perm[1] = 'r';
            perm[2] = !context.Channel.User.ReadOnly ? 'w' : '-';
            perm[0] = fileInfo is DirectoryInfo ? 'x' : '-';
            return perm;
        }

        private static string GetLength(FileSystemInfo fileInfo)
        {
            var charLength = 12;
            long sz = 0;
            if (fileInfo is FileInfo)
                sz = ((FileInfo) fileInfo).Length;

            return sz.ToString().PadLeft(charLength);
        }

        private static string GetLastModified(FileSystemInfo file)
        {
            return FtpDateUtils.FormatUnixDate(file.LastWriteTimeUtc);
        }
    }
}
