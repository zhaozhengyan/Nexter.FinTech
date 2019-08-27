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
  onShow: function(e) {
    var reminderTime = app.globalData.userInfos.reminderTime;
    console.log(reminderTime)
    if (!utils.isNull(reminderTime)){
      this.setData({
        time: reminderTime
      });
    }
  },
  formSubmit: function(e) {
    var that = this;
    wx.showModal({
      title: '提示',
      content: '当天只能提醒一次哦',
      success: function(sm) {
        if (sm.confirm) {
          that.goSubscribe(e.detail.formId);
        } else if (sm.cancel) {
          console.log('取消')
        }
      }
    })
  },
  onTimingSwitch: function(e) {
    this.setData({
      IsEnabled: e.detail.value
    });
  },
  goSubscribe: function(formId) {
    var url = app.globalData.httpGetUrl + 'me/timedreminder';
    utils.http_post(url, {
      time: this.data.time,
      IsEnabled: this.data.IsEnabled,
      formId: formId
    }, null);
  },
  onTimeChange: function(e) {
    this.setData({
      time: e.detail.value
    });
  }
})