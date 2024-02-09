namespace BP
{
    namespace GameConsole
    {
        namespace Behaviour
        {
            using Godot;
            public partial class Command : Node
            {
                public string CommandName { get; private set; }
                public virtual void Initialize(string name)
                {
                    CommandName = name;
                    CommandManager.Instance.AddChild(this);
                }
                public virtual void Execute(string[] keys) { }
            }
        }
    }
}
