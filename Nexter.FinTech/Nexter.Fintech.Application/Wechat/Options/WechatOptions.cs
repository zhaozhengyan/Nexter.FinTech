using Furion.ConfigurableOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexter.Fintech.Application.Wechat.Options
{
    public class WechatOptions : IConfigurableOptions
    {
        public string AuthUrl { get; set; }
        public string Appid { get; set; }
        public string Secret { get; set; }
        public string TemplateId { get; set; }
        public string TemplateMsg { get; set; }
        public string GoPage { get; set; }
    }
}
