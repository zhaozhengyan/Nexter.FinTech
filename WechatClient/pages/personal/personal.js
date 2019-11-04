// pages/personal/personal.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({

  /**
   * 页面的初始数据
   */
  data: {
    userInfos: null,
    tabBarSelected: {
      tabBarIndexSelected: false,
      tabBarPersonalSelected: true
    }
  },

  showPageData: function(res) {
    var userInfos = app.globalData.userInfos;
    if (userInfos == null) {
      userInfos = res;
    }
    userInfos.joinTime = res.joinTime;
    userInfos.totalMoney = res.totalMoney;
    userInfos.totalDays = res.totalDays;
    userInfos.count = res.count;
    userInfos.groupId = res.groupId;
    userInfos.reminderTime = res.reminderTime;
    app.globalData.userInfos = userInfos;
    console.log(app.globalData.userInfos);
    this.setData({
      userInfos: userInfos
    });
  },

  onOpenPageTap: function(event) {
    var url = event.currentTarget.dataset.pageUrl;
    if (url) {
      wx.navigateTo({
        url: url
      })
    }
  },

  /**
   * 生命周期函数--监听页面显示
   */
  onLoad: function() {
    wx.hideTabBar({
      aniamtion: false
    });
    var url = app.globalData.baseUrl + 'me';
    utils.http_get(url, this.showPageData);
  }
})