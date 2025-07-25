#nullable enable

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Lib.Api.ModelBinders
{

    /// <summary>
    /// An <see cref="IModelBinder"/> for simple types.
    /// </summary>
    /// <remarks>https://stackoverflow.com/questions/64903414/net-core-mvc-api-route-url-encoding-20-causes-null-pointer</remarks>
    public class StringBinder : IModelBinder
    {
        private readonly TypeConverter _typeConverter;

        /// <summary>
        /// Initializes a new instance of <see cref="StringBinder"/>.
        /// </summary>
        /// <param name="type">The type to create binder for.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public StringBinder(Type type, ILoggerFactory loggerFactory)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _typeConverter = TypeDescriptor.GetConverter(type);
        }

        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                // no entry
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            try
            {
                var value = valueProviderResult.FirstValue;

                object? model;
                if (bindingContext.ModelType == typeof(string))
                {
                    // Already have a string. No further conversion required but handle ConvertEmptyStringToNull.
                    // IsNullOrWhiteSpace 改為 IsNullOrEmpty，使 Model Binding 可接收空格
                    if (bindingContext.ModelMetadata.ConvertEmptyStringToNull && string.IsNullOrEmpty(value))
                    {
                        model = null;
                    }
                    else
                    {
                        model = value;
                    }
                }
                else if (string.IsNullOrEmpty(value))
                {
                    // Other than the StringConverter, converters Trim() the value then throw if the result is empty.
                    model = null;
                }
                else
                {
                    model = _typeConverter.ConvertFrom(
                        context: null,
                        culture: valueProviderResult.Culture,
                        value: value);
                }

                CheckModel(bindingContext, valueProviderResult, model);

                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                var isFormatException = exception is FormatException;
                if (!isFormatException && exception.InnerException != null)
                {
                    // TypeConverter throws System.Exception wrapping the FormatException,
                    // so we capture the inner exception.
                    exception = ExceptionDispatchInfo.Capture(exception.InnerException).SourceException;
                }

                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    exception,
                    bindingContext.ModelMetadata);

                // Were able to find a converter for the type but conversion failed.
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// If the <paramref name="model" /> is <see langword="null" />, verifies that it is allowed to be <see langword="null" />,
        /// otherwise notifies the <see cref="P:ModelBindingContext.ModelState" /> about the invalid <paramref name="valueProviderResult" />.
        /// Sets the <see href="P:ModelBindingContext.Result" /> to the <paramref name="model" /> if successful.
        /// </summary>
        protected virtual void CheckModel(
            ModelBindingContext bindingContext,
            ValueProviderResult valueProviderResult,
            object? model)
        {
            // When converting newModel a null value may indicate a failed conversion for an otherwise required
            // model (can't set a ValueType to null). This detects if a null model value is acceptable given the
            // current bindingContext. If not, an error is logged.
            if (model == null && !bindingContext.ModelMetadata.IsReferenceOrNullableType)
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    bindingContext.ModelMetadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor(
                        valueProviderResult.ToString()));
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(model);
            }
        }
    }
}
