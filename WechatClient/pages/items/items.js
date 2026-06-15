// pages/items/items.js
const utils = require('../../utils/util.js')
const app = getApp()

Page({
  data: {
    items: [],
    totalCount: 0,
    totalAsset: '0.00',
    dailyCost: '0.00',
    loading: false,
    sortBy: 'days', // days, price, name
    categoryId: null,
    categoryName: '全部',
    categories: [],
    tabBarSelected: {
      combinedTab: 'items',
      tabBarIndexSelected: false,
      tabBarItemsSelected: true,
      tabBarPersonalSelected: false
    }
  },

  onLoad: function () {
    wx.hideTabBar({ animation: false });
    this.setData({ 'tabBarSelected.combinedTab': app.globalData.combinedTab || 'index' });
    this.loadCategories()
    this.loadItems()
  },

  onCombinedTabTap: function() {
    var tab = app.globalData.combinedTab || 'items';
    if (tab !== 'items') {
      wx.switchTab({ url: '/pages/' + tab + '/' + tab });
    }
  },

  onCombinedTabLongPress: function() {
    app.globalData.combinedTab = 'index';
    wx.switchTab({ url: '/pages/index/index' });
  },

  onAddTap: function() {
    var tab = app.globalData.combinedTab || 'items';
    if (tab === 'items') {
      wx.navigateTo({ url: '../item-detail/item-detail' });
    } else {
      wx.navigateTo({ url: '../tally/tally' });
    }
  },

  onShow: function () {
    this.loadCategories()
    this.loadItems(true)
  },

  onPullDownRefresh: function () {
    this.loadItems(true) //静默刷新，下拉动画本身就是loading
  },

  loadCategories: function () {
    var that = this
    var url = app.globalData.baseUrl + 'item/categories'
    utils.http_get(url, function (res) {
      if (res && Array.isArray(res)) {
        that.setData({ categories: res })
      }
    }, null, true)
  },

  loadItems: function (silent) {
    this.setData({ loading: true })
    var url = app.globalData.baseUrl + 'item/list?sort=' + this.data.sortBy
    if (this.data.categoryId) {
      url += '&categoryId=' + this.data.categoryId
    }
    utils.http_get(url, this.onItemsLoaded.bind(this), null, silent)
  },

  onItemsLoaded: function (res) {
    if (!res) {
      this.setData({ loading: false, items: [] })
      wx.stopPullDownRefresh()
      return
    }

    var items = res.items || []

    // 后端已返回 days（数值）和 dailyCost（数值，Math.Round 保留2位），前端仅做展示格式化
    items.forEach(function (item) {
      item.days = item.days || 0
      item.dailyCost = Number(item.dailyCost || 0).toFixed(2)
    })

    this.setData({
      items: items,
      totalCount: res.totalCount || items.length,
      totalAsset: Number(res.totalAsset || 0).toFixed(2),
      dailyCost: Number(res.avgDailyCost || 0).toFixed(2),
      loading: false
    })
    wx.stopPullDownRefresh()
  },

  onItemTap: function (e) {
    var id = e.currentTarget.dataset.id
    wx.navigateTo({
      url: '../item-detail/item-detail?id=' + id
    })
  },

  onFilterTap: function () {
    var that = this
    var categories = this.data.categories
    var names = ['全部']
    for (var i = 0; i < categories.length; i++) {
      names.push(categories[i].name)
    }

    wx.showActionSheet({
      itemList: names,
      success: function (res) {
        if (res.tapIndex === 0) {
          that.setData({ categoryId: null, categoryName: '全部' })
        } else {
          var cat = categories[res.tapIndex - 1]
          that.setData({ categoryId: cat.id, categoryName: cat.name })
        }
        that.loadItems(true)
      }
    })
  },

  onSortTap: function () {
    var that = this
    wx.showActionSheet({
      itemList: ['按购买时间', '按价格', '按名称'],
      success: function (res) {
        var sorts = ['days', 'price', 'name']
        that.setData({ sortBy: sorts[res.tapIndex] })
        that.loadItems()
      }
    })
  }
})
