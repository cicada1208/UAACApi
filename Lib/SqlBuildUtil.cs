using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Lib
{
    public class SqlBuildUtil
    {
        ///// <summary>
        ///// SQL 指令的 Table name
        ///// </summary>
        //public string TableName { get; set; }

        public SqlBuildUtil() { }

        //public SqlBuildUtil(string tableName)
        //{
        //    TableName = tableName;
        //}

        /// <summary>
        /// Build Select SQL
        /// </summary>
        /// <param name="type">typeof(model), model 具有定義 TableAttribute</param>
        /// <param name="entity">entity, 未必定義 TableAttribute</param>
        public virtual SqlCmdUtil Select(Type type, dynamic entity = null, bool schemaOnly = false)
        {
            SqlCmdUtil sqlCmd = new SqlCmdUtil();
            //Dictionary<string, object> dicParams = new Dictionary<string, object>();
            var TableName = DBUtil.GetTableName(type);
            sqlCmd.Builder.Append($"select * from {TableName} ");

            //if (entity != null) // 若無傳入 entity params 則 select all
            //{
            //    string where = string.Empty;
            //    foreach (var prop in props) // entity.GetType().GetProperties()
            //    {
            //        propName = prop.Name;
            //        propValue = prop.GetValue(entity);
            //        // 組建 where：欄位值非null/空白且非Type List<>
            //        if (StrUtil.GetStr(propValue) != string.Empty && prop.PropertyType.Name != typeof(List<>).Name)
            //        {
            //            //dicParams.Add(propName, propValue);
            //            where += string.IsNullOrEmpty(where) ? $" where {propName} = @{propName}" : $" and {propName} = @{propName}";
            //        }
            //    }
            //    sqlCmd.Builder.Append(where);
            //    sqlCmd.Param = entity; // dicParams
            //}

            if (!schemaOnly)
            {
                // BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance // 排除繼承屬性
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                // 組建 where：欄位屬性非[NotMapped]、欄位Type非List<>、欄位值非null/空白
                var wheres = props.Where(p => p.GetCustomAttribute(typeof(NotMappedAttribute), false) == null &&
                p.PropertyType.Name != typeof(List<>).Name &&
                ((entity as object)?.GetType()?.GetProperty(p.Name)?.GetValue((entity as object))).NullableToStr(Params.StrParam.TrimType.None) != string.Empty) // p.GetValue(entity)
                .Select(p => p.Name).ToList();
                if (wheres != null && wheres.Any())
                {
                    sqlCmd.Builder.Append($"where {string.Join(" and ", wheres.Select(name => name + " = @" + name).ToArray())} ");
                    sqlCmd.Param = entity;
                }
            }
            else
                sqlCmd.Builder.Append($"where 1 = 0 ");

            return sqlCmd;
        }

        /// <summary>
        /// Build Insert SQL
        /// </summary>
        /// <param name="type">typeof(model), model 具有定義 TableAttribute</param>
        /// <param name="entity">entity, 未必定義 TableAttribute</param>
        /// <param name="excludeCol">insert 排除欄位</param>
        public virtual SqlCmdUtil Insert(Type type, dynamic entity, HashSet<string> excludeCol = null)
        {
            SqlCmdUtil sqlCmd = new SqlCmdUtil();
            var TableName = DBUtil.GetTableName(type);
            // BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance // 排除繼承屬性
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //IEnumerable<string> columnNames, parameters;
            //var hashSet = new HashSet<string>(); // HashSet集合未排序，且不能包含重複的元素

            //foreach (var prop in props)
            //{
            //    object value = prop.GetValue(entity);
            //    if (!string.IsNullOrEmpty(Convert.ToString(value)))
            //    {
            //        if (value.GetType().Name.Contains("List")) continue;
            //        hashSet.Add(prop.Name);
            //    }
            //}

            //columnNames = props.Where(prop => hashSet.Contains(prop.Name)).Select(prop => prop.Name);
            //parameters = props.Where(prop => hashSet.Contains(prop.Name)).Select(prop => "@" + prop.Name);
            //sqlCmd.Builder.Append($"INSERT INTO  {TableName} ( {string.Join(",", columnNames.ToArray())} ) VALUES ( {string.Join(",", parameters.ToArray())} ) ");
            //sqlCmd.Param = entity;

            // 組建 insert column：欄位屬性非[NotMapped]、欄位Type非List<>
            var filter = props.Where(p => p.GetCustomAttribute(typeof(NotMappedAttribute), false) == null &&
            p.PropertyType.Name != typeof(List<>).Name);
            if (excludeCol != null) filter = filter.Where(p => !excludeCol.Contains(p.Name));
            var columnNames = filter.Select(p => p.Name).ToList();
            sqlCmd.Builder.Append($"insert into {TableName} ( {string.Join(", ", columnNames.ToArray())} ) ");
            sqlCmd.Builder.Append($"values ( {string.Join(", ", columnNames.Select(name => "@" + name).ToArray())} ) ");
            sqlCmd.Param = entity;

            return sqlCmd;
        }

        /// <summary>
        /// Build Update SQL
        /// </summary>
        /// <param name="type">typeof(model), model 具有定義 TableAttribute</param>
        /// <param name="entity">entity, 未必定義 TableAttribute</param>
        /// <param name="excludeCol">update 排除欄位</param>
        public virtual SqlCmdUtil Update(Type type, dynamic entity, HashSet<string> excludeCol = null)
        {
            SqlCmdUtil sqlCmd = new SqlCmdUtil();
            var TableName = DBUtil.GetTableName(type);
            // BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance // 排除繼承屬性
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            ////var wheres = props.Where(p => p.GetCustomAttributes(false).Any(key => key.GetType() == typeof(KeyAttribute)))
            ////.Where(p => p.GetValue(entity) != null).Select(p => p.Name + " = @" + p.Name);
            // 組建 where：搜尋Key欄位、Key值非null但可空白
            //var wheres = props.Where(p => p.GetCustomAttribute(typeof(KeyAttribute), false) != null)
            //.Where(p => p.GetValue(entity) != null).Select(p => p.Name).ToList();
            // 組建 where：搜尋Key欄位、皆建立條件無論Key值null/空白
            var wheres = props.Where(p => p.GetCustomAttribute(typeof(KeyAttribute), false) != null)
            .Select(p => p.Name).ToList();

            // Key有null則報錯
            //var keyHasNull = props.Where(p => p.GetCustomAttribute(typeof(KeyAttribute), false) != null)
            //.Where(p => p.GetValue(entity) == null).Select(p => p.Name).ToList();

            ////var sets = props.Where(p => !wheres.Contains(p.Name))
            ////.Where(p => p.GetValue(entity) != null && !p.GetValue(entity).GetType().Name.Contains("List"))
            ////.Select(p => p.Name + " = @" + p.Name);
            // 組建 set：欄位屬性非[NotMapped]、欄位Type非List<>、排除Key欄位、欄位值非null
            //var sets = props.Where(p => p.GetCustomAttribute(typeof(NotMappedAttribute), false) == null &&
            //p.PropertyType.Name != typeof(List<>).Name && !wheres.Contains(p.Name) &&
            //entity.GetType().GetProperty(p.Name)?.GetValue(entity) != null) // p.GetValue(entity)
            //.Select(p => p.Name).ToList();
            // 組建 set：欄位屬性非[NotMapped]、欄位Type非List<>、排除Key欄位
            var filter = props.Where(p => p.GetCustomAttribute(typeof(NotMappedAttribute), false) == null &&
            p.PropertyType.Name != typeof(List<>).Name &&
            !wheres.Contains(p.Name));
            if (excludeCol != null) filter = filter.Where(p => !excludeCol.Contains(p.Name));
            var sets = filter.Select(p => p.Name).ToList();

            sqlCmd.Builder.Append($"update {TableName} set {string.Join(", ", sets.Select(name => name + " = @" + name).ToArray())} ");
            sqlCmd.Builder.Append($"where {string.Join(" and ", wheres.Select(name => name + " = @" + name).ToArray())} ");
            sqlCmd.Param = entity;

            // 沒有PK或PK有null則報錯
            //if (wheres == null || wheres.Count() == 0 || (keyHasNull != null && keyHasNull.Count() > 0))
            // 沒有PK則報錯
            if (wheres == null || !wheres.Any())
                throw new Exception($"Update TableName: {TableName}. Primary key is empty.");

            return sqlCmd;
        }

        /// <summary>
        /// Build Patch SQL
        /// </summary>
        /// <param name="type">typeof(model), model 具有定義 TableAttribute</param>
        /// <param name="entity">entity, 未必定義 TableAttribute</param>
        /// <param name="updateCol">update 欄位</param>
        public virtual SqlCmdUtil Patch(Type type, dynamic entity, HashSet<string> updateCol)
        {
            SqlCmdUtil sqlCmd = new SqlCmdUtil();
            var TableName = DBUtil.GetTableName(type);
            // BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance // 排除繼承屬性
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 組建 where：搜尋Key欄位、皆建立條件無論Key值null/空白
            var wheres = props.Where(p => p.GetCustomAttribute(typeof(KeyAttribute), false) != null)
            .Select(p => p.Name).ToList();

            // 組建 set：欄位屬性非[NotMapped]、欄位Type非List<>、排除Key欄位、只更新updateCol
            var filter = props.Where(p => p.GetCustomAttribute(typeof(NotMappedAttribute), false) == null &&
             p.PropertyType.Name != typeof(List<>).Name &&
             !wheres.Contains(p.Name) &&
            updateCol.Contains(p.Name));
            var sets = filter.Select(p => p.Name).ToList();

            sqlCmd.Builder.Append($"update {TableName} set {string.Join(", ", sets.Select(name => name + " = @" + name).ToArray())} ");
            sqlCmd.Builder.Append($"where {string.Join(" and ", wheres.Select(name => name + " = @" + name).ToArray())} ");
            sqlCmd.Param = entity;

            // 沒有PK則報錯
            if (wheres == null || !wheres.Any())
                throw new Exception($"Update TableName: {TableName}. Primary key is empty.");

            return sqlCmd;
        }

    }
}