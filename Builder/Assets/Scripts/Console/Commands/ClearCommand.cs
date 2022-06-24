using System;
using APISystem;

namespace Console
{
    public class ClearCommand : Command
    {
        public ClearCommand()
        {
            verb = "clear";
            help = "It Clears the terminal screen.";
        }
    
        public override void Execute(string[] parameters, Callback callback = null)
        {
            if(!MakeValidations(parameters)) return;
            
            Terminal.Instance.Clear();
        }
        
        protected override bool MakeValidations(string[] parameters)
        {
            if (!Validator.ValidateParametersCount(parameters.Length, 0)) return false;

            return true;
        }
    }
}
