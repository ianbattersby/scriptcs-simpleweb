###scriptcs-SimpleWeb

##Simple.Web Script Pack

```csharp
[UriTemplate("/")]
public class GetIndex : IGet, IOutput<RawHtml>
{
    public Status Get()
    {
        return 200;
    }

    public RawHtml Output
    {
        get { return "<h2>Simple.Web rules supreme!</h2>"; }
    }
}

var SimpleWeb = Require<SimpleWeb>();

SimpleWeb.StartServer(8080);
```

	scriptcs -install Flux
	scriptcs -install Simple.Web

	scriptcs start.csx