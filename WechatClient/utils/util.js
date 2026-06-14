// 格式化日期
function formatDate(date) {
  var year = date.getFullYear();
  var month = date.getMonth() + 1;
  var day = date.getDate();
  return year + '-' + filZero(month) + '-' + filZero(day);
}

function filZero(num) {
  return num.toString().length === 2 ? num : '0' + num;
}

function isNull(str) {
  return str == null || str === '' || str == undefined || str.toString().length == 0;
}

// 选择分类
function SelectIconFont(event, callBack) {
  var categoryId = event.currentTarget.dataset.categoryId;
  callBack(categoryId);
}

function getToken() {
  return wx.getStorageSync('token') || '';
}

function checkToken(url) {
  var token = getToken();
  if (isNull(token) && url.search('/transaction') == -1) {
    wx.navigateTo({
      url: '../login/login',
    })
    return false;
  }
  return true;
}

function request(url, method, data, callback, text) {
  if (!checkToken(url)) return;

  var token = getToken();
  wx.request({
    url: url,
    data: data,
    header: {
      'content-type': 'application/json',
      'token': token
    },
    dataType: 'json',
    method: method,
    success: function (res) {
      if (res.statusCode === 200 && res.data) {
        if (res.data.statusCode == 'Ok') {
          if (res.data.data != null) {
            callback && callback(res.data.data)
          } else if (callback != null) {
            callback();
          } else {
            wx.showToast({
              title: '提交成功'
            })
          }
        } else {
          wx.showToast({
            icon: 'none',
            title: res.data.message || '请求失败'
          })
        }
      } else if (res.statusCode === 401) {
        wx.removeStorageSync('token');
        wx.navigateTo({
          url: '../login/login',
        })
      } else {
        wx.showToast({
          icon: 'none',
          title: '服务器错误: ' + res.statusCode
        })
      }
    },
    fail: function (err) {
      console.error('[HTTP Error]', method, url, err);
      wx.showToast({
        title: text || '网络请求失败'
      })
    }
  })
}

function http_get(url, callback, text) {
  request(url, 'GET', null, callback, text);
}

function http_post(url, data, callback, text) {
  request(url, 'POST', data, callback, text);
}

function http_put(url, data, callback, text) {
  request(url, 'PUT', data, callback, text);
}

function http_delete(url, data, callback, text) {
  request(url, 'DELETE', data, callback, text);
}

module.exports = {
  formatDate: formatDate,
  isNull: isNull,
  SelectIconFont: SelectIconFont,
  http_get: http_get,
  http_post: http_post,
  http_put: http_put,
  http_delete: http_delete,
}