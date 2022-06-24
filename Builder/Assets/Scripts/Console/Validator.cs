using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Console
{
    public static class Validator
    {
        /// <summary>
        /// Warns if the number of parameters is greater or less than specified.
        /// </summary>
        /// <param name="parametersReceivedCount">The number of parameters the user submitted.</param>
        /// <param name="parametersClassCount">The number of expected parameters.</param>
        public static bool ValidateParametersCount(int parametersReceivedCount, int parametersClassCount)
        {
            if (parametersReceivedCount > parametersClassCount)
            {
                Terminal.Instance.Append("More parameters were received than expected.");
                return false;
            }
            
            if (parametersReceivedCount < parametersClassCount)
            {
                Terminal.Instance.Append("Less parameters were received than expected.");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Validates the parameter count is equals to one of the expected amount of parameters.
        /// </summary>
        /// <param name="parametersCount">The number of parameters the user submitted.</param>
        /// <param name="countsAccepted">All amounts of parameters accepted.</param>
        public static bool ValidateParametersCountEqualsTo(int parametersCount, params int[] countsAccepted)
        {
            foreach (var countAccepted in countsAccepted)
            {
                if (countAccepted == parametersCount)
                    return true;
            }
            
            Terminal.Instance.Append("Incorrect parameters count!");
            return false;
        }

        public static bool ValidateNumber(string input, string paramName)
        {
            if (int.TryParse(input, out _))
                return true;
            
            Terminal.Instance.Append($"{paramName} must be a number");
            return false;
        }

        public static bool ValidateNumberLessThan(int number, int lessThan)
        {
            if (number < lessThan) return true;
            Terminal.Instance.Append($"{number} is not less than {lessThan}. That's not allowed.");
            return false;
        }
    }
}
