﻿using Anotar.NLog;
using Ninject;
using Ninject.Extensions.Factory;
using SOVND.Lib;
using System;
using System.Text;
using SOVND.Lib.Handlers;
using SOVND.Lib.Models;
using SOVND.Server.Settings;
using System.Threading;
using System.Linq;
using SOVND.Server.Handlers;

namespace SOVND.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IKernel kernel = new StandardKernel();

                // Factories
                kernel.Bind<IChannelHandlerFactory>().ToFactory();
                kernel.Bind<IChatProviderFactory>().ToFactory();

                // Singletons
                kernel.Bind<RedisProvider>().ToSelf().InSingletonScope();

                // Standard lifetime
                kernel.Bind<IPlaylistProvider>().To<SortedPlaylistProvider>();
                kernel.Bind<IMQTTSettings>().To<ServerMqttSettings>();

                var server = kernel.Get<Server>();
                server.Run();

                if (args.Any(s => s.Equals("-d", StringComparison.CurrentCultureIgnoreCase)))
                {
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                LogTo.FatalException("Unhandled exception", e);
                throw;
            }
        }
    }
}
