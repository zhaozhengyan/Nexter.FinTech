// pages/login/login.js
const app = getApp();
Page({

    /**
     * 页面的初始数据
     */
	data: {
		// navigationBarTitle: '',
		buttonText: '微信登录'
	},

    /**
     * 生命周期函数--监听页面加载
     */
	onLoad: function (options) {
		var that = this;
		wx.getSetting({
			success: function (res) {
				if (res.authSetting['scope.userInfo']) {
					//如果已经授权
					that.setData({
						// navigationBarTitle: '正在登录...',
						buttonText: '正在登录...'
					});
					that.getUserInfoFn();
				}else{
					// 如果未授权
					that.setData({
						// navigationBarTitle: '登录',
						buttonText: '微信登录'
					});
				}
			}
		});
	},

	onGetUserInfo: function (e) {
		if (e.detail.userInfo) {
			this.getUserInfoFn();
		} else {
			wx.showModal({
				title: '拒绝授权怎么开始记账呢？',
				content: '拒绝授权将不能登录每日记账，请点击“微信登录”按钮后，点击允许重新授权。',
				showCancel: false
			})
		}
	},

	getUserInfoFn: function () {
		var that = this;
		wx.getUserInfo({
			success: function (res) {
				var userInfo = res.userInfo;
				var userInfos = {
					nickName: userInfo.nickName,
					gender: userInfo.gender,
					avatarUrl: userInfo.avatarUrl
				};
				app.globalData.userInfos = userInfos;
				wx.switchTab({
					url: '../index/index',
				})
			}
		})
	},

    /**
     * 生命周期函数--监听页面初次渲染完成
     */
	onReady: function () {
		// wx.setNavigationBarTitle({
		// 	title: this.data.navigationBarTitle,
		// })
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