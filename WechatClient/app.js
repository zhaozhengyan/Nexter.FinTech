//app.js
App({
  globalData: {
    userInfos: null,
    token: null,
    //httpGetUrl: 'https://fintech-api.zhaoblogs.com/'
    httpGetUrl: 'http://localhost:60933/'
  },
  onLaunch: function() {
    debugger;
    console.log(wx.env)
  }
})