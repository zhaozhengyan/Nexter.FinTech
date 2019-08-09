// pages/filter/filter.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({

	/**
	 * 页面的初始数据
	 */
	data: {
		currentCategoryId: 0,
		outCategorys: [],
		inCategorys: [],
		transferCategorys: []
	},

	/**
	 * 生命周期函数--监听页面加载
	 */
	onLoad: function (options) {
		var categoryUrl = app.globalData.httpGetUrl + 'category_data.json';
		utils.http_get(categoryUrl, this.getCategorys);

		this.setData({
			currentCategoryId: options.category_id
		});
	},

	getCategorys: function (res){
		var categorys = res.categorys;
		var outCategorys = [];//支出类别
		var inCategorys = [];//收入类别
		var transferCategorys = [];//转账
		var outCategoryAll = {
			'categoryId': -1,
			'categoryName': '全部',
			'categoryIcon': 'icon-quanbu'
		}; 
		var inCategoryAll = {
			'categoryId': -2,
			'categoryName': '全部',
			'categoryIcon': 'icon-quanbu'
		};
		for (let p in categorys){
			if (categorys[p].tallyType === '支出'){
				outCategorys = categorys[p].category;
				outCategorys.unshift(outCategoryAll);
			} else if (categorys[p].tallyType === '收入'){
				inCategorys = categorys[p].category;
				inCategorys.unshift(inCategoryAll);
			}else{
				transferCategorys = categorys[p].category;
			};
		}
		this.setData({
			outCategorys: outCategorys,
			inCategorys: inCategorys,
			transferCategorys: transferCategorys
		});
	},

	onSelectIconFontTap:function (event){
		console.log(event);
		utils.SelectIconFont(event, this.SelectIconFontId);
	},

	SelectIconFontId: function (res){
		this.setData({
			currentCategoryId: res
		});
		var pages = getCurrentPages();
		var prevPage = pages[pages.length - 2];
		prevPage.setData({
			categoryIdChange: res
		});
		wx.navigateBack({
			delta: 1
		})
	},

	// onGetTallyType: function (event){
	// 	var tallyType = event.currentTarget.dataset.tallyType;
	// 	this.setData({

	// 	});
	// }
	
})