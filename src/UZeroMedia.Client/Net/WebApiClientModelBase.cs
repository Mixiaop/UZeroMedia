using System;
using System.Reflection;
using Newtonsoft.Json;

namespace UZeroMedia.Client.Net
{
    /// <summary>
    /// Web 请求的模型基类，带默认的一些方法
    /// </summary>
    public abstract class WebApiClientModelBase : IWebApiClientModel
    {
        public WebApiClientModelBase()
        {
            SetDefaultValuesForAllProperties();
        }

        /// <summary>
        /// to json string
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        #region Utilities
        /// <summary>
        /// 检查所有属性不能为null
        /// </summary>
        public void CheckDefaultValuesForAllProperties()
        {
            foreach (PropertyInfo info in this.GetType().GetProperties())
            {
                if (info.PropertyType == typeof(string))
                {
                    if (info.GetValue(this) == null)
                        info.SetValue(this, string.Empty);
                }

                if (info.PropertyType.BaseType == typeof(WebApiClientModelBase))
                {
                    if (info.GetValue(this) == null)
                        info.SetValue(this, Activator.CreateInstance(info.PropertyType));
                }
            }
        }

        /// <summary>
        /// 为对象所有属性设置默认值
        /// </summary>
        protected void SetDefaultValuesForAllProperties()
        {
            foreach (PropertyInfo info in this.GetType().GetProperties())
            {
                if (info.PropertyType == typeof(string))
                {
                    info.SetValue(this, string.Empty);
                }
                if (info.PropertyType.BaseType == typeof(WebApiClientModelBase))
                {
                    info.SetValue(this, Activator.CreateInstance(info.PropertyType));
                }
            }
        }
        #endregion
    }
}
