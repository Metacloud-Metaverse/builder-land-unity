using System.Collections;
using System.Collections.Generic;
using APISystem;
using UnityEngine;

namespace Console
{
    public class CreateGuestCommand : Command
    {
        public CreateGuestCommand()
        {
            verb = "create-guest";
            help = "Connects to the users API and creates a guest. Returns the guest data.";
        }
        
        public override void Execute(string[] parameters, Callback callback)
        {
            if(!MakeValidations(parameters)) return;
            var connectionCallback = new APIConnection.ConnectionCallback(callback);
            SceneManagement.Instance.CreateGuest(connectionCallback);
            Terminal.Instance.Append("Petition sent...");
        }

        protected override bool MakeValidations(string[] parameters)
        {
            if(!Validator.ValidateParametersCount(parameters.Length, 0)) return false;

            return true;
        }
    }
}
