using System.ComponentModel.DataAnnotations;

namespace ScreenScrappingAzureFunctionDemo.Services.Models
{
    /// <summary>
    ///     This represents the model entity for error response.
    /// </summary>
    public class ErrorResponseModel
    {
        /// <summary>
        ///     Gets or sets the HTTP status code.
        /// </summary>
        [Display(Name = "Status Code")]
        public int StatusCode { get; set; }

        /// <summary>
        ///     Gets or sets the error message.
        /// </summary>
        [Display(Name = "Message")]
        [DataType(DataType.Text)]
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the error description.
        /// </summary>
        [Display(Name = "Description")]
        [DataType(DataType.Text)]
        public string Description { get; set; }
    }
}
