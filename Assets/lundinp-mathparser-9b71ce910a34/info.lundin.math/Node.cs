using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace info.lundin.math
{
    /// <summary>
    /// Class Node, represents a Node in a tree data structure representation of a mathematical expression.
    /// </summary>
    internal class Node
    {
        /// <summary>Represents the type variable</summary>
        internal static int TYPE_VARIABLE = 1;

        /// <summary>Represents the type constant ( numeric value )</summary>
        internal static int TYPE_CONSTANT = 2;

        /// <summary>Represents the type expression</summary>
        internal static int TYPE_EXPRESSION = 3;

        /// <summary>Reserved</summary>
        internal static int TYPE_END = 4;

        /// <summary>Used as initial value</summary>
        internal static int TYPE_UNDEFINED = -1;

        private String _operator = "";
        private Node _arg1 = null;
        private Node _arg2 = null;
        private int args = 0;
        private int type = TYPE_UNDEFINED;
        private double value = Double.NaN;
        private String variable = "";

        /// <summary>
        /// Creates a Node containing the specified Operator and arguments.
        /// This will automatically mark this Node as a TYPE_EXPRESSION
        /// </summary>
        /// <param name="_operator">the string representing an operator</param>
        /// <param name="_arg1">the first argument to the specified operator</param>
        /// <param name="_arg2">the second argument to the specified operator</param>
        internal Node(String _operator, Node _arg1, Node _arg2)
        {
            this._arg1 = _arg1;
            this._arg2 = _arg2;
            this._operator = _operator;
            this.args = 2;
            this.type = TYPE_EXPRESSION;
        }

        /// <summary>
        /// Creates a Node containing the specified Operator and argument.
        /// This will automatically mark this Node as a TYPE_EXPRESSION
        /// </summary>
        /// <param name="_operator">the string representing an operator</param>
        /// <param name="_arg1">the argument to the specified operator</param>
        internal Node(String _operator, Node _arg1)
        {
            this._arg1 = _arg1;
            this._operator = _operator;
            this.args = 1;
            this.type = TYPE_EXPRESSION;
        }

        /// <summary>
        /// Creates a Node containing the specified variable.
        /// This will automatically mark this Node as a TYPE_VARIABLE
        /// </summary>
        /// <param name="variable">the string representing a variable</param>
        internal Node(String variable)
        {
            this.variable = variable;
            this.type = TYPE_VARIABLE;
        }

        /// <summary>
        /// Creates a Node containing the specified value.
        /// This will automatically mark this Node as a TYPE_CONSTANT
        /// </summary>
        /// <param name="value">the value for this Node</param>
        internal Node(double value)
        {
            this.value = value;
            this.type = TYPE_CONSTANT;
        }

        /// <summary>
        /// Returns the String operator of this Node 
        /// </summary>
        internal String getOperator()
        {
            return (this._operator);
        }

        /// <summary>
        /// Returns the value of this Node 
        /// </summary>
        internal double getValue()
        {
            return (this.value);
        }

        /// <summary>
        /// Returns the String variable of this Node 
        /// </summary>
        internal String getVariable()
        {
            return (this.variable);
        }

        /// <summary>
        /// Returns the number of arguments this Node has
        /// </summary>
        internal int arguments()
        {
            return (this.args);
        }

        /// <summary>
        /// Returns the type of this Node
        /// </summary>
        /// <remarks>
        /// The type can be:
        ///	Node.TYPE_VARIABLE
        ///	Node.TYPE_CONSTANT
        ///	Node.TYPE_EXPRESSION
        /// </remarks>
        internal int getType()
        {
            return (this.type);
        }

        /// <summary>
        /// Returns the first argument of this Node
        /// </summary>
        internal Node arg1()
        {
            return (this._arg1);
        }

        /// <summary>
        /// Returns the second argument of this Node
        /// </summary>
        internal Node arg2()
        {
            return (this._arg2);
        }

    } // End class Node
}
