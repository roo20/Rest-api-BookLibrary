using System.Dynamic;
using System.Reflection;

namespace RESTful_api.Helpers;

public static class ObjectExtentions
{
    public static ExpandoObject ShapeData<TSource>(this TSource source, string? fields)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        //create an expandoObject => will be the result
        var dataShapedObject = new ExpandoObject();


        // create list of PropertyInfo object on TSource
        //Reflaction is Expinsive, so rather than doing that for 
        //each item we do it for a whole list and reuse the result
        //create ExpandoObject that will hold the
        //Selected Property and Value
        var propertyInfoList = new List<PropertyInfo>();



        if (string.IsNullOrWhiteSpace(fields))
        {

            //include all public properties
            var propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase
                | BindingFlags.Public
                | BindingFlags.Instance);

            foreach (var propertyInfo in propertyInfoList)
            {
                // GetValue return the value of the property on source object
                var propertyValue = propertyInfo.GetValue(source);

                //add the field to the ExpandoObject
                ((IDictionary<string, object?>)dataShapedObject)
                    .Add(propertyInfo.Name, propertyValue);

            }
            return dataShapedObject;
        }
        
            //all field separated by comma
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                //trim each fild incase it contains leading or trailing whitespaces
                //cant trim the var in foreach 
                var propertyName = field.Trim();

                //use Reflection to get the property on the source object
                //the public and instance need to be included because specifying the binding
                //flag will overwrite the already existing binding flags
                var propertyInfo = typeof(TSource).GetProperty(propertyName,
                    BindingFlags.IgnoreCase
                    | BindingFlags.Public
                    | BindingFlags.Instance
                    );

                if (propertyInfo == null)
                {
                    throw new Exception($"Property {propertyName} was not Found on {typeof(TSource)}");
                }
            // GetValue return the value of the property on source object
            var propertyValue = propertyInfo.GetValue(source);

            //add the field to the ExpandoObject
            ((IDictionary<string, object?>)dataShapedObject)
                .Add(propertyInfo.Name, propertyValue);

            }
        return dataShapedObject;

    }
}
