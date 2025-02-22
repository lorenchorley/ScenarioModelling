using ScenarioModelling.CoreObjects;
using ScenarioModelling.Execution.Dialog;

namespace ScenarioModelling.Mocks;

public class MockDialogExecutor : DialogExecutor
{
    private Action<Context> _beforeInitStory = context => { };

    public MockDialogExecutor(Context context, MetaStoryStack metaStoryStack, IServiceProvider serviceProvider) : base(context, metaStoryStack, serviceProvider)
    {
    }

    public void SetBeforeInitStory(Action<Context> beforeInitStory)
    {
        _beforeInitStory = beforeInitStory;
    }

    protected override void InitStory()
    {
        _beforeInitStory(Context);

        base.InitStory();
    }

}
