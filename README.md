# Slidable/Shows

This repo holds the primary Shows service, which is where viewers can follow along with a
presentation as it's being given.

The Shows service is backed by a PostgreSQL database, and is read-only except for a couple of
queue-driven background services that accept updates from the Presenter service. So everything
in here allows anonymous access, which makes things simpler.

## TODO

Right now, the [View Show page](https://github.com/slidable/Shows/blob/master/src/Slidable.Shows/Views/Home/Show.cshtml)
has hard-coded references to the Questions, Notes and Realtime 
services, and HTML elements for the Questions and Notes client code to attach to. I'd like to
get some sort of Service Discovery working here, to allow extra services to be added in
without needing to modify this code.
