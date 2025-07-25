﻿using FluentValidation;
using System;

namespace Lib
{
    public class RuleUtil
    {
    }

    public static class RuleExUtil
    {
        /// <summary>
        /// 最大字長限制(適用於 sybase、sqlserver 的 char、varchar)
        /// </summary>
        /// <typeparam name="T">Model or ViewMdel Type</typeparam>
        /// <typeparam name="TProperty">Property Type</typeparam>
        /// <param name="maxLenProvider">取得最大字長</param>
        public static IRuleBuilderOptions<T, TProperty> MaxLen<T, TProperty>
            (this IRuleBuilder<T, TProperty> ruleBuilder, Func<T, int> maxLenProvider)
        {
            // rootObject: is Model or ViewMdel
            return ruleBuilder.Must((rootObject, propertyVal, context) =>
            {
                int maxLen = maxLenProvider(rootObject);
                int propertyValLen = (propertyVal as string).StrLen();

                context.MessageFormatter
                  .AppendArgument("MaxLen", maxLen)
                  .AppendArgument("PropertyValLen", propertyValLen);

                return propertyValLen <= maxLen;
            })
            .WithMessage("'{PropertyName}' 字長限制{MaxLen}個字符。已達{PropertyValLen}個字符。");
        }

        /// <summary>
        /// 最大字長限制(適用於 sybase、sqlserver 的 nchar、nvarchar)
        /// </summary>
        /// <typeparam name="T">Model or ViewMdel Type</typeparam>
        /// <typeparam name="TProperty">Property Type</typeparam>
        /// <param name="maxLenProvider">取得最大字長</param>
        public static IRuleBuilderOptions<T, TProperty> MaxLenUnicode<T, TProperty>
            (this IRuleBuilder<T, TProperty> ruleBuilder, Func<T, int> maxLenProvider)
        {
            // rootObject: is Model or ViewMdel
            return ruleBuilder.Must((rootObject, propertyVal, context) =>
            {
                int maxLen = maxLenProvider(rootObject);
                int propertyValLen = (propertyVal as string)?.Length ?? 0;

                context.MessageFormatter
                  .AppendArgument("MaxLen", maxLen)
                  .AppendArgument("PropertyValLen", propertyValLen);

                return propertyValLen <= maxLen;
            })
            .WithMessage("'{PropertyName}' 字長限制{MaxLen}個字符。已達{PropertyValLen}個字符。");
        }

        /// <summary>
        /// 需為數值
        /// </summary>
        /// <typeparam name="T">Model or ViewMdel Type</typeparam>
        /// <typeparam name="TProperty">Property Type</typeparam>
        public static IRuleBuilderOptions<T, TProperty> MustNumeric<T, TProperty>
            (this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.Must((propertyVal) =>
            {   // return true 才為驗證成功
                return propertyVal.IsNumeric();
            })
            .WithMessage("'{PropertyName}' 需為數值。");
        }

    }
}
