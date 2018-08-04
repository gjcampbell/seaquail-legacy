using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaQuail.Data
{
    public enum SQFunctions 
    { 
        /// <summary>
        /// To uppercase, 1 paramter
        /// </summary>
        UCASE,
        /// <summary>
        /// To lowercase, 1 paramter
        /// </summary>
        LCASE,
        /// <summary>
        /// Extract characters from a text field. Paramaters: field, start position, length
        /// </summary>
        MID,
        /// <summary>
        /// Get the length of a text fields
        /// </summary>
        LEN,
        /// <summary>
        /// Round a numeric field. Parameters: field, number of decimals
        /// </summary>
        ROUND,
        /// <summary>
        /// Get current date. No parameters
        /// </summary>
        NOW
    }

    public enum SQAggregates 
    { 
        /// <summary>
        /// Average
        /// </summary>
        AVG, 
        COUNT, 
        MAX, 
        MIN, 
        SUM 
    }

    public static class EnumExtensions
    {
        public static string Create(this SQAggregates aggregate, params string[] parameters)
        {
            return Create(aggregate, SQAdapter.Instance, parameters);
        }
        public static string Create(this SQAggregates aggregate, SQAdapter adp, params string[] parameters)
        {
            return adp.CreateAggregate(aggregate, parameters);
        }

        public static string Create(this SQFunctions function, params string[] parameters)
        {
            return Create(function, SQAdapter.Instance, parameters);
        }
        public static string Create(this SQFunctions function, SQAdapter adp, params string[] parameters)
        {
            return adp.CreateFunction(function, parameters);
        }
    }
}
