using NAnt.Core;
using NAnt.Core.Attributes;

namespace SoftwareNinjas.NAnt.Tasks
{
    /// <summary>
    /// Represents a field inside an HTML form.
    /// </summary>
    [ElementName("input")]
    public class FormField : Element
    {
        /// <summary>
        /// The name of the field.
        /// </summary>
        [TaskAttribute("name", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string FieldName
        {
            get;
            set;
        }

        /// <summary>
        /// The value of the field.
        /// </summary>
        [TaskAttribute("value")]
        public string Value
        {
            get;
            set;
        }
    }
}
