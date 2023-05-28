using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Utility
{
    public static class ActivationServer
    {
        /// <summary>
        ///  Generate 6 Digit Random ActivationCode
        /// </summary>
        /// <returns>return: a string with 6 digit number that left number is greater than zero</returns>
        public static string GenerateActivationCode()
        {
            var rand = new Random();
            var leftNum = rand.Next(9);
            leftNum++;

            var activationCode = leftNum.ToString();

            for (var i = 0; i < 5; i++)
            {
                activationCode += rand.Next(9).ToString();
            }

            return activationCode;
        }
    }
}