namespace ScriptCs.SimpleWeb
{
    using System.Linq;

    using ScriptCs.Contracts;

    public class ScriptPack : IScriptPack
    {
        public void Initialize(IScriptPackSession session)
        {
            var namespaces = new[]
                {
                    "Simple.Web",
                };

            namespaces.ToList().ForEach(session.ImportNamespace);
        }

        public IScriptPackContext GetContext()
        {
            return new SimpleWeb();
        }

        public void Terminate()
        {
        }
    }
}