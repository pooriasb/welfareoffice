using System.Collections.Generic;
using System.EnterpriseServices;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.Core.Models.FieldTemplate
{
    internal interface IFieldTemplate
    {
        void SetTemplateData(Field field);


        Field CreateField(Form parentForm, Dictionary<string, object> args);

        string GetHtmlTag();


        void SetSubmissionValue(object value);

        ClientFormField GetFieldSubmissions();
    }
}