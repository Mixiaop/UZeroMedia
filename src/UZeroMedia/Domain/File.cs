﻿using System;
using U.Domain.Entities;
using U.Domain.Entities.Auditing;

namespace UZeroMedia.Domain
{
    /// <summary>
    /// 代表一个上传的“文件”
    /// </summary>
    public class File : U.Domain.Entities.Entity, IHasCreationTime
    {
        public File()
        {
            SeoFilename = "";
        }

        /// <summary>
        /// 图片类型
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// 后缀
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// 自定义文件名
        /// </summary>
        public string SeoFilename { get; set; }

        /// <summary>
        /// 是否新图
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreationTime { get; set; }
    }
}
