// pages/items/items.js
const utils = require('../../utils/util.js')
const app = getApp()

Page({
  data: {
    loggedIn: false,
    items: [],
    totalCount: 0,
    totalAsset: '0.00',
    dailyCost: '0.00',
    loading: false,
    sortBy: 'days', // days, daysAsc, price, priceAsc, name
    categoryId: null,
    categoryName: '全部',
    categories: [],
    // 筛选面板
    showFilter: false,
    filterName: '',
    filterStatus: null, // null, 'active', 'retired'
    filterCategoryId: null,
    hasFilter: false,
    // 拖拽排序
    isDragging: false,
    dragCurrentIndex: -1,
    dragTargetIndex: -1,
    dragStartY: 0,
    dragCloneY: 0,
    dragItemHeight: 0,
    dragTimer: null,
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
    if (!utils.isLoggedIn()) {
      wx.navigateTo({ url: '../login/login' });
      return;
    }
    var tab = app.globalData.combinedTab || 'items';
    if (tab === 'items') {
      wx.navigateTo({ url: '../item-detail/item-detail' });
    } else {
      wx.navigateTo({ url: '../tally/tally' });
    }
  },

  onShow: function () {
    var loggedIn = utils.isLoggedIn();
    this.setData({ loggedIn: loggedIn });
    if (loggedIn) {
      this.loadCategories()
      this.loadItems(true)
    }
  },

  onPullDownRefresh: function () {
    if (!utils.isLoggedIn()) {
      wx.stopPullDownRefresh();
      return;
    }
    this.loadItems(true)
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
    if (this.data.filterStatus) {
      url += '&status=' + this.data.filterStatus
    }
    if (this.data.filterName) {
      url += '&name=' + encodeURIComponent(this.data.filterName)
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
    if (this.data.sortBy === 'custom' || this.data.isDragging) return
    var id = e.currentTarget.dataset.id
    wx.navigateTo({
      url: '../item-detail/item-detail?id=' + id
    })
  },

  onFilterTap: function () {
    this.setData({ showFilter: true })
  },

  onCloseFilter: function () {
    this.setData({ showFilter: false })
  },

  onClearFilter: function () {
    this.setData({
      filterName: '',
      filterStatus: null,
      filterCategoryId: null,
      categoryId: null,
      categoryName: '全部',
      hasFilter: false
    })
  },

  onFilterNameInput: function (e) {
    this.setData({ filterName: e.detail.value })
  },

  onFilterStatusTap: function (e) {
    var status = e.currentTarget.dataset.status
    // 再次点击同一个状态则取消选择
    if (this.data.filterStatus === status) {
      this.setData({ filterStatus: null })
    } else {
      this.setData({ filterStatus: status })
    }
  },

  onFilterCategoryTap: function (e) {
    var id = e.currentTarget.dataset.id
    // 再次点击同一个分类则取消选择
    if (this.data.filterCategoryId == id) {
      this.setData({ filterCategoryId: null })
    } else {
      this.setData({ filterCategoryId: id })
    }
  },

  onConfirmFilter: function () {
    var hasFilter = !!(this.data.filterName || this.data.filterStatus || this.data.filterCategoryId)
    this.setData({
      showFilter: false,
      hasFilter: hasFilter,
      categoryId: this.data.filterCategoryId,
      categoryName: this.data.filterCategoryId ? this._findCategoryName(this.data.filterCategoryId) : '全部'
    })
    this.loadItems(true)
  },

  _findCategoryName: function (id) {
    var categories = this.data.categories
    for (var i = 0; i < categories.length; i++) {
      if (categories[i].id == id) return categories[i].name
    }
    return '全部'
  },

  onSortTap: function () {
    var that = this
    wx.showActionSheet({
      itemList: ['最近购买', '最早购买', '价格从高到低', '价格从低到高', '按名称', '自定义排序'],
      success: function (res) {
        var sorts = ['days', 'daysAsc', 'price', 'priceAsc', 'name', 'custom']
        that.setData({ sortBy: sorts[res.tapIndex] })
        that.loadItems(true)
      }
    })
  },

  // === 拖拽排序 ===
  onTouchStart: function (e) {
    if (this.data.sortBy !== 'custom') return

    var id = e.currentTarget.dataset.id
    if (!id) return

    var index = -1
    var items = this.data.items
    for (var i = 0; i < items.length; i++) {
      if (items[i].id == id) {
        index = i
        break
      }
    }
    if (index < 0) return

    var touch = e.touches[0]
    var that = this
    var startY = touch.clientY

    var timer = setTimeout(function () {
      wx.vibrateShort({ type: 'medium' })
      that.setData({
        isDragging: true,
        dragCurrentIndex: index,
        dragTargetIndex: index,
        dragStartY: startY,
        dragCloneY: startY - 50,
        dragItemHeight: 68
      })
    }, 300)

    this.setData({ dragTimer: timer })
  },

  onTouchMove: function (e) {
    if (!this.data.isDragging) {
      // Cancel long press if finger moved
      if (this.data.dragTimer) {
        clearTimeout(this.data.dragTimer)
        this.setData({ dragTimer: null })
      }
      return
    }

    var touch = e.touches[0]
    var offsetY = touch.clientY - this.data.dragStartY
    var newIndex = this.data.dragCurrentIndex + Math.round(offsetY / this.data.dragItemHeight)
    newIndex = Math.max(0, Math.min(this.data.items.length - 1, newIndex))

    this.setData({
      dragCloneY: touch.clientY - 50
    })

    if (newIndex !== this.data.dragTargetIndex) {
      wx.vibrateShort({ type: 'light' })
      this.setData({ dragTargetIndex: newIndex })
    }
  },

  onTouchEnd: function (e) {
    if (this.data.dragTimer) {
      clearTimeout(this.data.dragTimer)
    }
    if (!this.data.isDragging) {
      this.setData({ dragTimer: null })
      return
    }

    var fromIndex = this.data.dragCurrentIndex
    var toIndex = this.data.dragTargetIndex

    this.setData({
      isDragging: false,
      dragCurrentIndex: -1,
      dragTargetIndex: -1,
      dragTimer: null
    })

    // Reorder array if position changed
    if (fromIndex !== toIndex) {
      var items = this.data.items.slice()
      var movedItem = items.splice(fromIndex, 1)[0]
      items.splice(toIndex, 0, movedItem)
      this.setData({ items: items })
      this.saveOrder()
    }
  },

  saveOrder: function () {
    var ids = this.data.items.map(function (item) { return item.id })
    var url = app.globalData.baseUrl + 'item/reorder'
    utils.http_put(url, { ids: ids }, function () {}, null, true)
  }
})
