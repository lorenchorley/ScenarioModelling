using ProtoBuf;
using ScenarioModelling.Annotations.Attributes;

namespace ScenarioModelling.CoreObjects.TestCaseNodes.BaseClasses;

[ProtoContract]
public interface ITestCaseNode : ICategoryClass, IIdentifiable
{

}
