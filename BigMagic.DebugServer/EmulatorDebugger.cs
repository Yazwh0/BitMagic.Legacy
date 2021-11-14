using BitMagic.Common;
using BitMagic.Emulation;
using OmniSharp.Extensions.DebugAdapter.Server;
using Serilog;
using System;
using System.Threading.Tasks;

namespace BigMagic.DebugServer
{
    public class EmulatorDebugger
    {
        private readonly DebugAdapterServer _debugServer;
        private readonly Project _project;
        private Emulator? _emulator;

        public EmulatorDebugger(Project project)
        {
            _project = project;
            var options = new DebugAdapterServerOptions();
            options.WithOutput(Console.OpenStandardOutput());
            options.WithInput(Console.OpenStandardInput());

            //options.Capabilities.SupportsTerminateRequest = true;
            //options.Capabilities.SupportTerminateDebuggee = true;

            options.ConfigureLogging(
                x => x.AddSerilog(Log.Logger)
            );

            options.OnStarted(async (server, cancelToken) => {
                // server.
            });

            _debugServer = DebugAdapterServer.Create(options);            
        }

        public async Task DebugStart()
        {
            _emulator = new Emulator(_project);
            _emulator.LoadPrg();

            await _debugServer.Initialize(new System.Threading.CancellationToken());
            _emulator.Emulate(0x810);
        }
    }
}
