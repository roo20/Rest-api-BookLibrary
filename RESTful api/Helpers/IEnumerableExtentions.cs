using System.Dynamic;
using System.Reflection;

namespace RESTful_api.Helpers;

public static class IEnumerableExtentions
{
    public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string? fields)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        //create an expandoObject List => will be the result
        var expandoObjkectList = new List<ExpandoObject>();

        // create list of PropertyInfo object on TSource
        //Reflaction is Expinsive, so rather than doing that for 
        //each item we do it for a whole list and reuse the result

        var propertyInfoList= new List<PropertyInfo>();

        if (string.IsNullOrWhiteSpace(fields))
        {

            //include all public properties
            var propertyInfos=typeof(TSource).GetProperties(BindingFlags.IgnoreCase
                |BindingFlags.Public
                |BindingFlags.Instance);
            propertyInfoList.AddRange(propertyInfos);
        }
        else
        {
            //all field separated by comma
            var fieldsAfterSplit=fields.Split(',');

            foreach ( var field in fieldsAfterSplit)
            {
                //trim each fild incase it contains leading or trailing whitespaces
                //cant trim the var in foreach 
                var propertyName=field.Trim();

                //use Reflection to get the property on the source object
                //the public and instance need to be included because specifying the binding
                //flag will overwrite the already existing binding flags
                var propertyInfo= typeof(TSource).GetProperty(propertyName,
                    BindingFlags.IgnoreCase
                    | BindingFlags.Public
                    | BindingFlags.Instance
                    );

                if (propertyInfo == null )
                {
                    throw new Exception($"Property {propertyName} was not Found on {typeof(TSource)}");
                }
                //add PropertyInfo to the Llist
                propertyInfoList.Add( propertyInfo );
            }
        }

        //loop through the source objects
        foreach (TSource sourceObject in source)
        {
            //create ExpandoObject that will hold the
            //Selected Property and Value
            var dataShapedObject = new ExpandoObject();

            // Get the value of each property we have to return
            // therefore we run through the list
            foreach (var propertyInfo in propertyInfoList)
            {
                // GetValue return the value of the property on source object
                var propertyValue = propertyInfo.GetValue(sourceObject);

                //add the field to the ExpandoObject
                ((IDictionary<string,object?>) dataShapedObject)
                    .Add(propertyInfo.Name, propertyValue);

            }
            //add the ExpandoObject to the List
            expandoObjkectList.Add(dataShapedObject);
        }
        // return the list
        return expandoObjkectList;
    }

}
