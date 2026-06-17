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
    loggedIn: false,
    tabBarSelected: {
      combinedTab: 'index',
      tabBarIndexSelected: true,
      tabBarPersonalSelected: false
    },
    tallyData: { "count": 0, "monthTotalMoneys": [0.0, 0.0], "lists": [] }
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function(options) {
    wx.hideTabBar({
      aniamtion: false
    });
    this.setData({ 'tabBarSelected.combinedTab': app.globalData.combinedTab || 'index' });
    //初始化时间
    var toDayDate = utils.formatDate(new Date());
    var date = toDayDate.split('-')[0] + '年' + toDayDate.split('-')[1] + '月';
    var dateValue = toDayDate.slice(0, 7);
    var month = toDayDate.substr(5, 2);
    this.setData({
      date: date,
      dateValue: dateValue,
      month: month,
      endDate: toDayDate,
      loggedIn: utils.isLoggedIn()
    });
  },

  onCombinedTabTap: function() {
    var tab = app.globalData.combinedTab || 'index';
    if (tab !== 'index') {
      wx.switchTab({ url: '/pages/' + tab + '/' + tab });
    }
  },

  onCombinedTabLongPress: function() {
    app.globalData.combinedTab = 'items';
    wx.switchTab({ url: '/pages/items/items' });
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
  },

  onDateChange: function(e) {
    // 改变日期
    var date = e.detail.value.split('-')[0] + '年' + e.detail.value.split('-')[1] + '月';
    this.setData({
      date: date,
      dateValue: e.detail.value
    });
    this.queryTransaction(true);
  },
  // 类型筛选
  onOpenFilterTap: function(event) {
    var categoryId = event.currentTarget.dataset.categoryId;
    wx.navigateTo({
      url: '../filter/filter?category_id=' + categoryId,
    })
  },
  //查看账单详情
  onOpenTallyDetailTap: function(event) {
    var id = event.currentTarget.dataset.id;
    wx.navigateTo({
      url: '../detail/detail?id=' + id,
    })
  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function() {
    var loggedIn = utils.isLoggedIn();
    this.setData({ loggedIn: loggedIn });
    if (loggedIn) {
      this.queryTransaction(true);
    }
  },
  // 下拉刷新
  onPullDownRefresh: function() {
    if (!utils.isLoggedIn()) {
      wx.stopPullDownRefresh();
      return;
    }
    this.clearCache();
    this.queryTransaction(true); //静默刷新，下拉动画本身就是loading
  },
  /**
   * 筛选账单主函数
   */
  queryTransaction: function(silent) {
    var url = app.globalData.baseUrl + 'transaction?date=' + this.data.dateValue;
    if (this.data.categoryId !== this.data.categoryIdChange) {
      url = url + '&categoryId=' + this.data.categoryIdChange;
      this.setData({
        categoryId: this.data.categoryIdChange
      });
    };
    utils.http_get(url, this.outPutList.bind(this), null, silent);
  },
  outPutList: function(res) {
    this.setData({
      tallyData: res
    });
    wx.stopPullDownRefresh();
  },
  // 清缓存
  clearCache: function() {
    this.setData({
      tallyData: { "count": 0, "monthTotalMoneys": [0.0, 0.0], "lists": [] }
    });
  }
})
