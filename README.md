###scriptcs-SimpleWeb

##Simple.Web Script Pack

####Note
* Currently Flux/Fix 0.3 are only availble from our TeamCity CI NuGet source: http://teamcity.cloudapp.net/guestAuth/app/nuget/v1/FeedService.svc/

* ScriptCs nightly builds nuget source is required to support the below package story: http://www.myget.org/F/scriptcsnightly/

####Script

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

SimpleWeb.StartServer(3333);
```

####Install

	scriptcs -install Flux
	scriptcs -install Simple.Web

####Run

	scriptcs start.csx

####Browse
[http://localhost:3333](http://localhost:3333)