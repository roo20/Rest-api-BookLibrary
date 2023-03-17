using RESTful_api.Data;
using System.Linq.Dynamic.Core;
namespace RESTful_api.Helpers;

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(
        this IQueryable<T> source,
        string orderBy,
        Dictionary<string, PropertyMappingValue> mappingDictionary
        )
    {
        if ( source == null )  throw new ArgumentNullException( nameof(source));

        if (mappingDictionary == null) throw new ArgumentNullException(nameof(mappingDictionary));

        if (string.IsNullOrWhiteSpace(orderBy)) return source;

        var orderByString=string.Empty;

        var orderByAfterSplit= orderBy.Split(',');

        foreach (var orderByCase in orderByAfterSplit)
        {
            var trimmedOrderByCase = orderByCase.Trim();
            var orderDescending = trimmedOrderByCase.EndsWith(" desc");

            var indexOfFirstSpace=trimmedOrderByCase.IndexOf(" ");
            var propertyName=indexOfFirstSpace==-1? trimmedOrderByCase : trimmedOrderByCase.Remove(indexOfFirstSpace);

            if (!mappingDictionary.ContainsKey(propertyName))
            {
                throw new ArgumentException($"Key mapping for {propertyName} is missing");
            }

            var propertyMappingValue = mappingDictionary[propertyName];
            if (propertyMappingValue == null)
            {
                throw new ArgumentNullException(nameof(propertyMappingValue));
            }

            if (propertyMappingValue.Revert) 
            { 
                orderDescending = !orderDescending; 
            }
            foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
            {
                orderByString=orderByString+(string.IsNullOrWhiteSpace(orderByString)? string.Empty: ", ")
                    +destinationProperty
                    +(orderDescending? " descending" : " ascending");
            }

        }
        return source.OrderBy(orderByString);

    }
}
