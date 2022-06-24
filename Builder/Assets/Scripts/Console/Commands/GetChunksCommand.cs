using System;
using APISystem;

namespace Console
{
    public class GetChunksCommand : Command
    {
        public GetChunksCommand()
        {
            verb = "get-chunks";
            help = "Returns the chunks count of current scene.";
        }
    
        public override void Execute(string[] parameters, Callback callback = null)
        {
            if(!MakeValidations(parameters)) return;

            var chunks = SceneManagement.Instance.chunks;
            callback($"X:{chunks.x} ; Y:{chunks.y}");
        }
        
        protected override bool MakeValidations(string[] parameters)
        {
            if (!Validator.ValidateParametersCount(parameters.Length, 0)) return false;

            return true;
        }
    }
}
