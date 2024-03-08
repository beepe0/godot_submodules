using Godot;
using Godot.Collections;
using System;

namespace BP
{
    namespace ComponentSystem
    {
        public partial class ComponentSystem : Node
        {
            public static Dictionary<string, ComponentSystem> componentsSystems = new();
            public Dictionary<string, ComponentObject> componentsObjects = new();

            [Export] public string Tag = string.Empty;

            public override void _Ready()
            {
                Initialize();
            }
            private void Initialize()
            {
                Array<Node> children = GetChildren();
                if (children.Count > 0)
                {
                    foreach (Node node in children)
                    {
                        if (node is ComponentObject)
                        {
                            GameConsole.GameConsole.Instance.DebugWarning(node.Name);

                            componentsObjects.Add(node.Name, node as ComponentObject);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(Tag))
                {
                    componentsSystems.Add(Tag, this);
                }
            }
            public T GetComponent<T>() where T : Node, new()
            {
                if (!componentsObjects.ContainsKey(typeof(T).Name)) return null;
                GameConsole.GameConsole.Instance.DebugError($"{typeof(T).Name} :: {componentsObjects[typeof(T).Name].Name}");
                return componentsObjects[typeof(T).Name] as T;
            }
            public T AddComponent<T>() where T : Node, new()
            {
                if (componentsObjects.ContainsKey(typeof(T).Name)) return null;
                T component = new T();
                component.Name = typeof(T).Name;
                AddChild(component);
                return component;
            }
            public static ComponentSystem GetComponentSystemWithTag(string tag)
            {
                if (!componentsSystems.ContainsKey(tag)) return null;
                return componentsSystems[tag];
            }
        }
        public static class NodeExtensions
        {
            public static ComponentSystem GetComponentSystem(this Node node)
            {
                return node.GetNodeOrNull<ComponentSystem>($"Components");
            }
        }
    }
}
