namespace BP
{
    namespace GameConsole
    {
        namespace Attribute
        {
            using System;

            [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
            public class CommandAttribute : Attribute
            {
                public string KeyWord { get; set; }

                public CommandAttribute(string keyWord)
                {
                    KeyWord = keyWord;
                }
            }
        }
    }
}
