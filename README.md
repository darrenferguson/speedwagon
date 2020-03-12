# Speedwagon

Speedwagon is an experimental developer centric blogging tool written in .NET core.

This is just an expeirment, it isn't particularly production ready.

# About Speedwagon

Goals and objectives are:

- Database Free
- Non pescriptive - in terms of JS CSS, frameworks
- Highly/Easily Scalable
- Simple content deployments (using flat files)
- Easy content and configuration versioning

# History

This is a port of the .NET project I wrote and blogged about a few years back. It had been ported to .NET core and has a *very* basic UI for editing content and content types - with the goal of making it easy to develop a blog platform that .NET develops find easy to spin up and work with.

- https://blog.darren-ferguson.com/2015/06/14/a-runtime-for-umbraco/
- https://blog.darren-ferguson.com/2015/06/16/a-runtime-for-umbraco-part-2/
- https://blog.darren-ferguson.com/2015/06/20/a-runtime-for-umbraco-part-3/
- https://blog.darren-ferguson.com/2015/09/25/a-runtime-for-umbraco-part-4/

# What this allows you to do

- Easily add content management to a .NET core app
- Separate the runtime and content editing parts of the application
- Manage content and content type configuration in a UI or on the filesystem
- Version content and configuration using GIT or similar
- Easily add custom editors (fields in which you add content)
- Low barrier to entry - use the client side dependencies of your choice

# Getting started

After cloning the repository

- Change into the SpeedWagon.Web.UI
- Run npm install
- Run gulp build
- Run the SpeedWagon.Web.UI from within Visual Studio

# Questions

I'm happy to add more documentation - just raise an issue here.

I'll hopefully get around to adding some demos, screencast soon.


