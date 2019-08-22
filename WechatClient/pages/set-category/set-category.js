// pages/set-category/set-category.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({
  data: {
    outCategorys: {},
    inCategorys: {},
    newCategory: true,
    title: "添加一个分类",
    id: null,
    name: null,
    type: null
  },
  onShow: function(options) {
    this.queryData();
  },
  //添加分类弹框
  addCategory: function(e) {
    this.setData({
      newCategory: !this.data.newCategory,
      type: e.currentTarget.dataset.categoryType
    })
  },
  //修改分类弹框
  editCategory: function(id, name) {
    this.setData({
      newCategory: !this.data.newCategory,
      title: "请编辑分类名称",
      name: name,
      id: id
    })
  },
  //取消添加
  cancel: function() {
    this.setData({
      newCategory: true,
      name: null
    });
  },
  //确认添加  
  confirm: function(res) {
    var categoryUrl = app.globalData.httpGetUrl + 'category';
    if (this.data.id > 0) {
      utils.http_put(categoryUrl, {
        id: this.data.id,
        name: this.data.name
      }, this.queryData);
    } else {
      utils.http_post(categoryUrl, {
        name: this.data.name,
        icon: 'icon-hongbao',
        type: this.data.type
      }, this.queryData);
    }
    this.setData({
      newCategory: true,
      name: null,
      id: null
    });

  },
  /*获取名字***/
  nameInput: function(e) {
    this.setData({
      name: e.detail.value
    })
  },
  //长按菜单
  pressCategory: function(e) {
    var that = this;
    var categoryId = e.currentTarget.dataset.categoryId;
    var categoryName = e.currentTarget.dataset.categoryName;
    wx.showActionSheet({
      itemList: ['编辑名称', '删除'],
      success: function(res) {
        if (res.tapIndex == 0) {
          that.editCategory(categoryId, categoryName);
        }
        if (res.tapIndex == 1) { //删除
          that.deleteCategory(categoryId);
        }
      },
      fail: function(res) {
        console.log(res.errMsg)
      }
    })
  },
  deleteCategory: function(categoryId) {
    var that = this;
    var categoryUrl = app.globalData.httpGetUrl + 'category';
    wx.showModal({
      title: '提示',
      content: '确定要删除此分类吗？',
      success: function(res) {
        if (res.confirm) {
          utils.http_delete(categoryUrl, {
            id: categoryId
          }, that.queryData);
        } else if (res.cancel) {
          return false;
        }
      }
    })
  },
  getCategorys: function(res) {
    var categorys = res.categorys;
    var outCategorys = []; //支出类别
    var inCategorys = []; //收入类别
    var spending = {
      'categoryId': '',
      'categoryName': '添加',
      'categoryIcon': 'icon-icon02',
      'type': 'Spending',
      'isAdmin': null
    };
    var income = {
      'categoryId': '',
      'categoryName': '添加',
      'categoryIcon': 'icon-icon02',
      'type': 'Income',
      'isAdmin': null
    };
    for (let p in categorys) {
      switch (categorys[p].tallyType) {
        case '支出':
          outCategorys = categorys[p].category;
          outCategorys.push(spending);
          break;
        case '收入':
          inCategorys = categorys[p].category;
          inCategorys.push(income);
          break;
      }
    }
    this.setData({
      outCategorys: outCategorys,
      inCategorys: inCategorys
    });
  },
  queryData: function() {
    var categoryUrl = app.globalData.httpGetUrl + 'category';
    utils.http_get(categoryUrl, this.getCategorys);
  }
})