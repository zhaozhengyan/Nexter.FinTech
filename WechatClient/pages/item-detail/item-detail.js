// pages/item-detail/item-detail.js
const utils = require('../../utils/util.js')
const app = getApp()

// 默认分类（后端获取失败时的 fallback）
var defaultCategories = [
  { id: 1, name: '数码产品' },
  { id: 2, name: '日用品' },
  { id: 3, name: '服饰' },
  { id: 4, name: '家具' },
  { id: 5, name: '其他' }
]

Page({
  data: {
    id: null,
    isEdit: false,
    item: {},
    form: {
      name: '',
      price: '',
      purchaseDate: '',
      retireDate: '',
      warrantyEndDate: '',
      categoryId: '',
      categoryName: '',
      icon: 'ri-box-3-line',
      iconName: '默认',
      priceCalcMethod: 'time',
      calcMethodName: '按时间',
      note: ''
    },
    // 附加项
    additionals: [],
    showAdditionalModal: false,
    editAdditionalIndex: -1,
    editAdditionalForm: { name: '', type: 'expense', amount: '', date: '' },
    categories: defaultCategories,
    categoryIndex: 0,
    calcMethods: [
      { id: 'time', name: '按时间' },
      { id: 'usage', name: '按使用次数' }
    ],
    calcMethodIndex: 0,
    showIconModal: false,
    iconList: [
      { name: 'ri-box-3-line', label: '默认' },
      { name: 'ri-smartphone-line', label: '手机' },
      { name: 'ri-computer-line', label: '电脑' },
      { name: 'ri-tv-2-line', label: '显示器' },
      { name: 'ri-keyboard-line', label: '键盘' },
      { name: 'ri-mouse-line', label: '鼠标' },
      { name: 'ri-headphone-line', label: '耳机' },
      { name: 'ri-timer-line', label: '手表' },
      { name: 'ri-camera-line', label: '相机' },
      { name: 'ri-gamepad-line', label: '游戏' },
      { name: 'ri-t-shirt-line', label: '衣服' },
      { name: 'ri-footprint-line', label: '鞋子' },
      { name: 'ri-handbag-line', label: '包包' },
      { name: 'ri-home-4-line', label: '家居' },
      { name: 'ri-car-line', label: '汽车' },
      { name: 'ri-book-open-line', label: '书籍' }
    ]
  },

  onLoad: function (options) {
    this.loadCategories()
    if (options.id) {
      this.setData({ id: options.id })
      this.loadItem(options.id)
    } else {
      // 新增模式
      var today = utils.formatDate(new Date())
      this.setData({
        isEdit: true,
        'form.purchaseDate': today
      })
    }
  },

  loadCategories: function () {
    var that = this
    var url = app.globalData.baseUrl + 'item/categories'
    utils.http_get(url, function (res) {
      if (res && Array.isArray(res) && res.length > 0) {
        that.setData({ categories: res })
      }
    }, null, true)
  },

  loadItem: function (id) {
    var url = app.globalData.baseUrl + 'item/detail?id=' + id
    utils.http_get(url, this.onItemLoaded.bind(this))
  },

  onItemLoaded: function (res) {
    if (!res) return
    // 后端已返回 days 和 dailyCost（数值），前端兜底 + 格式化
    res.days = res.days || this.calcDays(res.purchaseDate)
    res.dailyCost = Number(res.dailyCost || (res.days > 0 ? res.price / res.days : res.price)).toFixed(2)
    this.setData({ item: res })
  },

  calcDays: function (dateStr) {
    if (!dateStr) return 0
    var purchaseDate = new Date(dateStr)
    var now = new Date()
    var diff = now.getTime() - purchaseDate.getTime()
    return Math.floor(diff / (1000 * 60 * 60 * 24))
  },

  onEditTap: function () {
    var item = this.data.item
    // 匹配分类索引
    var categoryIndex = 0
    var categories = this.data.categories
    for (var i = 0; i < categories.length; i++) {
      if (categories[i].id === item.categoryId) {
        categoryIndex = i
        break
      }
    }
    this.setData({
      isEdit: true,
      categoryIndex: categoryIndex,
      additionals: item.additionalItems || [],
      form: {
        name: item.name || '',
        price: item.price || '',
        purchaseDate: item.purchaseDate ? item.purchaseDate.substring(0, 10) : '',
        retireDate: item.retireDate ? item.retireDate.substring(0, 10) : '',
        warrantyEndDate: item.warrantyEndDate ? item.warrantyEndDate.substring(0, 10) : '',
        categoryId: item.categoryId || '',
        categoryName: categories[categoryIndex].name || '',
        icon: item.icon || 'ri-box-3-line',
        iconName: item.iconName || '默认',
        priceCalcMethod: item.priceCalcMethod || 'time',
        calcMethodName: item.priceCalcMethod === 'usage' ? '按使用次数' : '按时间',
        note: item.note || ''
      }
    })
  },

  onDeleteTap: function () {
    var that = this
    wx.showModal({
      title: '确认删除',
      content: '确定要删除这个物品吗？',
      success: function (res) {
        if (res.confirm) {
          var url = app.globalData.baseUrl + 'item?id=' + that.data.id
          utils.http_delete(url, {}, function () {
            wx.showToast({ title: '删除成功' })
            setTimeout(function () { wx.navigateBack() }, 1500)
          })
        }
      }
    })
  },

  onCancelTap: function () {
    if (this.data.id) {
      this.setData({ isEdit: false })
    } else {
      wx.navigateBack()
    }
  },

  // 表单输入
  onInputName: function (e) {
    this.setData({ 'form.name': e.detail.value })
  },

  onInputPrice: function (e) {
    this.setData({ 'form.price': e.detail.value })
  },

  // 附加项管理
  onShowAddAdditional: function () {
    var today = utils.formatDate(new Date())
    this.setData({
      showAdditionalModal: true,
      editAdditionalIndex: -1,
      editAdditionalForm: { name: '', type: 'expense', amount: '', date: today }
    })
  },

  onEditAdditional: function (e) {
    var index = e.currentTarget.dataset.index
    var item = this.data.additionals[index]
    this.setData({
      showAdditionalModal: true,
      editAdditionalIndex: index,
      editAdditionalForm: {
        name: item.name || '',
        type: item.type || 'expense',
        amount: String(item.amount || ''),
        date: item.date || ''
      }
    })
  },

  onDeleteAdditional: function (e) {
    var index = e.currentTarget.dataset.index
    var additionals = this.data.additionals.slice()
    additionals.splice(index, 1)
    this.setData({ additionals: additionals })
  },

  onAdditionalFormInput: function (e) {
    var field = e.currentTarget.dataset.field
    this.setData({ ['editAdditionalForm.' + field]: e.detail.value })
  },

  onAdditionalTypeTap: function (e) {
    this.setData({ 'editAdditionalForm.type': e.currentTarget.dataset.type })
  },

  onAdditionalDateChange: function (e) {
    this.setData({ 'editAdditionalForm.date': e.detail.value })
  },

  onCloseAdditionalModal: function () {
    this.setData({ showAdditionalModal: false })
  },

  onConfirmAdditional: function () {
    var form = this.data.editAdditionalForm
    if (!form.name) {
      wx.showToast({ icon: 'none', title: '请输入名称' })
      return
    }
    if (!form.amount) {
      wx.showToast({ icon: 'none', title: '请输入金额' })
      return
    }

    var newItem = {
      name: form.name,
      type: form.type,
      amount: parseFloat(form.amount),
      date: form.date
    }

    var additionals = this.data.additionals.slice()
    if (this.data.editAdditionalIndex >= 0) {
      additionals[this.data.editAdditionalIndex] = newItem
    } else {
      additionals.push(newItem)
    }

    this.setData({
      additionals: additionals,
      showAdditionalModal: false
    })
  },

  onInputNote: function (e) {
    this.setData({ 'form.note': e.detail.value })
  },

  // 日期选择
  onPurchaseDateChange: function (e) {
    this.setData({ 'form.purchaseDate': e.detail.value })
  },

  onRetireDateChange: function (e) {
    this.setData({ 'form.retireDate': e.detail.value })
  },

  onWarrantyDateChange: function (e) {
    this.setData({ 'form.warrantyEndDate': e.detail.value })
  },

  // 分类选择
  onCategoryChange: function (e) {
    var index = e.detail.value
    this.setData({
      categoryIndex: index,
      'form.categoryId': this.data.categories[index].id,
      'form.categoryName': this.data.categories[index].name
    })
  },

  // 图标选择
  onIconTap: function () {
    this.setData({ showIconModal: true })
  },

  onCloseIconModal: function () {
    this.setData({ showIconModal: false })
  },

  preventClose: function () {
    // 阻止冒泡
  },

  onSelectIcon: function (e) {
    var icon = e.currentTarget.dataset.icon
    var name = e.currentTarget.dataset.name
    this.setData({
      'form.icon': icon,
      'form.iconName': name,
      showIconModal: false
    })
  },

  // 计算方式
  onCalcMethodChange: function (e) {
    var index = e.detail.value
    this.setData({
      calcMethodIndex: index,
      'form.priceCalcMethod': this.data.calcMethods[index].id,
      'form.calcMethodName': this.data.calcMethods[index].name
    })
  },

  // 保存
  onSaveTap: function () {
    var form = this.data.form

    if (!form.name) {
      wx.showToast({ icon: 'none', title: '请输入物品名称' })
      return
    }
    if (!form.price) {
      wx.showToast({ icon: 'none', title: '请输入价格' })
      return
    }
    if (!form.purchaseDate) {
      wx.showToast({ icon: 'none', title: '请选择购买日期' })
      return
    }

    var url = app.globalData.baseUrl + 'item'
    var data = {
      name: form.name,
      price: parseFloat(form.price),
      additionalItemsJson: this.data.additionals.length > 0 ? JSON.stringify(this.data.additionals) : null,
      purchaseDate: form.purchaseDate,
      retireDate: form.retireDate || null,
      warrantyEndDate: form.warrantyEndDate || null,
      categoryId: form.categoryId ? parseInt(form.categoryId) : null,
      icon: form.icon,
      priceCalcMethod: form.priceCalcMethod,
      note: form.note
    }

    if (this.data.id) {
      data.id = this.data.id
      utils.http_put(url, data, function () {
        wx.showToast({ title: '保存成功' })
        setTimeout(function () { wx.navigateBack() }, 1500)
      })
    } else {
      utils.http_post(url, data, function () {
        wx.showToast({ title: '添加成功' })
        setTimeout(function () { wx.navigateBack() }, 1500)
      })
    }
  }
})
