// pages/index/index.js
const utils = require('../../utils/util.js');
const app = getApp();
Page({

    /**
     * 页面的初始数据
     */
    data: {
        date: '',
        endDate: '',
        month: '',
        filterModuleHide: true,
        categoryId: 0,
        categoryIdChange: 0,
        tabBarSelected: {
            tabBarIndexSelected: true,
            tabBarPersonalSelected: false
        },
		tallyData: {}
    },

    /**
     * 生命周期函数--监听页面加载
     */
    onLoad: function(options) {
        wx.hideTabBar({
            aniamtion: false
        });
        //初始化时间
        var toDayDate = utils.formatDate(new Date());
        var date = toDayDate.split('-')[0] + '年' + toDayDate.split('-')[1] + '月';
        var dateValue = toDayDate.slice(0, 7);
        var month = toDayDate.substr(5, 2);
        this.setData({
            date: date,
            dateValue: dateValue,
            month: month,
            endDate: toDayDate
        });

        //输出账单列表
		var url = app.globalData.httpGetUrl + 'tally_list_data.json';
        utils.http_get(url, this.outPutList);
    },

    outPutList: function(res) {
		var tallyData = res;
		this.setData({
			tallyData: tallyData
		});
    },

    onDateChange: function(e) {
        // 改变日期
        var date = e.detail.value.split('-')[0] + '年' + e.detail.value.split('-')[1] + '月';
        this.setData({
            date: date,
            dateValue: e.detail.value
        });
    },

    onOpenFilterTap: function(event) {
        // 类型筛选
        var categoryId = event.currentTarget.dataset.categoryId;
        wx.navigateTo({
            url: '../filter/filter?category_id=' + categoryId,
        })
    },

    onOpenTallyDetailTap: function(event) {
		//查看账单详情
		console.log(event);
		var idx = event.currentTarget.dataset.idx;
        wx.navigateTo({
            url: '../detail/detail?idx=' + idx,
        })
    },

    /**
     * 生命周期函数--监听页面显示
     */
    onShow: function() {
        var pages = getCurrentPages();
        var currentPage = pages[pages.length - 1];
        if (this.data.categoryId !== this.data.categoryIdChange) {
            //如果重新选择了筛选类型
            console.log('筛选成功');
            this.setData({
                categoryId: this.data.categoryIdChange
            });
        };
    }
})