# SpeedWagon Web

Getting started

## Modify your Program.cs to add logging

In Program.cs configure logging in the CreateHostBuilder method e.g.

```
public static IHostBuilder CreateHostBuilder(string[] args) =>
        
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder => {
            webBuilder.UseStartup<Startup>(); }
        )
        .ConfigureLogging(logging => {
            logging.ClearProviders();
            logging.AddConsole(); }
        );
    }
```

## Inject dependencies to your startup class

Modify startup.cs so that the following are injected:

- IWebHostEnvironment
- IConfiguration

This is as simple as adding some privacy properties and creating a constructor like below:

```
using Microsoft.Extensions.Configuration;

private readonly IWebHostEnvironment _env;
private IConfiguration _configuration;

public Startup(IConfiguration configuration, 
    IWebHostEnvironment env)
{
    this._configuration = configuration;
    this._env = env;
}
```

## Configure Services

In Startup.cs in your ConfigureServices method first decide where you
want speedwagon to store content.

```
string speedWagonContentPath = Path.Combine(this._env.ContentRootPath, "speedWagonContent");
```

