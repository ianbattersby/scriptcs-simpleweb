namespace ScriptCs.SimpleWebCs
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.Reflection;

    using Fix;
    using Flux;

    using ScriptCs.Contracts;

    public class SimpleWebPack : IScriptPackContext
    {
        private const int ComparisonCharMatch = 3;

        private Fixer _fixer;
        private bool _stop;
        private readonly Assembly[] _assemblies;

        public SimpleWebPack()
        {
            this._assemblies = new[] { typeof(Simple.Web.Behaviors.IETag).Assembly };
        }

        public void StartServer(int port = 3333)
        {
            using (var server = new Server(port))
            {
                _fixer = new Fixer(server.Start, server.Stop);

                FixUpAssemblies();

                Console.CancelKeyPress += ConsoleOnCancelKeyPress;
                Console.TreatControlCAsInput = false;

                _fixer.Start();

                Console.WriteLine(
                    "Flux: listening on port {0}. Press CTRL-C to stop.", port);

                while (!_stop)
                {
                    Console.ReadKey();
                }
            }
        }

        public void StopServer()
        {
            if (_fixer != null)
            {
                _fixer.Stop();
            }
            _stop = true;
        }

        private void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            this.StopServer();

            consoleCancelEventArgs.Cancel = false;
        }

        private void FixUpAssemblies()
        {
            using (var catalog = new AggregateCatalog())
            {
                foreach (var assembly in this._assemblies)
                {
                    try
                    {
                        if (assembly.FullName.StartsWith("Microsoft.") || assembly.FullName.StartsWith("System."))
                        {
                            continue;
                        }

                        var assemblyCatalog = new AssemblyCatalog(assembly);

                        catalog.Catalogs.Add(assemblyCatalog);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.Message);
                    }
                }

                var container = new CompositionContainer(catalog);

                container.ComposeParts(this._fixer);
            }
        }
    }
}