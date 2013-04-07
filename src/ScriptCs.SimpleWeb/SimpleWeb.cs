namespace ScriptCs.SimpleWeb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using Fix;
    using Flux;

    using ScriptCs.Contracts;

    public class SimpleWeb : IScriptPackContext
    {
        private const int ComparisonCharMatch = 3;

        private Fixer _fixer;
        private bool _stop;
        private readonly Assembly[] _assemblies;

        public SimpleWeb()
        {
            this._assemblies = new[] { FindTheCallingAssembly() };
        }

        public SimpleWeb(IEnumerable<Assembly> assemblies)
        {
            this._assemblies = assemblies.ToArray();
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
                    "Flux {0}: listening on port {1}. Press CTRL-C to stop.",
                    Assembly.GetExecutingAssembly().GetName().Version,
                    port);

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

        public Assembly FindTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            Assembly executingAssembly = Assembly.GetCallingAssembly();
            Assembly callingAssembly = null;

            for (int i = 0; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);
                Assembly assembly = frame.GetMethod().DeclaringType.Assembly;

                if (IsQualifyingAssembly(assembly.FullName, executingAssembly.FullName))
                {
                    callingAssembly = assembly;
                }
            }

            return callingAssembly;
        }


        private static bool IsQualifyingAssembly(string assemblyName, string matchAssembly)
        {
            for (var i = 0; ; i++)
            {
                if (i > ComparisonCharMatch || i > (assemblyName.Length - 1) || i > (matchAssembly.Length - 1) || assemblyName[i] != matchAssembly[i])
                {
                    return i >= ComparisonCharMatch;
                }
            }
        }
    }
}