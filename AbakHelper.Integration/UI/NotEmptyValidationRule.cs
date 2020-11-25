using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace AbakHelper.Integration.UI
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo, BindingExpressionBase owner)
        {
            return base.Validate(value, cultureInfo, owner);
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo, BindingGroup owner)
        {
            return base.Validate(value, cultureInfo, owner);
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "Field is required.")
                : ValidationResult.ValidResult;
        }
    }
}
