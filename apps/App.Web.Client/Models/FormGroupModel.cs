using Microsoft.AspNetCore.Mvc.Rendering;

namespace App.Web.Client.Models
{
    public class FormGroupModel
    {
        public string Label { get; set; }
        public string Icon { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // "text", "datetime-local", "file", "select", "textarea", etc.
        public string Placeholder { get; set; }
        public string Value { get; set; }
        public bool Multiple { get; set; }
        public IEnumerable<SelectListItem> Options { get; set; } // For select elements
        public bool IsInputGroup { get; set; } = false; // for things like calendar icons
        public string InputGroupAppendHtml { get; set; } // raw HTML for input group icons/buttons
    }
}
