using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace APISystem
{
    public class API
    {
        public string url;
        protected MonoBehaviour _invoker;
        public string token;
        
        public API(MonoBehaviour invoker)
        {
            _invoker = invoker;
        }

        public static string CreateEndpoint(string url, string path, params string[] parameters)
        {
            var route = url + path;

            var routeParameters = GetParametersNames(route);

            if (routeParameters == null)
            {
                if (parameters.Length > 0)
                    throw new System.Exception("There are no route parameters to change but parameters were passed");
                return route;
            }

            if (routeParameters.Count != parameters.Length) 
                throw new System.Exception("The number of route parameters does not match the number passed to the function");

            var sb = new StringBuilder(route);

            for (int i = 0; i < routeParameters.Count; i++)
            {
                sb.Replace('{' + routeParameters[i] + '}', parameters[i]);
            }

            return sb.ToString();
        }

        public static string CreateEndpoint(string url, string path, Dictionary<string, string> parameters)
        {
            var route = url + path;

            var routeParameters = GetParametersNames(route);

            if (routeParameters == null)
            {
                if (parameters.Count > 0)
                    throw new System.Exception("There are no route parameters to change but parameters were passed");
                return route;
            }

            if (routeParameters.Count != parameters.Count)
                throw new System.Exception("The number of route parameters does not match the number passed to the function");

            var sb = new StringBuilder(route);

            foreach (var parameter in parameters)
            {
                sb.Replace("{" + parameter.Key + "}", parameter.Value);
            }

            return sb.ToString();

        }

        private static List<string> GetParametersNames(string route)
        {
            var parts = route.Split('}');
            var parameters = new List<string>();

            for (int i = 0; i < parts.Length - 1; i++)
            {
                var index = parts[i].IndexOf('{') + 1;
                if (index != -1)
                    parameters.Add(parts[i].Substring(index));
            }

            if (parameters.Count > 0) return parameters;
            return null;
        }
    }
}
