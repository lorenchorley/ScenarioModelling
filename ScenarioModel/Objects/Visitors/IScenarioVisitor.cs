﻿using ScenarioModel.Objects.ScenarioNodes;

namespace ScenarioModel.Objects.Visitors;

public interface IScenarioVisitor
{
    object VisitChooseNode(ChooseNode chooseNode);
    object VisitDialogNode(DialogNode dialogNode);
    object VisitIfNode(IfNode ifNode);
    object VisitJumpNode(JumpNode jumpNode);
    object VisitTransitionNode(TransitionNode transitionNode);
    object VisitWhileNode(WhileNode whileNode);
}
