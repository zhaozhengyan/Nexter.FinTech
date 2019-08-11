// pages/index/index.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({

  /**
   * 页面的初始数据
   */
  data: {
    date: '',
    endDate: '',
    month: '',
    filterModuleHide: true,
    categoryId: 0,
    categoryIdChange: 0,
    tabBarSelected: {
      tabBarIndexSelected: true,
      tabBarPersonalSelected: false
    },
    tallyData: {}
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function(options) {
    wx.hideTabBar({
      aniamtion: false
    });
    //初始化时间
    var toDayDate = utils.formatDate(new Date());
    var date = toDayDate.split('-')[0] + '年' + toDayDate.split('-')[1] + '月';
    var dateValue = toDayDate.slice(0, 7);
    var month = toDayDate.substr(5, 2);
    this.setData({
      date: date,
      dateValue: dateValue,
      month: month,
      endDate: toDayDate
    });
  },

  outPutList: function(res) {
    var tallyData = res;
    this.setData({
      tallyData: tallyData
    });
  },

  onDateChange: function(e) {
    // 改变日期
    var date = e.detail.value.split('-')[0] + '年' + e.detail.value.split('-')[1] + '月';
    this.setData({
      date: date,
      dateValue: e.detail.value
    });
    this.queryTransaction();
  },

  onOpenFilterTap: function(event) {
    // 类型筛选
    var categoryId = event.currentTarget.dataset.categoryId;
    wx.navigateTo({
      url: '../filter/filter?category_id=' + categoryId,
    })
  },

  onOpenTallyDetailTap: function(event) {
    //查看账单详情
    var id = event.currentTarget.dataset.id;
    wx.navigateTo({
      url: '../detail/detail?id=' + id,
    })
  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function() {
    this.queryTransaction();
  },
   /**
   * 筛选账单主函数
   */
  queryTransaction:function(){
    var pages = getCurrentPages();
    var currentPage = pages[pages.length - 1];
    var url = app.globalData.httpGetUrl + 'transaction?date=' + this.data.dateValue;
    if (this.data.categoryId !== this.data.categoryIdChange) {
      url = url + '&categoryId=' + this.data.categoryIdChange;
      console.log('筛选成功');
      this.setData({
        categoryId: this.data.categoryIdChange
      });
    };
    utils.http_get(url, this.outPutList);
  }
})