// pages/account/account.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({

	/**
	 * 页面的初始数据
	 */
	data: {
		account: {}
	},

	/**
	 * 生命周期函数--监听页面加载
	 */
	onLoad: function (options) {
		var url = app.globalData.baseUrl + 'account';
		utils.http_get(url, this.showPageData.bind(this));
	},

	showPageData: function (res) {
		var accountList = res.account || [];
		var accountLen = accountList.length;
		var accountSum = 0;
		for (let p in accountList) {
			accountSum += accountList[p]['money'];
		}
		var account = {
			accountSum: accountSum,
			totalIncome: res.totalIncome,
			totalSpending: res.totalSpending,
			accountList: accountList,
			accountLen: accountLen
		}
		this.setData({
			account: account
		});
	}
})