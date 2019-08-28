// pages/set-group/set-group.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({

  /**
   * 页面的初始数据
   */
  data: {
    name: null
  },
  /*获取备注***/
  nameInput: function(e) {
    this.setData({
      name: e.detail.value
    })
  },
  AddGroup: function(e) {
    if (utils.isNull(this.data.name)) {
      wx.showToast({
        icon: 'none',
        title: '请填写名称哦'
      })
      return;
    }
    var url = app.globalData.baseUrl + 'group';
    utils.http_post(url, {
      name: this.data.name
    }, this.goToGroup, "阿偶！服务器打了个盹");
  },
  goToGroup: function(res) {
    wx.switchTab({
      url: '../personal/personal',
    })
  }
})