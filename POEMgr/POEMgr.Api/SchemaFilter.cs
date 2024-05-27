using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace POEMgr.Api
{
    public class SchemaFilter: ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!context.Type.IsClass || context.Type == typeof(string) || !context.Type.IsPublic || context.Type.IsArray) return;
            var obj = Activator.CreateInstance(context.Type);
            _ = (from sc in schema.Properties
                 join co in context.Type.GetProperties() on sc.Key.ToLower() equals co.Name.ToLower()
                 select sc.Value.Example = co.GetValue(obj) != null ? OpenApiAnyFactory.CreateFromJson(co.GetValue(obj).ToString()) : sc.Value.Example).ToList();
        }
    }
}
