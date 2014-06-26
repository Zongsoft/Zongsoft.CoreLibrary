using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zongsoft.Communication.Net.Ftp
{
    /// <summary>
    /// 列出文件列表
    /// </summary>
    internal class FtpMlsdCommand : FtpCommand
    {
        public FtpMlsdCommand() : base("MLSD")
        {
        }

        protected override void Run(FtpCommandContext context)
        {
            context.Channel.CheckLogin();

            context.Channel.CheckDataChannel();
            context.Channel.DataChannel.Receive();

            try
            {
                context.Channel.Status = FtpSessionStatus.List;

                var path = string.Empty;
                if (!string.IsNullOrWhiteSpace(context.Statement.Argument))
                    path = context.Statement.Argument;

                if (string.IsNullOrWhiteSpace(path))
                    path = context.Channel.CurrentDir;

                var localPath = context.Channel.MapVirtualPathToLocalPath(path);
                context.Statement.Result = localPath;
            
                if (File.Exists(localPath))
                {
                    context.Channel.Send("150 Opening data connection for directory list.");

                    var file = new FileInfo(localPath);
                    WriteFileInfo(context, file);
                }
                else if (Directory.Exists(localPath))
                {
                    context.Channel.Send("150 Opening data connection for directory list.");

                    var localDir = new DirectoryInfo(localPath);

                    //列举目录
                    foreach (var dir in localDir.GetDirectories())
                    {
                        WriteFileInfo(context, dir);
                    }

                    //列举文件
                    foreach (FileInfo file in localDir.GetFiles())
                    {
                        WriteFileInfo(context, file);
                    }
                }
                else
                {
                    throw new DirectoryNotFoundException(path);
                }

                context.Channel.Send("226 Transfer complete.");
            }
            catch (IOException e)
            {
                throw new InternalException(e.Message);
            }
            finally
            {
                context.Channel.CloseDataChannel();
                context.Channel.Status = FtpSessionStatus.Wait;
            }
        }

        private void WriteFileInfo(FtpCommandContext context, FileSystemInfo fileInfo)
        {
            var result = FtpMlstFileFormater.Format(context, fileInfo);

            var data = context.Channel.Encoding.GetBytes(result);
            context.Channel.DataChannel.Send(data, 0, data.Length);
        }
    }
}
