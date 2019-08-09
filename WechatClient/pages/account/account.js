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
		var url = app.globalData.httpGetUrl + 'account_data.json';
		utils.http_get(url, this.showPageData);
	},

	showPageData: function (res) {
		var accountLen = res.account.length;
		var accountSum = 0;
		for (let p in res.account) {
			accountSum += res.account[p]['money'];
		}
		var account = {
			accountSum: accountSum,
			totalIncome: res.totalIncome,
			totalSpending: res.totalSpending,
			accountList: res.account,
			accountLen: accountLen
		}
		this.setData({
			account: account
		});
	}
})