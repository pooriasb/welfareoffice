using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.Core.Models.FieldTemplate
{
    public class InputText : IFieldTemplate
    {
        //Key of This template in form meta table
        #region ConstName
        private const string FieldDefaultName = "InputText";

        private const string FieldNameKey = "FieldName";
        private const string TitleKey = "Name";
        private const string IdKey = "HtmlId";
        private const string NameKey = "HtmlName";
        private const string ValueKey = "HtmlValue";
        private const string MaxLengthKey = "MaxLengthLimit";
        private const string MinLengthKey = "MinLengthLimit";
        private const string IsRequiredKey = "IsRequired";
        private const string TooltipTextKey = "TooltipText";
        #endregion

        //Properties
        #region properies
        public string FieldName { get; set; }

        public string Title { get; set; }

        public string HtmlId { get; set; }

        public string HtmlName { get; set; }

        public string HtmlValue { get; set; }

        public int MaxLength { get; set; }

        public int MinLength { get; set; }

        public bool IsRequired { get; set; }

        public string TooltipText { get; set; }

        private Field _masterField;

        #endregion

        public Field CreateField(Form parentForm, Dictionary<string, object> args)
        {
            FieldName = args[FieldNameKey] != null ? args[FieldNameKey].ToString() : FieldDefaultName;
            Title = args[TitleKey] != null ? args[TitleKey].ToString() : FieldDefaultName;
            MaxLength = args[MaxLengthKey] != null ? int.Parse(args[MaxLengthKey].ToString()) : 0;
            MinLength = args[MinLengthKey] != null ? int.Parse(args[MinLengthKey].ToString()) : 0;
            IsRequired = (bool?) args[IsRequiredKey] ?? false;

            return CreateField(parentForm);
        }

        private Field CreateField(Form parentForm, string fieldName = FieldDefaultName,
            string title = FieldDefaultName, string tooltipText = "", bool isRequired = false,
            int maxLength = 0, int minLength = 0,
            bool isHtmlText = false, string helpText = "")
        {
            var newField = new Field
            {
                FormId = parentForm.Id,
                Form = parentForm,
                CreatedAt = DateTime.Now,
                Title = title,
                IsHtmlHelp = isHtmlText,
                HelpText = helpText,
                IsRequired = isRequired,
            };

            var fieldMetas = new List<FieldMeta>();
            var maxLengthMeta = new FieldMeta
            {
                Field = newField,
                Key = MaxLengthKey,
                Value = maxLength.ToString()
            };
            fieldMetas.Add(maxLengthMeta);

            var minLengthMeta = new FieldMeta
            {
                Field = newField,
                Key = MinLengthKey,
                Value = minLength.ToString()
            };
            fieldMetas.Add(minLengthMeta);

            var tooltipMeta = new FieldMeta
            {
                Field = newField,
                Key = TooltipTextKey,
                Value = tooltipText
            };
            fieldMetas.Add(tooltipMeta);

            newField.FieldMetas = fieldMetas;

            return newField;
        }
        public string GetHtmlTag()
        {
            return _masterField == null ? "" : "_InputText".RenderPartialToString(this);
        }

        public void SetTemplateData(Field field)
        {
            if (field.FieldTemplate.Type != ModelEnums.FieldTemplateE.TextBoxTemplate) return;

            _masterField = field;
            FieldName = ModelEnums.FieldTemplateE.TextBoxTemplate.ToString();
            Title = field.Title;
            HtmlId = $"form{field.FormId}field{field.Id}";
            HtmlName = HtmlId;
            IsRequired = field.IsRequired;

            HtmlValue = "";

            var fieldMetas = field.FieldMetas.ToList();
            foreach (var fieldMeta in fieldMetas)
            {
                switch (fieldMeta.Key)
                {
                    case MaxLengthKey:
                        MaxLength = string.IsNullOrEmpty(fieldMeta.Value) ? 0 : int.Parse(fieldMeta.Value);
                        break;
                    case MinLengthKey:
                        MinLength = string.IsNullOrEmpty(fieldMeta.Value) ? 0 : int.Parse(fieldMeta.Value);
                        break;
                    case TooltipTextKey:
                        TooltipText = fieldMeta.Value;
                        break;
                    default: break;
                }

            }
        }

        public void SetSubmissionValue(object value)
        {
            throw new NotImplementedException();
        }

        public ClientFormField GetFieldSubmissions()
        {
            if (_masterField == null)
                return null;

            //switch (_MasterField.Key)
            //{
            //    case FieldNameKey:
            //        _MasterField.Value = FieldName;
            //        break;
            //    case TitleKey:
            //        fieldSubmission.Value = Name;
            //        break;
            //    case IdKey:
            //        fieldSubmission.Value = HtmlId;
            //        break;
            //    case NameKey:
            //        fieldSubmission.Value = HtmlName;
            //        break;
            //    case ValueKey:
            //        fieldSubmission.Value = HtmlValue;
            //        break;
            //    case MaxLengthKey:
            //        fieldSubmission.Value = MaxLength.ToString();
            //        break;
            //    case MinLengthKey:
            //        fieldSubmission.Value = MinLength.ToString();
            //        break;
            //    case IsRequiredKey:
            //        fieldSubmission.Value = IsRequired.ToString();
            //        break;
            //    case TooltipTextKey:
            //        fieldSubmission.Value = TooltipText;
            //        break;
            //    default:
            //        break;
            //}


            return null;
        }

        public void SetFieldSubmissions(List<ClientFormField> fieldSubmissions)
        {
            //_fieldSubmissions = fieldSubmissions;

            //foreach (var fieldSubmission in fieldSubmissions)
            //{
            //    switch (fieldSubmission.Key)
            //    {
            //        case FieldNameKey:
            //            FieldName = fieldSubmission.Value;
            //            break;
            //        case TitleKey:
            //            Name = fieldSubmission.Value;
            //            break;
            //        case IdKey:
            //            HtmlId = fieldSubmission.Value;
            //            break;
            //        case NameKey:
            //            HtmlName = fieldSubmission.Value;
            //            break;
            //        case ValueKey:
            //            HtmlValue = fieldSubmission.Value;
            //            break;
            //        case MaxLengthKey:
            //            MaxLength = !string.IsNullOrEmpty(fieldSubmission.Value) ? int.Parse(fieldSubmission.Value) : 0;
            //            break;
            //        case MinLengthKey:
            //            MinLength = !string.IsNullOrEmpty(fieldSubmission.Value) ? int.Parse(fieldSubmission.Value) : 0;
            //            break;
            //        case IsRequiredKey:
            //            IsRequired = !string.IsNullOrEmpty(fieldSubmission.Value) && bool.Parse(fieldSubmission.Value);
            //            break;
            //        case TooltipTextKey:
            //            TooltipText = fieldSubmission.Value;
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

    }
}