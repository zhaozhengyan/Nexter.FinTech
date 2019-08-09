// pages/personal/personal.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({

	/**
	 * 页面的初始数据
	 */
	data: {
		userInfos: null,
		tabBarSelected: {
			tabBarIndexSelected: false,
			tabBarPersonalSelected: true
		}
	},

	/**
	 * 生命周期函数--监听页面加载
	 */
	onLoad: function (options) {
		wx.hideTabBar({
			aniamtion: false
		});
		
		var url = app.globalData.httpGetUrl + 'personal_data.json';
		utils.http_get(url, this.showPageData);
	},

	showPageData: function (res) {
		var userInfos = app.globalData.userInfos;
		userInfos.joinTime = res.joinTime;
		userInfos.totalMoney = res.totalMoney;
		userInfos.count = res.count;

		this.setData({
			userInfos: userInfos
		});
	},

	onOpenPageTap: function (event){
		var url = event.currentTarget.dataset.pageUrl;
		if (url){
			wx.navigateTo({
				url: url
			})
		}
	},

	/**
	 * 生命周期函数--监听页面初次渲染完成
	 */
	onReady: function () {

	},

	/**
	 * 生命周期函数--监听页面显示
	 */
	onShow: function () {

	},

	/**
	 * 生命周期函数--监听页面隐藏
	 */
	onHide: function () {

	},

	/**
	 * 生命周期函数--监听页面卸载
	 */
	onUnload: function () {

	},

	/**
	 * 页面相关事件处理函数--监听用户下拉动作
	 */
	onPullDownRefresh: function () {

	},

	/**
	 * 页面上拉触底事件的处理函数
	 */
	onReachBottom: function () {

	},

	/**
	 * 用户点击右上角分享
	 */
	onShareAppMessage: function () {

	}
})