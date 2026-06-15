//app.js
const utils = require('./utils/util.js')

App({
  globalData: {
    userInfos: null,
    token: null,
    defaultTab: 'index',
    combinedTab: 'index',
    // baseUrl: 'https://fintech-api.zhaoblogs.com/'
    baseUrl: 'http://localhost:5000/'
  },
  onLaunch: function() {
    // 检查登录状态并获取用户信息
    this.checkLogin();
  },

  checkLogin: function() {
    var token = wx.getStorageSync('token');
    if (token) {
      this.globalData.token = token;
      this.loadUserInfo();
    }
  },

  loadUserInfo: function(callback) {
    var that = this;
    var url = this.globalData.baseUrl + 'me';
    // 复用 utils 统一请求（含 token 注入、401 跳登录、错误提示）
    utils.http_get(url, function(res) {
      if (!res) {
        if (callback) callback('index');
        return;
      }
      that.globalData.userInfos = res;
      var tab = (res.defaultTab || '').toLowerCase();
      if (tab !== 'index' && tab !== 'items') tab = 'index';
      console.log('[App] 用户默认页配置:', res.defaultTab, '→ 实际跳转:', tab);
      that.globalData.defaultTab = tab;
      that.globalData.combinedTab = tab;
      // 启动时若当前页非默认页，则跳转到默认页
      that.navigateToDefaultTab(tab);
      if (callback) callback(tab);
    }, null, true);
  },

  // 根据用户设置的默认页进行 switchTab 跳转
  navigateToDefaultTab: function(tab) {
    var pagePathMap = {
      'index': 'pages/index/index',
      'items': 'pages/items/items'
    };
    // 只允许账单/物品作为默认页，其他值回退到账单
    if (!pagePathMap[tab]) tab = 'index';
    var targetPath = pagePathMap[tab];

    // 获取当前页面栈，避免重复跳转/非 tabBar 页跳转异常
    var pages = getCurrentPages();
    var currentRoute = pages.length > 0 ? pages[pages.length - 1].route : '';
    console.log('[App] navigateToDefaultTab — current:', currentRoute, 'target:', targetPath);
    if (currentRoute === targetPath) return;

    // 仅在首页或同为 tabBar 页时才切换，避免在普通页面被强制打断
    var tabBarPaths = ['pages/index/index', 'pages/items/items', 'pages/personal/personal'];
    if (tabBarPaths.indexOf(currentRoute) === -1) {
      console.log('[App] 当前页不在 tabBar 页列表中，跳过跳转');
      return;
    }

    wx.switchTab({
      url: '/' + targetPath,
      success: function() {
        console.log('[App] switchTab 成功:', targetPath);
      },
      fail: function(err) {
        console.error('[App] switchTab 失败:', err);
      }
    });
  }
})
