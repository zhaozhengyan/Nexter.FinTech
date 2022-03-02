## PigPig 记账

 `PigPig`项目致力于打造一个完整开源的理财微信小程序，采用现阶段流行技术实现。
 
### 扫码体验

<img src="https://github.com/zhaozhengyan/Notebook/raw/master/img/gh_7b0b4751951e_1280.jpg" width="200px">

 ### 代码结构

``` lua
├─Nexter.FinTech
│  ├─Nexter.Fintech.Application
│  │  ├─DTO --数据传输
│  │  ├─Job --自动任务
│  │  └─Wechat
│  │      ├─DTO
│  │      └─Options
│  ├─Nexter.Fintech.Core
│  ├─Nexter.Fintech.Database.Migrations
│  ├─Nexter.Fintech.EntityFramework.Core
│  ├─Nexter.Fintech.Web.Core
│  └─Nexter.Fintech.Web.Entry
└─WechatClient
    ├─data -- Mock Json数据
    ├─iconfont 
    ├─images
    │  ├─account
    │  └─index
    ├─pages
    │  ├─about --关于
    │  ├─account --资产帐户
    │  ├─detail --账单详情
    │  ├─filter --首页筛选
    │  ├─group --家庭组
    │  ├─icons-template --共用类别
    │  ├─index --首页
    │  ├─login --登录页
    │  ├─personal --个人中心
    │  ├─set-category --设置类别
    │  ├─set-group --创建家庭组
    │  ├─tabBar-template --底部导航栏
    │  ├─tally --记账
    │  └─timing --预约提醒
    └─utils --工具类
```

### 技术选型

#### 后端技术
技术 | 说明 | 官网
----|----|----
ASP.NET Web API | .Net API框架 | [https://docs.microsoft.com/en-us/aspnet/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api](https://docs.microsoft.com/en-us/aspnet/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api)
WebApiClientCore | REST库 | [https://github.com/dotnetcore/WebApiClient](https://github.com/dotnetcore/WebApiClient)
EntityFrameworkCore | Orm框架 | [https://www.nuget.org/packages/Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)

#### 前端技术

技术 | 说明 | 官网
----|----|----
js | 页面逻辑 | [https://developers.weixin.qq.com/miniprogram/dev/framework/structure.html](https://developers.weixin.qq.com/miniprogram/dev/framework/structure.html)
wxml | 	页面结构 | [https://developers.weixin.qq.com/miniprogram/dev/framework/structure.html](https://developers.weixin.qq.com/miniprogram/dev/framework/structure.html)
json | 页面配置 | [https://developers.weixin.qq.com/miniprogram/dev/framework/structure.html](https://developers.weixin.qq.com/miniprogram/dev/framework/structure.html)
wxss | 页面样式表 | [https://developers.weixin.qq.com/miniprogram/dev/framework/structure.html](https://developers.weixin.qq.com/miniprogram/dev/framework/structure.html)
