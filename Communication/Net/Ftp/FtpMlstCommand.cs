using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zongsoft.Communication.Net.Ftp
{
    /// <summary>
    /// 在命令行上列出对象的信息
    /// </summary>
    internal class FtpMlstCommand : FtpCommand
    {
        public FtpMlstCommand() : base("MLST")
        {
        }

        protected override void Run(FtpCommandContext context)
        {
            context.Channel.CheckLogin();

            try
            {
                var path = string.Empty;
                if (!string.IsNullOrWhiteSpace(context.Statement.Argument))
                    path = context.Statement.Argument;

                if (string.IsNullOrWhiteSpace(path))
                    path = context.Channel.CurrentDir;

                var localPath = context.Channel.MapVirtualPathToLocalPath(path);
                context.Statement.Result = localPath;

                FileSystemInfo fileInfo;

                if (File.Exists(localPath))
                    fileInfo = new FileInfo(localPath);
                else if (Directory.Exists(localPath))
                    fileInfo = new DirectoryInfo(localPath);
                else
                    throw new FileNotFoundException(path);
                
                var str = string.Format("250-Listing {0}\r\n{1}250 END", path, FtpMlstFileFormater.Format(context, fileInfo));
                context.Channel.Send(str);
            }
            catch (IOException e)
            {
                throw new InternalException(e.Message);
            }
        }
    }
}
