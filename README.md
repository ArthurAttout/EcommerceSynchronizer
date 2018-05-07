# Ecommerce Synchronizer
This program acts as a middleware to synchronize many POS systems inventory count with a single database. It is part of [acmarche/ecommerce](https://github.com/acmarche/ecommerce) project.
![Workflow](https://raw.githubusercontent.com/ArthurAttout/EcommerceSynchronizer/master/Readme%20capture.PNG)

The program consists of an ASP.NET application, that triggers background jobs (using [Hangfire](https://www.hangfire.io/)) which can be controlled through several endpoints.

## Supported POS
Currently, the synchronizer supports binding for the following POS systems : 
* [Hiboutik](https://www.hiboutik.com/en/) - A simple POS system 
* [Lightspeed Retail](https://www.lightspeedhq.com/pos/retail/) - A professional, easy-to-use POS interface
* [Square POS](https://squareup.com/pos) - A free POS system powered by SquareUp

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Setting up `appsettings.json`

`appsettings.json` is the file that will contain all the credentials to communicate with POS Systems. As per the section "Supported POS", 3 different POS Types are supported.

Each type has it's specific way of handling API requests and authorization. 
The `Type` field of each configuration must match an element of the [`EnumPOSTypes`](https://github.com/ArthurAttout/EcommerceSynchronizer/blob/master/EcommerceSynchronizer/Utilities/EnumPOSTypes.cs) enumeration. 

```
{
  "Logging": {
    "IncludeScopes": false,
    "Debug": {
      "LogLevel": {
        "Default": "Warning"
      }
    },
    "Console": {
      "LogLevel": {
        "Default": "Warning"
      }
    }
  },
  "hangfireDatabaseConnectionString": "connection_string_to_hangfire_database",
  "ecommerceDatabaseConnectionString": "connection_string_to_ecommerce_database",
  "PosAccess": {
    "PosConfigurations": [
      {
        "AccessToken": "your_squarePOS_access_token",
        "AccountID": "your_squarePOS_account_id",
        "Type": "1"
      },
      {
        "AccessToken": "your_hiboutik_access_token",
        "EmailAddress": "you@outlook.com",
        "AccountName": "your_hibouik_account_name",
        "MaximumRequests": 10000, 
        "Type": "2"
      },
      {
        "AccessToken": "your_lightspeed_access_token",
        "RefreshToken": "your_lightspeed_refresh_token",
        "AccountID": "your_lightspeed_account_id",
        "ClientID": "your_lightspeed_client_id",
        "ClientSecret": "your_lightspeed_cient_secret",
        "EmployeeID": 1,
        "RegisterID": 1,
        "ShopID": 1,
        
        "Type": "3"
      }
    ]
  } 
} 
```
You must register all the items you want to be synchronized between the main stock database and the POS systems. 
```
curl -X POST \
  http://localhost:port/api/items \
  -H 'Cache-Control: no-cache' \
  -H 'Content-Type: application/json' \
  -d '{
	"item_name":"The name of the item you want to keep synchronized",
	"item_ecommerce_id":{The ID of the item on the Ecommerce database},
	"account_id":"The account ID of the POS that holds the item"
}'
```

OR

```
curl -X POST \
  http://localhost:port/api/items \
  -H 'Cache-Control: no-cache' \
  -H 'Content-Type: application/json' \
  -d '{
	"item_pos_id":"The ID (on the POS) of the item you want to keep synchronized",
	"item_ecommerce_id":{The ID of the item on the Ecommerce database},
	"account_id":"The account ID of the POS that holds the item"
}'
```
### Controlling the synchronizer

Try to force an update to check if the main database successfully updates according to the POS systems you bound to `appsettings.json`

```
curl -X POST \
  http://localhost:port/api/synchronizer/forceUpdate
```

You may start or stop the synchronizer

```
curl -X POST \
  http://localhost:port/api/synchronizer/start
```

```
curl -X POST \
  http://localhost:port/api/synchronizer/stop
```
### Post a new sale

When an item is sold on the e-commerce website, a request must be triggered to dispatch the update on the appropriate POS system
```
curl -X POST \
  http://localhost:port/api/synchronizer/sale \
  -H 'Cache-Control: no-cache' \
  -H 'Content-Type: application/json' \
  -d '{
	"item_ecommerce_id":{The ID of the item sold},
	"delta":{The quantity sold},
	"balance":{The amount in cents paid by the customer}
}'
```

### Monitor the updates with Hangfire

Go to 
```
http://localhost:port/hangfire
```
### Change the CRON expression 

The frequency of the updates (polling) is based on a Hangfore CRON expression, which can be changed in the [`SynchronizerStatusController`](https://github.com/ArthurAttout/EcommerceSynchronizer/blob/master/EcommerceSynchronizer/Controllers/SynchronizerStatusController.cs#L39).

The structure of CRON expression used by Hangfire slightly differs from the original CRON. 

```
field #   meaning        allowed values
-------   ------------   --------------
   1      minute         0-59
   2      hour           0-23
   3      day of month   1-31
   4      month          1-12 (or names, see below)
   5      day of week    0-7 (0 or 7 is Sun, or use names)
```

I.E. `* 11,12,13 * * 1,2,3,4,5` is every minutes, of 11h,12h and 13h, of monday,tuesday,wednesday,thursday and friday.

## Built With

* [ASP.NET](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/?view=aspnetcore-2.1) - The framework used
* [Hangfire](https://www.hangfire.io/) - Background tasks management
* MySQL - The main database

## Authors

* **Arthur Attout** - As part of an internship during a Master's degree in Information Systems Architecture

## License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file for details

## Acknowledgments

This project is inspired by [Omnivore.io](https://omnivore.io/) and [SimplicityPOS](https://simplicitypos.io/)

