using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace info.lundin.math
{
    /// <summary>
    /// Class Operator, represents an Operator by holding information about it's symbol
    /// the number of arguments it takes and the operator precedence.
    /// </summary>
    internal class Operator
    {

        private String op = ""; // the string operator 
        private int args = 0; // the number of arguments this operator takes
        private int prec = System.Int32.MaxValue; // the precedence this operator has

        /// <summary>
        /// Creates an Operator with the specified String name, arguments and precedence
        /// </summary>
        internal Operator(String _operator, int arguments, int precedence)
        {
            this.op = _operator;
            this.args = arguments;
            this.prec = precedence;
        }

        /// <summary>
        /// Returns the precedence for this Operator.
        /// </summary>
        internal int precedence()
        {
            return (this.prec);
        }

        /// <summary>
        /// Returns the String name of this Operator.
        /// </summary>
        internal String getOperator()
        {
            return (this.op);
        }

        /// <summary>
        /// Returns the number of arguments this Operator can take.
        /// </summary>
        internal int arguments()
        {
            return (this.args);
        }

    } // End class Operator
}
