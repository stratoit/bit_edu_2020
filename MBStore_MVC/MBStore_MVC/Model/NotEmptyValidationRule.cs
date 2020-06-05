
using System.Globalization;
using System.Windows.Controls;

namespace MBStore_MVC.Model
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "입력 칸이 비어있습니다.")
                : ValidationResult.ValidResult;
        }
    }
}
