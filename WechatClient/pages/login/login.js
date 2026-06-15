// pages/login/login.js
const app = getApp();

Page({
  data: {
    buttonText: '微信登录',
    avatarUrl: '',
    nickName: ''
  },

  onLoad: function(options) {
    if (options.inviterId) {
      wx.setStorageSync('inviterId', options.inviterId)
    }
  },

  onChooseAvatar: function(e) {
    this.setData({ avatarUrl: e.detail.avatarUrl })
  },

  onNicknameInput: function(e) {
    this.setData({ nickName: e.detail.value })
  },

  onLogin: function() {
    if (!this.data.nickName) {
      wx.showToast({ icon: 'none', title: '请输入昵称' })
      return
    }

    this.setData({ buttonText: '登录中...' })

    const inviterId = wx.getStorageSync('inviterId') || ''
    const that = this

    wx.login({
      success(res) {
        if (res.code) {
          var url = app.globalData.baseUrl + 'me'
          var token = wx.getStorageSync('token') || ''
          wx.request({
            url: url,
            method: 'POST',
            data: {
              code: res.code,
              nickName: that.data.nickName,
              avatar: that.data.avatarUrl || '',
              inviterId: inviterId
            },
            header: {
              'content-type': 'application/json',
              'token': token
            },
            success: function(resp) {
                if (resp.statusCode === 200 && resp.data && resp.data.statusCode === 'Ok' && resp.data.data) {
                wx.setStorageSync('token', resp.data.data.token)
                app.globalData.token = resp.data.data.token
                // 先跳转到默认页，避免 /me 接口慢时用户在登录页干等
                var targetTab = app.globalData.defaultTab || 'index'
                wx.switchTab({ url: '../' + targetTab + '/' + targetTab })
                // 异步加载用户信息（含 defaultTab）
                app.loadUserInfo()
              } else {
                wx.showToast({ icon: 'none', title: (resp.data && resp.data.message) || '登录失败' })
                that.setData({ buttonText: '微信登录' })
              }
            },
            fail: function() {
              wx.showToast({ icon: 'none', title: '网络请求失败' })
              that.setData({ buttonText: '微信登录' })
            }
          })
        } else {
          wx.showToast({ icon: 'none', title: '登录失败：' + res.errMsg })
          that.setData({ buttonText: '微信登录' })
        }
      },
      fail() {
        wx.showToast({ icon: 'none', title: '微信登录失败' })
        that.setData({ buttonText: '微信登录' })
      }
    })
  }
})
