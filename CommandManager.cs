namespace BP
{
    namespace GameConsole
    {
        using BP.GameConsole.Attribute;
        using BP.GameConsole.Behaviour;
        using Godot;
        using Godot.Collections;
        using System;
        using System.Linq;
        using System.Reflection;

        public partial class CommandManager : Node
        {
            public static CommandManager Instance { get; private set; }
            private Dictionary<string, Command> _commands = new Dictionary<string, Command>();

            public override void _EnterTree()
            {
                Instance = this;
            }
            public override void _Ready()
            {
                Initialize();
            }
            private void Initialize()
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] types = assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(CommandAttribute), true).Any()).ToArray();

                foreach (Type t in types)
                {
                    CommandAttribute commandAttribute = t.GetCustomAttribute(typeof(CommandAttribute), true) as CommandAttribute;
                    Command instance = Activator.CreateInstance(t) as Command;

                    instance.Initialize(commandAttribute.KeyWord);

                    if (instance != null && commandAttribute != null)
                    {
                        GameConsole.Instance.DebugLog($"Type with CommandAttribute: {t.FullName}, KeyWord: {commandAttribute.KeyWord}");
                        _commands.Add(commandAttribute.KeyWord, instance);
                    }
                }
            }
            public bool InvokeCommand(string text)
            {
                if (string.IsNullOrEmpty(text.Trim())) return false;

                string[] keys = text.Split(" ");
                Command command = GetCommand(keys[0]);

                if (command != null)
                {
                    command.Execute(keys);
                    return true;
                }

                return false;
            }
            public Command GetCommand(string key)
            {
                if (_commands.ContainsKey(key)) return _commands[key];
                else return null;
            }
        }
    }
}