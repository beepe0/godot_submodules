using Godot;
using Godot.Collections;
using System;

namespace BP
{
    namespace ComponentSystem
    {
        public partial class ComponentSystem : Node
        {
            public static readonly Dictionary<string, ComponentSystem> ComponentsSystems = new();
            public Dictionary<string, ComponentObject> ComponentsObjects = new();

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
                            ComponentsObjects.Add(node.Name, node as ComponentObject);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(Tag))
                {
                    ComponentsSystems.Add(Tag, this);
                }
            }
            public T GetComponent<T>() where T : Node, new()
            {
                if (!ComponentsObjects.ContainsKey(typeof(T).Name)) return null;
                return ComponentsObjects[typeof(T).Name] as T;
            }
            public T AddComponent<T>() where T : Node, new()
            {
                if (ComponentsObjects.ContainsKey(typeof(T).Name)) return null;
                T component = new T();
                component.Name = typeof(T).Name;
                AddChild(component);
                return component;
            }
            public static ComponentSystem GetComponentSystemWithTag(string tag)
            {
                if (!ComponentsSystems.ContainsKey(tag)) return null;
                return ComponentsSystems[tag];
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
