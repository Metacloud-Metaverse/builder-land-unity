namespace Console
{
    public class SaveSceneCommand : Command
    {
        public SaveSceneCommand()
        {
            verb = "save-scene";
            paramNames = new string[] { "scene id" };
            help = "Creates or update a scene.";
        }
        
        public override void Execute(string[] parameters, Callback callback = null)
        {
            if(!MakeValidations(parameters)) return;
            SceneManagement.Instance.Save();
        }

        protected override bool MakeValidations(string[] parameters)
        {
            if (!Validator.ValidateParametersCountEqualsTo(parameters.Length, 0, paramNames.Length)) return false;

            return true;
        }
    }
}