// pages/personal/personal.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({

  /**
   * 页面的初始数据
   */
  data: {
    loggedIn: false,
    userInfos: null,
    tabBarSelected: {
      combinedTab: 'index',
      tabBarIndexSelected: false,
      tabBarPersonalSelected: true
    },
    defaultTab: 'index',
    defaultTabText: '账单'
  },

  showPageData: function(res) {
    var userInfos = app.globalData.userInfos;
    if (userInfos == null) {
      userInfos = res;
    }
    userInfos.joinTime = res.joinTime ? res.joinTime.split('T')[0] : '';
    userInfos.totalMoney = res.totalMoney;
    userInfos.totalDays = res.totalDays;
    userInfos.count = res.count;
    userInfos.groupId = res.groupId;
    userInfos.reminderTime = res.reminderTime;
    app.globalData.userInfos = userInfos;

    var defaultTab = res.defaultTab || 'index';
    var tabMap = {
      'index': '账单',
      'items': '物品'
    };

    this.setData({
      loggedIn: true,
      userInfos: userInfos,
      defaultTab: defaultTab,
      defaultTabText: tabMap[defaultTab] || '账单'
    });
  },

  onOpenPageTap: function(event) {
    if (!utils.isLoggedIn()) {
      wx.navigateTo({ url: '../login/login' });
      return;
    }
    var url = event.currentTarget.dataset.pageUrl;
    if (url) {
      wx.navigateTo({
        url: url
      })
    }
  },

  onDefaultTabTap: function(e) {
    var selectedTab = e.currentTarget.dataset.tab;
    if (selectedTab === this.data.defaultTab) return;

    var tabNames = { 'index': '账单', 'items': '物品' };
    var that = this;
    var url = app.globalData.baseUrl + 'me/defaulttab';
    utils.http_put(url, { tab: selectedTab }, function() {
      that.setData({
        defaultTab: selectedTab,
        defaultTabText: tabNames[selectedTab]
      });
    });
  },

  onLoad: function() {
    wx.hideTabBar({
      aniamtion: false
    });
    this.setData({ 'tabBarSelected.combinedTab': app.globalData.combinedTab || 'index' });
    // 首次加载时如果有缓存数据先展示
    if (app.globalData.userInfos) {
      this.setData({ loggedIn: true, userInfos: app.globalData.userInfos });
    }
  },

  onShow: function() {
    this.setData({ 'tabBarSelected.combinedTab': app.globalData.combinedTab || 'index' });
    var loggedIn = utils.isLoggedIn();
    this.setData({ loggedIn: loggedIn });
    if (loggedIn) {
      var url = app.globalData.baseUrl + 'me';
      utils.http_get(url, this.showPageData.bind(this), null, true);
    }
  },

  onCombinedTabTap: function() {
    var tab = app.globalData.combinedTab || 'index';
    wx.switchTab({ url: '/pages/' + tab + '/' + tab });
  },

  onCombinedTabLongPress: function() {
    var current = app.globalData.combinedTab || 'index';
    var next = current === 'index' ? 'items' : 'index';
    app.globalData.combinedTab = next;
    wx.switchTab({ url: '/pages/' + next + '/' + next });
  },

  onAddTap: function() {
    if (!utils.isLoggedIn()) {
      wx.navigateTo({ url: '../login/login' });
      return;
    }
    var tab = app.globalData.combinedTab || 'index';
    if (tab === 'items') {
      wx.navigateTo({ url: '../item-detail/item-detail' });
    } else {
      wx.navigateTo({ url: '../tally/tally' });
    }
  }
})
