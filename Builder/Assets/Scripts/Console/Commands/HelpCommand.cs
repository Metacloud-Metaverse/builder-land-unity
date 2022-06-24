using System.Collections;
using System.Collections.Generic;
using APISystem;
using UnityEngine;

namespace Console
{
    public class HelpCommand : Command
    {
        private readonly Callback _callback;
        
        public HelpCommand(Callback callback)
        {
            _callback = callback;
            verb = "help";
            paramNames = new string[] { "command name" };
            help = "Displays all commands data.";
        }
        
        public override void Execute(string[] parameters, Callback callback)
        {
            if(!MakeValidations(parameters)) return;
            _callback(parameters.Length == 0 ? null : parameters[0]);
        }

        protected override bool MakeValidations(string[] parameters)
        {
            if (!Validator.ValidateParametersCountEqualsTo(parameters.Length, 0, paramNames.Length)) return false;

            return true;
        }
    }

}
