// pages/detail/detail.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({

  /**
   * 页面的初始数据
   */
  data: {
    idex: '',
    idx: '',
    id: 0,
    details: {}
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function(options) {
    // var sparator = options.idx.indexOf(',');
    // var idex = options.idx.slice(0, sparator);
    // var idx = options.idx.slice(sparator + 1);
    var id = options.id;
    this.setData({
      // idex: idex,
      // idx: idx,
      id: id
    });
    var url = app.globalData.httpGetUrl + 'Transaction/detail?id=' + id;
    utils.http_get(url, this.getDetail);
  },

  getDetail: function(res) {
    this.setData({
      details: res
    });
  },

  DeleteTally: function(e) {
    //删除一笔账单
    var id = this.data.details.id;
    var url = app.globalData.httpGetUrl + 'transaction?id=' + id;
    utils.http_delete(url, {}, this.goToIndex, "阿歐！服務器打了個盹");
  },
  goToIndex: function(res) {
    wx.switchTab({
      url: '../index/index',
    })
  },
})