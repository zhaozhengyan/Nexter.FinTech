// pages/timing/timing.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({

  /**
   * 页面的初始数据
   */
  data: {
    time: '21:00'
  },
  onTimingSwitch: function(e) {
    var url = app.globalData.httpGetUrl + 'me/timedreminder';
    var timeSwitch = e.detail.value;
      utils.http_post(url, {
        time: this.data.time,
        IsEnabled: timeSwitch
      }, null);
    console.log(e.detail.value);
  },

  onTimeChange: function(e) {
    this.setData({
      time: e.detail.value
    });
  }
})