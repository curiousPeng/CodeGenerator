﻿using Generator.Common;
using Generator.Core.Config;
using Generator.Template;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generator.Core
{
    public abstract class BaseDALGenerator : BaseGenerator
    {
        private List<string> _keywords = new List<string> { "Type" };

        public BaseDALGenerator(GlobalConfiguration config)
            : base(config)
        { }

        public virtual string RenderDALFor(TableMetaData table)
        {
            var model = new ViewInfoWapper(this);
            model.Config = _config;
            model.TableInfo = table;

            return Render("DAL/dal_master.cshtml", model);
        }

        public virtual string GetPartialViewPath(string method)
        {
            switch (method.ToLower())
            {
                case "exists":
                    return "DAL/Exists/exist.cshtml";
                case "insert":
                    return "DAL/Insert/insert.cshtml";
                case "delete":
                    return "DAL/Delete/delete.cshtml";
                case "update":
                    return "DAL/Update/update.cshtml";
                case "get":
                    return "DAL/Get/get.cshtml";
                case "page":
                    return "DAL/Page/page.cshtml";
            }

            throw new System.ArgumentException($"暂不支持生成{method}相关方法");
        }

        public abstract string RenderBaseTableHelper();

        /// <summary>
        /// 生成的dapper查询时使用的表名
        /// </summary>
        public abstract string NormalizeTableName(string tableName);

        /// <summary>
        /// 生成的dapper查询时使用的列名
        /// </summary>
        public abstract string NormalizeFieldName(string fieldName);

        /// <summary>
        /// 生成的方法的参数列表
        /// </summary>
        public virtual string MakeMethodParam(IEnumerable<ColumnMetaData> columns)
        {
            var sb = new StringBuilder();
            foreach (var item in columns)
                sb.Append($"{item.DbType} {item.Name}, ");
            return sb.TrimEnd(", ").ToString();
        }

        /// <summary>
        /// 生成的方法注释信息中包含的参数说明文字
        /// </summary>
        public virtual string MakeMethodParamComment(IEnumerable<ColumnMetaData> columns)
        {
            var sb = new StringBuilder();
            foreach (var item in columns)
                sb.Append($"/// <param name=\"{item.Name}\">{item.Comment}</param>");
            return sb.ToString();
        }

        public bool IsUpdateExcludeColumn(string table, string colunm)
        {
            if (_config.UpdateExcludeColumns == null)
                return false;

            return _config.UpdateExcludeColumns.ContainsKey("*") && _config.UpdateExcludeColumns["*"].Any(p => p.ColumnName == colunm) ||
                _config.UpdateExcludeColumns.ContainsKey(table) && _config.UpdateExcludeColumns[table].Any(p => p.ColumnName == colunm);
        }
    }
}
