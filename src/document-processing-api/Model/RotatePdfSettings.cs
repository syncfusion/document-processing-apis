using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DocumentProcessing.API.Model
{
    /// <summary>
    /// Settings to rotate a PDF document.
    /// </summary>
    [ModelBinder(BinderType = typeof(FormDataJsonBinder))]
    public class RotatePdfSettings
    {
        /// <summary>
        /// Rotation angle in degrees.
        /// </summary>
        [Required]
        [RotationAngleValidationAttribute]
        public string RotationAngle { get; set; }

        /// <summary>
        /// Specifies the input file to rotate.
        /// </summary>
        [Required]
        public string File { get; set; }

        /// <summary>
        /// Password to open protected PDF.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Page ranges to rotate the PDF.
        /// </summary>
        [Required]
        [MinLength(1)]
        public List<PageRange> PageRanges { get; set; }

    }

    /// <summary>
    /// Page range to rotate.
    /// </summary>
    public class PageRange
    {
        /// <summary>
        /// Start page of the range.
        /// </summary>
        public int Start { get; set; }
        /// <summary>
        /// End page of the range
        /// </summary>
        public int End { get; set; }
    }

    /// <summary>
    /// Rotation valdation attribute
    /// </summary>
    public class RotationAngleValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validate the rotation angle
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var rotationAngle = value as string;
            if (rotationAngle != null && (rotationAngle == "0" || rotationAngle == "90" || rotationAngle == "180" || rotationAngle == "270"))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Rotation angle must be 0, 90, 180, or 270.");
        }
    }

}
