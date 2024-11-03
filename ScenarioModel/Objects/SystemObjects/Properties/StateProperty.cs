//using OneOf;
//using ScenarioModel.Objects.SystemObjects.States;
//using ScenarioModel.References;

//namespace ScenarioModel.Objects.SystemObjects.Properties;

//public class StateProperty
//{
//    public System System { get; }
//    private OneOf<State, StateReference> _state;

//    public StateProperty(System System, OneOf<State, StateReference> state)
//    {
//        this.System = System;
//        _state = state;
//    }

//    public void SetState(State state)
//    {
//        _state = state;
//    }

//    public void SetState(StateReference reference)
//    {
//        _state = reference;
//    }

//    public State? ResolvedValue
//    {
//        get
//        {
//            return _state.Match(
//                state => state,
//                reference => reference.ResolveReference(System).Match(
//                    state => state,
//                    () => throw new Exception("State reference could not be resolved.")
//                )
//            );
//        }
//    }

//}
