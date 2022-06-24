namespace Console
{
    public abstract class Command
    {
        public delegate void Callback(string param);
        public string verb;
        public string[] paramNames;
        public string help;

        public abstract void Execute(string[] parameters, Callback callback = null);

        protected abstract bool MakeValidations(string[] parameters);
    }
}