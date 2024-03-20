# ChurchLiveScheduler.api

ChurchLiveScheduler.api is a collection of API endpoints to retrieve and maintain a church's live stream schedule.  This project is implemented as Azure HTTP Trigger functions.

## Database

[SQLite](https://www.sqlite.org/) is used here in an attempt to keep the cost of hosting to a
minimum.  The database is found in the file schedule.db