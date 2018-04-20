# Slidable/Shows

This repo holds the primary Shows service, which is where viewers can follow along with a
presentation as it's being given.

The Shows service is backed by a PostgreSQL database, and is read-only except for a couple of
queue-driven background services that accept updates from the Presenter service. So everything
in here allows anonymous access, which makes things simpler.
