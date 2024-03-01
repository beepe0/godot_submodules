using Godot;

namespace BP
{
    namespace ComponentSystem
    {
        public static class NodeExtensions
        {
            public static T GetComponent<T>(this Node node) where T : Node, new()
            {
                return node.GetNodeOrNull<T>($"Components/{typeof(T).Name}");
            }
            public static T AddComponent<T>(this Node node) where T : Node, new()
            {
                T component = new T();
                component.Name = typeof(T).Name;
                node.AddChild(component);
                return component;
            }
        }
    }
}
