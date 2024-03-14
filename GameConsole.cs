namespace BP
{
    namespace GameConsole
    {
        using Godot;
        using System;
        using System.Collections.Generic;

        public partial class GameConsole : Control
        {
            public static GameConsole Instance { get; private set; }

            [ExportGroup("Console Settings")]
            [Export] private ushort _numberOfLines = 30;
            [Export] private float _fontSize = 20;
            [Export] private ushort _scrollSpeed = 1;

            [ExportGroup("UI")]
            [Export] private RichTextLabel _richTextLable;
            [Export] private LineEdit _lineEdit;

            private List<string> _consoleContent = new();
            private int _currentLine = 0;

            public override void _EnterTree()
            {
                Instance = this;
            }
            public override void _Ready()
            {
                _lineEdit.Text = string.Empty;
                _richTextLable.GuiInput += OnRichTextLabelGuiInput;
            }
            public override void _Input(InputEvent @event)
            {
                _currentLine -= (int)(_scrollSpeed * @event.GetActionStrength("scroll_wheel_up"));
                _currentLine += (int)(_scrollSpeed * @event.GetActionStrength("scroll_wheel_down"));

                if (@event.IsActionPressed("show_console"))
                {
                    Input.MouseMode = (!Visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured);
                    Visible = !Visible;
                }
                else if (@event.IsActionPressed("accept"))
                {
                    Debug(_lineEdit.Text);

                    CommandManager.Instance.InvokeCommand(_lineEdit.Text);
                    _lineEdit.Text = string.Empty;
                }
            }
            public void OnRichTextLabelGuiInput(InputEvent @event)
            {
                UpdateContent(ref _currentLine);
            }
            public void UpdateContent(ref int startLine)
            {
                string content = string.Empty;
                int sl = Math.Clamp(startLine, 0, Math.Max(_consoleContent.Count - (_numberOfLines - 1), 0));
                int el = _numberOfLines + sl;

                for (int i = sl, k = 0; i < _consoleContent.Count && k < el; k++, i++)
                {
                    content += _consoleContent[i] + "\n";
                }

                startLine = sl;

                _richTextLable.Clear();
                _richTextLable.AppendText(content);
            }

            public static string SetColor(string text, Color color)
            {
                return $"[color={color.ToHtml()}]{text}[/color]";
            }
            public static string SetColor(string text, string hex)
            {
                return $"[color={hex}]{text}[/color]";
            }
            public void Debug(string text)
            {
                _currentLine++;
                _consoleContent.Add($"{SetColor(text, "#808080")}");
                UpdateContent(ref _currentLine);
            }
            public void DebugLog(string text)
            {
                _currentLine++;
                _consoleContent.Add($"{SetColor($"[{DateTime.Now.ToLongTimeString()}]:", "#21799e")} {text}");
                UpdateContent(ref _currentLine);
            }
            public void DebugWarning(string text)
            {
                _currentLine++;
                _consoleContent.Add($"{SetColor($"[{DateTime.Now.ToLongTimeString()}]:", "#eb8334")} {text}");
                UpdateContent(ref _currentLine);
            }
            public void DebugError(string text)
            {
                _currentLine++;
                _consoleContent.Add($"{SetColor($"[{DateTime.Now.ToLongTimeString()}]:", "#963636")} {text}");
                UpdateContent(ref _currentLine);
            }
            public void ClearConsole()
            {
                _currentLine = 0;
                _richTextLable.Clear();
                _consoleContent.Clear();
                UpdateContent(ref _currentLine);
            }
            public void DebugCallDeferrd(string text)
            {
                CallDeferred(MethodName.Debug, text);
            }
            public void DebugLogCallDeferrd(string text)
            {
                CallDeferred(MethodName.DebugLog, text);
            }
            public void DebugWarningCallDeferrd(string text)
            {
                CallDeferred(MethodName.DebugWarning, text);
            }
            public void DebugErrorCallDeferrd(string text)
            {
                CallDeferred(MethodName.DebugError, text);
            }
            public void ClearConsoleCallDeferrd()
            {
                CallDeferred(MethodName.ClearConsole);
            }
        }
    }
}