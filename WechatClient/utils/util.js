// 格式化日期
function formatDate(date){
	var year = date.getFullYear();
	var month = date.getMonth() + 1;
	var day = date.getDate();
	return year + '-' + filZero(month) + '-' + filZero(day);
}
function filZero(num){
	//补0
	return num.toString().length === 2 ? num : '0' + num;
}

// 选择分类
function SelectIconFont(event,callBack) {
	var categoryId = event.currentTarget.dataset.categoryId;
	callBack(categoryId);
}

function http_get(url, callback, text){
	wx.request({
		url: url,  
		header: {'content-type':'application/json'},
		dataType: 'json',
		method: 'GET',
		success: function (res){
			callback(res.data)
		},
		fail: function (){
			wx.showToast({
				title: text ? text : '数据获取失败'
			})
		}
	})
}


module.exports = {
	formatDate: formatDate,
	SelectIconFont: SelectIconFont,
	http_get: http_get
}