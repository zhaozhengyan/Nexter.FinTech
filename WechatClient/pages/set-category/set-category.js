// pages/set-category/set-category.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({

	/**
	 * 页面的初始数据
	 */
	data: {
		outCategorys: {},
		inCategorys: {}
	},

	/**
	 * 生命周期函数--监听页面加载
	 */
	onLoad: function (options) {
		var categoryUrl = app.globalData.httpGetUrl + 'category_data.json';
		utils.http_get(categoryUrl, this.getCategorys);
	},

	getCategorys: function (res) {
		var categorys = res.categorys;
		var outCategorys = [];//支出类别
		var inCategorys = [];//收入类别
		var addCategory = {
			'categoryId': '',
			'categoryName': '添加',
			'categoryIcon': 'icon-icon02'
		};
		for (let p in categorys) {
			switch (categorys[p].tallyType){
				case '支出':
					outCategorys = categorys[p].category;
					outCategorys.push(addCategory);
					break;
				case '收入':
					inCategorys = categorys[p].category;
					inCategorys.push(addCategory);
					break;
			}
		}
		this.setData({
			outCategorys: outCategorys,
			inCategorys: inCategorys
		});
	}
})