using System;
using System.Collections.Generic;
using System.Text;

namespace LadyIDE.WPF.CanvasElements
{
    public class Action
    {
        public bool PipeAndFilterAction { get; } = false;

        public List<Parameter> Input { get; } = new();

        public List<Parameter> Output { get; } = new();

        public List<string> Logic { get; } = new();

        public Action()
        {
            Logic.Add("()");
            Logic.Add("AND");
            Logic.Add("OR");
            Logic.Add("map");
            Logic.Add("flatten");
            Logic.Add("forall");
            Logic.Add("==");
            Logic.Add("!=");
            Logic.Add(":=");
            Logic.Add("code");
            Logic.Add("create");
            Logic.Add("!");
            Logic.Add("EMPTY");
        }
    }
}
