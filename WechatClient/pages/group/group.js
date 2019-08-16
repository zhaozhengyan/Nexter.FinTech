// pages/group/group.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({
  data: {
    group: {}
  },

  onLoad: function(options) {
    wx.showShareMenu({
      withShareTicket: true //// 要求小程序返回分享目标信息
    });
    var url = app.globalData.httpGetUrl + 'group';
    utils.http_get(url, this.showPageData);
  },
  showPageData: function(res) {
    var accountLen = res.members.length;
    var group = {
      totalMoney: res.totalMoney,
      totalIncome: res.totalIncome,
      totalSpending: res.totalSpending,
      members: res.members,
      accountLen: accountLen
    }
    this.setData({
      group: group
    });
  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function() {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function(ops) {
    debugger;
    if (ops.from === 'button') {
      // 来自页面内转发按钮
      console.log(ops.target)
    }
    var nickName = app.globalData.userInfos.nickName;
    return {
      imageUrl: '/images/logo.png',
      title: nickName + '邀请你加入家庭',
      path: 'pages/index/index',
      success: function(res) {
        debugger;
        // 转发成功
        console.log("转发成功:" + JSON.stringify(res));
        var shareTickets = res.shareTickets;
        if (shareTickets.length == 0) {
          return false;
        }
        //可以获取群组信息
        wx.getShareInfo({
          shareTicket: shareTickets[0],
          success: function (res) {
            console.log(res)
          }
        })
      },
      fail: function(res) {
        // 转发失败
        console.log("转发失败:" + JSON.stringify(res));
      }
    }
  }
})