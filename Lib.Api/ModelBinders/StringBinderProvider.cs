using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Lib.Api.ModelBinders
{
    public class StringBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(string))
            {
                var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
                return new StringBinder(context.Metadata.ModelType, loggerFactory);
            }

            return null;
        }
    }
}
