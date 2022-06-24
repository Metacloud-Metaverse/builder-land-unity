using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

namespace Console
{
    public class Terminal : MonoBehaviour
    {
        private List<string> _commandsSaved = new List<string>();
        private int _currentCommandSelected;
        private Command[] _commands;
        [SerializeField]
        private TMP_InputField _input;
        [SerializeField]
        private TMP_InputField _commandsUsed;
        public static Terminal Instance { get; private set; }
        
        void Awake()
        {
            if (Instance == null) Instance = this;
            else throw new Exception("More than one Terminal instance");
                
            CreateCommands();
        }
    
        private void CreateCommands()
        {
            _commands = new Command[]
            {
                new CreateGuestCommand(),
                new LoginCommand(),
                new GetChunksCommand(),
                new ClearCommand(),
                new SetFreeCameraSpeedCommand(),
                new HelpCommand(AppendHelp)
            };
        }
        
        private void LogResponse(string response)
        {
            Append(response);
        }
        
        private void ExecuteCommand(string command)
        {
            Append($"> {command}");
            _input.text = "";
            SaveCommand(command);
            FocusInput();
            var commandArray = command.Split(" ");
            var verb = commandArray[0];
            var parameters = GetCommandParameters(commandArray);
            var commandFounded = SearchCommand(verb);
            commandFounded?.Execute(parameters, LogResponse);
        }

        private void SaveCommand(string command)
        {
            _commandsSaved.Add(command);
            _currentCommandSelected = _commandsSaved.Count;
        }

        private void PreviousCommand()
        {
            if (_commandsSaved.Count == 0) return;
            _currentCommandSelected--;
            if (_currentCommandSelected < 0) _currentCommandSelected = _commandsSaved.Count - 1;
            _input.text = _commandsSaved[_currentCommandSelected];
            _input.caretPosition = _input.text.Length;
        }
        
        private void NextCommand()
        {
            if (_commandsSaved.Count == 0) return;
            _currentCommandSelected++;
            if (_currentCommandSelected >= _commandsSaved.Count) _currentCommandSelected = 0;
            _input.text = _commandsSaved[_currentCommandSelected];
            _input.caretPosition = _input.text.Length;
        }

        
        private Command SearchCommand(string verb)
        {
            foreach (var command in _commands)
            {
                if (command.verb == verb)
                    return command;
            }
            Append("Command not found. Type help to see the full list of commands.");
            return null;
        }
    
        public void Append(string line)
        {
            _commandsUsed.text += $"\n{line}";
        }
    
        private string[] GetCommandParameters(string[] commandArray)
        {
            var parameters = new string[commandArray.Length - 1];
    
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = commandArray[i + 1];
            }
    
            return parameters;
        }
        
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Return))
                ExecuteCommand(_input.text);
            if(Input.GetKeyDown(KeyCode.UpArrow))
                PreviousCommand();
            if(Input.GetKeyDown(KeyCode.DownArrow))
                NextCommand();
        }

        private void FocusInput()
        {
            _input.Select();
            _input.ActivateInputField();
        }
        
        private void OnEnable()
        {
            SceneManagement.Instance.activeEditor = false;
            FocusInput();
        }

        private void OnDisable()
        {
            SceneManagement.Instance.activeEditor = true;
        }

        private void AppendHelp(string commandName)
        {
            if (commandName != null)
            {
                var command = SearchCommand(commandName);
                WriteCommandInfo(command);
                return;
            }
            
            foreach (var command in _commands)
            {
                Append("");
                WriteCommandInfo(command);
            }
        }

        private void WriteCommandInfo(Command command)
        {
            string commandParams;
            if (command.paramNames != null)
            {
                var stringBuilder = new StringBuilder();
                foreach (var paramName in command.paramNames)
                {
                    stringBuilder.Append($"<{paramName}> ");
                }

                commandParams = stringBuilder.ToString();
            }
            else commandParams = "";
                    
            Append($"{command.verb} {commandParams}");
            Append(command.help);
        }

        public void Clear()
        {
            _commandsUsed.text = "";
        }
    }
}