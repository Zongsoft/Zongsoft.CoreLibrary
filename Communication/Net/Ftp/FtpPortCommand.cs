using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Zongsoft.Communication.Net.Ftp
{
    internal class FtpPortCommand : FtpCommand
    {
        public FtpPortCommand() : base("PORT")
        {
        }

        protected override void Run(FtpCommandContext context)
        {
            context.Channel.CheckLogin();

            if (string.IsNullOrWhiteSpace(context.Statement.Argument))
                throw new SyntaxException();

            var split = context.Statement.Argument.Split(',');

            if (split.Length != 6)
                throw new SyntaxException();

            var ip = string.Join(".", split, 0, 4);
            int port = int.Parse(split[4]) * 256 + int.Parse(split[5]);
            var address = new IPEndPoint(IPAddress.Parse(ip), port);

            context.Channel.CreatePortDataChannel(address);
        }
    }
}
