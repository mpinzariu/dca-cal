using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;

namespace dca_cal.Data
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();

                var enumType = context.Type;

                // Add each enum value as an integer with a description
                foreach (var value in Enum.GetValues(enumType))
                {
                    var memberInfo = enumType.GetMember(value.ToString()).FirstOrDefault();
                    var descriptionAttribute = memberInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                                        .FirstOrDefault() as DescriptionAttribute;
                    var description = descriptionAttribute?.Description ?? value.ToString();

                    // Add enum value with integer and description as metadata
                    schema.Enum.Add(new OpenApiInteger((int)value));
                    schema.Description += $"{(int)value} - {description}\n";
                }
            }
        }
    }
}

