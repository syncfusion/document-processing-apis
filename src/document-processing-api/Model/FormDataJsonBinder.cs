using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace DocumentProcessing.API.Model
{
    /// <summary>
    /// Json binder for form data.
    /// </summary>
    public class FormDataJsonBinder : IModelBinder
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initialize the json binder
        /// </summary>
        /// <param name="loggerFactory"></param>
        public FormDataJsonBinder(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FormDataJsonBinder>();
        }

        /// <summary>
        /// Bind the model from the form data.
        /// </summary>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Check if the request data has form content
            var form = bindingContext.HttpContext.Request.Form;
            if (form == null || form.Count == 0)
            {
                _logger.LogWarning("No form data found.");
                return Task.CompletedTask;
            }

            try
            {
                // Read form data and bind JSON
                var jsonData = form[bindingContext.ModelName];
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonData, bindingContext.ModelType);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error binding model.");
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}
