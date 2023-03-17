﻿using RESTful_api.Dtos;
using RESTful_api.Models;

namespace RESTful_api.Data;

public class PropertyMappingService : IPropertyMappingService
{
    private readonly Dictionary<string, PropertyMappingValue> _bookPropertyMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        {"Id", new(new[] {"Id"})},
       
        {"Title", new(new[] {"Title"})},
        {"Author", new(new[] {"Author"})},
        {"Genre", new(new[] {"Genre"})},
        {"Price", new(new[] {"Price"})},
        {"PublishDate", new(new[] {"PublishDate"})},
        {"Description", new(new[] {"Description"})},
    };

    private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();
    public PropertyMappingService()
    {
        _propertyMappings.Add(new PropertyMapping<BookReadDto, Book>(_bookPropertyMapping));
    }
    public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
    {
        var matchingMapping = _propertyMappings
            .OfType<PropertyMapping<TSource, TDestination>>();

        if (matchingMapping.Count() == 1)
        {
            return matchingMapping.First().MappingDictionary;
        }

        throw new Exception($"Cannot find exact property mapping instance " +
            $"for <{typeof(TSource)},{typeof(TDestination)}");

    }
}
