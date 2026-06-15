// pages/group/group.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({
  data: {
    group: {}
  },

  onLoad: function(options) {
    var url = app.globalData.baseUrl + 'group';
    utils.http_get(url, this.showPageData.bind(this));
  },
  showPageData: function(res) {
    var members = res.members || [];
    var accountLen = members.length;
    var group = {
      id: res.id,
      isAdmin: res.isAdmin,
      totalMoney: res.totalMoney,
      totalIncome: res.totalIncome,
      totalSpending: res.totalSpending,
      members: members,
      accountLen: accountLen
    }
    this.setData({
      group: group
    });
  },
  leaveGroup: function() {
    if (!this.data.group.id) {
      wx.showToast({ icon: 'none', title: '数据加载中，请稍后' });
      return;
    }
    var url = app.globalData.baseUrl + 'group/quit?id=' + this.data.group.id;
    utils.http_post(url, {}, this.goToIndex.bind(this));
  },
  goToIndex: function(res) {
    wx.showToast({
      title: '退出成功',
      complete: function() {
        wx.switchTab({
          url: '../personal/personal',
        })
      }
    })
  },
  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function() {
    var url = app.globalData.baseUrl + 'group';
    utils.http_get(url, this.showPageData.bind(this), null, true);
  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function(ops) {
    if (ops.from === 'button') {
      // 来自页面内转发按钮
      console.log(ops.target)
    }
    var nickName = (app.globalData.userInfos && app.globalData.userInfos.nickName) || '';
    var token = wx.getStorageSync('token');
    return {
      imageUrl: '/images/logo.png',
      title: nickName + '邀请你加入家庭',
      path: 'pages/login/login?inviterId=' + token,
      success: function(res) {
        // 转发成功
        console.log("转发成功:" + JSON.stringify(res));
        var shareTickets = res.shareTickets;
        if (shareTickets.length == 0) {
          return false;
        }
        //可以获取群组信息
        wx.getShareInfo({
          shareTicket: shareTickets[0],
          success: function(res) {
            console.log(res)
          }
        })
      },
      fail: function(res) {
        // 转发失败
        console.log("转发失败:" + JSON.stringify(res));
      },
      complete: function(res) {
        console.log("转发完成:" + JSON.stringify(res));
      }
    }
  }
})