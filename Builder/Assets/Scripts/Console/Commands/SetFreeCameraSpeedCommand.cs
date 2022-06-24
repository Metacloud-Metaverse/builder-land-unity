using System;
using APISystem;
using UnityEngine;

namespace Console
{
    public class SetFreeCameraSpeedCommand : Command
    {
        private const int _SPEED_VALUE = 0;
        
        public SetFreeCameraSpeedCommand()
        {
            verb = "set-freecam-speed";
            paramNames = new[] { "speed value" };
            help = "Sets the speed of the first person controller in preview mode.";
        }
        
        public override void Execute(string[] parameters, Callback callback = null)
        {
            if(!MakeValidations(parameters)) return;
            
            SceneManagement.Instance.fpc.GetComponent<FirstPersonController>().speed = float.Parse(parameters[_SPEED_VALUE]);
            callback($"The speed was assigned to {parameters[_SPEED_VALUE]} to the first person controller");
        }
        
        protected override bool MakeValidations(string[] parameters)
        {
            if (!Validator.ValidateParametersCount(parameters.Length, paramNames.Length)) return false;
            if (!Validator.ValidateNumber(parameters[_SPEED_VALUE], paramNames[_SPEED_VALUE])) return false;
            
            return true;
        }
    }
}
