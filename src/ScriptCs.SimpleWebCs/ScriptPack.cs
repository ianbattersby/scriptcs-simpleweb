namespace ScriptCs.SimpleWebCs
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
                    "Simple.Web.Behaviors",
                };

            namespaces.ToList().ForEach(session.ImportNamespace);
        }

        public IScriptPackContext GetContext()
        {
            return new SimpleWebPack();
        }

        public void Terminate()
        {
        }
    }
}