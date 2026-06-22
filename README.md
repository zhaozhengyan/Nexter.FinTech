## PigPig Finance

`PigPig` is an open-source personal finance WeChat Mini Program, built with modern technologies for a complete bookkeeping experience.

### Features

- **Transaction Tracking** — Record income/expenses with categories, filtering, and sorting on the home page
- **Item Management** — Track purchases with price, usage days, and auto-calculated daily cost; supports additional items (income/expense) as JSON list for effective cost calculation; retired items with distinct gray styling; custom drag-to-sort ordering
- **Asset Accounts** — Multi-account asset management
- **Family Group** — Create a family group for shared bookkeeping across members
- **Timed Reminders** — Bookkeeping reminders (pending subscription message migration)

### Project Structure

``` lua
├─Nexter.FinTech
│  ├─Nexter.Fintech.Application
│  │  ├─DTO --Data transfer objects
│  │  ├─Job --Background jobs
│  │  └─Wechat
│  │      ├─DTO
│  │      └─Options
│  ├─Nexter.Fintech.Core
│  ├─Nexter.Fintech.Database.Migrations
│  ├─Nexter.Fintech.EntityFramework.Core
│  ├─Nexter.Fintech.Web.Core
│  └─Nexter.Fintech.Web.Entry
└─WechatClient
    ├─iconfont --Icon fonts (Remix Icon + iconfont)
    ├─images
    ├─pages
    │  ├─about --About page
    │  ├─account --Asset accounts
    │  ├─detail --Transaction detail
    │  ├─filter --Transaction filter
    │  ├─group --Family group
    │  ├─index --Home (transactions)
    │  ├─item-detail --Item detail/edit
    │  ├─items --Item list
    │  ├─login --Login page
    │  ├─personal --Profile center
    │  ├─set-category --Category settings
    │  ├─set-group --Create family group
    │  ├─tabBar-template --Custom tab bar template
    │  ├─tally --Add transaction
    │  └─timing --Reminder settings
    └─utils --Utilities (HTTP requests, auth, etc.)
```

### Tech Stack

#### Backend

Technology | Description | Version
----|----|----
Furion | Enterprise-grade .NET application framework | 4.9.8.96
ASP.NET Core | Web API framework | .NET 10
EntityFrameworkCore | ORM | 10.x
Oracle MySql.EntityFrameworkCore | MySQL database provider | 10.x
WebApiClientCore | Declarative REST API client | —

#### Frontend

Technology | Description
----|----
WeChat Mini Program (native) | WXML + WXSS + JS
Remix Icon | Icon font for items (subsetted, base64 embedded)
iconfont | Icon font for transactions

### Deployment

#### Database Migration

```bash
# Generate SQL script
dotnet ef migrations script -o migration.sql

# Or apply migrations directly
dotnet ef database update
```

#### Configuration

Edit `appsettings.json`:

- `ConnectionStrings.Fintech` — MySQL connection string
- `Wechat.BaseUrl` — WeChat API base URL (`https://api.weixin.qq.com`)
- `Wechat.Appid` / `Wechat.Secret` — Mini Program credentials
