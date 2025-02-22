using ScenarioModelling.CoreObjects;
using ScenarioModelling.Mocks.Utils;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.Mocks;

public class MetaStoryMockBuilder
{
    private bool _isBuilt = false;
    private readonly IServiceProvider _serviceProvider;

    public string TargetMetaStoryName { get; private set; } = string.Empty;
    internal BusinessToStateMap BusinessObjectToStateMap { get; }

    public MetaStoryMockBuilder(IServiceProvider serviceProvider, MetaState metaState)
    {
        _serviceProvider = serviceProvider;
        BusinessObjectToStateMap = new(metaState);
    }

    public MetaStoryMockBuilder WithTargetMetaStoryName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

        if (_isBuilt)
            throw new MetaStoryMockException("Cannot modify a finalised MetaStoryMock");

        TargetMetaStoryName = name;

        return this;
    }

    public MetaStoryMockBuilder RegisterBusinessType<TBusinessValue>(string businessTypeName) where TBusinessValue : notnull
    {
        if (_isBuilt)
            throw new MetaStoryMockException("Cannot modify a finalised MetaStoryMock");

        BusinessObjectToStateMap.RegisterBusinessType<TBusinessValue>(businessTypeName);

        return this;
    }

    public MetaStoryMockBuilder AddValueToStateTranslator<TBusinessValue>(string businessTypeName, Func<TBusinessValue, string> businessValueToStateTranslator) where TBusinessValue : notnull
    {
        if (_isBuilt)
            throw new MetaStoryMockException("Cannot modify a finalised MetaStoryMock");

        BusinessObjectToStateMap.AddValueToStateTranslator(businessTypeName, businessValueToStateTranslator);

        return this;
    }

    public MetaStoryMockBuilder AddStateToValueTranslation<TBusinessValue>(string stateName, string businessTypeName, TBusinessValue businessValue) where TBusinessValue : notnull
    {
        if (_isBuilt)
            throw new MetaStoryMockException("Cannot modify a finalised MetaStoryMock");

        BusinessObjectToStateMap.AddStateToValueTranslation(stateName, businessTypeName, businessValue);

        return this;
    }

    internal MetaStoryMockBuilder AddBusinessTypeToStatefulObjectRelation(string businessTypeName, string statefulObjectName)
    {
        if (_isBuilt)
            throw new MetaStoryMockException("Cannot modify a finalised MetaStoryMock");

        BusinessObjectToStateMap.AddStateToValueTranslation(businessTypeName, statefulObjectName);

        return this;
    }

    public MetaStoryMock Build()
    {
        if (_isBuilt)
            throw new MetaStoryMockException("Cannot modify a finalised MetaStoryMock");

        MetaStoryMock metaStoryMock = new(_serviceProvider);

        if (string.IsNullOrWhiteSpace(TargetMetaStoryName))
            throw new MetaStoryMockException("TargetMetaStoryName must be set before building a MetaStoryMock");

        metaStoryMock.TargetMetaStoryName = TargetMetaStoryName;
        metaStoryMock.BusinessObjectToStateMap = BusinessObjectToStateMap;

        _isBuilt = true;

        return metaStoryMock;
    }

}
