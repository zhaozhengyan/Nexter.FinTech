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
  // 公开接口白名单：不需要 token 就能访问
  var publicPaths = ['/login', '/register'];
  var isPublic = publicPaths.some(function(path) {
    return url.indexOf(path) !== -1;
  });

  if (isNull(token) && !isPublic) {
    wx.reLaunch({
      url: '../login/login',
    });
    return false;
  }
  return true;
}

function request(url, method, data, callback, text, silent) {
  if (!checkToken(url)) return;

  var token = getToken();
  var loadingActive = false;

  if (!silent) {
    wx.showLoading({ title: '加载中...', mask: true });
    loadingActive = true;
  }

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
            loadingActive = false;
            wx.showToast({
              title: '提交成功'
            })
          }
        } else {
          loadingActive = false;
          wx.showToast({
            icon: 'none',
            title: res.data.message || '请求失败'
          })
        }
      } else if (res.statusCode === 401) {
        loadingActive = false;
        wx.removeStorageSync('token');
        wx.reLaunch({
          url: '../login/login',
        })
      } else {
        loadingActive = false;
        wx.showToast({
          icon: 'none',
          title: '服务器错误: ' + res.statusCode
        })
      }
    },
    fail: function (err) {
      console.error('[HTTP Error]', method, url, err);
      loadingActive = false;
      wx.showToast({
        icon: 'none',
        title: text || '网络请求失败'
      })
    },
    complete: function () {
      if (loadingActive) {
        wx.hideLoading();
      }
    }
  })
}

function http_get(url, callback, text, silent) {
  request(url, 'GET', null, callback, text, silent);
}

function http_post(url, data, callback, text, silent) {
  request(url, 'POST', data, callback, text, silent);
}

function http_put(url, data, callback, text, silent) {
  request(url, 'PUT', data, callback, text, silent);
}

function http_delete(url, data, callback, text, silent) {
  request(url, 'DELETE', data, callback, text, silent);
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