using System;
using APISystem;

namespace Console
{
    public class LoginCommand : Command
    {
        private const int _PARAM_USER = 0;
        private const int _PARAM_PASS = 1;
        
        public LoginCommand()
        {
            verb = "login-guest";
            paramNames = new string[] { "user id", "pass" };
            help = "Saves the token that will be used for following petitions";
        }
    
        public override void Execute(string[] parameters, Callback callback = null)
        {
            if(!MakeValidations(parameters)) return;
            
            var connectionCallback = new APIConnection.ConnectionCallback(callback);
            SceneManagement.Instance.Login(int.Parse(parameters[_PARAM_USER]), parameters[_PARAM_PASS], connectionCallback);
            Terminal.Instance.Append("Petition sent...");
        }
        
        protected override bool MakeValidations(string[] parameters)
        {
            if (!Validator.ValidateParametersCount(parameters.Length, paramNames.Length)) return false;
            if (!Validator.ValidateNumber(parameters[_PARAM_USER], paramNames[_PARAM_USER])) return false;

            return true;
        }
    }
}
