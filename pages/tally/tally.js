// pages/tally/tally.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({

    /**
     * 页面的初始数据
     */
    data: {
        date: '',
        toDayDate: '',
        currentTallyType: '支出',
        currentCategoryId: '',
		categorys: [],
		categoryArray: [],
		selectedAccount: {},
		accountOut: {},
		accountIn: {},
		accounts: []
    },

    /**
     * 生命周期函数--监听页面加载
     */
    onLoad: function(options) {
        //初始化日期
        var toDayDate = utils.formatDate(new Date());

		//获取类别数据
		var categoryUrl = app.globalData.httpGetUrl + 'category_data.json';
		utils.http_get(categoryUrl, this.getCategorys);

		//获取账户
		var accountUrl = app.globalData.httpGetUrl + 'account_data.json';
		utils.http_get(accountUrl, this.getAccounts);

		this.setData({
			toDayDate: toDayDate,
			date: toDayDate
		});
    },

	OnDateChange: function (e) {
		//选择日期
		this.setData({
			date: e.detail.value
		});
	},

	getCategorys: function(res) {
        var categorys = res.categorys;
		this.setData({
			categorys: categorys
		});
		this.showCategoryList();
    },

	showCategoryList: function (){
		//显示类别列表
		var categorys = this.data.categorys;
		if (categorys.length !== 0){
			var categoryArray = [];
			for (let p in categorys) {
				if (categorys[p]['tallyType'] === this.data.currentTallyType) {
					categoryArray = categorys[p]['category'];
				};
			};
			this.setData({
				categoryArray: categoryArray
			});
		}
	},

    onSelectIconFontTap: function(event) {
		utils.SelectIconFont(event, this.SelectIconFontId);
    },
    SelectIconFontId: function(res) {
        this.setData({
            currentCategoryId: res
        });
    },

	onTextTab: function (event) {
		//支出|收入|转账 切换
		var text = event.target.dataset.text;
		this.setData({
			currentTallyType: text
		});
		this.showCategoryList();
	},

	getAccounts: function (res) {
		//获取账户信息
		var accounts = res.account;

		//设置收入支出的默认选择账户
		var selectedAccount = wx.getStorageSync('selectedAccount');
		if (!selectedAccount) {
			selectedAccount = accounts[2];//没有缓存，默认微信账户
		};
		//设置转出的默认选择账户
		var accountOut = wx.getStorageSync('accountOut');
		if (!accountOut) {
			accountOut = accounts[4];//没有缓存，默认银行
		};
		//设置转出的默认选择账户
		var accountIn = wx.getStorageSync('accountIn');
		if (!accountIn) {
			accountIn = accounts[2];//没有缓存，默认支付宝账户
		};

		this.setData({
			accounts: accounts,
			selectedAccount: selectedAccount,
			accountOut: accountOut,
			accountIn: accountIn
		});
	},

    selectAccountTap: function() {
        //选择支出/收入账户
		this.setAccountFn('selectedAccount');
    },

	onAccountOut: function (){
		//选择转出账户
		this.setAccountFn('accountOut');
	},

	onAccountIn: function (){
		//选择转入账户
		this.setAccountFn('accountIn');
	},

	setAccountFn: function (key){
		//选择账户
		var accountList = this.data.accounts;
		var itemList = [];
		for (let p in accountList) {
			itemList.push(accountList[p].accountName);
		};
		var that = this;
		wx.showActionSheet({
			itemList: itemList,
			itemColor: '#116cc5',
			success: function (res) {
				wx.setStorageSync(key, accountList[res.tapIndex]); //缓存最近选择的账户
				var data = {};
				data[key] = accountList[res.tapIndex];
				that.setData(data);
			}
		});
	},

    /**
     * 生命周期函数--监听页面初次渲染完成
     */
    onReady: function() {

    },

    /**
     * 生命周期函数--监听页面显示
     */
    onShow: function() {

    },

    /**
     * 生命周期函数--监听页面隐藏
     */
    onHide: function() {

    },

    /**
     * 生命周期函数--监听页面卸载
     */
    onUnload: function() {

    },

    /**
     * 页面相关事件处理函数--监听用户下拉动作
     */
    onPullDownRefresh: function() {

    },

    /**
     * 页面上拉触底事件的处理函数
     */
    onReachBottom: function() {

    },

    /**
     * 用户点击右上角分享
     */
    onShareAppMessage: function() {

    }
})