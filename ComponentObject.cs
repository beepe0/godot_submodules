using Godot;

namespace BP
{
    namespace ComponentSystem
    {
        public partial class ComponentObject : Node
        {
            public ComponentSystem Parent { get; set; }

            public override void _Ready()
            {
                Initialize();
            }
            private void Initialize()
            {
                Parent = GetParent() as ComponentSystem;
            }
        }
    }
}
